<#
.Synopsis
    Serializes database data to the continuous integration repository.
#>

$scriptsPath = Get-Location

Set-Location -Path ../src/KBank.Web

Write-Host 'Storing CI files'

dotnet run --no-build --kxp-ci-store

if ($LASTEXITCODE -ne 0) {
    Set-Location -Path $scriptsPath
    Write-Error "CI store failed."
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}
else{
    Write-Host 'CI files stored'
}

Set-Location -Path $scriptsPath

Read-Host -Prompt "Press Enter to exit"