import { test, expect } from '@playwright/test';

test('API realmente parada: proxy retorna falha e a tela informa indisponibilidade', async ({ page }) => {
  const responsePromise = page.waitForResponse(response => response.url().endsWith('/api/foundation'));
  await page.goto('/');
  const response = await responsePromise;
  expect(response.status()).toBeGreaterThanOrEqual(500);
  await expect(page.getByText('Não foi possível conectar. Tente novamente em instantes.')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Verificar conexão' })).toBeEnabled();
  await page.screenshot({ path: '../../local-data/evidencia-t01-falha-real.png', fullPage: true });
});
