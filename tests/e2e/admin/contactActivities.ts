import { expect, type Locator, type Page } from '@playwright/test';

/**
 * Opens Contact management > Activities, searches for the activity value and returns the matching
 * row once it appears.
 *
 * Activities are not always in the list the moment the request that logs them returns, so the
 * search is repeated until the row shows up.
 */
export async function findLoggedActivity(page: Page, activityValue: string): Promise<Locator> {
  await page.goto('/admin');
  await page.getByRole('button', { name: 'Digital marketing' }).click();
  // The application list and its tile view both link to Contact management.
  await page.getByTestId('application-list').getByRole('link', { name: 'Contact management' }).click();
  await page.getByRole('link', { name: 'Activities', exact: true }).click();

  const search = page.getByRole('textbox', { name: 'Search' });
  const row = page.getByRole('row').filter({ has: page.getByRole('cell', { name: activityValue, exact: true }) });

  await expect(async () => {
    await search.fill(activityValue);
    // The listing filters on Enter, not as you type.
    await search.press('Enter');
    await expect(row).toBeVisible({ timeout: 3_000 });
  }).toPass({ timeout: 30_000 });

  return row;
}
