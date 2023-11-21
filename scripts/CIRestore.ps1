<#
.Synopsis
    Restores objects serialized in the CI repository into the database.
#>
param (
	# Displays time elapsed for the restore operation including migrations
	[switch] $DisplayTimeElapsed
)

. .\Get-ConnectionString.ps1

$scriptsPath = Get-Location
$beforeList = "Before.txt"
$afterList = "After.txt"
$repositoryPath = "App_data\CIRepository"
$migrationFolder = "@migrations"

Set-Location -Path ../src/TrainingGuides.Web

$path = Get-Location

<#
.DESCRIPTION
   Runs a database migration with the given name
#>
function Run-Migration {
	param(
		[System.Data.SqlClient.SqlConnection] $Connection,
		[System.Data.SqlClient.SqlTransaction] $Transaction,
		[string] $MigrationName
	)
	
	$migrationPath = "$path\$repositoryPath\$migrationFolder\$MigrationName.sql"
	if (!(Test-Path $migrationPath)) {
		Write-Error "The file $migrationPath does not exist."
		return $FALSE
	}
	
	$sourceScript = Get-Content $migrationPath

	$sqlCommand = ""
	$sqlList = @()

	foreach ($line in $sourceScript) { 
		if ($line -imatch "^\s*GO\s*$") { 
			$sqlList += $sqlCommand
			$sqlCommand = ""
		}
		else { 			 
			$sqlCommand += $line + "`r`n" 
		}
	}
	
	$sqlList += $sqlCommand

	$rowsAffected = 0
	foreach ($sql in $sqlList) {
		if ([bool]$sql.Trim()) {
			$command = New-Object System.Data.SqlClient.SqlCommand($sql, $Connection)
			$command.Transaction = $Transaction

			try {
				$rowsAffectedInBatch = $command.ExecuteNonQuery()

				if ($rowsAffectedInBatch -gt 0) {
					$rowsAffected += $rowsAffectedInBatch
				}
			}
			catch {
				Write-Error $_.Exception.Message					
				return $FALSE
			}
		}
	}

	Log-RowsAffected -Connection $Connection -Transaction $Transaction -MigrationName $MigrationName -RowsAffected $rowsAffected

	return $TRUE
}


<#
.DESCRIPTION
   Logs rows affected by the migration.
#>
function Log-RowsAffected {
	param(
		[System.Data.SqlClient.SqlConnection] $Connection,
		[System.Data.SqlClient.SqlTransaction] $Transaction,
		[string] $MigrationName,
		[int] $RowsAffected
	)

	$logRowsAffectedQuery = "UPDATE CI_Migration SET RowsAffected = $RowsAffected WHERE MigrationName = '$MigrationName'"
	$logRowsAffectedCommand = New-Object System.Data.SqlClient.SqlCommand($logRowsAffectedQuery, $Connection)
	$logRowsAffectedCommand.Transaction = $Transaction

	try {
		$logRowsAffectedCommand.ExecuteNonQuery()
	}
	catch {
		Write-Host "Can't log rows affected: $_.Exception.Message"
	}
}

<#
.DESCRIPTION
   Checks if a migration with the given name was already applied. If not, the method returns false and the migration is marked as applied.
#>
function Check-Migration {
	param(
		[System.data.SqlClient.SQLConnection] $Connection,
		[System.Data.SqlClient.SqlTransaction] $Transaction,
		[string] $MigrationName
	)

	$sql = "DECLARE @migrate INT
			EXEC @migrate = Proc_CI_CheckMigration '$MigrationName'
			SELECT @migrate"

	$command = New-Object system.data.sqlclient.sqlcommand($sql, $Connection)
	$command.Transaction = $Transaction

	return $command.ExecuteScalar()
}


<#
.DESCRIPTION
   Runs all migrations in the migration list
#>
function Run-MigrationList {
	param(
		[string] $ConnectionString,
		[string] $MigrationList
	)

	$migrations = Get-Content "$path\$repositoryPath\$MigrationList"

	$connection = New-Object system.data.SqlClient.SQLConnection($ConnectionString)
	$connection.Open()
	foreach ($migrationName in $migrations) {
		$transaction = $connection.BeginTransaction("MigrationTransaction")

		if (Check-Migration -Connection $connection -Transaction $transaction -MigrationName $migrationName) {
			Write-Host "Applying migration '$migrationName'."
			if (!(Run-Migration -Connection $Connection -Transaction $transaction -MigrationName $migrationName)) {
				$transaction.Rollback()
				$connection.Close()
				return $FALSE
			}
		}

		$transaction.Commit()
	}

	$connection.Close()

	return $TRUE
}


<#
.DESCRIPTION
   Restores the repository to the database and executes migrations before and after the restore.
#>
function Run-Restore {
	param(
		[string] $Path
	)
	
	$connectionString = Get-ConnectionString -Path $Path	
	
	# Creates an 'App_Offline.htm' file to stop the website
	"<html><head></head><body>Continuous Integration restore in progress...</body></html>" > "$Path\App_Offline.htm"

	# Executes migration scripts before the restore
	if (!(Run-MigrationList $connectionString $beforeList)) {
		Write-Error "Database migrations before the restore failed."
        Read-Host -Prompt "Press Enter to exit"
		exit 1
	}
	
	$configuration = "Release";
	if (Test-Path (Join-Path $Path "bin\Debug"))
	{
		$configuration = "Debug";	
	}

	# Runs the restore CLI command
	dotnet run --project $Path --no-build -c "$configuration" -- --kxp-ci-restore
	if ($LASTEXITCODE -ne 0) {
		Write-Error "Restore failed."
        Read-Host -Prompt "Press Enter to exit"
		exit 1
	}

	# Executes migration scripts after the restore
	if (!(Run-MigrationList $connectionString $afterList)) {
		Write-Error "Database migrations after the restore failed."
        Read-Host -Prompt "Press Enter to exit"
		exit 1
	}

	# Removes the 'App_Offline.htm' file to bring the site back online
	Remove-Item "$Path\App_Offline.htm"	

	Write-Host "Done"
}

$sw = [System.Diagnostics.Stopwatch]::StartNew()

Run-Restore -Path $path

$sw.Stop()
if ($DisplayTimeElapsed) {
	Write-Host "Time Elapsed: $($sw.Elapsed)"
}


Set-Location -Path $scriptsPath

Read-Host -Prompt "Press Enter to exit"