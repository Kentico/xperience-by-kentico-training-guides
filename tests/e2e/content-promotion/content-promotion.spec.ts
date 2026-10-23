import { expect, test as base } from '@playwright/test';
import { type CreatedPage, createEmptyPage, deletePagePermanently, publishOpenPage } from '../admin/websiteChannelPages';
import { ContentPromotionEditor, type ManualPromotion } from './contentPromotionEditor';

// Each test gets its own uniquely named page, removed afterwards even when the test fails.
const test = base.extend<{ testPage: CreatedPage }>({
  testPage: async ({ page }, use) => {
    const createdPage = await createEmptyPage(page, `e2e-content-promotion-${Date.now()}`);
    await use(createdPage);
    await deletePagePermanently(page, createdPage);
  },
});

test('an editor publishes a manual promotion and a visitor clicks it', async ({ page, browser, baseURL, testPage }) => {
  const editor = new ContentPromotionEditor(page);
  const promotion: ManualPromotion = {
    title: 'Talk to an adviser',
    // The description override is plain text: anything that looks like markup must show literally.
    description: 'Free 30-minute call. <b>No obligation.</b>',
    callToActionText: 'Get in touch',
    linkUrl: '/contact-us',
    trackingValue: `e2e-${Date.now()}`,
  };

  await test.step('a new widget tells the editor it has nothing to show yet', async () => {
    await editor.addWidget();
    await expect(editor.pageBuilder.getByText('This promotion has nothing to show yet.', { exact: false })).toBeVisible();
  });

  await test.step('manual mode offers a link URL and hides the options that need a selected item', async () => {
    await editor.openProperties();
    await editor.chooseContentSource('Manual content');

    await expect(editor.field('Link URL')).toBeVisible();
    await expect(page.getByText('Hide elements')).toBeHidden();
    await expect(page.getByRole('checkbox', { name: 'Show type-specific extras' })).toBeHidden();
  });

  await test.step('the authored card renders in Page Builder', async () => {
    await editor.fillManualPromotion(promotion);
    await editor.applyProperties();

    await expect(editor.pageBuilder.getByRole('heading', { name: promotion.title })).toBeVisible();
    await expect(editor.pageBuilder.getByText(promotion.description, { exact: true })).toBeVisible();
    await expect(editor.pageBuilder.getByRole('link', { name: promotion.callToActionText })).toBeVisible();
  });

  await test.step('the editor publishes the page', async () => {
    await publishOpenPage(page);
  });

  // A fresh context with no cookies: an anonymous visitor, not the signed-in editor.
  const visitorContext = await browser.newContext({ baseURL, ignoreHTTPSErrors: true, storageState: { cookies: [], origins: [] } });
  const visitor = await visitorContext.newPage();
  const card = visitor.locator('.c-content-promotion');

  try {
    await test.step('a visitor sees the card, with one link and no editor notices', async () => {
      await visitor.goto(testPage.livePath);

      await expect(card.getByRole('heading', { name: promotion.title })).toBeVisible();
      await expect(card.getByText(promotion.description, { exact: true })).toBeVisible();
      await expect(card.getByRole('link')).toHaveCount(1);
      await expect(card.getByRole('link', { name: promotion.callToActionText })).toHaveAttribute('href', promotion.linkUrl);
      await expect(visitor.getByText('Only editors see this')).toHaveCount(0);
    });

    await test.step('without cookie consent, the click tracking script is not served', async () => {
      await expect(visitor.locator('script[src*="ContentPromotionActivityLogger"]')).toHaveCount(0);
    });

    await test.step('after consent, clicking anywhere on the card logs the click and follows the link', async () => {
      const consentSaved = visitor.waitForResponse((response) => response.url().endsWith('/cookies/cookiebannersubmit') && response.ok());
      await visitor.getByRole('button', { name: 'Accept all cookies' }).click();
      await consentSaved;
      // Consent takes effect on the next request; the tracking script is served from then on.
      await visitor.reload();
      await expect(visitor.locator('script[src*="ContentPromotionActivityLogger"]')).toHaveCount(1);

      const trackingRequest = visitor.waitForRequest((request) => request.url().endsWith('/contentpromotionclick'));
      // Click the card's corner, away from the button: the stretched link makes the whole card clickable.
      await card.click({ position: { x: 20, y: 20 } });

      expect((await trackingRequest).postData()).toBe(`TrackingValue=${promotion.trackingValue}`);
      await expect(visitor).toHaveURL(promotion.linkUrl);
    });
  } finally {
    await visitorContext.close();
  }
});
