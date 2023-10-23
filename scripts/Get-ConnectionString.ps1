<#
.Synopsis
    Contains functions for use in other scripts
#>

<#
.DESCRIPTION
   Gets the database connection string from the config file
#>
function Get-ConnectionString {
    param(
        [string] $Path
    )

    # Try to get the connection string from user secrets first
    $connectionString = dotnet user-secrets list --project $Path `
        | Select-String -Pattern "ConnectionStrings:" `
        | ForEach-Object { $_.Line -replace '^ConnectionStrings:CMSConnectionString \= ','' }        

    if (-not [string]::IsNullOrEmpty($connectionString)) {
        Write-Host 'Using ConnectionString from user-secrets'

        return $connectionString
    }

    Write-Host 'Unable to find connection string in user secrets.'

    $appSettingsFileNames = 'appSettings.json'
    
    foreach ($appSettingFileName in $appSettingsFileNames)
    {
        $jsonFilePath = Join-Path $Path $appSettingFileName
        
        if (Test-Path $jsonFilePath)
        {
            $appSettingsJson = Get-Content $jsonFilePath | Out-String | ConvertFrom-Json
            $connectionString = $appSettingsJson.ConnectionStrings.CMSConnectionString;
            
            if ($connectionString)
            {
                Write-Host "Using ConnectionString from $appSettingFileName"

                return $connectionString;
            }
        }
    }
    
    Write-Error "Connection string not found."
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}