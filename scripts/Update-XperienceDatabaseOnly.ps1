<#
.Synopsis
    Updates Xperience by Kentico to the version specified by the installed NuGet packages.
.Description
    Requires Xperience by Kentico refresh 31.6.0 or newer, which introduced the
    '--kxp-ci-disable' and '--kxp-ci-enable' CLI commands.
#>
param (
	# Skips the database backup prompt of the update command, so that the script can run unattended
	[switch] $SkipConfirmation
)

$originalLocation = Get-Location
Set-Location -Path $PSScriptRoot

function Handle-Error {
	param(
		[string] $Message
	)
	Set-Location -Path $originalLocation
    Write-Error $Message
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}

<#
.DESCRIPTION
   Runs one of the continuous integration state commands of the .NET CLI and returns its result.
   Returns $null if the command fails.
#>
function Invoke-CI-State-Command {
    param(
        [string] $Option
    )

    #The '--format json' option makes the command print its result as a single line of JSON
    $output = dotnet run --no-build -- $Option --format json

    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    #'dotnet run' also prints build output, so pick out the line that holds the JSON result
    $json = $output | Where-Object { $_.Trim().StartsWith('{') } | Select-Object -Last 1

    if ([string]::IsNullOrWhiteSpace($json)) {
        return $null
    }

    return $json | ConvertFrom-Json
}

Set-Location -Path ..\src\TrainingGuides.Web

Write-Host 'Disabling continuous integration'

$result = Invoke-CI-State-Command '--kxp-ci-disable'

if ($null -eq $result -or -not $result.success) {
    Handle-Error 'Unable to disable continuous integration to perform the update.'
}

Write-Host $result.message

#The 'changed' value is false when continuous integration was already disabled, in which case it must stay disabled after the update
$isUsingCI = $result.changed

Write-Host 'Starting Xperience update'

#Without the '--skip-confirmation' option, the update command waits for a keypress to confirm the database backup prompt
if($SkipConfirmation){
    dotnet run --no-build -- --kxp-update --skip-confirmation
}
else{
    dotnet run --no-build --kxp-update
}

if ($LASTEXITCODE -ne 0) {
    #Leave continuous integration in the state it was in before the update
    if($isUsingCI){
        Write-Host 'Re-enabling continuous integration after the failed update'

        Invoke-CI-State-Command '--kxp-ci-enable' | Out-Null
    }

    Handle-Error "Update failed."
}

if($isUsingCI){
    Write-Host 'Re-enabling continuous integration'

    $result = Invoke-CI-State-Command '--kxp-ci-enable'

    if ($null -eq $result -or -not $result.success) {
        Handle-Error 'Unable to re-enable continuous integration.'
    }

    dotnet run --kxp-ci-store

    if($LASTEXITCODE -ne 0) {
        Handle-Error 'Unable to store continuous integration. Make sure to run the store operation after fixing any issues.'
    }
}

Set-Location -Path $originalLocation

Read-Host -Prompt "Press Enter to exit"