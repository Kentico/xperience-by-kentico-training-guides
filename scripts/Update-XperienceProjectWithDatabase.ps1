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
   ✓ A working database connection for the project, through the 'CMSConnectionString'
     connection string in appsettings.json, user secrets, or an environment variable.
     Every step of this script starts the application, which cannot run without it.
   ✓ PowerShell 5.1 or later
   ✓ Project structure: ProjectRoot/src/ and ProjectRoot/scripts/

2. CONFIGURATION REQUIRED:
   ✓ Update launch profile names if different (line ~180)
   ✓ Update project file name if different (line ~90)
   ✓ Verify package list matches your project needs (line ~100)

3. HOW TO RUN:
   > cd C:\YourProject\scripts
   > .\Update-XperienceProjectWithDatabase.ps1 -Version 31.7.3

   The -Version parameter is required. To find the latest available version, run:
   > dotnet list .\src\YourProject.Web\YourProject.Web.csproj package --outdated

   Add the -SkipConfirmation switch to run unattended (e.g. in a pipeline):
   > .\Update-XperienceProjectWithDatabase.ps1 -Version 31.7.3 -SkipConfirmation

4. WHAT IT DOES:
   • Temporarily disables CI mode via the .NET CLI (if it is enabled)
   • Updates all Xperience NuGet packages to the specified version
   • Runs Xperience update process to sync database schema
   • Re-enables CI mode
   • Provides detailed progress feedback

   Note the order: CI mode is disabled BEFORE the packages are updated. The CI state
   commands start the application, and the application refuses to start once the package
   version and the database version differ.

5. TROUBLESHOOTING:
   • If project not found: Verify folder structure and project file name
   • If update fails: Check .NET SDK is installed and project builds
   • If a command reports "Cannot access the database specified by the 'CMSConnectionString'
     connection string": set the connection string in the appsettings.json file of your project
   • If CI commands are not recognized: Verify your project uses refresh 31.6.0 or newer
   • If your project uses Central Package Management and duplicate 'PackageVersion' entries
     appear (warning NU1506): the package IDs in Directory.Packages.props do not use the
     casing published on NuGet. Correct the casing, for example 'kentico.xperience.webapp'
     to 'Kentico.Xperience.WebApp'.
   • If a CI command reports "The database version ... does not match the project version ...":
     the packages were updated while CI mode was still enabled. Roll the package versions back,
     let the script disable CI mode first, or disable it in the administration before updating.
   • If the build fails with "Detected package downgrade" (error NU1605) after the package step:
     an Xperience package used in your solution is missing from the package list in the
     configuration section, so it stayed on the old version. Note that CI mode is disabled at
     this point and cannot be re-enabled until the versions match again. To recover, revert the
     package version changes, add the missing package to the list, and run the script again.

===============================================================================
#>

<#
.SYNOPSIS
    Updates Xperience by Kentico NuGet packages and synchronizes the local database schema.

.DESCRIPTION
    This script automates the process of updating Xperience by Kentico NuGet packages to a specified
    version and then updates the local database schema to match. It performs the following steps:
    
    1. Temporarily disables CI mode using the '--kxp-ci-disable' CLI command
    2. Updates all specified Xperience by Kentico NuGet packages to the version given by 'Version'
    3. Runs the Xperience update process to synchronize database schema
    4. Re-enables CI mode using the '--kxp-ci-enable' CLI command

.NOTES
    - This script must be run from the scripts folder
    - Requires a working database connection for the project ('CMSConnectionString'), because every
      step starts the application through 'dotnet run'
    - Requires Xperience by Kentico refresh 31.6.0 or newer, which introduced the
      '--kxp-ci-disable' and '--kxp-ci-enable' CLI commands
    - CI mode is disabled before the NuGet packages are updated. The CI state commands run through
      'dotnet run', which starts the application, and the application refuses to start when the
      package version and the database version differ. Once the packages are updated, the CI state
      can no longer be changed through the CLI or the administration until the database is updated.
    - The Xperience project must be in the ../src folder relative to this script
    - Requires dotnet CLI to be installed and available in PATH

.PARAMETER Version
    Version to update the Xperience by Kentico packages to, for example '31.7.3'.

    The version is required so that every run is reproducible, and because
    projects that use Central Package Management pin their versions in a 'Directory.Packages.props'
    file, which 'dotnet add package' does not raise on its own. Passing an explicit version works
    for both project layouts.

    To find the latest available version, run 'dotnet list <project> package --outdated'.

.PARAMETER SkipConfirmation
    Skips the database backup prompt of the update command, so that the script can run unattended.
    Without it, the update command waits for a keypress and fails if console input is redirected.

.EXAMPLE
    PS C:\dev\YourProject\scripts> .\Update-XperienceProjectWithDatabase.ps1 -Version 31.7.3
    
    Updates the Xperience by Kentico packages to version 31.7.3 and synchronizes the database.

.EXAMPLE
    PS C:\dev\YourProject\scripts> .\Update-XperienceProjectWithDatabase.ps1 -Version 31.7.3 -SkipConfirmation

    Runs the update process without prompting for confirmation of the database backup.

.CONFIGURATION
    Before running, ensure the following are configured correctly:
    - Package list in $xperiencePackages array
    - Launch profiles match your project's launchSettings.json
#>
param (
    # Version to update the Xperience by Kentico packages to, for example '31.7.3'
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+(\.\d+){1,3}(-[A-Za-z0-9.]+)?$')]
    [string] $Version,

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
    "kentico.xperience.core",            # Core API, often referenced directly by class library projects
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

<#
.DESCRIPTION
    Displays the output of a failed command, so that its cause is visible.

    The CLI commands report problems such as an unreachable database or a package version that no
    longer matches the database version through their output, not through the exit code, so the
    output has to be shown for the failure to be actionable.
#>
function Write-CommandOutput {
    param([object[]]$Output)

    if (-not $Output) {
        return
    }

    Write-Error "Output of the failed command:"
    $Output | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
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

    # '2>&1' keeps the error output, which holds the reason when the command fails to start
    $output = Invoke-Expression "$command 2>&1"

    if ($LASTEXITCODE -ne 0) {
        Write-CommandOutput $output
        throw "Command '$Option' failed with exit code $LASTEXITCODE"
    }

    # 'dotnet run' also prints build output, so pick out the line that holds the JSON result
    $json = $output | Where-Object { "$_".Trim().StartsWith('{') } | Select-Object -Last 1

    if ([string]::IsNullOrWhiteSpace($json)) {
        Write-CommandOutput $output
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
    3. Update the Xperience by Kentico NuGet packages to the specified version
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
    Sets all Xperience by Kentico NuGet packages to the version given by the 'Version' parameter.

    An explicit version is used rather than the latest available one, because it keeps runs
    reproducible and because it works for projects that use Central Package Management, where the
    versions are pinned in a 'Directory.Packages.props' file that 'dotnet add package' does not
    raise on its own.

    The package list can be customized by modifying the $xperiencePackages array below.
    Add or remove packages as needed for your specific project requirements.
    
    Common Xperience by Kentico packages:
    - kentico.xperience.admin: Administration interface
    - kentico.xperience.webapp: Core web application functionality
    - kentico.xperience.core: Core API, often referenced directly by class library projects
    - kentico.xperience.imageprocessing: Image processing capabilities
    - kentico.xperience.azurestorage: Azure Blob Storage integration
    - kentico.xperience.mjml: MJML email template support

    IMPORTANT: the list must cover every Xperience package referenced anywhere in your solution,
    not just in the project this script targets. Under Central Package Management all versions
    live in a single shared 'Directory.Packages.props' file, so a package left out of the list
    keeps its old version while the rest move forward, which breaks the build with a package
    downgrade error (NU1605). Run 'dotnet list <solution> package' to see the full set.
#>
Write-Status "Updating Xperience by Kentico NuGet packages to version $Version..."

# Use the configured package list
$xperiencePackages = $SCRIPT_CONFIG_XPERIENCE_PACKAGES

foreach ($pkg in $xperiencePackages) {
    Write-Status "Updating NuGet package: $pkg"

    $updateCmd = "dotnet add `"$projectFile`" package $pkg --version $Version"

    try {
        Invoke-ExpressionWithException $updateCmd
        Write-Notification "Updated $pkg to $Version."
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
