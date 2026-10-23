---
name: design-conventions
description: Styling and design conventions for the TrainingGuides Xperience by Kentico site — the SCSS build pipeline, which files are generated, and where component styles belong. Use when working on UI, CSS, SCSS, layout, or any visual change to the site.
---

# Design conventions

## The styling pipeline

SCSS in `src/TrainingGuides.Web/scss/` is the **source of truth**. It compiles to CSS in `src/TrainingGuides.Web/wwwroot/assets/css/`.

```
src/TrainingGuides.Web/scss/styles.scss   →   wwwroot/assets/css/styles.css      (expanded, + .map)
                                          →   wwwroot/assets/css/styles.min.css  (compressed, + .map)
```

Compilation is driven by the **Live Sass Compile** VS Code extension, configured in `.vscode/settings.json` (`watchOnLaunch` is enabled, source maps are generated, both output formats are produced).

### Rules

- **Never hand-edit `wwwroot/assets/css/styles.css`, `styles.min.css`, or either `.map` file.** They are generated output, and they *are* committed to the repository — an edit there will be silently overwritten on the next compile, and it will diverge from the SCSS everyone else reads.
- **`bootstrap.min.css` is a vendored third-party file.** Do not edit it. Override Bootstrap in the project SCSS instead.
- After changing any SCSS, make sure the CSS was regenerated and commit the regenerated output alongside the source. If the watcher was not running, compile before committing.

## SCSS structure

`styles.scss` is the only entry point. It does nothing but `@use` the partials:

| Partial | Scope |
| --- | --- |
| `_variables.scss` | Shared values. Not imported by `styles.scss` directly — pull it in where needed. |
| `_mixins.scss` | Shared mixins. Same. |
| `_font.scss` | Typeface setup. |
| `_shared.scss` | Styles used across features. |
| `_header.scss`, `_footer.scss`, `_section.scss` | Page chrome and layout structure. |
| `_button.scss`, `_card.scss`, `_form.scss` | Reusable UI components. |
| `_article.scss`, `_gallery.scss`, `_product.scss`, `_data-protection.scss` | Feature-specific styles. |

### Adding styles

- **A new reusable component** → new `_<component>.scss` partial, registered with `@use` in `styles.scss`.
- **A new feature area** → new `_<feature>.scss` partial, named to match the feature folder in `src/TrainingGuides.Web/Features/`, registered in `styles.scss`.
- **A variant of something that exists** → extend the existing partial rather than adding a near-duplicate.
- Reach for `_variables.scss` and `_mixins.scss` before introducing new literal values or repeating a block.

## Markup and components

- Razor views and view components live with their feature under `src/TrainingGuides.Web/Features/<Feature>/`. Keep a component's markup next to its code, and its styles in the matching SCSS partial.
- Page Builder widgets, sections and templates must have their identifiers registered in `src/TrainingGuides.Web/ComponentIdentifiers.cs`.
- Bootstrap is available. Prefer its grid and utilities over bespoke layout code, and override through SCSS rather than inline styles.

## Verifying a visual change

1. Build and run the site: `dotnet run --project src/TrainingGuides.Web` → <https://localhost:53415>
2. Check the affected page in the browser, and check it responsively — the compiled CSS, not just the SCSS, is what ships.
3. Confirm the generated CSS in `wwwroot/assets/css` reflects the SCSS change.
4. Check user-facing text for localization correctness.

## Brand, typography and imagery guidelines

> **TODO** — not documented yet. This repository has no committed brand, typography, spacing-scale, imagery or accessibility specification, so none is asserted here.
>
> If you have a design system or brand guideline to follow, add it to this section (or link it), covering: design direction, brand colors and logo usage, typographic scale, layout and spacing rules, the component inventory, imagery treatment, and the accessibility standard the site targets. Until then, follow the patterns already present in the SCSS partials rather than inventing new ones.
