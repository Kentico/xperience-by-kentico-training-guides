using CMS.ContentEngine;
using CMS.Websites;
using TrainingGuides.Web.OneTimeCode;
using Xunit;

namespace TrainingGuides.Web.Tests.OneTimeCode;

/// <summary>
/// Tests for the ConversionAttempt class that tracks the conversion process from old Article content items 
/// to new GeneralArticle (Reusable Field Schema) content items.
/// </summary>
public class ConversionAttemptTests
{
    private const string ENGLISH_LANGUAGE_NAME = "en-US";
    private const string SPANISH_LANGUAGE_NAME = "es-ES";
    private const string FRENCH_LANGUAGE_NAME = "fr-FR";
    private const int CONTENT_ITEM_ID = 100;

    private static Article CreateSampleArticle(int contentItemId = CONTENT_ITEM_ID) => new()
    {
        ArticleTitle = "Test Article",
        ArticleSummary = "Test Summary",
        ArticleText = "Test Text",
        ArticleTeaser = [],
        SystemFields = new ContentItemFields
        {
            ContentItemID = contentItemId,
            ContentItemName = "test-article"
        }
    };

    #region Constructor Tests

    [Fact]
    public void Constructor_WithAllParameters_InitializesCorrectly()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var newId = 200;
        var exception = new Exception("Test exception");
        var logMessage = "Test log message";

        // Act
        var attempt = new ConversionAttempt(oldArticle, newId, exception, logMessage);

        // Assert
        Assert.Equal(oldArticle, attempt.OldArticle);
        Assert.Equal(newId, attempt.NewArticleContentItemId);
        Assert.Single(attempt.Exceptions);
        Assert.Equal(exception, attempt.Exceptions[0]);
        Assert.Single(attempt.LogMessages);
        Assert.Equal(logMessage, attempt.LogMessages[0]);
        Assert.Empty(attempt.FinishedLanguagesReusable);
        Assert.Empty(attempt.FinishedLanguagesPage);
    }

    [Fact]
    public void Constructor_WithNullNewId_InitializesWithNullId()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();

        // Act
        var attempt = new ConversionAttempt(oldArticle, null);

        // Assert
        Assert.Null(attempt.NewArticleContentItemId);
    }

    [Fact]
    public void Constructor_WithoutExceptionAndLog_InitializesEmptyLists()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var newId = 200;

        // Act
        var attempt = new ConversionAttempt(oldArticle, newId);

        // Assert
        Assert.Empty(attempt.Exceptions);
        Assert.Empty(attempt.LogMessages);
        Assert.Empty(attempt.FinishedLanguagesReusable);
        Assert.Empty(attempt.FinishedLanguagesPage);
    }

    [Fact]
    public void Constructor_WithNullException_InitializesEmptyExceptionsList()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();

        // Act
        var attempt = new ConversionAttempt(oldArticle, 200, null, "Test log");

        // Assert
        Assert.Empty(attempt.Exceptions);
        Assert.Single(attempt.LogMessages);
    }

    [Fact]
    public void Constructor_WithNullLogMessage_InitializesEmptyLogMessagesList()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();

        // Act
        var attempt = new ConversionAttempt(oldArticle, 200, new Exception("Error"), null);

        // Assert
        Assert.Single(attempt.Exceptions);
        Assert.Empty(attempt.LogMessages);
    }

    #endregion

    #region Exception Management Tests

    [Fact]
    public void Exceptions_CanAddSingleException()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);
        var exception = new Exception("Error 1");

        // Act
        attempt.Exceptions.Add(exception);

        // Assert
        Assert.Single(attempt.Exceptions);
        Assert.Equal(exception, attempt.Exceptions[0]);
    }

    [Fact]
    public void Exceptions_CanAddMultipleExceptions()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.Exceptions.Add(new Exception("Error 1"));
        attempt.Exceptions.Add(new Exception("Error 2"));
        attempt.Exceptions.Add(new Exception("Error 3"));

        // Assert
        Assert.Equal(3, attempt.Exceptions.Count);
    }

    [Fact]
    public void Exceptions_PreservesOrderOfAddition()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);
        var error1 = new Exception("Error 1");
        var error2 = new Exception("Error 2");
        var error3 = new Exception("Error 3");

        // Act
        attempt.Exceptions.Add(error1);
        attempt.Exceptions.Add(error2);
        attempt.Exceptions.Add(error3);

        // Assert
        Assert.Equal(error1, attempt.Exceptions[0]);
        Assert.Equal(error2, attempt.Exceptions[1]);
        Assert.Equal(error3, attempt.Exceptions[2]);
    }

    [Fact]
    public void Exceptions_CanContainDifferentExceptionTypes()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.Exceptions.Add(new Exception("Generic error"));
        attempt.Exceptions.Add(new InvalidOperationException("Invalid operation"));
        attempt.Exceptions.Add(new ArgumentNullException("paramName", "Argument was null"));

        // Assert
        Assert.Equal(3, attempt.Exceptions.Count);
        Assert.IsType<Exception>(attempt.Exceptions[0]);
        Assert.IsType<InvalidOperationException>(attempt.Exceptions[1]);
        Assert.IsType<ArgumentNullException>(attempt.Exceptions[2]);
    }

    #endregion

    #region Log Message Management Tests

    [Fact]
    public void LogMessages_CanAddSingleMessage()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.LogMessages.Add("Log message 1");

        // Assert
        Assert.Single(attempt.LogMessages);
        Assert.Equal("Log message 1", attempt.LogMessages[0]);
    }

    [Fact]
    public void LogMessages_CanAddMultipleMessages()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.LogMessages.Add("Log 1");
        attempt.LogMessages.Add("Log 2");
        attempt.LogMessages.Add("Log 3");

        // Assert
        Assert.Equal(3, attempt.LogMessages.Count);
    }

    [Fact]
    public void LogMessages_PreservesOrderOfAddition()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.LogMessages.Add("First log");
        attempt.LogMessages.Add("Second log");
        attempt.LogMessages.Add("Third log");

        // Assert
        Assert.Equal("First log", attempt.LogMessages[0]);
        Assert.Equal("Second log", attempt.LogMessages[1]);
        Assert.Equal("Third log", attempt.LogMessages[2]);
    }

    [Fact]
    public void LogMessages_CanContainEmptyStrings()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.LogMessages.Add("");
        attempt.LogMessages.Add("Non-empty log");

        // Assert
        Assert.Equal(2, attempt.LogMessages.Count);
        Assert.Equal("", attempt.LogMessages[0]);
        Assert.Equal("Non-empty log", attempt.LogMessages[1]);
    }

    #endregion

    #region Language Tracking Tests

    [Fact]
    public void FinishedLanguagesReusable_CanTrackSingleLanguage()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.FinishedLanguagesReusable.Add(ENGLISH_LANGUAGE_NAME);

        // Assert
        Assert.Single(attempt.FinishedLanguagesReusable);
        Assert.Contains(ENGLISH_LANGUAGE_NAME, attempt.FinishedLanguagesReusable);
    }

    [Fact]
    public void FinishedLanguagesReusable_CanTrackMultipleLanguages()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.FinishedLanguagesReusable.Add(ENGLISH_LANGUAGE_NAME);
        attempt.FinishedLanguagesReusable.Add(SPANISH_LANGUAGE_NAME);
        attempt.FinishedLanguagesReusable.Add(FRENCH_LANGUAGE_NAME);

        // Assert
        Assert.Equal(3, attempt.FinishedLanguagesReusable.Count);
        Assert.Contains(ENGLISH_LANGUAGE_NAME, attempt.FinishedLanguagesReusable);
        Assert.Contains(SPANISH_LANGUAGE_NAME, attempt.FinishedLanguagesReusable);
        Assert.Contains(FRENCH_LANGUAGE_NAME, attempt.FinishedLanguagesReusable);
    }

    [Fact]
    public void FinishedLanguagesPage_CanTrackSingleLanguage()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.FinishedLanguagesPage.Add(ENGLISH_LANGUAGE_NAME);

        // Assert
        Assert.Single(attempt.FinishedLanguagesPage);
        Assert.Contains(ENGLISH_LANGUAGE_NAME, attempt.FinishedLanguagesPage);
    }

    [Fact]
    public void FinishedLanguagesPage_CanTrackMultipleLanguages()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.FinishedLanguagesPage.Add(ENGLISH_LANGUAGE_NAME);
        attempt.FinishedLanguagesPage.Add(SPANISH_LANGUAGE_NAME);

        // Assert
        Assert.Equal(2, attempt.FinishedLanguagesPage.Count);
        Assert.Contains(ENGLISH_LANGUAGE_NAME, attempt.FinishedLanguagesPage);
        Assert.Contains(SPANISH_LANGUAGE_NAME, attempt.FinishedLanguagesPage);
    }

    [Fact]
    public void FinishedLanguages_ReusableAndPageAreIndependent()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.FinishedLanguagesReusable.Add(ENGLISH_LANGUAGE_NAME);
        attempt.FinishedLanguagesReusable.Add(SPANISH_LANGUAGE_NAME);
        attempt.FinishedLanguagesPage.Add(ENGLISH_LANGUAGE_NAME);

        // Assert
        Assert.Equal(2, attempt.FinishedLanguagesReusable.Count);
        Assert.Single(attempt.FinishedLanguagesPage);
        Assert.Contains(ENGLISH_LANGUAGE_NAME, attempt.FinishedLanguagesReusable);
        Assert.Contains(SPANISH_LANGUAGE_NAME, attempt.FinishedLanguagesReusable);
        Assert.Contains(ENGLISH_LANGUAGE_NAME, attempt.FinishedLanguagesPage);
        Assert.DoesNotContain(SPANISH_LANGUAGE_NAME, attempt.FinishedLanguagesPage);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void OldArticle_RetainsReferenceToOriginalArticle()
    {
        // Arrange
        var oldArticle = CreateSampleArticle(123);

        // Act
        var attempt = new ConversionAttempt(oldArticle, 456);

        // Assert
        Assert.Same(oldArticle, attempt.OldArticle);
        Assert.Equal(123, attempt.OldArticle.SystemFields.ContentItemID);
    }

    [Fact]
    public void NewArticleContentItemId_CanBeSet()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act
        attempt.NewArticleContentItemId = 999;

        // Assert
        Assert.Equal(999, attempt.NewArticleContentItemId);
    }

    [Fact]
    public void NewArticleContentItemId_CanBeSetToNull()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, 500);

        // Act
        attempt.NewArticleContentItemId = null;

        // Assert
        Assert.Null(attempt.NewArticleContentItemId);
    }

    [Fact]
    public void OldArticle_CanBeModified()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, 200);

        // Act
        attempt.OldArticle = CreateSampleArticle(999);

        // Assert
        Assert.Equal(999, attempt.OldArticle.SystemFields.ContentItemID);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void ConversionAttempt_CanTrackCompleteConversionProcess()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act - Simulate a conversion process
        // Step 1: Create reusable article
        attempt.NewArticleContentItemId = 200;
        attempt.LogMessages.Add("Created new reusable article with ID [200]");
        attempt.FinishedLanguagesReusable.Add(ENGLISH_LANGUAGE_NAME);

        // Step 2: Add Spanish version
        attempt.LogMessages.Add("Added language version [es-ES] for article");
        attempt.FinishedLanguagesReusable.Add(SPANISH_LANGUAGE_NAME);

        // Step 3: Update page references
        attempt.LogMessages.Add("Updated page reference for English");
        attempt.FinishedLanguagesPage.Add(ENGLISH_LANGUAGE_NAME);

        // Step 4: Log a warning
        attempt.Exceptions.Add(new Exception("Warning: Spanish page reference could not be updated"));

        // Assert
        Assert.Equal(200, attempt.NewArticleContentItemId);
        Assert.Equal(3, attempt.LogMessages.Count);
        Assert.Equal(2, attempt.FinishedLanguagesReusable.Count);
        Assert.Single(attempt.FinishedLanguagesPage);
        Assert.Single(attempt.Exceptions);
    }

    [Fact]
    public void ConversionAttempt_CanRepresentFailedConversion()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, null);

        // Act - Simulate a failed conversion
        attempt.Exceptions.Add(new Exception("Failed to create new article: Database connection failed"));
        attempt.Exceptions.Add(new Exception("Retry attempt 1 failed"));
        attempt.Exceptions.Add(new Exception("Retry attempt 2 failed"));

        // Assert
        Assert.Null(attempt.NewArticleContentItemId);
        Assert.Equal(3, attempt.Exceptions.Count);
        Assert.Empty(attempt.LogMessages);
        Assert.Empty(attempt.FinishedLanguagesReusable);
        Assert.Empty(attempt.FinishedLanguagesPage);
    }

    [Fact]
    public void ConversionAttempt_CanRepresentPartialSuccess()
    {
        // Arrange
        var oldArticle = CreateSampleArticle();
        var attempt = new ConversionAttempt(oldArticle, 300);

        // Act - Simulate partial success
        attempt.LogMessages.Add("Created reusable article");
        attempt.FinishedLanguagesReusable.Add(ENGLISH_LANGUAGE_NAME);
        attempt.LogMessages.Add("Published English version");

        attempt.Exceptions.Add(new Exception("Failed to create Spanish version"));
        attempt.Exceptions.Add(new Exception("Page reference update failed for English"));

        // Assert
        Assert.Equal(300, attempt.NewArticleContentItemId);
        Assert.Single(attempt.FinishedLanguagesReusable);
        Assert.Empty(attempt.FinishedLanguagesPage);
        Assert.Equal(2, attempt.LogMessages.Count);
        Assert.Equal(2, attempt.Exceptions.Count);
    }

    #endregion
}
