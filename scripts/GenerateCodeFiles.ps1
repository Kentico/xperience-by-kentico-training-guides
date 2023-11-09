<#
.Synopsis
    Generates code for classes and content types stored in the database.
#>
$scriptsPath = Get-Location
Set-Location -Path ../src/KBank.Web

# https://docs.xperience.io/xp/developers-and-admins/development/content-retrieval/generate-code-files-for-xperience-objects
dotnet run --no-build -- --kxp-codegen --type "All" --location "../KBank.Entities/{type}/{name}"

if ($LASTEXITCODE -ne 0) {
    Set-Location -Path $scriptsPath
    Write-Error "Code generation failed."
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}
else{
    Write-Host "Code files generated."
}

Set-Location -Path $scriptsPath

Read-Host -Prompt "Press Enter to exit"