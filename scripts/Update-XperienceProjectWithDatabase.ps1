<#
===============================================================================
                    XPERIENCE BY KENTICO UPDATE SAMPLE SCRIPT
===============================================================================

OVERVIEW:
This script automates the process of updating Xperience by Kentico NuGet
packages and synchronizing your local database schema to match the updated
package versions. It is built on top of Xperience-Update.ps1 script in the Community Portal  
"https://github.com/Kentico/community-portal/blob/main/scripts/Update-Xperience.ps1" 
to include updating Xperience NuGet Packages.

SETUP INSTRUCTIONS FOR NEW DEVELOPERS:

1. PREREQUISITES:
   ✓ .NET SDK installed (check with: dotnet --version)
   ✓ SQL Server with your project database
   ✓ PowerShell 5.1 or later
   ✓ Project structure: ProjectRoot/src/ and ProjectRoot/Utilities/

2. CONFIGURATION REQUIRED:
   ✓ Update the connection string in Get-ConnectionString function (line ~55)
   ✓ Update launch profile names if different (line ~207)
   ✓ Update project file name if different (line ~118)
   ✓ Verify package list matches your project needs (line ~135)

3. HOW TO RUN:
   > cd C:\YourProject\Utilities
   > .\Update-XperienceProjectWithDatabase.ps1

4. WHAT IT DOES:
   • Updates all Xperience NuGet packages to latest prerelease versions
   • Temporarily disables CI mode in database
   • Runs Xperience update process to sync database schema
   • Re-enables CI mode
   • Provides detailed progress feedback

5. TROUBLESHOOTING:
   • If connection fails: Check your database connection string
   • If project not found: Verify folder structure and project file name
   • If update fails: Check .NET SDK is installed and project builds
   • If CI mode errors: Verify you have admin rights to the database

===============================================================================
#>

<#
.SYNOPSIS
    Updates Xperience by Kentico NuGet packages and synchronizes the local database schema.

.DESCRIPTION
    This script automates the process of updating Xperience by Kentico NuGet packages to their latest versions
    and then updates the local database schema to match the updated packages. It performs the following steps:
    
    1. Updates all specified Xperience by Kentico NuGet packages to their latest prerelease versions
    2. Temporarily disables CI mode in the database
    3. Runs the Xperience update process to synchronize database schema
    4. Re-enables CI mode in the database

.NOTES
    - This script must be run from the Utilities folder
    - Requires a valid SQL Server connection to the project's database
    - The Xperience project must be in the ../src folder relative to this script
    - Requires dotnet CLI to be installed and available in PATH

.PARAMETER None
    This script does not accept parameters. Configuration is done by modifying variables within the script.

.EXAMPLE
    PS C:\dev\YourProject\Utilities> .\Update-XperienceProjectWithDatabase.ps1
    
    Runs the update process for the Xperience by Kentico project.

.CONFIGURATION
    Before running, ensure the following are configured correctly:
    - Database connection string in Get-ConnectionString function
    - Package list in $xperiencePackages array
    - Launch profiles match your project's launchSettings.json
#>

#region Configuration - UPDATE THESE VALUES FOR YOUR PROJECT
<#
===============================================================================
                           CONFIGURATION SECTION
===============================================================================
Update the values in this section to match your project setup.
All TODOs from throughout the script have been consolidated here.
===============================================================================
#>

# 1. DATABASE CONNECTION STRING
# Update this connection string to match your database configuration
$SCRIPT_CONFIG_CONNECTION_STRING = "Server=your_server_name;Database=your_database_name;Integrated Security=true;TrustServerCertificate=true"
<# WARNING: Do not directly copy-paste the connection string from your .NET project’s `appsettings.json` file. The format should be one of the following common formats:
   - Local SQL Server: "Server=.;Database=YourDbName;Integrated Security=true;TrustServerCertificate=true"
   - SQL Server with credentials: "Server=ServerName;Database=YourDbName;User Id=username;Password=password;TrustServerCertificate=true"
   - SQL Express: "Server=.\SQLEXPRESS;Database=YourDbName;Integrated Security=true;TrustServerCertificate=true"
#>

# 2. PROJECT CONFIGURATION
# Update these paths to match your project structure
$SCRIPT_CONFIG_PROJECT_FOLDER = "YourProjectFolder"           # Folder containing your .csproj file (relative to the script parent directory, including the parent directory name)
$SCRIPT_CONFIG_PROJECT_FILE = "YourProject.csproj"   # Name of your .csproj file

# 3. LAUNCH PROFILES
# Update these profile names to match your project's launchSettings.json
$SCRIPT_CONFIG_CI_LAUNCH_PROFILE = "YourProject.WebCI"    # Launch profile for CI environment
$SCRIPT_CONFIG_DEV_LAUNCH_PROFILE = "YourProject.Web"     # Launch profile for development environment

# 4. NUGET PACKAGES
# Update this list based on your project's specific package requirements
$SCRIPT_CONFIG_XPERIENCE_PACKAGES = @(
    "kentico.xperience.admin",           # Administration interface
    "kentico.xperience.azurestorage",    # Azure Blob Storage integration
    "kentico.xperience.imageprocessing", # Image processing capabilities
    "kentico.xperience.mjml",            # MJML email template support
    "kentico.xperience.webapp"           # Core web application functionality
)

<#
===============================================================================
                        END CONFIGURATION SECTION
===============================================================================
#>
#endregion

#region Helper Functions
<#
.DESCRIPTION
    Executes a command and throws an exception if it fails (non-zero exit code).
    Provides visual feedback by displaying the command being executed.
#>
function Invoke-ExpressionWithException {
    param([string]$Command)
    Write-Host "Executing: $Command" -ForegroundColor Yellow
    Invoke-Expression $Command
    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code $LASTEXITCODE"
    }
}

<#
.DESCRIPTION
    Returns the database connection string for the project.
    IMPORTANT: Update this connection string to match your database configuration.
#>
function Get-ConnectionString {
    return $SCRIPT_CONFIG_CONNECTION_STRING
}

<#
.DESCRIPTION
    Displays a status message in green color for major operation updates.
#>
function Write-Status {
    param([string]$Message)
    Write-Host $Message -ForegroundColor Green
}

<#
.DESCRIPTION
    Displays a notification message in cyan color for successful operations.
#>
function Write-Notification {
    param([string]$Message)
    Write-Host $Message -ForegroundColor Cyan
}

<#
.DESCRIPTION
    Displays an error message in red color for failed operations.
#>
function Write-Error {
    param([string]$Message)
    Write-Host $Message -ForegroundColor Red
}
#endregion

#region Project Path Configuration
<#
.DESCRIPTION
    Sets up the project paths relative to the script location.
    This assumes the following folder structure:
    
    ProjectRoot/
    ├── Utilities/          (This script's location)
    │   └── Update-XperienceProjectWithDatabase.ps1
    └── src/               (Project location)
        └── YourProject.csproj   (Main project file)
#>
Write-Status "Configuring project paths..."

# Set project path using configuration variables
$projectPath = Join-Path (Split-Path $PSScriptRoot -Parent) $SCRIPT_CONFIG_PROJECT_FOLDER
$projectFile = Join-Path $projectPath $SCRIPT_CONFIG_PROJECT_FILE

# Validate that the project file exists
if (!(Test-Path $projectFile)) {
    throw "Project file not found at: $projectFile"
}
Write-Notification "Project file validated: $projectFile"
#endregion

#region NuGet Package Updates
<#
.DESCRIPTION
    Updates all Xperience by Kentico NuGet packages to their latest prerelease versions.
    
    The package list can be customized by modifying the $xperiencePackages array below.
    Add or remove packages as needed for your specific project requirements.
    
    Common Xperience by Kentico packages:
    - kentico.xperience.admin: Administration interface
    - kentico.xperience.webapp: Core web application functionality
    - kentico.xperience.imageprocessing: Image processing capabilities
    - kentico.xperience.azurestorage: Azure Blob Storage integration
    - kentico.xperience.mjml: MJML email template support
#>
Write-Status "Checking for latest Xperience by Kentico NuGet packages..."

# Use the configured package list
$xperiencePackages = $SCRIPT_CONFIG_XPERIENCE_PACKAGES

# Update each package to the latest prerelease version
foreach ($pkg in $xperiencePackages) {
    Write-Status "Updating NuGet package: $pkg"
    $updateCmd = "dotnet add `"$projectFile`" package $pkg --prerelease"
    try {
        Invoke-ExpressionWithException $updateCmd
        Write-Notification "Updated $pkg to latest version."
    }
    catch {
        Write-Error "Failed to update NuGet package ${pkg}: $($_.Exception.Message)"
    }
}
#endregion

#region Database Configuration Functions
<#
.DESCRIPTION
    Sets the CMSEnableCI settings key in the database to enable or disable CI mode.
    
    CI mode affects how Xperience handles continuous integration scenarios.
    It's temporarily disabled during updates to prevent conflicts.
    
.PARAMETER Connection
    Active SQL connection to the database
    
.PARAMETER Value
    Should be 'True' to enable CI mode or 'False' to disable it
#>
function Write-CMSEnableCI {
    param(
        [System.Data.SqlClient.SqlConnection] $Connection,
        [string] $Value
    )

    $updateQuery = "UPDATE CMS_SettingsKey SET KeyValue = N'$Value' WHERE KeyName = N'CMSEnableCI'"
    $updateCommand = New-Object System.Data.SqlClient.SqlCommand($updateQuery, $Connection)

    try {
        $result = $updateCommand.ExecuteNonQuery()
        if ($result -eq 0) {
            throw "CMS_SettingsKey update did not affect any rows."
        }
        elseif ($result -eq 1) {
            Write-Notification "CMSEnableCI set to $Value"
        }
    }
    catch {
        Write-Error "Can't update Settings Key CMSEnableCI: $_.Exception.Message"
    }
}
#endregion

#region Main Update Process
<#
.DESCRIPTION
    Main execution block that orchestrates the Xperience by Kentico update process.
    
    The process follows these steps:
    1. Determine the appropriate launch profile and build configuration
    2. Connect to the database
    3. Disable CI mode to prevent conflicts during update
    4. Run the Xperience update command
    5. Re-enable CI mode
    6. Clean up database connection
    
    Environment Configuration:
    - If ASPNETCORE_ENVIRONMENT = "CI": Uses "YourProject.WebCI" profile with Release configuration
    - Otherwise: Uses "YourProject.Web" profile with Debug configuration
    
    Note: Ensure your project's launchSettings.json contains the referenced launch profiles.
#>

# Determine launch profile and configuration based on environment
# Uses if-else approach to support PowerShell 5.1+.
if ($Env:ASPNETCORE_ENVIRONMENT -eq "CI") {
    $launchProfile = $SCRIPT_CONFIG_CI_LAUNCH_PROFILE
    $configuration = "Release"
}
else {
    $launchProfile = $SCRIPT_CONFIG_DEV_LAUNCH_PROFILE
    $configuration = "Debug"
}

Write-Status "Using launch profile: $launchProfile with configuration: $configuration"
Write-Status "Begin Xperience Update"
Write-Host "`n"

# Establish database connection
try {
    $connection = New-Object system.data.SqlClient.SQLConnection(Get-ConnectionString)
    $connection.Open()
    Write-Notification "Database connection established"
}
catch {
    Write-Error "Failed to connect to database: $($_.Exception.Message)"
    throw
}

try {
    # Step 1: Disable CI mode to prevent conflicts during update
    Write-Status "Disabling CI mode for update process..."
    Write-CMSEnableCI $connection 'False'

    # Step 2: Execute the Xperience update command
    Write-Status "Running Xperience update process..."
    $command = "dotnet run " + `
        "--project `"$projectFile`" " + `
        "--launch-profile $launchProfile " + `
        "-c $configuration " + `
        "--kxp-update"

    Invoke-ExpressionWithException $command

    # Step 3: Re-enable CI mode
    Write-Status "Re-enabling CI mode..."
    Write-CMSEnableCI $connection 'True'
}
finally {
    # Always close the database connection
    if ($connection.State -eq 'Open') {
        $connection.Close()
        Write-Notification "Database connection closed"
    }
}

Write-Host "`n"
Write-Status "Update Complete"

# Optional: Extend the script to run additional CI operations
# Uncomment the line below if you have a Store-CI.ps1 script to run
# Write-Status "Running CI store operations..."
# For inspiration: "https://github.com/Kentico/community-portal/blob/main/scripts/Store-CI.ps1"
# & (Join-Path $PSScriptRoot "Store-CI.ps1")
#endregion