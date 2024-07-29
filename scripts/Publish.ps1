<#
.Synopsis
    Creates a deployment package.
#>
    [CmdletBinding()]
param ([switch]$KeepProductVersion)

$originalLocation = Get-Location
Set-Location -Path $PSScriptRoot

$outputFolderPath = "./bin/Deployment/"
$buildNumber = (Get-Date).ToUniversalTime().ToString("yyyyMMddHHmm")

Set-Location -Path ../src/TrainingGuides.Web

# Publish the application in the 'Release' mode
$publishCommand = "dotnet publish --nologo -c Release --self-contained true --runtime win-x64 -o $OutputFolderPath"

if (!$KeepProductVersion) {
    $publishCommand += " --version-suffix $buildNumber"
}

Write-Host $publishCommand

Invoke-Expression $publishCommand

if ($LASTEXITCODE -ne 0) {
    Set-Location -Path $originalLocation
    Write-Error "Publishing the website failed."
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}

Set-Location -Path $originalLocation
Read-Host -Prompt "Press Enter to exit"