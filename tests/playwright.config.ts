import { defineConfig, devices } from '@playwright/test';
import { adminStorageState, config } from './e2e/shared/config';

export default defineConfig({
  testDir: './e2e',
  // Admin tests create and publish real pages; running them one at a time keeps the content tree
  // and the editor's session predictable.
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  // The admin client and Page Builder are slow on a cold start.
  timeout: 120_000,
  expect: { timeout: 15_000 },
  reporter: [['list'], ['html', { open: 'never' }]],

  use: {
    baseURL: config.baseUrl,
    // The local site runs on the ASP.NET Core development certificate.
    ignoreHTTPSErrors: true,
    viewport: { width: 1600, height: 1000 },
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'admin-setup',
      testMatch: /admin\/auth\.setup\.ts/,
      // Traces and screenshots of the sign-in would capture the typed password.
      use: { ...devices['Desktop Chrome'], viewport: { width: 1600, height: 1000 }, trace: 'off', screenshot: 'off' },
    },
    {
      name: 'admin',
      testIgnore: /\.setup\.ts/,
      dependencies: ['admin-setup'],
      use: {
        ...devices['Desktop Chrome'],
        viewport: { width: 1600, height: 1000 },
        storageState: adminStorageState,
      },
    },
  ],

  // Reuses a site you already started locally; starts one in CI.
  webServer: {
    command: 'dotnet run --project ../src/TrainingGuides.Web',
    url: `${config.baseUrl}/admin`,
    ignoreHTTPSErrors: true,
    reuseExistingServer: !process.env.CI,
    timeout: 300_000,
  },
});
