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
| T5-T9 | Not started |
| T10 integration tests | Not started, added during T3 |

Full web suite at the T4 stop point: **139 passing, 0 failing.**

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
| Tag display-name resolution from `TagReference` | T6a implementation | §13.2 |
| `IProductService` stock-by-content-item-ID surface | T6c implementation | §13.3 |
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
