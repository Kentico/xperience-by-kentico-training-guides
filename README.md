# Xperience by Kentico Training Guides starter repository

This repository is the starting point for the [Xperience by Kentico technical Training Guides](https://docs.kentico.com/guides/development). Use this branch to follow along with the Training guides.

> [!NOTE]  
> This repository uses Xperience by Kentico version **30.11.1**.

The [finished branch](https://github.com/Kentico/xperience-by-kentico-training-guides/tree/finished) contains the cumulative results of the Training guides, as well as some sample widgets, and can be used for reference. As it contains several independent examples, it is not built comprehensively, and we recommend against using it as a boilerplate. We continuously add new features to this repository as we create new Training guide materials. 

The goal of the [Training Guides](https://docs.kentico.com/guides/development) is to teach recommended development practices with Xperience by Kentico.
However, keep in mind that the code in this repository is intended to be examined in the context of individual guides -- some aspects are simplified to keep attention on the main topic of the corresponding lesson.

## Installation requirements

- Installation of this repository requires the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- This repository targets the .NET 8 SDK.
  - If you are using a different .NET version, you can update the target framework in your solution. Note that the files in this repository use [C# 12 features and syntax](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12), some of which are not supported in lower versions.

## Getting started

1. Clone or download this repository to your development environment
1. Open **TrainingGuides.sln** under the **src** folder, restore all NuGet packages, and rebuild the solution.
1. Open a command line in the **~/src/TrainingGuides.Web** folder and run the `dotnet tool restore` command to restore the *dbmanager* tool.
1. Edit the following command to target your server and license key, then run it:
    ```
    dotnet kentico-xperience-dbmanager -- -s "[YOUR SQL SERVER]" -a "TrainingGuides123*" -d "Xperience.TrainingGuides" --hash-string-salt "59642433-67b2-4230-9c5b-ad98d02b0c72" --license-file "[OPTIONAL: PATH TO TEXT A FILE CONTAINING YOUR LICENSE KEY]"
    ```

    > [!NOTE]
    > This automatically updates your **appsettings.json** file, creating a connection string and setting the value of `CMSHashStringSalt`.
    
1. Run a [*Continuous integration restore*](https://docs.xperience.io/xp/developers-and-admins/ci-cd/continuous-integration#ContinuousIntegration-Restorerepositoryfilestothedatabase) to populate the database with the necessary data
1. Log in with the username **administrator** and the password **TrainingGuides123\***
1. If you didn't include your license key when you installed the database, add it to the instance now:
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
