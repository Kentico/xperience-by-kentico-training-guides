import { expect, type Page } from '@playwright/test';
import { config } from '../shared/config';

/** A page the test created, with what it needs to visit and remove it again. */
export interface CreatedPage {
  name: string;
  /** Path of the page on the live site, for example "/e2e-content-promotion-1759748000000". */
  livePath: string;
  /** The admin URL of the page, for navigating back to it. */
  adminUrl: string;
}

/**
 * Creates an "Empty page" with the empty page template at the root of the website channel and
 * leaves the editor on the page's Page Builder tab.
 *
 * The name is used as the URL slug, so it must be lower case letters, digits and hyphens only.
 */
export async function createEmptyPage(page: Page, name: string): Promise<CreatedPage> {
  await page.goto('/admin');
  await page.getByRole('link', { name: config.websiteChannelName }).click();
  await page.getByRole('button', { name: 'Create new page' }).click();

  await page.getByRole('textbox', { name: 'Page name' }).fill(name);
  await page.getByRole('button', { name: 'Empty page', exact: true }).click();
  await page.getByRole('button', { name: 'Continue' }).click();

  await page.getByRole('button', { name: 'Empty page content type template' }).click();
  await page.getByRole('button', { name: 'Continue' }).click();

  // The slug is derived from the name. Check it, rather than assume where the page will live.
  const livePath = `/${name}`;
  await expect(page.getByText(`${config.baseUrl}${livePath}`, { exact: true })).toBeVisible();

  await page.getByRole('button', { name: 'Save', exact: true }).click();
  await expect(page.getByText('Page was created successfully.')).toBeVisible();
  const adminUrl = page.url();

  await page.getByRole('link', { name: 'Page Builder' }).click();
  return { name, livePath, adminUrl };
}

/** Publishes the page that is open in the editor, including unsaved Page Builder changes. */
export async function publishOpenPage(page: Page): Promise<void> {
  await page.getByRole('button', { name: 'Publish', exact: true }).click();
  await page.getByRole('button', { name: 'Confirm' }).click();
  await expect(page.getByText('The page has been published.')).toBeVisible();
}

/** Moves the page to the recycle bin, then deletes it permanently so no test data is left behind. */
export async function deletePagePermanently(page: Page, createdPage: CreatedPage): Promise<void> {
  await page.goto(createdPage.adminUrl);

  const treeItem = page.getByRole('treeitem', { name: createdPage.name });
  await treeItem.hover();
  await treeItem.getByTestId('tree-item-menu-button').click();
  await page.getByRole('button', { name: 'Move to recycle bin' }).click();
  // The confirmation button has the same name as the menu item that opened it.
  await page.getByRole('button', { name: 'Move to recycle bin' }).last().click();
  await expect(page.getByText(`The page '${createdPage.name}' was moved to the recycle bin.`)).toBeVisible();

  await page.goto('/admin/recycle-bin');
  const row = page.getByRole('row', { name: createdPage.name });
  await row.getByTestId('button-Delete permanently').click();
  await page.getByTestId('confirm-action').click();
  await expect(row).toHaveCount(0);
}
