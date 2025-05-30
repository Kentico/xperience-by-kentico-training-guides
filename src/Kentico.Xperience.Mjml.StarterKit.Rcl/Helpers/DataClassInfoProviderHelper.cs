using CMS.DataEngine;

namespace Kentico.Xperience.Mjml.StarterKit.Rcl.Helpers;

/// <summary>
/// Helper class for retrieving class information from the DataClassInfoProvider.
/// </summary>
internal static class DataClassInfoProviderHelper
{
    /// <summary>
    /// Retrieves class GUIDs for the specified content type code names.
    /// </summary>
    /// <param name="codeNames">Collection of content type code names to retrieve GUIDs for.</param>
    /// <returns>Collection of unique GUIDs corresponding to the provided code names.</returns>
    public static IEnumerable<Guid> GetClassGuidsByCodeNames(IEnumerable<string> codeNames)
    {
        var enumerable = codeNames as string[] ?? codeNames.ToArray();

        if (enumerable.Length == 0)
        {
            return [];
        }

        List<Guid> result = [];

        foreach (var codeName in enumerable)
        {
            var classInfo = DataClassInfoProvider.ProviderObject.Get(codeName);

            if (classInfo != null)
            {
                result.Add(classInfo.ClassGUID);
            }
        }

        return result.Distinct();
    }
}
