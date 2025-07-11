# Xperience by Kentico Training Guides starter repository

This repository is the starting point for the [Xperience by Kentico technical Training Guides](https://docs.kentico.com/guides/development). Use this branch to follow along with the Training guides.

> [!NOTE]  
> This repository uses Xperience by Kentico version **30.6.2**.

The [finished branch](https://github.com/Kentico/xperience-by-kentico-training-guides/tree/finished) contains the cumulative results of the Training guides, as well as some sample widgets and can be used for reference. As it contains several independent examples, it is not built comprehensively, and we recommend against using it as a boilerplate. We continuously add new features to this repository as we create new Training guide materials. 

The goal of the [Training Guides](https://docs.kentico.com/guides/development) is to teach recommended development practices with Xperience by Kentico.
However, keep in mind that the code in this repository is intended to be examined in the context of individual guides -- some aspects are simplified to keep attention on the main topic of the corresponding lesson.

## Installation requirements

- The database backup included in this repository requires SQL Server 2022 or newer. 
  - If you are using an older version of SQL server, you can prepare a fresh database by installing a fresh instance of Xperience by Kentico version **30.6.2**
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
1. Apply your license key to the instance:
    1. Access the **Settings** application.
    1. Paste your license key into the **License key** field under the **System → License** category and click **Save**.
    ![Screenshot of Settings application](/images/SettingsApp.png)
    ![Screenshot of license key settings](/images/SettingsLicense.png)

> [!TIP]
> You can obtain a license key from your agency, supervisor, or team lead.  
> Alternatively, you can create an account at the [Client Portal](https://client.kentico.com/), and generate a [temporary key](https://client.kentico.com/evaluation-keys) valid for 30 days.  
> Learn more about licensing in our [documentation](https://docs.kentico.com/developers-and-admins/installation/licenses).

## Contributing

This repository is related to Training Guides for developers and has an educational purpose. For this reason, please do not submit ideas for new functionality. However, please do let us know if you encounter a bug that needs to be fixed, either by submitting an issue or contributing a fix directly.

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.
