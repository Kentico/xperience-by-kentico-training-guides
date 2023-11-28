using System.Data;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.OnlineForms;
using TrainingGuides.Web.Features.DataProtection.Collectors;

namespace TrainingGuides.Web.Features.DataProtection.Services;

public interface IFormCollectionService
{
    public Dictionary<Guid, FormDefinition> GetForms();
    public ObjectQuery<BizFormItem> GetBizFormItems(ICollection<string> emails, ObjectQuery<ConsentAgreementInfo> consentAgreementGuids, DataRow row, FormDefinition formDefinition);
}