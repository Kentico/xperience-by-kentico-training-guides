# AGENTS.md

Instructions for AI coding assistants working in this repository. Tool-agnostic — `CLAUDE.md` and `.github/copilot-instructions.md` both point here.

## Project overview

- **Name:** TrainingGuides
- **Stack:** Xperience by Kentico 31.9.0, .NET 10 (SDK `10.0.100`, see `src/global.json`), ASP.NET Core MVC + Razor views, SCSS compiled to CSS, unit tests on the `CMS.Tests` base, MJML starter kit for emails.
- **Purpose:** Reference/demo site backing the Kentico Training Guides — each feature folder demonstrates one Xperience capability (Page Builder widgets, commerce, membership, data protection, personalization, email builder).
- **Running app URL:** <https://localhost:53415> (see `src/TrainingGuides.Web/Properties/launchSettings.json`). The admin UI is at `/admin`.

## Repository layout

| Path | What lives there |
| --- | --- |
| `src/TrainingGuides.sln` | Solution entry point for the whole project. |
| `src/TrainingGuides.Web/` | Main web application — `Program.cs`, DI registration, Razor views, static assets. |
| `src/TrainingGuides.Web/Features/` | **Feature folders** — the primary organizing unit. Each holds its own widgets, sections, services, models and views. |
| `src/TrainingGuides.Web/Features/Shared/` | Cross-feature building blocks: services, helpers, view components, sections, templates, option providers, visibility conditions. |
| `src/TrainingGuides.Web/ComponentIdentifiers.cs` | Central registry of string identifiers for every widget, section and template. Add new components here. |
| `src/TrainingGuides.Web/scss/` | SCSS source of truth. Compiled output lands in `wwwroot/assets/css`. |
| `src/TrainingGuides.Web/App_Data/CIRepository/` | Continuous Integration serialized objects — the content model and site configuration as files. |
| `src/TrainingGuides.Entities/` | **Generated** code files for content types, schemas, classes and forms. Do not hand-edit; regenerate instead. |
| `src/TrainingGuides.Admin/` | Admin UI customizations — custom pages, extenders, localization. |
| `src/TrainingGuides.Web.Tests/` | Unit tests for the web project, mirroring the `Features/` structure. |
| `src/TrainingGuides.Admin.Tests/` | Unit tests for admin customizations. |
| `src/Directory.Packages.props` | Central package version management — change package versions here, not in individual `.csproj` files. |
| `src/.editorconfig` | Authoritative formatting and analyzer rules. |
| `scripts/` | PowerShell helpers for CI restore/store, code generation, publishing. |

## Useful commands

Run from the repository root unless noted. Scripts are PowerShell.

| Task | Command |
| --- | --- |
| Build solution | `dotnet build src/TrainingGuides.sln` |
| Run site | `dotnet run --project src/TrainingGuides.Web` |
| Run web tests | `dotnet test src/TrainingGuides.Web.Tests` |
| Run admin tests | `dotnet test src/TrainingGuides.Admin.Tests` |
| Restore CI data into the database | `./scripts/CIRestore.ps1` |
| Store database objects into CI files | `./scripts/CIStore.ps1` |
| Regenerate content type code | `./scripts/GenerateCodeFiles.ps1` |
| Publish | `./scripts/Publish.ps1` |

A working database and connection string are required for anything that runs the app or touches CI data. Tests that fail with a connection-string error are usually missing that setup.

## Content changes

If you change the content model (add or remove fields, define new content types or reusable field schemas, add forms or custom module classes), regenerate the code files:

```powershell
./scripts/GenerateCodeFiles.ps1
```

This wraps the `--kxp-codegen` commands for reusable content types, page content types, email content types, reusable field schemas, custom module classes and forms, writing them into `src/TrainingGuides.Entities/{type}/{name}` under the `TrainingGuides` namespace. See <https://docs.kentico.com/documentation/developers-and-admins/api/generate-code-files-for-system-objects>.

The project must build before the script runs — it uses `dotnet run --no-build`.

After changing the model in the admin UI, also run `./scripts/CIStore.ps1` so the change is serialized into `App_Data/CIRepository` and committed alongside the code.

## Coding conventions

- **`src/.editorconfig` is authoritative.** It defines formatting, naming and analyzer severities; `IDE0055` (formatting) is set to `error`, so unformatted code fails the build. Run `dotnet format` if unsure.
- **Feature folders.** New functionality goes in `src/TrainingGuides.Web/Features/<Feature>/`, not into a global folder grouped by technical type. Shared building blocks go in `Features/Shared/`.
- **Register component identifiers.** Every widget, section and page template gets its identifier constant in `ComponentIdentifiers.cs`.
- **Never hand-edit generated code** in `src/TrainingGuides.Entities/` — regenerate it.
- **Never hand-edit compiled CSS** in `wwwroot/assets/css/` — edit the SCSS. See the `design-conventions` skill.
- **Central package versions.** Add or bump packages in `src/Directory.Packages.props`.
- **Implicit global usings are enabled.** Code copied out of this repo may need extra `using` statements.
- For content retrieval, Page Builder widgets, sections and templates, follow current Xperience guidance — verify against the Kentico Docs MCP rather than relying on recalled API shapes.

## Kentico MCP servers

- **Kentico Docs MCP** is the **primary source** for any question about Xperience by Kentico. Prefer it over web search and over your prior knowledge.
- **Kentico Management MCP** is used to work with content inside the Xperience by Kentico project. Prefer it over manual management through the admin interface.

### Where they are configured

- `.mcp.json` (repository root) defines `xperience-management-mcp` for any MCP-capable client.
- `.vscode/mcp.json` defines both the Docs MCP and the Management MCP for VS Code / GitHub Copilot.
- The Docs MCP is **deliberately absent from the root `.mcp.json`**: the KentiCopilot plugins register it globally at CLI level, and adding it again per-workspace creates two instances and roughly doubles token usage when its tools run. If you are not using those plugins, add it yourself:

  ```json
  { "type": "http", "url": "https://docs.kentico.com/mcp" }
  ```

### Management MCP setup

The Management MCP talks to a local, running instance through the management API. The application side is already wired up (`Kentico.Xperience.ManagementApi` package, `AddKenticoManagementApi()` / `UseKenticoManagementApi()` in `Program.cs`), but it is **opt-in and off until a secret is supplied**.

To turn it on locally, set a secret of at least 32 characters in the `MANAGEMENT_API_SECRET` environment variable. Both the application and the MCP server read that same variable, so there is one source of truth and nothing sensitive is committed:

```powershell
$secret = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
[Environment]::SetEnvironmentVariable('MANAGEMENT_API_SECRET', $secret, 'User')
```

Restart your IDE and shell afterwards so both pick the variable up, then run the application. The MCP server and the application can start in either order; if the application is not running when the server starts, tools appear as soon as it becomes reachable.

Alternatively, the secret can be set as the `ManagementApiSecret` configuration value (the project has user secrets configured, and this key is already populated locally) — but then the MCP server still needs `MANAGEMENT_API_SECRET` set to the same value.

With no secret set, `AddKenticoManagementApi()` is never called and the application behaves exactly as before.

Notes:

- The management API is for **local development only**. It provides basic authentication and no per-operation authorization. The registration is guarded by `builder.Environment.IsDevelopment()` so it cannot activate in production — do not remove that guard.
- Keep `Kentico.Xperience.ManagementApi` in `src/Directory.Packages.props` at the same version as the other Kentico packages, and update it together with them.
- Restart the MCP server after updating Xperience NuGet packages — it reads its tool list from the running application at startup.
- Once running, the OpenAPI spec is at `/kentico-api/management/v1/openapi.json`.

Full instructions: <https://docs.kentico.com/documentation/developers-and-admins/api/management-api/configure-management-mcp-server>

## Guidance skills

- `.claude/skills/design-conventions/` — how styling and design work is done in this repository (SCSS pipeline, generated-output rules).

## Validation of changes

- Always build the solution and run the relevant tests after making changes.
- If you touched SCSS, confirm the compiled CSS in `wwwroot/assets/css` was regenerated and is consistent with the source.
- If you touched the content model, regenerate entity code and store CI data.
- Always validate user-facing changes for content, layout, styling and localization correctness before committing.
