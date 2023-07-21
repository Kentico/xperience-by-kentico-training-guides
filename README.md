# Xperience by Kentico Quick Guide starter repository
This repository is the starting point for the [Xperience by Kentico technical quick start guides](https://docs.xperience.io/tutorial/quickstart-guides). Use this branch to follow along with the trainings.

To see the end result, check the [finished branch](https://github.com/Kentico/xperience-by-kentico-quickguides/tree/finished).

## Getting started
1. Clone or download this repository to your development environment
1. Open **Kentico.KBank.sln** under the **src** folder and restore all NuGet packages.
1. Restore **Kentico.KBank.Dev_latest.bak** from the **Database** folder to your SQL server
1. Update the connection string in the **appsettings.json** file under the **.\src\Kentico.KBank.Web** folder to point to your newly restored database
1. Run a [Continuous Integration Restore](https://docs.xperience.io/xp/developers-and-admins/ci-cd/continuous-integration#ContinuousIntegration-Restorerepositoryfilestothedatabase) to populate the database with the necessary data
1. Clean and rebuild the solution