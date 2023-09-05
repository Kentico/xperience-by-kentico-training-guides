<#
.Synopsis
    Creates a deployment package.
#>
    [CmdletBinding()]
param ([switch]$KeepProductVersion)

$scriptsPath = Get-Location
$outputFolderPath = "./bin/Deployment/"
$buildNumber = (Get-Date).ToUniversalTime().ToString("yyyyMMddHHmm")

cd ../src/KBank.Web

# Publish the application in the 'Release' mode
$publishCommand = "dotnet publish --nologo -c Release --self-contained true --runtime win-x64 -o $OutputFolderPath"

if (!$KeepProductVersion) {
    $publishCommand += " --version-suffix $buildNumber"
}

echo $publishCommand

Invoke-Expression $publishCommand

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publishing the website failed."
    exit 1
}

Set-Location -Path $scriptsPath
Read-Host -Prompt "Press any key to exit"