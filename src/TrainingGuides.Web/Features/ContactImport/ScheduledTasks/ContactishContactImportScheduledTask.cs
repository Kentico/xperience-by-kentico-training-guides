using System.Xml;
using CMS.Scheduler;
using TrainingGuides.Web.Features.ContactImport;
using TrainingGuides.Web.Features.Shared.Logging;

[assembly: RegisterScheduledTask(identifier: ContactishContactImportScheduledTask.IDENTIFIER, typeof(ContactishContactImportScheduledTask))]

namespace TrainingGuides.Web.Features.ContactImport;

public class ContactishContactImportScheduledTask(
    ILogger<ContactishContactImportScheduledTask> logger,
    IContactImportService contactImportService,
    IWebHostEnvironment webHostEnvironment) : IScheduledTask
{
    public const string IDENTIFIER = "TrainingGuides.ContactishContactImportScheduledTask";
    private const string IMPORT_FOLDER_PATH = "App_Data\\TrainingGuidesContactImport\\Contactish";

    public async Task<ScheduledTaskExecutionResult> Execute(ScheduledTaskConfigurationInfo taskConfiguration, CancellationToken cancellationToken)
    {
        string directoryPath = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, IMPORT_FOLDER_PATH);

        if (!Directory.Exists(directoryPath))
        {
            logger.LogError(EventIds.ImportPathNotFound,
                "Contact import directory does not exist: {DirectoryPath}",
                directoryPath);
            return await Task.FromResult(new ScheduledTaskExecutionResult("Failed - Directory does not exist"));
        }

        // Find all XML files in the directory
        var xmlFiles = Directory.EnumerateFiles(directoryPath, "*.xml", SearchOption.TopDirectoryOnly)
            .ToList();

        foreach (string xmlFile in xmlFiles)
        {
            var doc = new XmlDocument();

            using var fileStream = new FileStream(System.IO.Path.Combine(directoryPath, xmlFile), FileMode.Open);
            try
            {
                // Load the XML document from the file stream
                doc.Load(fileStream);
            }
            catch (XmlException ex)
            {
                logger.LogError(EventIds.ImportContactsFromFileError,
                    ex,
                    "Failed to load XML document from file: {XmlFile}",
                    xmlFile);
                return await Task.FromResult(new ScheduledTaskExecutionResult(ex.Message));
            }

            // Import contacts from the XML document
            try
            {
                int contactsImported = contactImportService.ImportContactsFromXml(doc);

                logger.LogInformation(EventIds.ImportContactsFromFileInfo,
                    "Successfully imported {ContactsImported} contacts from file: {XmlFile}",
                    contactsImported,
                    xmlFile);
            }
            catch (Exception ex)
            {
                logger.LogError(EventIds.ImportContactsFromFileError,
                    ex,
                    "An error occurred while importing contacts from file: {XmlFile}",
                    xmlFile);

                return await Task.FromResult(new ScheduledTaskExecutionResult(ex.Message));
            }

            // Ensure the Contactish contact group exists, and rebuild the group to populate it with imported contacts
            try
            {
                await contactImportService.EnsureContactGroup(rebuildContactGroup: true);
            }
            catch (Exception ex)
            {
                logger.LogError(EventIds.EnsureContactGroupError,
                    ex,
                    "An error occurred while ensuring the contact group exists.");

                return await Task.FromResult(new ScheduledTaskExecutionResult(ex.Message));
            }

            // Ensure the Contactish recipient lists exist, so that we can add contacts to them
            try
            {
                await contactImportService.EnsureRecipientLists();
            }
            catch (Exception ex)
            {
                logger.LogError(EventIds.EnsureRecipientListError,
                    ex,
                    "An error occurred while ensuring the recipient list exists.");

                return await Task.FromResult(new ScheduledTaskExecutionResult(ex.Message));
            }
        }
        return await Task.FromResult(ScheduledTaskExecutionResult.Success);
    }
}