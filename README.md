# Xperience by Kentico Quickstart Guide starter repository
This repository is the starting point for the [Xperience by Kentico technical quickstart guides](https://docs.xperience.io/tutorial/quickstart-guides/development). Use this branch to follow along with the trainings.

To see the end result, check the [finished branch](https://github.com/Kentico/xperience-by-kentico-quickguides/tree/finished).

### :pencil2:**Note:** *This repository is based on the **KBank** demo site, but is not a one-to-one recreation.*

## Installation requirements
- The database backup included in this repository requires SQL Server 2022 or newer. 
  - If you are using an older version of SQL server, you can prepare a fresh database by installing a fresh instance of Xperience by Kentico version **26.4.0**
- This repository targets the .NET 6 SDK.
  - If you are using .NET 7 SDK or newer, you can update the project properties to target your version.
## Getting started
1. Clone or download this repository to your development environment
1. Open **`KBank.sln`** under the **src** folder and restore all NuGet packages.
1. Restore **`KBank.QuickstartGuides.bak`** from the **Database** folder to your SQL server
1. Update the connection string in the **`appsettings.json`** file under the **.\src\Kentico.KBank.Web** folder to point to your newly restored database
1. Run a [Continuous Integration Restore](https://docs.xperience.io/xp/developers-and-admins/ci-cd/continuous-integration#ContinuousIntegration-Restorerepositoryfilestothedatabase) to populate the database with the necessary data
1. Clean and rebuild the solution
1. Log in with the username **administrator** and the password **QuickGuides123\***