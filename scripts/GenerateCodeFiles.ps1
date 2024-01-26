<#
.Synopsis
    Generates code for classes, forms, and content types stored in the database.
#>
$exitCode = 0

$scriptsPath = Get-Location
Set-Location -Path ../src/TrainingGuides.Web

# https://docs.xperience.io/xp/developers-and-admins/development/content-retrieval/generate-code-files-for-xperience-objects

#Reusable content types
dotnet run --no-build -- --kxp-codegen --type "ReusableContentTypes" --location "../TrainingGuides.Entities/{type}/{name}"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Reusable content type code generation failed."
    $exitCode = 1
}
else{
    Write-Host "Reusable content type code generation succeeded." -ForegroundColor Green
}
Write-Host

#Page content types
dotnet run --no-build -- --kxp-codegen --type "PageContentTypes" --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation

if ($LASTEXITCODE -ne 0) {
    Write-Error "Page content type code generation failed."
    $exitCode = 1
}
else{
    Write-Host "Page content type code generation succeeded." -ForegroundColor Green
}
Write-Host

#Custom module classes
dotnet run --no-build -- --kxp-codegen --type "Classes" --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation

if ($LASTEXITCODE -ne 0) {
    Write-Error "Class code generation failed."
    $exitCode = 1
}
else{
    Write-Host "Class code generation succeeded." -ForegroundColor Green
}
Write-Host

#Forms
dotnet run --no-build -- --kxp-codegen --type "Forms" --location "../TrainingGuides.Entities/{type}/{name}" --skip-confirmation

if ($LASTEXITCODE -ne 0) {
    Write-Error "Form code generation failed."
    $exitCode = 1
}
else{
    Write-Host "Form code generation succeeded." -ForegroundColor Green
}
Write-Host

Set-Location -Path $scriptsPath

if ($exitCode -ne 0) {
    Write-Error "Completed with errors. See above."
    Read-Host -Prompt "Press Enter to exit"
    exit $exitCode
}

Read-Host -Prompt "Press Enter to exit"