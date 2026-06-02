import { test, expect } from '@playwright/test';

test('loads the SPA root', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('app-root')).toBeVisible();
});

test('shows shell chrome after bootstrap', async ({ page }) => {
  await page.goto('/bienvenida');
  await expect(page.locator('app-root')).toBeVisible();
  await expect(page.locator('.shell')).toBeVisible({ timeout: 30_000 });
  await expect(page.locator('.header-title')).toContainText('Legal AI AR');
});
