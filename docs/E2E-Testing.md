# End-to-end testing

Playwright tests that drive the running site the way an editor and a visitor would: through the
administration, Page Builder and the live site. They complement the unit tests in
`src/*.Tests`, which cover the logic behind a widget but cannot show that the whole flow works.

The first suite covers the content promotion widget: an editor adds it to a new page, configures
it in manual mode and publishes the page, then a visitor sees the card, gives cookie consent and
clicks it, which logs the click activity.

## Layout

Tests are grouped by feature (vertical slices), not by technical type. A feature folder holds its
specs and the page objects only it uses.

```text
tests/
├── playwright.config.ts         # projects, webServer, timeouts
├── package.json                 # npm scripts
└── e2e/
    ├── shared/config.ts         # every environment variable, with defaults
    ├── admin/                   # signing in, plus page lifecycle any admin test can reuse
    │   ├── auth.setup.ts
    │   └── websiteChannelPages.ts
    └── content-promotion/
        ├── contentPromotionEditor.ts
        └── content-promotion.spec.ts
```

The config defines two projects:

- **`admin-setup`** signs in once and saves the session to `tests/playwright/.auth/admin.json`
  (git-ignored).
- **`admin`** depends on it and starts every test already signed in through `storageState`.

## Setup

1. Have a working local database (see the repository README).
2. Store the admin credentials in the web project's user secrets, next to the site's other local
   secrets. They are never committed:

   ```powershell
   dotnet user-secrets set "E2E:AdminUsername" "<user>" --project src/TrainingGuides.Web
   dotnet user-secrets set "E2E:AdminPassword" "<password>" --project src/TrainingGuides.Web
   ```

   The tests read the `secrets.json` file directly, so no shell restart is needed. Environment
   variables (`XBK_ADMIN_USERNAME`, `XBK_ADMIN_PASSWORD`) take precedence, which is how CI supplies
   them. Every setting and its default is in `tests/e2e/shared/config.ts`.

3. Install the test dependencies:

   ```powershell
   cd tests
   npm ci
   npm run install:browsers
   ```

## Running

From `tests/`:

| Command | What it does |
| --- | --- |
| `npm test` | Runs everything, headless |
| `npm run test:headed` | Same, with a visible browser |
| `npm run test:ui` | Playwright UI mode, for stepping through a test |
| `npm run report` | Opens the last HTML report, with traces of failed tests |

If the site is already running, the tests use it. Otherwise Playwright starts it with
`dotnet run`. In CI it always starts a fresh one.

## Test data

Each test creates what it needs, with a unique name, and removes it afterwards, even when the
test fails. The content promotion spec does this with a Playwright fixture (`testPage`). It creates
an Empty page named `e2e-content-promotion-<timestamp>`, then moves it to the recycle bin and
deletes it permanently. Because the site uses Continuous Integration, check that a run leaves
`App_Data/CIRepository` unchanged.

A run does leave contacts and "Content promotion click" activities behind, because the visitor in
the test is an ordinary anonymous visitor. They are marked by tracking values that start with `e2e-`.

## Writing selectors

In priority order:

1. Role and accessible name: `getByRole('button', { name: 'Publish', exact: true })`
2. Label: `getByRole('textbox', { name: 'Link URL' })`
3. Stable visible text: `getByText('The page has been published.')`
4. `data-testid` or `title`, only where the admin UI offers nothing better
5. Attributes from this project's own code, such as `data-component-identifier` or the widget's
   CSS classes

### Xperience admin and Page Builder gotchas

These were found by exploring the UI with the Playwright MCP and are not visible in any config:

- **Page Builder is an iframe**: `page.getByTestId('page-builder').contentFrame()`.
- **Page Builder controls have no accessible names.** Use their titles (`getByTitle('Add widget')`,
  `'Configure widget'`). In the widget picker, use the identifier from `ComponentIdentifiers.cs`:
  `[data-component-identifier="TrainingGuides.ContentPromotionWidget"]`.
- **The widget properties dialog is in the admin page**, not in the iframe, and it is a normal
  labelled form.
- **Admin radio buttons sit under a styled overlay**, so clicking the radio itself times out. Click
  the label text inside the `radiogroup`, then assert the radio is checked.
- **Some confirmation buttons reuse the name of the action that opened them** (for example
  "Move to recycle bin"). Take `.last()`, or use the dialog's `confirm-action` test ID.
- **Admin-mode links are rewritten** through `/cmsctx/...`. Assert real `href` values on the live
  site, not in Page Builder.
- **A stretched-link card swallows clicks on its children.** Playwright reports that the title
  "intercepts" the click, which is the intended behaviour. Click the card itself at a position
  instead: `card.click({ position: { x: 20, y: 20 } })`.
- **Cookie consent takes effect on the next request.** Wait for the
  `/cookies/cookiebannersubmit` response, then reload. Consent-gated scripts are only served
  after that.
- **Click tracking uses `navigator.sendBeacon`.** `page.waitForRequest` still sees it (resource
  type `ping`), so the request body can be asserted.

## Adding a test

The approach follows two Kentico Community articles:
[E2E testing Xperience's administration UI extension with confidence](https://community.kentico.com/blog/e2e-testing-xperience-s-administration-ui-extension-with-confidence)
and
[Virtual inbox, real tests](https://community.kentico.com/blog/virtual-inbox-real-tests-ai-driven-e2e-automation-for-xperience-by-kentico-membership-flows).

1. **Explore.** Walk the scenario in a real browser with the Playwright MCP (configured in
   `.mcp.json`) and note the selectors, the waits and anything surprising. If the UI is hard to
   follow, that is a finding worth fixing before you test it.
2. **Codify.** Turn the walkthrough into a deterministic spec in the feature's folder. Reuse
   `admin/` helpers, put feature-only page objects next to the spec, and give each behaviour a
   `test.step` that reads like a sentence.
3. **Prove it.** Run the test until it passes repeatedly (`--repeat-each=3`), confirm it fails
   when the behaviour breaks, and confirm it cleaned up after itself.
4. **Record new gotchas** in the section above.
