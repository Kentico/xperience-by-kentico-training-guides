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
| T11 product card defects | Not started, reported after T9 |
| T12 CTA editor notices | Done — 2 tests, notice only (no fallback label) |
| T13–T21 pre-publish review fixes | Not started, raised at code review 2026-10-06 — see [Pre-publish code review](#pre-publish-code-review) |

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

### Page selector reachability — RESOLVED, workaround being removed in T13

> **Resolved 2026-10-06.** The reachability problem was a bug, and the workaround below is no longer
> necessary. T13 removes it. The analysis is kept for history only.

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
| `FromProduct` does not inherit `ProductSchemaImages`, though article and service both inherit an image | T2 — never picked up there; reassigned to **T11, defect 1** |
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
$$
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

---

## T11 — Product promotions render wrong: no image, no extras, escaped HTML description

**Reported symptom:** *"the variant of the content promotion widget that is supposed to display
product page data doesn't display correctly the image of product, nor the additional type data. the
product description also shows html tags instead of the clean text."*

Three separate defects, one of which is not product-specific at all. Each is analyzed below with the
code that causes it; none is speculative — all three are readable in the current source.

### Defect 1 — the product family never inherits an image

`ContentPromotionService.FromProduct` (`ContentPromotionService.cs:126`) builds a
`PromotedItemSource` with `Title` and `Description` and **no `Image`**. `FromArticle` and
`FromService` both set one. So a product promotion can only ever show an image if the editor
uploads an override, and the symptom is "no product image" in both page and content hub mode.

This was already recorded in T6's *"Found at review, left for the tickets that own them"* table and
assigned to T2, where it was never picked up. T11 owns it now.

**Why it is not a one-line fix.** Articles and services carry `IEnumerable<Asset>`
(`ArticleSchemaTeaser`, `ServiceMedia`), which `GetImage(Asset?)` already maps.
`IProductSchema.ProductSchemaImages` is `IEnumerable<ProductImage>` — a content type of its own
(`ProductImage.generated.cs`) holding a `ContentItemAsset ProductImageAsset` plus a separate
`ProductImageAltText` string. There is no `Asset` anywhere in that chain, so
`AssetViewModel.GetViewModel` cannot be reused; a second mapping is needed, the way
`ProductService.GetImageViewModels` (`ProductService.cs:355`) does it — `image.ProductImageAsset.Url`
and `image.ProductImageAltText`.

**Parent/variant fallback.** A `ProductPage` links a *parent* (`CatFood`, `DogCollar`), and parents
may carry no images of their own while their variants do. `ProductService.GetViewModel` handles this
with `GetImageViewModels(variant).UnionBy(GetImageViewModels(parent))`. The card needs one image, so
the rule is simpler: the parent's first image, falling back to the first image of its first variant.

### Defect 2 — product extras are unreachable from a `ProductPage`

`ResolveExtras` (`ContentPromotionService.cs:185`) dispatches on
`IProductPriceSchema variant => await ProductExtras(variant)`. Only variants implement
`IProductPriceSchema`:

| Type | Implements | Reached by `ResolveExtras` |
| --- | --- | --- |
| `CatFoodVariant`, `DogCollarVariant` | `IProductSchema, IProductSkuSchema, IProductPriceSchema, …` | yes |
| `CatFood`, `DogCollar` | `IProductSchema, IProductParentSchema` | **no** |
| `ProductPage` → `ProductPageProducts` | `IEnumerable<IProductSchema>`, in practice a parent | **no** |

So every product *page* promotion — the exact case the user reported — falls through to
`_ => new PromotionExtrasViewModel()`, `HasContent` is false, and the extras block is not emitted.

**This is spec §7.2 behaving as written, and the spec is wrong.** T6c criteria 2 and 3 assert
"parent → no price, no stock" and call it *"correct behaviour, not a bug."* The reasoning was that a
parent has no price of its own. But the rest of the site disagrees with that reasoning in public:
`ProductListingWidget` renders a price and a stock status for exactly these parent products, by
falling back to the first variant (`ProductService.cs:230-237`) and to `GetListingStockForProduct`
(`ProductService.cs:725`). A promotion card for a product page that shows nothing, sitting on a page
whose product listing shows `$24.99 · In stock`, reads as broken regardless of what the spec says.

**Decision for this ticket: match the listing.** A parent inherits the first variant's catalog price
and the listing stock status. Nothing new is invented — both rules already exist and are already
shipped; they are just private.

**Interface change required.** `GetListingStockForProduct` is private on `ProductService`. It goes
public and onto `IProductService`, exactly as T6 did for `GetCatalogPrice` and
`GetProductStockStatus`. It already folds in the SKU-level check and the variant walk, so no stock
logic is written here either.

**This inverts two shipped tests.** T6c criteria 2 and 3 currently assert the empty-extras
behaviour. They are not deleted — they are rewritten to assert the fallback, and the spec §7.2
paragraph is amended to say so. Changing a test to match new behaviour is only legitimate when the
behaviour was decided to be wrong; that decision is recorded here.

### Defect 3 — descriptions are rich text rendered as encoded text

The Razor view renders
`<p class="c-content-promotion__description">@Model.DisplayValues.Description</p>`
(`ContentPromotionWidget.cshtml:63`). Razor HTML-encodes it, so stored markup arrives on screen as
visible `<p>` and `<strong>` tags.

**This affects all three families, not products.** Every other consumer in the repository treats
these same fields as HTML:

| Field | Consumer | Treatment |
| --- | --- | --- |
| `ProductSchemaDescription` | `ProductService.cs:198-199` | `new HtmlString(...)` |
| `ArticleSchemaSummary` | `ArticlePageService.cs:37` | `new HtmlString(...)` |
| `ServiceShortDescription` | `ServicePageService.cs:34`, `ServiceComparatorWidgetViewComponent.cs:103`, `HeroBannerWidgetViewComponent.cs:160` | `new HtmlString(...)` |

The widget is the only place that does not. The user noticed it on a product because product
descriptions in this project carry the most markup; article and service cards have the same bug.

**The override complicates it.** `ContentPromotionWidgetProperties.Description` is a
`TextAreaComponent` — plain text, not a rich text editor. Rendering an author's typed `<` raw would
be wrong, and swapping the component to a rich text editor changes authoring for a field whose whole
purpose is a short plain override.

**Recommended:** `ContentPromotionDisplayValues` gains `DescriptionHtml` (an `HtmlString`) alongside
or in place of the string. Inherited rich text passes through unchanged; a typed override is
HTML-encoded before wrapping, so it stays literal. `DisplayValues.HasContent` must then test the
underlying string, not the `HtmlString`, or a card with only a description starts reporting
`NothingAuthored`.

**Alternative, if the team prefers one path:** switch the override property to
`RichTextEditorComponent` and pass both through raw. Cheaper in code, more expensive in authoring,
and it makes the property inconsistent with `Title` and `CallToActionText`. Not recommended.

---

### Seams and tests

Following the file's house rules: confirm each seam before writing its test, one criterion at a
time, red before green. Same stack as every other ticket — plain xunit + Moq, no new packages,
pattern copied from `LinkOrSignOutWidgetViewComponentTests`.

**Seam A — `ResolvePromotedItem`'s product projection.** `IContentItemRetrieverService` substituted.

1. `contentItem` mode, a `CatFoodVariant` with `ProductSchemaImages` → the card image URL is the
   variant's `ProductImageAsset.Url`, and its alt text is `ProductImageAltText`.
2. `page` mode, a `ProductPage` whose parent carries images → the parent's first image.
3. `page` mode, parent with **no** images but a variant that has one → the variant's image.
4. Parent with no images and no variant images → no image, not an empty one. (`GetImage`'s existing
   empty-model trap, per T5.)
5. The override image still beats the inherited product image, and `HideElements` still beats both —
   the T2 rule must not have been bypassed by a family-specific path.

**Seam B — `ResolveExtras` product dispatch.** `IProductService` substituted.

6. Promoted content is a variant → price and stock as today. (Regression guard on T6c 1, 4-6.)
7. Promoted content is a parent with variants → the first variant's catalog price appears.
8. Promoted content came from a `ProductPage` → same as 7.
9. Parent whose listing stock is `Unknown` → stock omitted, price still shown.
10. Parent with no variants at all → no price, no stock, empty extras. The one case where the
    original T6c behaviour survives.

**Seam C — description resolution.** Pure, no mocks.

11. Inherited description containing markup → survives to the view model unescaped.
12. Typed override containing `<` → encoded, rendered literally.
13. Override still beats inherit, hide still beats both. (Regression guard on T2.)
14. Description present but title empty → still not `NothingAuthored`.

**Not covered by tests, verify by hand:** that a real `ProductPage` promotion renders image, price
and stock together; that article and service descriptions also lost their visible tags; and that the
admin textarea override has not started accepting markup.

---

### Front-end work

**FE1 — the description is now a rich text container.** `.c-content-promotion__description` is
currently `margin: 0` on a `<p>` (`scss/_content-promotion.scss:82`). Once it emits stored markup it
holds its own `<p>`, `<ul>` and `<strong>` children, each with Bootstrap's default bottom margin, so
the card's internal rhythm breaks and the last child adds a trailing gap above the CTA. The element
also has to stop being a `<p>` — block children inside a paragraph are invalid and the browser will
close the paragraph early, dropping them outside the styled box.

- Change the element to a `<div>` in the view.
- Style descendants: reset the last child's bottom margin, give paragraphs and lists a consistent
  rhythm, and keep the font size inherited from the card rather than from Bootstrap's defaults.

**FE2 — links inside the description break the stretched link.** Spec §6.3 and the view's own
comment require the card to contain exactly one interactive element, because `.stretched-link`
covers the whole card. A rich text description can contain anchors, which then sit *under* the
stretched link and are unreachable, while still being focusable by keyboard — a card that tab-stops
onto a link that cannot be clicked.

Decide one of:

- **Strip anchors** from the inherited description before it reaches the view (backend, and then it
  is Seam C's business). Honest about the constraint, lossy for the editor.
- **Drop `stretched-link`** when the description contains an anchor, leaving the CTA as the only
  clickable element. Keeps the content intact, makes the card's clickability inconsistent between
  instances.

Recommend the first: the card is a promotion, not an article body, and its one destination is the
point of the widget. Either way this needs a decision before FE1 is finished, since both change the
same markup.

**FE3 — product extras now actually render, for the first time.** `__price` and `__stock`
(`scss/_content-promotion.scss:109-118`) have never been seen on screen: before this ticket no
product promotion ever reached them, and the styling shipped in T7 unverified. Check them against
the other two families' extras, which *have* been seen:

- price and stock sit on one flex line with `gap: 0.5rem`, so `$24.99` and `In stock` run together
  without a separator;
- `__stock` is `opacity: 0.8` with no state colouring, so out-of-stock reads identically to in-stock;
- on the `ImageOverlay` and `Gradient` designs, extras sit over the image — `__category` has a
  gradient-specific override at line 216, `__price` and `__stock` have none.

Verify by measurement, not by eye — T7's "verified by eye" note is what let its defects through. The
Playwright harness described in T7 is the precedent.

**FE4 — no front-end change for the image itself.** The `<img>` path is already correct; defect 1 is
purely that nothing is ever handed to it. The missing `srcset`/`sizes` recorded in T7's "Still open"
is unchanged by this ticket and stays out of scope.

---

### Unverified — check before implementing

- **Linked-item field population at depth.** `ProductPage` → parent → `ProductSchemaImages` →
  `ProductImage` → `ProductImageAsset` is three hops; `LINKED_ITEMS_DEPTH` is 3, which covers it on
  paper. But `ProductImageAsset` is a content type-specific field on `ProductImage`, and T6 learned
  the hard way that content type-specific fields need `WithContentTypeFields()` and
  `OfContentType(...)` — at the *top level* of the query. Whether linked items returned by
  `WithLinkedItems(depth)` carry their own type-specific fields is **not confirmed here**. It
  evidently works for `ProductService`, which reads the same chain, but that is a different query.
  Confirm against the Kentico Docs MCP (*Reference — Content item query*) before concluding the
  image mapping is the only thing missing; if linked items come back bare, defect 1 is a retrieval
  bug as well as a mapping one, and the unit tests above will not catch it.
- **Variant stock at depth.** `GetListingStockForProduct` queries stock separately, so depth does
  not apply — but it does need the parent's `ProductParentSchemaVariants` populated.
  `ProductParentSchema` is a reusable schema, which `ForContentTypes` includes, so this should hold.

### Suggested order

Defect 3 first — it is the smallest, it is the only one that is not product-specific, and FE1
depends on it. Then defect 1, then defect 2 with its interface change and its two rewritten T6c
tests. FE3 last, once there is finally something to look at.

**Spec updates this ticket must make:** §7.2 (parent price and stock fallback) and §7.1 if the
omission rule needs rewording. Leaving the spec saying the opposite of the code is how T6c's
criteria came to be wrong in the first place.

---

## Pre-publish code review

Raised by a senior-developer review on 2026-10-06, before publishing the widget to the Training
guides repository. Baseline at review time: the solution builds and the 82 ContentPromotion tests
pass (`--filter "FullyQualifiedName~ContentPromotion"`) — none of them catch T12 or T13.

Same house rules as every ticket above: confirm the seam, one criterion at a time, red before green.
Tickets marked **refactor** change no behaviour; the existing tests are the safety net and must stay
green without edits other than renames.

| Ticket | Kind | Priority |
| --- | --- | --- |
| T12 CTA fallback — no silent unclickable card | Bug | Blocker |
| T13 Remove the page selector workaround | Cleanup + drift guard | Blocker |
| T14 `HideElements` admin form component | Unfinished feature | Blocker |
| T15 Cached retrieval and cheaper link lookup | Performance | High |
| T16 Link and source-mode correctness leftovers | Bug | High |
| T17 Culture-aware price formatting | Bug | Medium |
| T18 Activity endpoint hardening | Correctness / security | Medium |
| T19 Click logger script | Quality | Low |
| T20 Service and model cleanup | Refactor | Low |
| T21 Comment pass for public readers | Docs | Low |

Order: T13 first (it deletes code the others would otherwise touch), then T12 and T14, then T16,
T15, the rest in any order. T21 last, so it reads the final code.

---

### T12 — CTA fallback: never render a card that cannot be clicked

**Problem.** The anchor renders only when `Link is not null` **and** `CallToActionText` is non-empty
(`ContentPromotionWidget.cshtml:122`). `PromotedItemSource.CallToActionText` is never populated by
`FromArticle`, `FromProduct` or `FromService`, so in page and content hub mode the card has a
destination but no anchor unless the editor types CTA text. The result: no stretched link, no click
tracking, and `MisconfigurationReason.None`, so edit mode shows no notice. Hiding the CTA through
`HideElements` produces the same card. Spec §6.2 calls silent degradation "the worst outcome", and
§13 / the localization section already anticipate "CTA fallback text".

**Decision (2026-10-06): notice only, no fallback label.** The public card stays as it is; the
editor is told in edit mode. Two new warnings — not misconfigurations, the public still sees the
card — in `MisconfigurationReason`: `CallToActionMissing` (destination, no CTA text) and
`CallToActionHidden` (destination, CTA hidden). The view model gets a `CallToActionHidden` flag set
by the view component from `HideElements`, because the resolved text is empty either way.

**Implemented (T12 done).** Seam: `ContentPromotionWidgetViewModel.MisconfigurationReason` only.
Tests: `DestinationButNoCallToActionText_…ReportsCallToActionMissing` and
`DestinationButCallToActionHidden_…ReportsCallToActionHidden`. The `FullyConfigured` and
`OnlyADescriptionWasResolved` fixtures now carry CTA text — a link with no CTA text is no longer
"fully configured". Spanish strings added to `SharedResources.es.resx`. `CallToActionHidden` is
only reachable once T14 gives `HideElements` a form component. `PromotedItemSource.CallToActionText`
is left for T20.

The fallback-label plan below is kept for reference and was **not** implemented.

**Seam:** `IContentPromotionService.ResolveDisplayValues` (pure) and
`ContentPromotionWidgetViewModel.MisconfigurationReason` (pure).

**Acceptance criteria — one test each:**

1. Destination present, no typed CTA, nothing inherited → `CallToActionText` is the fallback label.
2. Typed CTA text beats the fallback.
3. CTA in `HideElements` → no CTA text, and no fallback.
4. CTA hidden with a destination present → a new `MisconfigurationReason.CallToActionHidden`
   (warning, not misconfigured — the public still sees the card).
5. No destination → no fallback is invented (still `NoDestination`).

**Also:** remove `PromotedItemSource.CallToActionText`, which no family fills, or populate it — do
not leave it dead. Add the fallback label and the new notice to `SharedResources.es.resx`.

**Manual check:** a page-mode article promotion with an empty CTA field renders a clickable card
and logs a click activity.

---

### T13 — Remove the page selector workaround

**Context.** The T6 workaround (*Page selector reachability*) was a bug, not a platform limitation,
and is no longer necessary. The working tree already removes `EmptyPage` and `StoreSection` from the
`SelectedPage` selector, and `Interview` from the `SelectedContentItem` selector — but **only in the
attribute**. Everything built around the workaround is still there, and the two lists retrieval
uses now disagree with the selectors.

**Remove:**

- `ContentPromotionContentTypes.CONTAINER_PAGES`; `PAGES` becomes the three promotable types
  (fold `PROMOTABLE_PAGES` into it).
- `Interview` from `ContentPromotionContentTypes.CONTENT_ITEMS`, matching the selector.
- The `EmptyPage or StoreSection` arm in `ResolvePromotedItem`, and `PromotedItemResult.SelectionUnsupported`.
- `MisconfigurationReason.UnsupportedPageType`, its branch in the view model, the view's
  "That page has nothing to promote" notice, and its `SharedResources.es.resx` entry.
- The tests covering those paths in `ContentPromotionServiceTests` and
  `ContentPromotionWidgetViewModelTests`.
- The stray comments: "see the note on the SelectedPage property" in the service, and the
  "Keep in step with ContentPromotionContentTypes.PAGES" comment, which currently sits between
  `ExplanationText` and `MaximumItems` instead of on the list it describes.

**Add — a drift guard, so this cannot recur silently.** Attribute arguments must be compile-time
constants, so the selector lists cannot share the arrays directly.

**Seam:** reflection over `ContentPromotionWidgetProperties`.

1. The content types allowed by `SelectedPage`'s `ContentItemSelectorComponent` equal
   `ContentPromotionContentTypes.PAGES` (order-insensitive).
2. The content types allowed by `SelectedContentItem` equal `ContentPromotionContentTypes.CONTENT_ITEMS`.

**Docs:** mark the T6 *Page selector reachability* section as resolved — it was a bug — and drop
the "report to Kentico" heading. Update spec references to `UnsupportedPageType` if any.

**Manual check:** in the admin, select a `ProductPage` under `/Store` and a `ServicePage` under
`/Products` with the narrowed selector — both must be reachable.

---

### T14 — `HideElements` needs an admin form component

**Problem.** `HideElements` has no form component, and the property carries a
`NOTE: ... not settled yet` comment. Every hide branch in the service, view model and tests is
unreachable from the admin UI. This is open question §13.1, deferred since T2. A published widget
must not ship a feature editors cannot use.

**Decide first:** implement it (a multi-select — for example `GeneralSelectorComponent` or a
checkbox-list component with a data provider over `ContentPromotionElement`), or remove the property
and every hide branch. Spec §5 argues it is needed for personalization variants, so implementing is
the expected answer. Verify the component's current API against the Kentico Docs MCP.

**Seam:** the data provider, if one is written — it returns exactly the four `ContentPromotionElement`
values with localized labels. The resolution logic is already covered by T2's tests.

**Work not covered by tests:** the attribute, localization keys in all three admin `.resx` files,
removing the `NOTE` comment.

**Manual check:** hide each element in turn in the admin and confirm the card drops it; confirm a
stored value survives a save-and-reopen.

---

### T15 — Cached retrieval and a cheaper link lookup

**Problem.**

1. The non-generic `RetrieveContentItemByGuid(guid, types, …)` and
   `RetrieveWebPageByContentItemGuid(guid, types, …)` added for this widget call
   `IContentQueryExecutor` directly, which is **not cached**. Every render of every widget instance
   queries the database. The generic overloads go through `IContentRetriever`, which caches.
2. `RetrieveLinkTargetPage` retrieves the link target with `LINKED_ITEMS_DEPTH` (3) only to build a
   URL — linked items are never read.
3. Worst case, one card makes about seven sequential I/O calls (item, override image, link page,
   URL, taxonomy, price calculation, stock), including the full `IPriceCalculationService` pipeline.

**Do:**

- Move both non-generic methods onto `IContentRetriever` (`RetrieveContentOfContentTypes` /
  `RetrievePagesOfContentTypes` with a `Where` on `ContentItemGUID`), so results are cached with
  correct dependency keys. **Unverified:** these method names and signatures come from the comment at
  `ContentItemRetrieverService.cs:380`, not from the docs — confirm against the Kentico Docs MCP,
  including whether they return content type-specific fields without `WithContentTypeFields()`.
- Retrieve the link target at depth 0 with no content type fields.
- Do **not** parallelise with `Task.WhenAll` — Kentico data APIs in one request scope are not meant
  for concurrent use.
- Note the per-card cost of price calculation in the guide text, or cache the extras.

**Seam:** existing service tests stay green; assert the depth passed for the link target is 0.
Retrieval correctness belongs to **T10** — this ticket makes T10 more urgent, not optional.

**Manual check:** with SQL profiling or Xperience debug on, a second load of a page with the widget
issues no content queries for it. Publishing a change to the promoted item still updates the card
(cache dependency works).

---

### T16 — Link and source-mode correctness leftovers

Two findings from the T6 *left for the tickets that own them* table that were never closed.

1. **Stale `LinkUrl` in page mode.** In page mode with an unresolvable page, `ResolveLink` falls
   through to `properties.LinkUrl` — a value hidden in page mode but still stored from an earlier
   content hub or manual configuration. The card then links somewhere the editor cannot see.
2. **Case-sensitive source comparison.** `ResolvePromotedItem` and `ResolveLink` compare
   `ContentSource` with `==`, while the visibility conditions use `OrdinalIgnoreCase`.

**Seam:** `ResolveLink` and `ResolvePromotedItem`.

1. Page mode, page resolves to null, `LinkUrl` stored → link is null.
2. Content hub mode, no target page, `LinkUrl` set → link is `LinkUrl` (regression guard).
3. `ContentSource = "MANUAL"` behaves like `manual`; `"ContentItem"` like `contentItem`.

---

### T17 — Culture-aware price formatting

**Problem.** The view prints `$@Model.Extras.Price.Value.ToString("n2")`
(`ContentPromotionWidget.cshtml:97`): a hard-coded dollar sign plus the current culture's number
format, so es-MX and fr-FR get a dollar sign with local separators. `ProductListingWidget` uses
`ToString("C")`, so the card and the listing can disagree — which T6 promised would not happen.

**Do:** format the price the same way the listing does. Better, move the formatting into one shared
helper used by both, so they cannot drift.

**Work not covered by tests:** the view change. If a shared helper is extracted, test it with an
explicit culture.

**Manual check:** the same product shows the identical price string in the listing and in a
promotion card, in each site language.

---

### T18 — Activity endpoint hardening

**Problems in `ContentPromotionActivityController`:**

1. `ActivityTitle` is `"Content promotion click - " + value`, up to 276 characters, while only the
   value is capped at 250. **Unverified:** check the `OM_Activity.ActivityTitle` column length; if
   it is 250, cap or truncate the title.
2. `requestModel?.` is redundant — MVC model binding always creates a complex-type parameter. Add
   `[FromForm]` to make the binding source explicit.
3. It uses a classic constructor while the rest of the feature uses primary constructors.
4. Like `/pagelike`, it is an anonymous POST without antiforgery. A `sendBeacon` form post is a
   CORS simple request, so another site can forge click activities for a consenting visitor. Low
   impact and consistent with the existing pattern — but the remarks block should state it, not
   only argue for anonymous access.

**Seam:** the controller action.

1. A 250-character tracking value → the logged `ActivityTitle` fits the column limit.
2. Existing T9 criteria stay green.

---

### T19 — Click logger script

**Problems in `wwwroot/assets/js/ContentPromotionActivityLogger.js`:**

- `handleContentPromotionClick` is a global function — wrap the script in an IIFE.
- It binds per-element on `load`. Use one delegated `document` listener with
  `event.target.closest(".js-content-promotion-link")`.
- Middle-click and Ctrl+click fire `auxclick`, not `click`, so they are not tracked.
- The `sendBeacon` return value is ignored; a refused beacon should fall back to `fetch`.
- The endpoint path is duplicated between the script and the controller route — consider emitting
  it into a `data-` attribute from the view.

**No automated tests** (no JS test stack). **Manual check:** left-, middle- and Ctrl-click each log
exactly one activity with consent granted, and none without.

---

### T20 — Service and model cleanup (refactor)

- `ContentFamily` / `PromotedItemResult.Family` is set and never read. Remove it, or use it to drive
  `ResolveExtras` instead of a second type switch.
- `PromotedItemResult` encodes state as flags (`SelectionFailed`, and until T13 `SelectionUnsupported`)
  that `ResolvePromotedItem` derives from each other. Replace with one status enum.
- `ContentPromotionContentTypes` exposes `public static readonly string[]` — any caller can mutate it.
  Use `ImmutableArray<string>` or `IReadOnlyList<string>`.
- `PromotionExtrasViewModel.Categories` / `Benefits` are `IEnumerable<string>` but always lists, and
  `HasContent` and the view enumerate them repeatedly — use `IReadOnlyList<string>`.
- `.Join(" ")` from `CMS.Helpers` → `string.Join`. Drop `?? string.Empty` on non-nullable properties.
- Pass `HttpContext.RequestAborted` down to `GetCatalogPrice`, which already accepts a token.
- Group `ContentPromotionService` members: public API first, then the `From*` mappers together
  (`FromService` currently sits after `ResolveDisplayValues`), then helpers.
- **Decide:** `WithoutAnchors` edits HTML with a regex. Readers of a training repo will copy it.
  Either switch to an HTML parser (AngleSharp or HtmlAgilityPack — a new package, add it in
  `Directory.Packages.props`) or keep the regex with its limits stated, as now.
- **Optional:** `IContentPromotionService` takes `ContentPromotionWidgetProperties` (a widget-layer
  type) and exposes five steps the view component must call in order. A single
  `BuildCard(properties)` would be a deeper module; weigh it against the tests that target each step.

**Seam:** none new — all existing tests stay green.

---

### T21 — Comment pass for public readers

The feature's comments are far denser than the rest of the repository, and some point at things a
reader of the published repo cannot see.

- Remove or link references to "spec section 6.3", "spec section 7.1" and "the spec names".
- Cut comments that argue with rejected alternatives instead of explaining the code — for example
  the `ResolveDescription` remarks on "a second copy of the ordering rule".
- Keep the *why* comments that a learner needs (stretched-link single anchor, catalog price vs
  schema price, rich text vs encoded override); aim for roughly half the current volume.

**No automated tests.** Run `dotnet format` and the full web suite afterwards.
