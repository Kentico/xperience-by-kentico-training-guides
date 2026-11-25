import { expect, type FrameLocator, type Locator, type Page } from '@playwright/test';

/** Matches ComponentIdentifiers.Widgets.CONTENT_PROMOTION in the web project. */
const WIDGET_IDENTIFIER = 'TrainingGuides.ContentPromotionWidget';

export interface ManualPromotion {
  title: string;
  description: string;
  callToActionText: string;
  linkUrl: string;
  trackingValue: string;
}

/**
 * The editor's side of the content promotion widget: Page Builder and the widget properties dialog.
 *
 * Page Builder's own controls have no accessible names, only titles and data attributes, so those
 * are the selectors here. The properties dialog is a regular admin form and is driven by its labels.
 */
export class ContentPromotionEditor {
  /** The page as Page Builder renders it, inside the admin's iframe. */
  readonly pageBuilder: FrameLocator;

  constructor(private readonly page: Page) {
    this.pageBuilder = page.getByTestId('page-builder').contentFrame();
  }

  async addWidget(): Promise<void> {
    await this.pageBuilder.getByTitle('Add widget').click();
    await this.pageBuilder.locator(`[data-component-identifier="${WIDGET_IDENTIFIER}"]`).click();
  }

  async openProperties(): Promise<void> {
    await this.pageBuilder.getByTitle('Configure widget').click();
    await expect(this.page.getByText('Content promotion properties')).toBeVisible();
  }

  async chooseContentSource(source: 'Page' | 'Content hub item' | 'Manual content'): Promise<void> {
    // The native radio sits under a styled overlay, so click the visible label like an editor would.
    await this.page.getByRole('radiogroup').getByText(source, { exact: true }).click();
    await expect(this.page.getByRole('radio', { name: source })).toBeChecked();
  }

  field(label: string): Locator {
    return this.page.getByRole('textbox', { name: label, exact: true });
  }

  async fillManualPromotion(promotion: ManualPromotion): Promise<void> {
    await this.field('Title').fill(promotion.title);
    await this.field('Description').fill(promotion.description);
    await this.field('Call to action text').fill(promotion.callToActionText);
    await this.field('Link URL').fill(promotion.linkUrl);
    await this.field('Tracking value').fill(promotion.trackingValue);
  }

  async applyProperties(): Promise<void> {
    await this.page.getByRole('button', { name: 'Apply' }).click();
    await expect(this.page.getByText('Content promotion properties')).toBeHidden();
  }
}
