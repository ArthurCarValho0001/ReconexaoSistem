import { test, expect } from '@playwright/test';

test.beforeAll(async ({ request }) => {
  await expect.poll(async () => (await request.get('/api/foundation')).status(),
    { timeout: 15000, message: 'API real deve estar pronta antes dos cenários de sucesso.' }).toBe(200);
});

test('frontend consulta API real e apresenta conexão estabelecida', async ({ page }) => {
  const responsePromise = page.waitForResponse(response => response.url().endsWith('/api/foundation'));
  await page.goto('/');
  const response = await responsePromise;
  expect(response.status()).toBe(200);
  expect(await response.json()).toEqual({ name: 'Reconexão', status: 'ready' });
  await expect(page.getByText('Conexão estabelecida.')).toBeVisible();
  await page.getByRole('button', { name: 'Verificar conexão' }).click();
  await expect(page.getByText('Conexão estabelecida.')).toBeVisible();
  await page.screenshot({ path: '../../local-data/evidencia-t01-desktop.png', fullPage: true });
});

test('falha de rede informa erro e permite recuperar com API real', async ({ page }) => {
  await page.route('**/api/foundation', route => route.abort('failed'));
  await page.goto('/');
  await expect(page.getByText('Não foi possível conectar. Tente novamente em instantes.')).toBeVisible();
  await page.unroute('**/api/foundation');
  await page.getByRole('button', { name: 'Verificar conexão' }).click();
  await expect(page.getByText('Conexão estabelecida.')).toBeVisible();
});

test('tela inicial cabe em viewport móvel e permite navegação por teclado', async ({ page }) => {
  await page.setViewportSize({ width: 360, height: 780 });
  await page.goto('/');
  await expect(page.getByText('Conexão estabelecida.')).toBeVisible();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
  await page.keyboard.press('Tab');
  await expect(page.getByRole('button', { name: 'Verificar conexão' })).toBeFocused();
  await page.screenshot({ path: '../../local-data/evidencia-t01-mobile.png', fullPage: true });
});
