<#
.Synopsis
    Serializes database data to the continuous integration repository.
#>

$originalLocation = Get-Location
Set-Location -Path $PSScriptRoot/../src/TrainingGuides.Web

Write-Host 'Storing CI files'

dotnet run --no-build --kxp-ci-store

if ($LASTEXITCODE -ne 0) {
    Set-Location -Path $originalLocation
    Write-Error "CI store failed."
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}
else{
    Write-Host 'CI files stored'
}

Set-Location -Path $originalLocation

Read-Host -Prompt "Press Enter to exit"