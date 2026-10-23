import { expect, test as setup } from '@playwright/test';
import { adminStorageState, config } from '../shared/config';

// Signs in once and saves the session, so every admin test starts already signed in.
setup('sign in to the administration', async ({ page }) => {
  if (!config.adminPassword) {
    throw new Error(
      'No admin password. Set the "E2E:AdminPassword" user secret on src/TrainingGuides.Web '
      + '(and "E2E:AdminUsername" if not "administrator"), or the XBK_ADMIN_PASSWORD environment variable.');
  }

  await page.goto('/admin');
  const passwordField = page.getByRole('textbox', { name: 'Password' });

  try {
    await page.getByRole('textbox', { name: 'User name' }).fill(config.adminUsername);
    await passwordField.fill(config.adminPassword);
    await page.getByRole('button', { name: 'Sign in' }).click();

    // Wait for whichever comes first, so wrong credentials fail at once and say so.
    const signedIn = page.getByRole('button', { name: 'Channels' });
    const rejected = page.getByText('We couldn\'t sign you in');
    await expect(signedIn.or(rejected)).toBeVisible();
    if (await rejected.isVisible()) {
      throw new Error(`The administration rejected the credentials for user "${config.adminUsername}". Check the E2E:AdminUsername and E2E:AdminPassword user secrets.`);
    }
  } finally {
    // On failure Playwright records the page's text, including the password field's value, in
    // error-context.md and the HTML report. Leave nothing in it to record.
    if (await passwordField.isVisible()) {
      await passwordField.clear();
    }
  }

  await page.context().storageState({ path: adminStorageState });
});
