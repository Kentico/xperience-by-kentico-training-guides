# Content promotion widget — specification

Status: T1-T4 implemented, T5-T9 outstanding. See the ticket breakdown for progress.
Branch: `feat/GH-192-agentic-widget-development-example`

## 1. Purpose

A single Page Builder widget that promotes one piece of content — an article, a product
or a service — as a card, with every displayed value overridable on the widget instance.

The existing single-item widgets (`FeaturedArticle`, `ProductWidget`, `ServiceWidget`)
expose almost no content properties, so a Page Builder personalization variant of one of
them has nothing to vary. This widget exists so that the same promoted item can be
presented differently per contact group without touching the content item.

Secondary purpose: it is a reference implementation for the training guides, so clarity
of the demonstrated pattern outranks feature completeness.

## 2. Scope boundaries (settled before design)

- No content model changes. No new reusable field schema.
- Existing widgets are not modified or refactored.
- Selector scoping uses explicit content type name arrays, not schema filters.
- Card markup is standalone; no shared card partial is extracted.
- The widget is fully localized; other widgets in this repo mostly are not.
- Styling correctness is verified by eye, not by tests.

## 3. Placement

New feature folder, since the widget spans Articles, Commerce and FinancialServices and
belongs to none of them:

```text
src/TrainingGuides.Web/Features/ContentPromotion/
├── Widgets/ContentPromotion/
│   ├── ContentPromotionWidget.cshtml
│   ├── ContentPromotionWidgetProperties.cs
│   ├── ContentPromotionWidgetViewComponent.cs
│   └── ContentPromotionWidgetViewModel.cs
├── Controllers/ContentPromotionActivityController.cs
├── Models/  (PromotedItemViewModel, PromotionExtrasViewModel, MisconfigurationReason)
└── Services/IContentPromotionService.cs + ContentPromotionService.cs
```

Widget identifier `TrainingGuides.ContentPromotionWidget`, registered as a constant in
`src/TrainingGuides.Web/ComponentIdentifiers.cs` under `Widgets`, per AGENTS.md.

## 4. Content source modes

A radio group with three values:

| Value | Label | Meaning |
| --- | --- | --- |
| `page` | Page | A page from a website channel |
| `contentItem` | Content hub item | A reusable content item |
| `manual` | Manual content | No item selected; the override fields are the content |

`manual` is an explicit third option rather than "a source is selected but the selector is
empty", so that "the editor chose to author this by hand" is distinguishable from
"the selection is broken or unpublished". The editor warning in section 8 depends on that
distinction being representable.

### 4.1 Content type allow-lists

Explicit string arrays on the selector attributes.

Page selector (`page` mode):

- `TrainingGuides.ArticlePage`
- `TrainingGuides.ProductPage`
- `TrainingGuides.ServicePage`

Content hub selector (`contentItem` mode):

- `TrainingGuides.GeneralArticle`
- `TrainingGuides.Interview`
- `TrainingGuides.Service`
- `TrainingGuides.CatFood`
- `TrainingGuides.DogCollar`
- `TrainingGuides.CatFoodVariant`
- `TrainingGuides.DogCollarVariant`

The legacy `TrainingGuides.Article` type is deliberately excluded: it has no category
field (categories live on `IArticleSchema.ArticleSchemaCategory`, implemented only by
`GeneralArticle` and `Interview`), so it could never participate in the article extras.
An article-family member that structurally cannot show its extras would muddy the
demonstration.

Product variants are included alongside parents — see section 7.2 for why.

### 4.2 Runtime family detection

The widget does not ask the editor what family the item is. After retrieval the view
component inspects the resolved item:

- implements `IArticleSchema` -> article family
- implements `IProductSchema` -> product family
- is `Service` -> service family
- page types unwrap first: `ArticlePage.ArticlePageArticleContent`,
  `ProductPage.ProductPageProducts`, `ServicePage.ServicePageService`

`ServiceWidgetViewComponent` already does post-retrieval content type verification via
`IContentTypeService.GetContentTypeId`; the same guard applies here.

## 5. Override semantics

Title, description, image and call-to-action text are optional widget properties.

Resolution order per element, evaluated strictly in this order:

1. **Hidden** — the element name appears in `HideElements`. Render nothing.
2. **Overridden** — the widget property has a value. Render that value.
3. **Inherited** — render the value from the selected content item.

In `manual` mode step 3 yields nothing, so the override fields are the entire content.

`HideElements` is a single multi-select property listing Title / Description / Image /
Call to action, rather than four companion "hide this" checkboxes. It costs one property
instead of four. Title is hideable like the rest; the editor owns the consequence.

Without it, "empty means inherit" would make suppression inexpressible — a personalization
variant could never say "show this promo with no description".

**Open implementation detail:** the admin form component for a checkbox-list multi-select
must be confirmed against the Kentico Docs MCP before coding. Do not assume
`MultipleChoiceComponent` exists with that shape.

### 5.1 Image override

The image override is a content item selector limited to `TrainingGuides.Asset`
(`MaximumItems = 1`), not a URL text field.

Alt text comes from the selected asset's `AssetAltText` in both the override and the
inherit case. There is no separate alt-text property. A URL field would silently drop alt
text, which a fully localized widget must not do.

## 6. Link and click behaviour

### 6.1 Destination

| Mode | Destination |
| --- | --- |
| `page` | The selected page links to itself, via `IWebPageUrlRetriever` |
| `contentItem` | Editor-chosen link target page, **or** a typed URL — never both |
| `manual` | Editor-chosen link target page, **or** a typed URL — never both |

Both properties are visible when **source is not `page`**.

**Either-or, enforced in the form.** Once a link target page is selected, the URL input is
hidden, so an editor cannot author both. `LinkUrl` is therefore visible when source is not
`page` **and** `LinkTargetPage` is empty.

**But a hidden property keeps its stored value.** A visibility condition removes the input
from the form; it does not clear what was already saved. An editor who types a URL and then
picks a page leaves both values in the widget's stored configuration. The resolution code
must therefore still apply a rule, and cannot assume the form guaranteed exclusivity:

> If a link target page is set, it wins. The typed URL is ignored, whatever it contains.

Page wins because it matches the form's intent — the page selector is the field that was
still visible when the editor last chose — and because a page reference survives URL changes
while a typed URL does not.

Use the project's `TrainingGuidesWebPageUrlRetriever` (registered over `IWebPageUrlRetriever`),
which returns an empty URL rather than throwing on an unresolvable page.

**Resolved during implementation:** `VisibleIfEmpty` does exist in
`Kentico.Xperience.Admin.Base.FormAnnotations`, even though no other widget in this repo
uses it. `LinkUrl` carries both `[VisibleIfNotEqualTo(ContentSource, page)]` and
`[VisibleIfEmpty(nameof(LinkTargetPage))]`, which AND together. The `LinkType` radio
fallback is not needed and the property count stays at 20.

### 6.2 No destination

Content hub or manual mode with neither a link target nor a URL:

- Public visitors: the card renders, the call to action is omitted, the card is not clickable.
- Edit mode: the card renders plus an editor-only notice explaining why the CTA is absent.

Silent degradation is the worst outcome for a reference site whose audience is learners.

### 6.3 Clickable surface

The whole card is clickable **and** a visible call-to-action button is rendered. This is
one anchor in the DOM — the CTA anchor — expanded to the card surface with a stretched-link
overlay (an `::after` covering the card, with the card positioned relative).

One anchor means one accessible name and one place to attach click tracking. It also means
the card must contain **no other interactive elements**; the extras block in section 7 is
therefore read-only text, never links or buttons.

### 6.4 Click activity

A tracking value on the widget is logged as a custom activity when a visitor clicks.

Mechanism, following the existing `PageLike` pattern rather than the `kxt` script:

1. A new custom activity type, code name `contentpromotionclick`, created in the admin UI
   and serialized to `App_Data/CIRepository/@global/om.activitytype/` via `./scripts/CIStore.ps1`.
2. `ContentPromotionActivityController` exposes a POST endpoint that logs via
   `ICustomActivityLogger` with `ActivityValue` set to the widget's tracking value.
3. A small JS click handler on the card anchor posts to it.

The controller must gate on `ICookieConsentService.CurrentContactCanBeTracked()` exactly as
`PageLikeController` does — so the promo tracks nothing without consent. This is expected
behaviour, not a defect.

This is the one place the widget touches `CIRepository`. The "no content model changes"
rule is read as governing the content model, not activity type configuration.

## 7. Type-specific extras

An optional read-only block, never overridable. Hidden entirely in `manual` mode via a
visibility condition on the source radio (the condition only reads a sibling radio value,
so it is cheap).

| Family | Extras |
| --- | --- |
| Article | Categories, from `IArticleSchema.ArticleSchemaCategory` tag references |
| Product | Price, from `IProductPriceSchema.ProductPriceSchemaPrice`; stock status |
| Service | Benefits, from `Service.ServiceBenefits` mapped to `Benefit` items |

### 7.1 Missing or partial data

Per-item omission. Each extra renders only if it has a value; the extras block disappears
when all of them are empty. One exception: **stock is meaningful when zero**, so a zero or
out-of-stock value renders as an explicit out-of-stock state rather than vanishing.

If the toggle is on but the selected item's family has nothing to show, the block renders
nothing and no error is raised. The toggle is not hidden in that case — a visibility
condition would have to inspect the selected item's *type* at form-render time, which is
disproportionate machinery for a cosmetic win.

### 7.2 Products: price and stock reachability

This is the sharpest constraint in the design.

- `ProductSchemaPrice` does not exist. Price lives on `IProductPriceSchema`, implemented by
  `CatFoodVariant` and `DogCollarVariant` **only** — not by `CatFood` / `DogCollar`, and not
  reachable from `ProductPage`, which links `IProductSchema` (name / images / description).
- Stock is not a content field at all. It is a custom Info object,
  `ProductAvailableStockInfo` (object type `trainingguidees.productavailablestock`), keyed by
  the **variant's** content item ID and read through `IProductService`.

Decision: price and stock render **only when the selected item itself carries
`IProductPriceSchema`**. Selecting a parent product or a `ProductPage` shows the card
correctly with the product extras silently omitted, per the section 7.1 rule.

Variant types are in the allow-list so the extras are demonstrable at all. Parents and
pages stay in the allow-list so an editor picking the obvious item does not get a
broken-looking result.

**Explicit non-goal:** resolving a parent's variants to show a "from X" cheapest price and
an aggregated stock state. That needs an extra retrieval hop, a cheapest-price rule and an
aggregation rule across five stock states — a feature of its own, and not what this widget
demonstrates.

## 8. Misconfiguration reporting

The shared `IWidgetViewModel` contract exposes a single `bool IsMisconfigured`, which cannot
distinguish this widget's three distinct states:

| State | Public output | Edit mode |
| --- | --- | --- |
| Manual mode, nothing authored | Nothing | Notice: add content or select an item |
| Selection broken or unpublished | Nothing | Notice: the selected item could not be loaded |
| Selected, but no destination | Card without CTA | Notice: choose a link target or URL |

The view model implements `IsMisconfigured` to satisfy the interface, and **adds** a
`MisconfigurationReason` enum that the edit-mode block renders. Additive — the contract
holds and no existing widget changes.

No hard validation attributes. A conditional-required rule (title required only in manual
mode) is more machinery than the failure mode justifies, and required-field errors mid-edit
are a poor Page Builder experience.

Follow the `ProductWidget` convention, not the `FeaturedArticle` one: the placeholder is
wrapped in `Context.Kentico().PageBuilder().EditMode` so the public sees nothing.

## 9. Styling

### 9.1 Card designs

A `CardDesignOption` enum: `Standard`, `ImageOverlay`, `Gradient`, `Spotlight`, `Portrait`.

`ImageOverlay` and `Gradient` paint their own background, so the **colour scheme property is
hidden for those two** and visible for the rest — two stacked `VisibleIfNotEqualTo`
conditions, which AND together.

Note: **no gradient exists anywhere in the project SCSS.** Existing overlays are flat-colour
`::before` pseudo-elements (`_article.scss`, `_section.scss`). `Gradient` is net-new styling,
not a reuse of an existing pattern.

### 9.2 Reused infrastructure

- `DropdownEnumOptionProvider<T>` for every dropdown
- `ColorSchemeOption`, `CornerStyleOption`, `ColumnLayoutOption` — existing shared enums
- `LinkStyleOption` for the CTA style — it already ships both button variants
  (`Dark`, `Medium`, `Light1`-`Light3`) and plain-link variants
  (`TransparentDark` / `TransparentMedium` / `TransparentLight`)
- the `tg-component-style` tag helper (`color-scheme`, `corner-style`)
- the `tg-styled-image` tag helper
- `IComponentStyleEnumService` for enum to CSS class mapping
- All CSS class computation happens in the view component; the Razor view stays dumb,
  per `ServiceWidgetViewComponent`

### 9.3 New: shared text alignment enum

**There is no shared text-alignment option provider in this repo.** Alignment exists only as
`ContentAlignmentOption`, declared locally at the bottom of `ServiceWidgetProperties.cs`.

A new shared `TextAlignmentOption` is added in
`Features/Shared/OptionProviders/TextAlignment/`. The duplicate inside `ServiceWidget` is
left untouched — it is pre-existing debt this widget is not obliged to pay off, and touching
it would violate the "existing widgets are not modified" boundary. The duplication is
deliberate, not accidental.

### 9.4 SCSS

New partial `src/TrainingGuides.Web/scss/_content-promotion.scss`, imported from
`styles.scss`. Follow the `c-` / `u-` / `tg-` / `j-` conventions in the design-conventions
skill. Never hand-edit `wwwroot/assets/css/`.

## 10. Properties

Twenty properties, grouped so that related settings sit together and every styling property
hides behind one toggle. The advanced block starts at Order 140, stepping by 10, matching
`ServiceWidget`.

| Order | Property | Component | Visible when |
| --- | --- | --- | --- |
| 10 | `ContentSource` | Radio group (`page` / `contentItem` / `manual`) | always |
| 20 | `SelectedPage` | Content item selector, page allow-list, max 1 | source is `page` |
| 30 | `SelectedContentItem` | Content item selector, hub allow-list, max 1 | source is `contentItem` |
| 40 | `Title` | Text input | always |
| 50 | `Description` | Text area | always |
| 60 | `Image` | Content item selector, `Asset`, max 1 | always |
| 70 | `HideElements` | Multi-select (component to be verified) | source is not `manual` |
| 80 | `ShowExtras` | Checkbox | source is not `manual` |
| 90 | `CallToActionText` | Text input | always |
| 100 | `LinkTargetPage` | Content item selector, page allow-list, max 1 | source is not `page` |
| 110 | `LinkUrl` | Text input | source is not `page` AND `LinkTargetPage` is empty |
| 120 | `OpenInNewTab` | Checkbox | always |
| 130 | `TrackingValue` | Text input | always |
| 140 | `ShowAdvanced` | Checkbox, "Adjust design" | always |
| 150 | `CardDesign` | Dropdown, `CardDesignOption` | advanced |
| 160 | `ColorScheme` | Dropdown, `ColorSchemeOption` | advanced AND design is neither ImageOverlay nor Gradient |
| 170 | `ColumnLayout` | Dropdown, `ColumnLayoutOption` | advanced |
| 180 | `TextAlignment` | Dropdown, new `TextAlignmentOption` | advanced |
| 190 | `CornerStyle` | Dropdown, `CornerStyleOption` | advanced |
| 200 | `CallToActionStyle` | Dropdown, `LinkStyleOption` | advanced |

Twenty is defensible for a widget whose entire reason for existing is that the others expose
nothing to personalize — provided the advanced toggle hides six of them and the source
conditions hide the irrelevant branch. At most fourteen are visible at once.

## 11. Localization

Two separate mechanisms with different culture coverage in this repo.

**Admin / Page Builder property labels** — `{$TrainingGuides.ContentPromotionWidget.*$}`
macros, following `CallToActionWidgetProperties` as the reference implementation. Keys added
to all three cultures:

- `src/TrainingGuides.Admin/Localization/Resources/en-US/custom/LocalizationCustom.en-US.resx`
- `.../es-MX/custom/LocalizationCustom.es-MX.resx`
- `.../fr-FR/custom/LocalizationCustom.fr-FR.resx`

**Rendered front-end strings** — `IStringLocalizer<SharedResources>`, with translations added
to `src/TrainingGuides.Web/Resources/SharedResources.es.resx`.

**Known gap, accepted deliberately:** there is no `SharedResources.fr.resx` in this project,
so rendered strings (CTA fallback text, stock status labels, extras headings) are localized
to Spanish only, while property labels are localized to Spanish and French. Creating a French
front-end resource that no other feature populates would be a half-empty file — worse than a
consistent gap. Revisit if French front-end localization is added project-wide.

## 12. Verification

- `dotnet build src/TrainingGuides.sln` — `IDE0055` is an error, so run `dotnet format` if unsure.
- `dotnet test src/TrainingGuides.Web.Tests` — unit tests cover the override resolution order
  (section 5) and the family detection (section 4.2). Styling is verified by eye, per section 2.
- Confirm compiled CSS in `wwwroot/assets/css` was regenerated from the SCSS.
- `./scripts/CIStore.ps1` after creating the activity type, and commit the resulting XML.
- No entity code regeneration is needed — the content model is unchanged.

## 13. To verify against Kentico Docs MCP before coding

Per AGENTS.md, verify rather than rely on recalled API shapes:

1. The admin form component for a checkbox-list multi-select (section 5, `HideElements`).
2. Retrieving taxonomy tag display names for article categories from `TagReference` values.
3. The current `IProductService` surface for reading stock status by content item ID.
4. Custom activity type creation and the current `ICustomActivityLogger` contract.
5. ~~Whether a `VisibleIfEmpty` attribute exists~~ — **resolved: it does.** See section 6.1.

## 14. Decisions taken during implementation

These changed the design as written above and are recorded here rather than silently applied.

### 14.1 A non-generic content item retrieval method was added to a shared service

The content hub selector allows seven content types, but `IContentItemRetrieverService`
exposed only a generic `RetrieveContentItemByGuid<T>`, which cannot retrieve an item whose
type is not known in advance. Pages already had a non-generic
`RetrieveWebPageByContentItemGuid` returning `IWebPageFieldsSource?`.

A matching `RetrieveContentItemByGuid` returning `IContentItemFieldsSource?` was added to
`IContentItemRetrieverService` and `ContentItemRetrieverService`. The alternative was up to
seven sequential retrievals per render.

This is additive and touches no widget, so it stays inside the "existing widgets are not
modified" boundary — but it does mean this feature changed shared code.

### 14.2 That new query has no integration test

The unit tests substitute `IContentItemRetrieverService`, so they prove nothing about
whether the new query works against a database. The solution has no integration-test
infrastructure (both test projects are xunit + Moq, with no `Kentico.Xperience.Core.Tests`).

**Decision: integration tests are wanted and this gap is not waived.** Tracked as ticket T10
in the ticket breakdown. Until then, verify by hand: select each of the seven allowed hub
types in the admin and confirm the card renders.

### 14.3 The override image arrives pre-resolved

`properties.Image` is a `ContentItemReference`, which needs retrieval to become an
`AssetViewModel`. `ResolveDisplayValues` therefore takes the resolved override image as an
optional third parameter, and the caller resolves it. This keeps the resolution rule pure
and testable without mocks.

### 14.4 Family detection uses one retrieval plus pattern matching

Section 4.2 did not say how the page is retrieved. The implementation retrieves once through
the non-generic overload and pattern-matches the concrete type (`ArticlePage`, `ProductPage`,
`ServicePage`, then `IArticleSchema`, `IProductSchema`, `Service`). Retrieving as a guessed
generic type would map the wrong content type when the selector allows several.
