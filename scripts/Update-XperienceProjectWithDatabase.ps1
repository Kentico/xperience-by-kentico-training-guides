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
   ✓ Xperience by Kentico refresh 31.6.0 or newer (for the CI state CLI commands)
   ✓ SQL Server with your project database
   ✓ PowerShell 5.1 or later
   ✓ Project structure: ProjectRoot/src/ and ProjectRoot/scripts/

2. CONFIGURATION REQUIRED:
   ✓ Update launch profile names if different (line ~180)
   ✓ Update project file name if different (line ~90)
   ✓ Verify package list matches your project needs (line ~100)

3. HOW TO RUN:
   > cd C:\YourProject\scripts
   > .\Update-XperienceProjectWithDatabase.ps1

   Add the -SkipConfirmation switch to run unattended (e.g. in a pipeline):
   > .\Update-XperienceProjectWithDatabase.ps1 -SkipConfirmation

4. WHAT IT DOES:
   • Temporarily disables CI mode via the .NET CLI (if it is enabled)
   • Updates all Xperience NuGet packages to their latest versions
   • Runs Xperience update process to sync database schema
   • Re-enables CI mode
   • Provides detailed progress feedback

   Note the order: CI mode is disabled BEFORE the packages are updated. The CI state
   commands start the application, and the application refuses to start once the package
   version and the database version differ.

5. TROUBLESHOOTING:
   • If project not found: Verify folder structure and project file name
   • If update fails: Check .NET SDK is installed and project builds
   • If CI commands are not recognized: Verify your project uses refresh 31.6.0 or newer
   • If a CI command reports "The database version ... does not match the project version ...":
     the packages were updated while CI mode was still enabled. Roll the package versions back,
     let the script disable CI mode first, or disable it in the administration before updating.

===============================================================================
#>

<#
.SYNOPSIS
    Updates Xperience by Kentico NuGet packages and synchronizes the local database schema.

.DESCRIPTION
    This script automates the process of updating Xperience by Kentico NuGet packages to their latest versions
    and then updates the local database schema to match the updated packages. It performs the following steps:
    
    1. Temporarily disables CI mode using the '--kxp-ci-disable' CLI command
    2. Updates all specified Xperience by Kentico NuGet packages to their latest versions
    3. Runs the Xperience update process to synchronize database schema
    4. Re-enables CI mode using the '--kxp-ci-enable' CLI command

.NOTES
    - This script must be run from the scripts folder
    - Requires Xperience by Kentico refresh 31.6.0 or newer, which introduced the
      '--kxp-ci-disable' and '--kxp-ci-enable' CLI commands
    - CI mode is disabled before the NuGet packages are updated. The CI state commands run through
      'dotnet run', which starts the application, and the application refuses to start when the
      package version and the database version differ. Once the packages are updated, the CI state
      can no longer be changed through the CLI or the administration until the database is updated.
    - The Xperience project must be in the ../src folder relative to this script
    - Requires dotnet CLI to be installed and available in PATH

.PARAMETER SkipConfirmation
    Skips the database backup prompt of the update command, so that the script can run unattended.
    Without it, the update command waits for a keypress and fails if console input is redirected.

.EXAMPLE
    PS C:\dev\YourProject\scripts> .\Update-XperienceProjectWithDatabase.ps1
    
    Runs the update process for the Xperience by Kentico project.

.EXAMPLE
    PS C:\dev\YourProject\scripts> .\Update-XperienceProjectWithDatabase.ps1 -SkipConfirmation

    Runs the update process without prompting for confirmation of the database backup.

.CONFIGURATION
    Before running, ensure the following are configured correctly:
    - Package list in $xperiencePackages array
    - Launch profiles match your project's launchSettings.json
#>
param (
    # Skips the database backup prompt of the update command, so that the script can run unattended
    [switch] $SkipConfirmation
)

#region Configuration - UPDATE THESE VALUES FOR YOUR PROJECT
<#
===============================================================================
                           CONFIGURATION SECTION
===============================================================================
Update the values in this section to match your project setup.
All TODOs from throughout the script have been consolidated here.
===============================================================================
#>

# 1. PROJECT CONFIGURATION
# Update these paths to match your project structure
$SCRIPT_CONFIG_PROJECT_FOLDER = "src/YourProject.Web"           # Folder containing your .csproj file (relative to the script parent directory, including the parent directory name)
$SCRIPT_CONFIG_PROJECT_FILE = "YourProject.Web.csproj"   # Name of your .csproj file

# 2. LAUNCH PROFILES
# Update these profile names to match your project's launchSettings.json
$SCRIPT_CONFIG_CI_LAUNCH_PROFILE = "YourProject.WebCI"    # Launch profile for CI environment
$SCRIPT_CONFIG_DEV_LAUNCH_PROFILE = "YourProject.Web"     # Launch profile for development environment

# 3. NUGET PACKAGES
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

#region Continuous Integration Functions
<#
.DESCRIPTION
    Runs one of the continuous integration state commands of the .NET CLI and returns its result.

    CI mode affects how Xperience handles continuous integration scenarios.
    It's temporarily disabled during updates to prevent conflicts.

    Requires Xperience by Kentico refresh 31.6.0 or newer.

.PARAMETER Option
    The CLI option to run: '--kxp-ci-disable' or '--kxp-ci-enable'

.PARAMETER ProjectFile
    Path to the .csproj file of the Xperience project

.PARAMETER LaunchProfile
    Launch profile to run the command with

.PARAMETER Configuration
    Build configuration to run the command with
#>
function Invoke-CIStateCommand {
    param(
        [string] $Option,
        [string] $ProjectFile,
        [string] $LaunchProfile,
        [string] $Configuration
    )

    # The '--format json' option makes the command print its result as a single line of JSON
    $command = "dotnet run " + `
        "--project `"$ProjectFile`" " + `
        "--launch-profile $LaunchProfile " + `
        "-c $Configuration " + `
        "-- $Option --format json"

    Write-Host "Executing: $command" -ForegroundColor Yellow

    $output = Invoke-Expression $command

    if ($LASTEXITCODE -ne 0) {
        throw "Command '$Option' failed with exit code $LASTEXITCODE"
    }

    # 'dotnet run' also prints build output, so pick out the line that holds the JSON result
    $json = $output | Where-Object { $_.Trim().StartsWith('{') } | Select-Object -Last 1

    if ([string]::IsNullOrWhiteSpace($json)) {
        throw "Command '$Option' did not return a JSON result."
    }

    $result = $json | ConvertFrom-Json

    if (-not $result.success) {
        throw "Command '$Option' did not succeed: $($result.message)"
    }

    Write-Notification $result.message

    return $result
}

<#
.DESCRIPTION
    Re-enables CI mode if it was enabled before the update, without masking an earlier failure.

    Once the NuGet packages are updated, the CI state commands only work again after the database
    has been updated to match. If the update itself failed, re-enabling CI mode is therefore
    expected to fail too, so this reports the problem instead of throwing over the original error.

.PARAMETER WasEnabled
    Whether CI mode was enabled before the update started

.PARAMETER ProjectFile
    Path to the .csproj file of the Xperience project

.PARAMETER LaunchProfile
    Launch profile to run the command with

.PARAMETER Configuration
    Build configuration to run the command with
#>
function Restore-CIState {
    param(
        [bool] $WasEnabled,
        [string] $ProjectFile,
        [string] $LaunchProfile,
        [string] $Configuration
    )

    if (-not $WasEnabled) {
        return
    }

    try {
        Invoke-CIStateCommand '--kxp-ci-enable' $ProjectFile $LaunchProfile $Configuration | Out-Null
    }
    catch {
        Write-Error "Could not re-enable CI mode: $($_.Exception.Message)"
        Write-Error "Re-enable it manually once the database version matches the package version again."
    }
}
#endregion

#region Main Update Process
<#
.DESCRIPTION
    Main execution block that orchestrates the Xperience by Kentico update process.
    
    The process follows these steps:
    1. Determine the appropriate launch profile and build configuration
    2. Disable CI mode, noting whether it was enabled beforehand
    3. Update the Xperience by Kentico NuGet packages
    4. Run the Xperience update command to bring the database up to the new package version
    5. Re-enable CI mode if it was enabled before the update, including when the update fails

    Why CI mode is disabled before the packages are updated:
    The CI state commands run through 'dotnet run', which starts the application, and the
    application refuses to start when the package version and the database version differ.
    Updating the packages first would therefore make '--kxp-ci-disable' fail with
    "The database version ... does not match the project version ...".
    
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

# Step 1: Disable CI mode before anything changes the package version.
# The CI state commands start the application, which refuses to start once the package version and
# the database version differ, so this must happen before the NuGet packages are updated.
Write-Status "Disabling CI mode for update process..."
$result = Invoke-CIStateCommand '--kxp-ci-disable' $projectFile $launchProfile $configuration

# The 'changed' value is false when CI mode was already disabled, in which case it must stay disabled after the update
$isUsingCI = $result.changed

# Step 2: Update the Xperience by Kentico NuGet packages
#region NuGet Package Updates
<#
.DESCRIPTION
    Updates all Xperience by Kentico NuGet packages to their latest stable versions.
    
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

# Update each package to the latest stable version
foreach ($pkg in $xperiencePackages) {
    Write-Status "Updating NuGet package: $pkg"
    $updateCmd = "dotnet add `"$projectFile`" package $pkg"
    try {
        Invoke-ExpressionWithException $updateCmd
        Write-Notification "Updated $pkg to latest version."
    }
    catch {
        Write-Error "Failed to update NuGet package ${pkg}: $($_.Exception.Message)"
    }
}
#endregion

# Step 3: Execute the Xperience update command
Write-Status "Running Xperience update process..."
$command = "dotnet run " + `
    "--project `"$projectFile`" " + `
    "--launch-profile $launchProfile " + `
    "-c $configuration " + `
    "-- --kxp-update"

# Without the '--skip-confirmation' option, the update command waits for a keypress to confirm the database backup prompt
if ($SkipConfirmation) {
    $command += " --skip-confirmation"
}

try {
    Invoke-ExpressionWithException $command
}
catch {
    # Leave CI mode in the state it was in before the update
    if ($isUsingCI) {
        Write-Status "Re-enabling CI mode after the failed update..."
        Restore-CIState $isUsingCI $projectFile $launchProfile $configuration
    }
    throw
}

# Step 4: Re-enable CI mode if it was enabled before the update
if ($isUsingCI) {
    Write-Status "Re-enabling CI mode..."
    Invoke-CIStateCommand '--kxp-ci-enable' $projectFile $launchProfile $configuration | Out-Null
}

Write-Host "`n"
Write-Status "Update Complete"

# Optional: Extend the script to run additional CI operations
# Uncomment the line below if you have a Store-CI.ps1 script to run
# Write-Status "Running CI store operations..."
# For inspiration: "https://github.com/Kentico/community-portal/blob/main/scripts/Store-CI.ps1"
# & (Join-Path $PSScriptRoot "Store-CI.ps1")
#endregion
