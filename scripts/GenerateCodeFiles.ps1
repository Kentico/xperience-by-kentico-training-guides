<#
.Synopsis
    Generates code for classes, forms, and content types stored in the database.
#>
$exitCode = 0

$originalLocation = Get-Location
Set-Location -Path $PSScriptRoot/../src/TrainingGuides.Web

# https://docs.xperience.io/xp/developers-and-admins/development/content-retrieval/generate-code-files-for-xperience-objects

$contentTypesNamespace = "TrainingGuides"

function Write-Result-Get-Exit-Code {
    param(
        [string] $type
    )    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "$type code generation failed."
        return 1;
    }
    else{
        Write-Host "$type code generation succeeded." -ForegroundColor Green
        return 0;
    }
    Write-Host
}

#Reusable content types
dotnet run --no-build -- --kxp-codegen --type "ReusableContentTypes" --namespace $contentTypesNamespace --location "../TrainingGuides.Entities/{type}/{name}"
$exitCode = Write-Result-Get-Exit-Code "Reusable content type"

#Page content types
dotnet run --no-build -- --kxp-codegen --type "PageContentTypes" --namespace $contentTypesNamespace --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation
$exitCode = Write-Result-Get-Exit-Code "Page content type"

#Reusable field schemas
dotnet run --no-build -- --kxp-codegen --type "ReusableFieldSchemas" --namespace $contentTypesNamespace --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation
$exitCode = Write-Result-Get-Exit-Code "Reusable field schema"

#Custom module classes
dotnet run --no-build -- --kxp-codegen --type "Classes" --with-provider-class False --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation
$exitCode = Write-Result-Get-Exit-Code "Class"

#Forms
dotnet run --no-build -- --kxp-codegen --type "Forms" --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation
$exitCode = Write-Result-Get-Exit-Code "Form"

if ($exitCode -ne 0) {
    Set-Location -Path $originalLocation
    Write-Error "Completed with errors. See above."
    Read-Host -Prompt "Press Enter to exit"
    exit $exitCode
}

Set-Location -Path $originalLocation
Read-Host -Prompt "Press Enter to exit"