# Xperience by Kentico Quickstart Guide starter repository
This repository is the starting point for the [Xperience by Kentico technical quickstart guides](https://docs.xperience.io/tutorial/quickstart-guides/development). Use this branch to follow along with the training guides.

The [finished branch](https://github.com/Kentico/xperience-by-kentico-quickguides/tree/finished) contains the cumulative results of the quickstart guides, as well as some sample widgets and can be used for reference. As it contains several independent examples, it is not built comprehensively, and we recommend against using it as a boilerplate.

## Installation requirements
- The database backup included in this repository requires SQL Server 2022 or newer. 
  - If you are using an older version of SQL server, you can prepare a fresh database by installing a fresh instance of Xperience by Kentico version **28.4.3**
- This repository targets the .NET 8 SDK.
  - If you are using a different .NET version, you can update the target framework in your solution. Note that the files in this repository use [C# 12 features and syntax](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12), some of which are not supported in lower versions.
## Getting started
1. Clone or download this repository to your development environment
1. Open **`TrainingGuides.sln`** under the **src** folder and restore all NuGet packages.
1. Restore **`Xperience.TrainingGuides.bak`** from the **Database** folder to your SQL server
1. Update the connection string in the **`appsettings.json`** file under the **.\src\TrainingGuides.Web** folder to point to your newly restored database
1. Run a [*Continuous integration restore*](https://docs.xperience.io/xp/developers-and-admins/ci-cd/continuous-integration#ContinuousIntegration-Restorerepositoryfilestothedatabase) to populate the database with the necessary data
1. Clean and rebuild the solution
1. Log in with the username **administrator** and the password **TrainingGuides123\***
