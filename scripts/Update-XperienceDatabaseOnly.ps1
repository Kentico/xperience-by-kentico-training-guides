<#
.Synopsis
    Updates Xperience by Kentico to the version specified by the installed NuGet packages.
#>

$originalLocation = Get-Location
Set-Location -Path $PSScriptRoot

. .\Get-ConnectionString.ps1

function Handle-Error {
	param(
		[string] $Message
	)
	Set-Location -Path $originalLocation
    Write-Error $Message
    Read-Host -Prompt "Press Enter to exit"
    exit 1
}

#Query that executes a command without returning a dataset.
function Execute-SQL-Command {
    param(
        [string] $ConnectionString,
        [string] $CommandText
    )
    $connection = New-Object system.data.SqlClient.SQLConnection($ConnectionString)
    
    $connection.Open()
    $command = new-object system.data.sqlclient.sqlcommand($CommandText,$connection)
    $transaction = $connection.BeginTransaction()
    $command.Transaction = $transaction

    try {
        $rowsAffected = $command.ExecuteNonQuery()
        Write-Host 'Command: '$CommandText
        Write-Host 'Rows affected: '$rowsAffected
        $transaction.Commit()
    }
    catch {
        Write-Error $_.Exception.Message
        return $FALSE
    }    

    $connection.Close()

    return $TRUE
}

#Query that retrieves a data set
function Execute-SQL-Data-Query {
    param(
        [string] $ConnectionString,
        [string] $CommandText
    )
    $connection = New-Object System.Data.SqlClient.SQLConnection($ConnectionString)
    
    $connection.Open()

    $command = New-Object System.Data.SqlClient.SqlCommand($CommandText,$connection)
    $dataAdapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $dataset = new-object System.Data.Dataset
    $dataAdapter.Fill($dataset)

    $connection.Close()

    return $dataset
}

Set-Location -Path ..\src\TrainingGuides.Web

$appPath = Get-Location

$connectionString = Get-ConnectionString -Path $appPath -OriginalLocation $originalLocation

$resultDataSet = Execute-SQL-Data-Query -ConnectionString $connectionString -CommandText "SELECT KeyValue FROM CMS_SettingsKey WHERE KeyName = N'CMSEnableCI'"

$isUsingCD = $resultDataSet.Tables[0].Rows[0][0]

$readyToUpdate = $True

#Since the settings key value is a string and could theoretically be something other than true or false, compare the value rather than treating it as a boolean expression on its own
if($isUsingCD -eq 'True'){
    Write-Host 'Disabling continuous integration'
    $commandResult = Execute-SQL-Command -ConnectionString $connectionString -CommandText "UPDATE CMS_SettingsKey SET KeyValue = N'False' WHERE KeyName = N'CMSEnableCI'"
    $readyToUpdate = $commandResult
}

if($readyToUpdate){
    Write-Host 'Starting Xperience update'

    dotnet run --no-build --kxp-update

    if ($LASTEXITCODE -ne 0) {
        Handle-Error "Update failed."
    }
}
else{
    Handle-Error 'Unable to disable continuous integration to perform the update.'
}

if($isUsingCD -eq 'True'){
    Write-Host 'Re-enabling continuous integration'

    $commandResult = Execute-SQL-Command -ConnectionString $connectionString -CommandText "UPDATE CMS_SettingsKey SET KeyValue = N'True' WHERE KeyName = N'CMSEnableCI'"    
    
    if(-not $commandResult){
        Handle-Error 'Unable to re-enable continuous integration.'
    }

    dotnet run --kxp-ci-store

    if($LASTEXITCODE -ne 0) {
        Handle-Error 'Unable to store continuous integration. Make sure to run the store operation after fixing any issues.'
    }
}

Set-Location -Path $originalLocation

Read-Host -Prompt "Press Enter to exit"