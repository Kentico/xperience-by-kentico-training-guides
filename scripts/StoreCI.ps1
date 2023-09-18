<#
.Synopsis
    Serializes database data to the continuous integration repository.
#>

$scriptsPath = Get-Location

cd ../src/KBank.Web

Write-Host 'Storing CI files'

dotnet run --no-build --kxp-ci-store

if ($LASTEXITCODE -ne 0) {
    Write-Error "CI store failed."
    Read-Host -Prompt "Press any key to exit"
    exit 1
}
else{
    Write-Host 'CI files stored'
}

Set-Location -Path $scriptsPath

Read-Host -Prompt "Press any key to exit"