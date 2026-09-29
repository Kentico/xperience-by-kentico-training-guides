# Content promotion widget — ticket breakdown

Companion to [content-promotion-widget-spec.md](./content-promotion-widget-spec.md).
Section references below (§4, §5, …) point into that spec.

Implementation flow: the `tdd` skill's red → green loop, one vertical slice at a time.

## How these tickets are meant to be worked

- **Seams are confirmed before any test is written.** Every ticket names its seam and its
  test tier. Confirm the seam list at the top of each ticket before starting it. A ticket
  whose seam you have not confirmed does not get a test.
- **Vertical slices, not horizontal.** Within a ticket, work one acceptance criterion at a
  time: one failing test, then the minimal code to pass it, then the next. Do not write all
  of a ticket's tests up front — that is the horizontal-slicing anti-pattern, and it tests
  imagined behaviour rather than real behaviour.
- **Red before green.** Each criterion starts with a test that fails for the right reason.
  Run it and see it fail before writing implementation.
- **Refactoring is not part of the loop.** It happens at review, via the `code-review` skill.
- Tickets marked **no automated tests** are verified by eye or in the admin UI, per spec §2.

## Test stack — read this before the first test

AGENTS.md states "unit tests on the `CMS.Tests` base", and the `automated-tests` skill's
tier table assumes NUnit with `UnitTests` / `IntegrationTests` base classes.

**Neither describes this repository.** `TrainingGuides.Web.Tests` and
`TrainingGuides.Admin.Tests` are **xunit + Moq**, with no `Kentico.Xperience.Core.Tests`
package reference in the solution. The established pattern — see
`Features/Membership/Widgets/LinkOrSignOut/LinkOrSignOutWidgetViewComponentTests.cs` — is a
plain xunit class that mocks project-owned interfaces (`IContentItemRetrieverService`,
`IHttpRequestService`, `IPreferredLanguageRetriever`) with Moq.

Follow the repository. Adding NUnit or the Kentico test base packages for this widget is out
of scope and would leave the solution with two test stacks.

### The pattern every test in these tickets follows

Copy the shape of
`src/TrainingGuides.Web.Tests/Features/Membership/Widgets/LinkOrSignOut/LinkOrSignOutWidgetViewComponentTests.cs`:

- A plain `public class <Thing>Tests` — no base class, no `[TestFixture]`, no attributes on
  the class itself.
- `Mock<T>` fields for each substituted dependency, assigned in the **constructor**, with
  default `Setup(...)` calls there so individual tests override only what they care about.
- `[Fact]` for a single case, `[Theory]` + `[InlineData]` where the same assertion runs over
  several inputs.
- `Assert.Equal`, `Assert.Null`, `Assert.True` — xunit assertions, not NUnit's `Assert.That`.
- Constants for test data at the top of the class (`private const string TITLE = "…";`),
  matching the existing convention.
- Test file path mirrors the source path under `Features/`.

Substitute only project-owned interfaces — `IContentItemRetrieverService`,
`IWebPageUrlRetriever`, `IProductService`, `ICustomActivityLogger`, `ICookieConsentService`.
Do not mock our own view models, mapping code, or `IContentPromotionService` itself when it
is the thing under test.

### Consequences of the stack

- Every seam below is a plain xunit test with Moq doubles. There is no `Fake<>()` available,
  and no `UnitTests` / `IntegrationTests` base class to inherit from.
- **Correction (was wrong when written).** This section originally said the widget introduces
  no new retrieval adapter, so the integration-test rule added no obligation. That turned out
  to be false: T3 had to add a non-generic `RetrieveContentItemByGuid` to
  `IContentItemRetrieverService`, which is a real content query covered only by substitutes.
  **Integration tests are wanted and the gap is not waived** — see T10.
- Tests do not cover rendered markup, styling, Page Builder edit mode, or localization
  resolution. Those are manual checks, called out per ticket.

## Commands

| Step | Command (from repository root) |
| --- | --- |
| Build | `dotnet build src/TrainingGuides.sln` |
| Run one test class | `dotnet test src/TrainingGuides.Web.Tests --filter "FullyQualifiedName~<TestClassName>"` |
| Full web suite | `dotnet test src/TrainingGuides.Web.Tests` |
| Format | `dotnet format` (IDE0055 is an error) |

## Progress

| Ticket | Status |
| --- | --- |
| T1 scaffolding | Done |
| T2 override resolution | Done — 18 tests |
| T3 family detection | Done — 8 tests |
| T4 link resolution | Done — 8 tests |
| T5 misconfiguration | Done — 6 tests |
| T6 type-specific extras | Done — 16 tests |
| T7 styling | Done - 3 tests; responsive and visual fixes applied after shipping, see T7 |
| T8 localization | Done |
| T9 click activity | Done - 4 tests |
| T10 integration tests | Not started, added during T3 |

Full web suite at the T9 stop point: **168 passing, 0 failing.**

## Ticket map

```text
T1 scaffolding ──┬── T2 override resolution ──┐
                 │                            ├── T6 extras ── T7 styling ── T9 activity
                 ├── T3 family detection ─────┤
                 └── T4 link resolution ──────┘
                                              └── T5 misconfiguration
                                                    T8 localization (any time after T1)
```

T2, T3 and T4 are independent of each other and can be worked in any order once T1 lands.

---

## T1 — Scaffolding and registration

**Goal:** the widget appears in the Page Builder widget list and renders a placeholder.

**Seams: none. No automated tests.** Registration attributes and default Page Builder
behaviour are framework behaviour, not ours — the `automated-tests` skill is explicit that
these are not tested.

**Work:**

- Create the feature folder tree from spec §3.
- `ContentPromotionWidgetProperties` implementing `IWidgetProperties`, empty for now.
- `ContentPromotionWidgetViewModel` implementing `IWidgetViewModel`.
- `ContentPromotionWidgetViewComponent` with `IDENTIFIER = "TrainingGuides.ContentPromotionWidget"`
  and the `RegisterWidget` assembly attribute.
- Register the constant in `src/TrainingGuides.Web/ComponentIdentifiers.cs` under `Widgets`.
- Razor view rendering only the edit-mode placeholder, wrapped in
  `Context.Kentico().PageBuilder().EditMode` (the `ProductWidget` convention, spec §8).

**Done when:** solution builds, and the widget can be added to a page in the admin UI and
shows the configure-widget instructions.

**Manual check:** add the widget to the Widget samples page in the admin.

---

## T2 — Override resolution: hide → override → inherit

**Goal:** the core rule of the widget (spec §5) — the reason it exists.

**Seam:** a pure resolution function on `IContentPromotionService`, taking the widget
properties plus an already-resolved source item projection, returning the display values.
No Xperience API involved.

**Tier:** plain xunit, no mocks needed. This is the cleanest TDD slice in the widget —
start the red-green habit here.

**Confirm before starting:** that the resolution function is the seam, not the view
component. Testing through the view component would drag in retrieval mocks for logic that
has nothing to do with retrieval.

**Acceptance criteria — one test each, in this order:**

1. Property has a value, item has a different value → property value wins.
2. Property is empty, item has a value → item value is used.
3. Property is empty, item has no value → nothing is rendered for that element.
4. Element is listed in `HideElements` and the property has a value → nothing is rendered.
   (Hide beats override — this is the ordering the whole rule turns on.)
5. Element is listed in `HideElements` and only the item has a value → nothing is rendered.
6. Manual mode: no item at all, property has a value → property value is used.
7. Manual mode: no item, no property value → nothing is rendered.

Cover title, description and call-to-action text. Image resolution is the same rule but
carries alt text — assert that alt text travels with the image in both the override and the
inherit case (spec §5.1).

**Blocked on:** the `HideElements` admin form component is unverified (spec §13.1). This
ticket does **not** need it resolved — the resolution function takes a collection of hidden
element names, whatever component eventually produces it. Do not let the unknown component
block the logic.

---

## T3 — Content source modes and runtime family detection

**Goal:** turn a selector value into a normalized item projection plus a family (spec §4).

**Seam:** the retrieval-and-normalization method on `IContentPromotionService`.
`IContentItemRetrieverService` is substituted with Moq, following
`LinkOrSignOutWidgetViewComponentTests`.

**Tier:** xunit + Moq.

**Do not assert on retrieval parameters** — linked-items depth, caching, query builder
calls. That couples the test to implementation; those are code-review concerns.

**Acceptance criteria — one test each:**

1. `page` mode, an `ArticlePage` is returned → article family, fields taken from the
   unwrapped `ArticlePageArticleContent`.
2. `page` mode, a `ProductPage` → product family, via `ProductPageProducts`.
3. `page` mode, a `ServicePage` → service family, via `ServicePageService`.
4. `contentItem` mode, a `GeneralArticle` → article family.
5. `contentItem` mode, a `Service` → service family.
6. `contentItem` mode, a `CatFoodVariant` → product family.
7. `manual` mode → no retrieval call is made, no family.
8. Selector is empty in a non-manual mode → no item, and the result is distinguishable from
   manual mode (T5 depends on this).
9. Retrieval returns null (unpublished or deleted) → broken-selection result, not a crash.

**Note:** criterion 9 is the case that makes the spec §8 three-state model necessary. Get it
right here and T5 is mostly bookkeeping.

---

## T4 — Link resolution and the no-destination case

**Goal:** spec §6.1 and §6.2.

**Seam:** the link-building method on `IContentPromotionService`, returning the project's
`LinkViewModel`. `IWebPageUrlRetriever` is substituted with Moq.

**Tier:** xunit + Moq.

**Acceptance criteria — one test each:**

1. `page` mode → the link URL is the selected page's own URL.
2. `contentItem` mode with a link target page → that page's URL.
3. `contentItem` mode with a typed URL and no target page → the typed URL.
4. Both a target page and a typed URL are set in stored configuration → **the page wins,
   the URL is ignored.** This state is unreachable through the form (the URL input hides
   once a page is picked) but fully reachable in stored data, because a visibility
   condition hides an input without clearing its saved value. Do not skip this test on the
   grounds that "the form prevents it" — the form prevents authoring it, not storing it.
5. `contentItem` mode with neither → no destination; the result says so explicitly rather
   than returning an empty string that the view has to interpret.
6. `manual` mode with neither → same as 5.
7. `IWebPageUrlRetriever` returns an empty URL (the project's retriever does this instead of
   throwing) → treated as no destination, not as a link to `""`.
8. `OpenInNewTab` is carried through.

**Work beyond the tests:** hide `LinkUrl` once `LinkTargetPage` is set, per spec §6.1.
Verify first that a `VisibleIfEmpty` attribute exists — only `VisibleIfEqualTo`,
`VisibleIfNotEqualTo`, `VisibleIfTrue`, `VisibleIfFalse` and `VisibleIfNotEmpty` are
confirmed present in this repo. If it does not exist, fall back to an explicit `LinkType`
radio (spec §13.5), which changes the property count from 20 to 21.

**Criterion 7 matters:** `TrainingGuidesWebPageUrlRetriever` swallows
`InvalidOperationException` and returns an empty `WebPageUrl`. Code that assumes a non-empty
string will render `href=""`, which links to the current page — a silent wrong answer.

---

## T5 — Misconfiguration reporting

**Goal:** spec §8 — three distinct states behind a single-bool interface.

**Seam:** the view model's `IsMisconfigured` and the added `MisconfigurationReason`.

**Tier:** plain xunit.

**Acceptance criteria — one test each:**

1. Manual mode, nothing authored → misconfigured, reason "nothing authored".
2. Non-manual mode, retrieval returned null → misconfigured, reason "item could not be loaded".
3. Item resolved, no destination → **not** misconfigured; the card renders. Reason carries
   "no destination" so edit mode can show the notice.
4. Fully configured → not misconfigured, no reason.

**Criterion 3 is the subtle one.** No-destination is a warning, not a misconfiguration: the
public still sees a valid card. Conflating the two would blank a working card for visitors.

**Manual check:** edit mode shows the right notice for each of the three states. Edit-mode
rendering is not covered by these tests.

**What T5 actually landed, beyond the four criteria:**

- The view component was wired up (`InvokeAsync` calling `ResolvePromotedItem`,
  `ResolveOverrideImage`, `ResolveDisplayValues`, `ResolveLink`), `IContentPromotionService`
  was registered in `ServiceCollectionExtensions`, and a minimal card was added to the Razor
  view. Without this the three states cannot be seen in edit mode at all, so the manual check
  would have been impossible. This is the T2/T3/T4 glue no ticket owned.
- `PromotedItemResult.Page` carries the page that page-mode retrieval already loaded, so
  `ResolveLink` does not query for the same page a second time.
- **Two extra tests, found at review.** `SelectionFailed` was set only when retrieval returned
  null, so two other broken selections — an unsupported content type, and a page whose linked
  content item is missing — reported `NothingAuthored` instead of `ItemCouldNotBeLoaded`.
  `ResolvePromotedItem` now reports a failed selection whenever retrieval returned something
  but no item could be projected from it. Spec §8 row 2 covers these cases.
- `AssetViewModel.GetViewModel` returns an *empty* model rather than null for a missing asset,
  which read as "there is an image" downstream and would have rendered `<img src="">`. The
  service now normalizes that to null.

**Handed to T7 (styling):** the card markup added here emits `c-content-promotion`,
`c-content-promotion__image`, `__title` and `__description`, and **no `_content-promotion.scss`
exists yet** — those classes are currently dead. T7 also needs to replace the bare `<img>` with
`tg-styled-image` and move CSS-class computation into the view component, per spec §9.2.

**Handed to T8 (localization):** the three edit-mode notice strings in the Razor view are
hard-coded English.

---

## T6 — Type-specific extras

**Goal:** spec §7. Split into three sub-slices; land them one at a time.

**Seam:** an extras-building method per family on `IContentPromotionService`, returning a
`PromotionExtrasViewModel`. `IProductService` and the taxonomy retriever are substituted.

**Tier:** xunit + Moq.

### T6a — Article categories

1. Item has categories → each category name appears.
2. Item has no categories → the extras block is empty.
3. `ShowExtras` is off → no extras regardless of data.

**Unverified (spec §13.2):** how to resolve tag display names from `TagReference` values.
Verify against the Kentico Docs MCP before writing the implementation, not before the test —
the test asserts on names, whatever API produces them.

### T6b — Service benefits

1. Service has benefits → each benefit description appears.
2. Service has no benefits → empty extras.

### T6c — Product price and stock

This is the sub-slice most likely to surprise; re-read spec §7.2 first.

1. Selected item implements `IProductPriceSchema` (a variant) → price appears.
2. Selected item is a parent (`CatFood`, `DogCollar`) → **no price, no stock**, extras block
   empty. This is correct behaviour, not a bug.
3. Selected item came from a `ProductPage` → same as 2.
4. Variant with stock above the low threshold → in-stock state.
5. Variant with **zero** stock → explicit out-of-stock state, **not** omitted. This is the
   documented exception to per-item omission (spec §7.1).
6. No stock record exists for the variant → unknown state, omitted.

**Unverified (spec §13.3):** the current `IProductService` surface for reading stock by
content item ID. Verify before implementing.

### What T6 actually landed

**Seam, as worked:** one public entry point, `ResolveExtras(properties, PromotedItemResult)`, which
pattern-matches the promoted item and delegates to a private builder per family. The ticket said
"a method per family"; a public method per family would have pushed the family dispatch into the
view component, where it does not belong. The acceptance criteria are all expressible through the
single entry point, so nothing was lost.

**Both open questions are settled:**

- **§13.2, tag display names.** `ITaxonomyRetriever.RetrieveTags(IEnumerable<Guid>, string language)`
  returns `IEnumerable<Tag>`; `Tag.Title` is the display name. The language comes from
  `IPreferredLanguageRetriever`, as everywhere else in this project. Confirmed against the Kentico
  Docs MCP (*Taxonomies*, *Reference — Admin UI form components*).
- **§13.3, stock.** `IProductService` had **no** public stock member —
  `ProductService.GetProductStockStatus(IProductSkuSchema?)` was private. It is now public and on
  the interface. It already returns `ProductStockEnum.Unknown` when no stock record exists and
  `OutOfStock` at zero, which is exactly the three-way distinction criteria 4-6 need, so no new
  stock logic was written.

**Beyond the acceptance criteria:**

- `PromotedItemResult` gained `PromotedContent` — the unwrapped content item the card's values were
  projected from. `PromotedItemSource` is normalized across families and deliberately carries none
  of the type-specific fields, so the extras had nothing to read without it.
- `ShowExtras` added to the widget properties at Order 70, hidden in manual mode via
  `VisibleIfNotEqualTo` (spec §7).
- `PromotionExtrasViewModel.StockStatus` is `ProductStockEnum?` and is **null** for `Unknown`, so
  "no stock record" and "out of stock" cannot be confused by the view. Zero stock arrives as
  `OutOfStock` and renders, per spec §7.1.
- The view component and Razor view were wired up, as in T5 — extras that no page renders cannot be
  checked by eye.

**Handed to T7 (styling):** the extras markup emits `c-content-promotion__extras`, `__categories`,
`__category`, `__benefits`, `__benefit`, `__price` and `__stock`. All dead until
`_content-promotion.scss` exists.

**Handed to T8 (localization):** the four stock labels in the Razor view are hard-coded English, as
are the `ShowExtras` label and explanation text.

**Not covered by tests, verify by hand:** that the extras block actually renders for an article with
categories, a service with benefits, and a `CatFoodVariant`; and that the `ShowExtras` checkbox
disappears when the source is set to Manual.

### Found at review, fixed in T6

**Two retrieval bugs that broke the widget outright — the exact gap T10 exists to close.**
`ContentItemQueryBuilder.ForContentTypes` returns content item metadata and reusable field schema
data only; content type-specific fields need `WithContentTypeFields()` (confirmed in the Docs MCP,
*Reference — Content item query*). Neither `RetrieveContentItemByGuid` (added by T3) nor the shared
`RetrieveWebPages` helper called it, so:

- **Page mode was broken for all three page types.** `ArticlePageArticleContent`,
  `ProductPageProducts` and `ServicePageService` are content type-specific, so every one came back
  empty, `FromArticle(null)` returned an empty result, and `SelectionFailed` flipped to true — a
  correctly configured page reported "The selected item could not be loaded."
- **Content hub mode was broken for `Service`**, whose fields (`ServiceName`, `ServiceBenefits`)
  are content type-specific rather than schema fields. Articles and products survived because
  their fields come from reusable schemas, which `ForContentTypes` does include.

Both are fixed by adding `WithContentTypeFields()` — but **not on its own**. The first attempt added
only that call and threw at runtime:

```text
System.InvalidOperationException: Cannot generate query without limiting content types.
   at CMS.ContentEngine.DynamicContentQuery.GetExecutingQuery(DataQuerySettings settings)
```

`WithContentTypeFields` is only legal on a subquery limited to specific content types: a query
spanning every content type cannot name the columns it would have to select. `ForWebsite(channel)`
does **not** count as limiting them — it restricts the channel, not the types. So both queries now
also call `OfContentType(...)`, naming exactly the types the matching selector offered.

This changed two signatures on `IContentItemRetrieverService`:

- `RetrieveContentItemByGuid` (non-generic, added by T3, this widget its only caller) now takes the
  content type names. Its doc comment no longer says "without knowing its content type" — the caller
  must know the candidates, even if not which one.
- A second `RetrieveWebPageByContentItemGuid` overload takes content type names. The existing
  overload is **unchanged**, so `ArticleList`, `ProductListing`, `SimpleCallToAction` and
  `LinkOrSignOut` are untouched — they read only the URL and do not need the extra columns. The
  widget's own link-target lookup keeps using the untouched overload for the same reason.

The type lists live in `ContentPromotionContentTypes` so retrieval cannot drift from the selector.
The selector attributes still repeat them inline, with a pointer comment, because an attribute
argument must be a compile-time constant and a `string[]` is not one.

**No test covers any of this.** They are content queries reachable only through a database, which is
precisely T10's remit — and the `InvalidOperationException` above is what that gap costs: a green
suite of 157 tests, a clean build, and a widget that threw on the first real selection. T10 must
cover `RetrieveWebPageByContentItemGuid` as well as `RetrieveContentItemByGuid`; its "At minimum
this must cover" list is now short by one, and should assert that content type-specific fields are
actually populated rather than only that an item comes back.

**Deliberate spec deviation.** Spec §7.2 says price comes from `IProductPriceSchema.ProductPriceSchemaPrice`.
The card now renders `IProductService.GetCatalogPrice` instead, which applies catalog discounts and
falls back to the schema price when none apply. The raw value would have shown a discounted variant
at one price on the promotion card and another in the Product widget on the same page.
`GetCatalogPrice` was private, like `GetProductStockStatus`; both are now on `IProductService`.

### Page selector reachability — WORKAROUND IN PLACE, report to Kentico

**Symptom.** With the page selector scoped to the three promotable page types
(`ArticlePage`, `ProductPage`, `ServicePage`), a `ProductPage` cannot be selected at all. The
editor cannot expand `/Store` in the selector's content tree, so nothing underneath it is
reachable.

**The tree.** Promotable pages in this project do not all sit in branches made only of promotable
types:

| Path | Content type | Promotable |
| --- | --- | --- |
| `/News` → `/News/About-cats` | `ArticlePage` → `ArticlePage` | yes → yes |
| `/Products` → `/Products/Lorem-x` | `EmptyPage` → `ServicePage` | **no** → yes |
| `/Store` → `/Store/Dog-collar` → `/Store/Dog-collar/Trail-flex-reflective-dog-collar` | `EmptyPage` → `StoreSection` → `ProductPage` | **no** → **no** → yes |

Articles work because their whole chain from the channel root is `ArticlePage`. Products and
services do not.

**Hypothesis (unconfirmed).** The combined content selector (`ContentItemSelectorComponent`) renders
only those branches of the tree whose pages are all of an allowed content type, so an allowed page
is unreachable when any ancestor is disallowed. The docs for the component describe its
configuration properties but say nothing about how it treats ancestors, so this is inference from
observed behaviour, not a documented rule.

**Supporting evidence.** `SimpleCallToActionWidgetProperties` — the canonical example in Kentico's
own Page Builder guide — lists `DownloadsPage`, `EmptyPage`, `LandingPage` and `ProfilePage`
alongside the types it renders. Those look like container types added for exactly this reason. That
widget still cannot reach the store branch, because `StoreSection` is missing from its list.

**Not confirmed by the obvious experiment.** `ProductWidget` scopes its selector to `ProductPage`
alone, which would be the clean test — but that widget throws when placed on a page that is not a
product page, so its selector could not be exercised.

**Still worth testing:** whether `ServicePage` selection is broken the same way under `/Products`.
The hypothesis predicts it is.

**What we did (workaround).** `EmptyPage` and `StoreSection` were added to the page selector's
allow-list purely to make the tree walkable. They are tracked separately from the promotable types
in `ContentPromotionContentTypes` (`PROMOTABLE_PAGES`, `CONTAINER_PAGES`, `PAGES`), and
`ResolvePromotedItem` recognises a container selection explicitly:
`PromotedItemResult.SelectionUnsupported` and `MisconfigurationReason.UnsupportedPageType` give the
editor "That page has nothing to promote" instead of the misleading "could not be loaded" they
would otherwise get.

**Why that is only a workaround.** It makes unpromotable pages *selectable* in order to make
promotable pages *reachable*. Those are different questions and the component conflates them. The
editor is offered choices the widget must then reject at render time, which is precisely the kind
of invalid state the rest of this widget is designed to prevent.

**What would fix it properly.** A way to keep the tree navigable while scoping what is
*selectable* — for example a separate "navigable content types" configuration, or having the
selector render ancestors of allowed types as expandable-but-unselectable nodes automatically. The
page selector (`WebPageSelectorComponent`) has `ItemModifierType` for disabling individual items,
which is the shape of the thing; the combined content selector has no equivalent.

**Remove this workaround** — both container types, the `SelectionUnsupported` flag, the
`UnsupportedPageType` reason and its notice — once the platform can express that distinction.

### Found at review, left for the tickets that own them

| Finding | Owner |
| --- | --- |
| `PromotedItemSource.CallToActionText` is never populated by any family, so a card with no typed CTA text renders no anchor at all and no warning | T2/T5 — needs a spec answer on the fallback label |
| `FromProduct` does not inherit `ProductSchemaImages`, though article and service both inherit an image | T2 |
| `HideElements` still has no admin form component, so every hide branch is unreachable from the UI | T2, explicitly deferred there (§13.1) |
| `ContentSource` is compared case-sensitively in the service but `OrdinalIgnoreCase` in the visibility conditions | T3 |
| In page mode with an unresolvable page, the link falls through to a stale stored `LinkUrl` | T4 |
| `target="_blank"` without `rel="noopener noreferrer"` | T7 markup |
| `ResolveOverrideImage` always queries, even when the image element is hidden | T7 |

---

## T7 — Styling, card designs and SCSS

**Goal:** spec §9 and the advanced half of §10.

**Seams: none for the visual result. No automated tests for styling** — spec §2 settles that
styling is verified by eye.

One exception worth a test:

**Seam:** the view component's CSS-class computation (it pre-computes class strings so the
Razor view stays dumb, per `ServiceWidgetViewComponent`).

**Tier:** plain xunit.

1. Card design `Standard` with a chosen colour scheme → the colour scheme class is present.
2. Card design `ImageOverlay` → colour scheme class is absent even if a scheme is set.
3. Card design `Gradient` → same as 2.

That covers the one styling rule with real branching. Everything else is by eye.

**Work:**

- `CardDesignOption` enum with the five designs.
- New shared `TextAlignmentOption` in `Features/Shared/OptionProviders/TextAlignment/`
  (spec §9.3 — the duplicate in `ServiceWidget` is left alone, deliberately).
- Advanced properties from Order 140, stepping by 10, per spec §10.
- Colour scheme hidden for `ImageOverlay` and `Gradient` via two stacked
  `VisibleIfNotEqualTo` attributes.
- `scss/_content-promotion.scss`, imported from `styles.scss`.
- Stretched-link overlay for the clickable card surface (spec §6.3). The card must contain
  no other interactive elements.

**`Gradient` has no precedent in this project** — there is no gradient anywhere in the SCSS.
Budget for it accordingly.

**Manual checks:** all five designs at desktop and mobile widths; colour scheme dropdown
appears and disappears as the design changes; compiled CSS in `wwwroot/assets/css` was
regenerated from the SCSS and never hand-edited.

### Applied after T7 shipped — responsive and visual fixes

Reported symptom: *"the image doesn't behave responsively, both on the live site and in the Page
Builder administration interface."* The manual checks above are what let this through — "all five
designs at desktop and mobile widths" was done by eye, and every defect below is invisible to the
eye but obvious in a measurement.

**How these were verified.** Not by eye. A Playwright script rendered the widget's real markup
against the shipped `styles.min.css` and read computed geometry back across viewport and container
widths. Every number below is measured, not estimated. Two harnesses were used: a synthetic one
covering all design × column-layout combinations, and the live preview URL of a real widget
instance. The synthetic harness was necessary because **the widget was not placed on any page** —
Page Builder content lives in the database, not in `App_Data/CIRepository`, so scanning all 63
published paths found zero instances to inspect.

#### Root cause shared by the two responsiveness defects

The card's width comes from the widget zone it sits in. It queried the viewport instead. Those are
almost never the same number: a three-column section on a wide screen hands each card a few hundred
pixels, and the Page Builder canvas is narrower than the window around it.

| # | Defect | Evidence | Fix |
| --- | --- | --- | --- |
| 1 | All three split layouts gated on `@media (min-width: 768px)`. The rule's own comment promised "single column below the medium breakpoint"; it never fired. | At a **1400px viewport**, a card in a 260px container still split two ways: an **80×60px** media box beside a 116px text column. | New `.c-content-promotion-wrapper` element carrying `container-type: inline-size`; split and spotlight rules moved to `@container (min-width: 34rem)`. No viewport queries remain in the partial. |
| 2 | `__media` declared both `aspect-ratio: 16/9` and `max-height: 36rem`. Below ~1024px of card width the ratio won; above it the clamp won. | Width sweep 320 → 1920: ratio held **1.78** to a 1360px viewport, then jumped to **2.08** at 1400 (where Bootstrap's `.container` steps to 1320px) and froze. The photo visibly re-cropped and stopped growing. | `max-height` removed; wide cards take a deliberate step to `21/9` via `@container (min-width: 60rem)`. |

After: the ratio is always exactly what is declared, one designed step, and the image tops out at
**1200×514 in a 647px card** instead of 1200×576 in a 709px card.

#### A regression introduced, then caught by re-measuring

Removing `max-height` stripped the ceiling that commit `df084d19` added to stop the image taking
over the card — 3/4 on a full-width card is a ~1600px image. The first attempt capped portrait
inside `@container (min-width: 45rem)`. Re-running the container sweep showed that was the wrong
instrument: at a **560px container, portrait stacked came out 683px tall where it had been 576px**.
A threshold cannot fix something whose trigger has nothing to do with a breakpoint.

Final form is an unconditional `max-width: 27rem; margin-inline: auto` on the portrait media, which
lands the ceiling at the same 36rem of height the old `max-height` enforced while keeping the box
exactly 3:4 at every width. Verified: portrait media never exceeds 576px tall at any container
width tested (260 / 360 / 560 / 760 / 1140px).

**The lesson worth keeping:** the regression was introduced *by the fix for the previous defect*,
and was caught only because the verification harness was re-run rather than trusted from the first
pass.

#### Visual fixes requested after review

| Request | What was actually wrong | Fix |
| --- | --- | --- |
| "The button is just text — make it a real pill." | The live card's CTA was already a correct pill (the `Medium` default). Rendering **all eight `LinkStyleOption` values** found the real culprits: the three "Plain link" options resolve to `.tg-bg-none`, and `button-mixin` sets `background-color: none` — not a color, so the declaration is dropped; and "Button, light 1" paints the pill white, invisible on the light cards it is paired with. Both kept the pill's padding and radius with nothing drawn around them. | `border: 1px solid currentcolor` on `.c-content-promotion__cta.tg-bg-none, .tg-bg-light-1`. Filled styles untouched. |
| "Portrait centres the image but the text is left-aligned." | The capped media is centred; the content column was not, so copy started at the card's padding edge, left of the image it sits under. | `width: 100%; max-width: 27rem; margin-inline: auto` on the content, scoped to `--portrait.--stacked`. Measured on a 760px card: media box 176 → 608, content box **176 → 608**, for all three text alignments. Split layouts deliberately untouched — image and text are in separate grid columns there. |
| "Spotlight looks too much like Standard — add a shadow." | Confirmed. | Two-layer `box-shadow` reusing the shape already used for the sticky header in `_header.scss` (broad soft cast + tight contact shadow), tinted from `$color-dark` rather than introducing a second elevation language. Follows the card's `border-radius`, so all three corner styles render correctly. |

#### Pre-existing bugs surfaced while measuring

Neither was introduced by this work; both were found because a computed value was read back rather
than glanced at.

- **Spotlight padding never applied.** `.c-card.md` is specificity 0,2,0 and
  `.c-content-promotion--spotlight` is 0,1,0, so the card's own padding always won. Confirmed
  against the **pre-change stylesheet from `HEAD`**: 24px at a 1200px viewport, never the declared
  `3rem 2rem`. Since "given room" is half of what makes spotlight distinct, this was fixed with
  `.c-content-promotion--spotlight.c-card` — qualified with `.c-card` rather than `.c-card.md` so it
  survives the widget emitting a different size class. Now measures `48px 32px`.
- **"Button, light 2" was shapeless site-wide.** `scss/_button.scss` pairs a base selector with
  `:hover` for every variant except `tg-bg-light-2`, which had only the `:hover` half — so the
  button-mixin never applied at rest. It rendered as a bare Bootstrap button (114×38, 6px radius)
  until hovered.

#### Files touched

| File | Note |
| --- | --- |
| `Features/ContentPromotion/Widgets/ContentPromotion/ContentPromotionWidget.cshtml` | Wrapper element only. The one markup change; it needs an app restart to take effect (no Razor runtime compilation in this project). |
| `scss/_content-promotion.scss` | All widget styling changes. |
| `scss/_button.scss` | **The only change outside the widget.** Compiles to `.btn.tg-bg-light-2`, so it reaches buttons and CTA links set to "Button, light 2" and nothing else — the standalone `.tg-bg-light-2` background utility is untouched, so no card backgrounds or section colour schemes change. |
| `wwwroot/assets/css/styles*.css`, `*.map` | Regenerated, never hand-edited. |

**Tests:** no new automated tests — spec §2 still settles that styling is verified by eye, and T7's
three class-computation tests are unaffected. Full suite after the changes: **168 web + 1 admin
passing, 0 failing.**

#### Not verified

The claim that the Page Builder canvas renders in an iframe, and therefore resolves media queries
against canvas width rather than window width — making the same card crop differently in the admin
than on the live site — **is inference, not measurement.** No admin credentials were available, and
an out-of-origin iframe harness was blocked by `X-Frame-Options: SAMEORIGIN`. It follows from the
defect mechanism and matches the reported symptom, but it was never observed in the running admin
UI. Treat it as a hypothesis.

#### Still open

**The image is not responsive in the responsive-images sense.** The `<img>` emitted by
`tg-styled-image` has no `srcset`, no `sizes` and no `loading`. The live card downloads the full
**1920×1371, 350 KB** original and paints it into a 216px-wide box at a 320px viewport. Xperience's
`getContentAsset` endpoint accepts `width` / `maxSideSize`, so a `srcset` is feasible — but the tag
helper is shared with `ServiceWidget` and `ServicePagePageTemplate`, so it is a separate change with
a wider blast radius than anything above.

#### Tooling note for anyone repeating this

`AGENTS.md` lists build, test, CI and codegen commands but **no way to compile SCSS**, and the
`design-conventions` skill states the pipeline is driven by the Live Sass Compile VS Code extension
— which an agent cannot invoke, while the same skill requires regenerated CSS to be committed.

Compiling with plain `sass` **silently drops autoprefixer output** (8 × `-o-object-fit`, among
others) across the whole stylesheet. This was caught on diff review, reverted, and recompiled
through `sass` + `postcss`/`autoprefixer`, confirming the prefix count matched the previously
committed output. A documented headless command would remove the trap.

---

## T8 — Localization

**Goal:** spec §11. Can be worked any time after T1, but is easiest once the property list
has stopped moving (after T7).

**Seams: none. No automated tests.** Resource resolution is framework behaviour.

**Work:**

- Replace every property label, explanation and option label with
  `{$TrainingGuides.ContentPromotionWidget.*$}`, following `CallToActionWidgetProperties`.
- Add keys to all three admin resx files: `en-US`, `es-MX`, `fr-FR`.
- Add rendered front-end strings via `IStringLocalizer<SharedResources>`, with translations
  in `SharedResources.es.resx`.

**Accepted gap:** no `SharedResources.fr.resx` exists in this project, so rendered strings
are Spanish-only while property labels are Spanish and French. Deliberate — see spec §11.

**Manual check:** switch the admin UI language and confirm labels resolve; confirm no raw
`{$…$}` macros leak into the UI (the usual symptom of a missing key).

---

## T9 — Click activity tracking

**Goal:** spec §6.4. Last, because it depends on a working card with a working link.

**Seam:** the controller action on `ContentPromotionActivityController`.
`ICustomActivityLogger` and `ICookieConsentService` are substituted with Moq.

**Tier:** xunit + Moq.

**Acceptance criteria — one test each:**

1. Contact can be tracked → the activity is logged once, with the widget's tracking value as
   `ActivityValue`.
2. `CurrentContactCanBeTracked()` is false → **nothing is logged**, and the response is still
   a success. Consent gating is the point; a failure response would surface a console error
   on a perfectly normal no-consent visit.
3. Tracking value is empty → nothing is logged.

**Work not covered by tests:**

- Create the `contentpromotionclick` custom activity type in the admin UI, then
  `./scripts/CIStore.ps1`, and commit the resulting XML under
  `App_Data/CIRepository/@global/om.activitytype/`. This is the widget's only
  `CIRepository` change.
- The JS click handler on the card anchor.
- Secure the endpoint — see the "Secure custom endpoints" page in the widget skill's docs map.

**Unverified (spec §13.4):** custom activity type creation and the current
`ICustomActivityLogger` contract. Verify against the Kentico Docs MCP before implementing.

**Manual check:** click the promo with consent granted and confirm the activity appears in
Contact management; repeat with consent withheld and confirm nothing is logged.

---

## Open questions to settle before the tickets they block

| Question | Blocks | Spec ref |
| --- | --- | --- |
| Admin form component for a checkbox-list multi-select | T2 implementation (not its tests) | §5, §13.1 |
| ~~Tag display-name resolution from `TagReference`~~ | ~~T6a~~ — settled, see T6 | §13.2 |
| ~~`IProductService` stock-by-content-item-ID surface~~ | ~~T6c~~ — settled, see T6 | §13.3 |
| Custom activity type creation + `ICustomActivityLogger` contract | T9 implementation | §13.4 |
| Whether `VisibleIfEmpty` exists, for the either-or link rule | T4 implementation | §6.1, §13.5 |

All five are facts to verify against the Kentico Docs MCP at the start of the ticket that
needs them — none needs a decision from you. Link precedence is settled: the page wins.

Only the last one can change the design: if `VisibleIfEmpty` does not exist, the fallback
`LinkType` radio takes the property count from 20 to 21.

---

## T10 — Integration test infrastructure (added during T3)

**Goal:** cover the retrieval queries this feature added, which unit substitutes cannot reach.

**Why this exists:** T3 added a non-generic `RetrieveContentItemByGuid` to
`IContentItemRetrieverService` so the content hub branch can retrieve an item whose content
type is not known in advance. The widget's unit tests substitute that interface, so **nothing
verifies the query itself.** The `automated-tests` skill requires at least one integration
test per retrieval adapter, and the user has confirmed integration tests are wanted rather
than the gap being recorded as out of scope.

**The obstacle:** the solution has no integration-test infrastructure. Both test projects are
xunit + Moq with no `Kentico.Xperience.Core.Tests` reference. The skill's tiers
(`IntegrationTests`, `IsolatedIntegrationTests`) are NUnit-based, so adding them introduces
a second test stack alongside the 123 existing xunit tests.

**Decide before starting:**

1. NUnit integration project alongside the xunit unit projects (two stacks, follows the skill), or
   an xunit integration project wired to a test database by hand (one stack, diverges from the skill).
2. Which project it lives in — a new `TrainingGuides.Web.IntegrationTests`, or a marked
   category inside the existing test project.
3. How the test database is prepared and where its connection string comes from in CI.

**At minimum this must cover:**

- `RetrieveContentItemByGuid` (non-generic) returns the correct item for each of the seven
  allowed content hub types, mapped to the right concrete type so the family pattern match works.
- It returns null for a GUID that does not exist, rather than throwing.

**Until this lands,** verify by hand: select each of the seven hub types in the admin and
confirm the card renders with the right title and description.
