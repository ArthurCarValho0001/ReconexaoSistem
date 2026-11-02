import { test, expect, type Page, type APIRequestContext } from '@playwright/test';
import { readFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { randomUUID } from 'node:crypto';

const password: string = JSON.parse(readFileSync(fileURLToPath(new URL('../../../local-data/auth-test.json', import.meta.url)), 'utf8')).password;

async function login(page: Page, email = 'aluno.a@example.test') {
  await page.goto('/');
  await page.getByLabel('E-mail', { exact: true }).fill(email);
  await page.getByLabel('Senha', { exact: true }).fill(password);
  await page.getByRole('button', { name: 'Entrar', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Sua conta', exact: true })).toBeVisible();
}

async function post(request: APIRequestContext, text: string) {
  const csrf = await request.get('/api/auth/csrf');
  if (!csrf.ok()) throw new Error('CSRF indisponível para fixture de teste.');
  const { token } = await csrf.json();
  return request.post('/api/relatos', { data: { text }, headers: { 'X-CSRF-TOKEN': token } });
}

test.beforeAll(async ({ request }) => {
  await expect.poll(async () => (await request.get('/api/foundation')).status(), { timeout: 15000 }).toBe(200);
});

test('relato valida Unicode, preserva falha e só limpa confirmação; bloqueia clique duplo', async ({ page }) => {
  await login(page);
  const input = page.getByLabel('Seu relato', { exact: true });
  const send = page.getByRole('button', { name: 'Enviar relato', exact: true });
  await expect(send).toBeDisabled();
  await input.fill(' \t\n\u2003\u0085');
  await expect(send).toBeDisabled();
  await input.fill('😀'.repeat(1000));
  await expect(page.getByText('1000 de 1.000 caracteres', { exact: true })).toBeVisible();
  await expect(send).toBeEnabled();
  await input.fill('😀'.repeat(1001));
  await expect(page.getByText('1001 de 1.000 caracteres', { exact: true })).toBeVisible();
  await expect(send).toBeDisabled();
  await input.fill('e\u0301');
  await expect(page.getByText('2 de 1.000 caracteres', { exact: true })).toBeVisible();
  await input.fill('a\r\nb');
  await expect(page.getByText('3 de 1.000 caracteres', { exact: true })).toBeVisible();
  const text = `Relato fictício de envio ${randomUUID()}`;
  await input.fill(text);
  await page.route('**/api/relatos', route => route.abort('failed'));
  await send.click();
  await expect(page.getByRole('alert')).toContainText('Seu texto foi preservado');
  expect((await input.inputValue()) === text).toBe(true);
  await page.unroute('**/api/relatos');

  let release!: () => void;
  let reached!: () => void;
  const blocked = new Promise<void>(resolve => { release = resolve; });
  const persisted = new Promise<void>(resolve => { reached = resolve; });
  let requests = 0;
  await page.route('**/api/relatos', async route => {
    requests++;
    const response = await route.fetch();
    expect(response.status()).toBe(201);
    reached();
    await blocked;
    await route.fulfill({ response });
  });
  await send.click();
  await persisted;
  await expect(page.getByRole('button', { name: 'Enviando relato…', exact: true })).toBeDisabled();
  expect((await input.inputValue()) === text).toBe(true);
  await input.evaluate(element => {
    const form = element.closest('form')!;
    form.requestSubmit(); form.requestSubmit();
  });
  expect(requests).toBe(1);
  release();
  await expect(page.getByText('Relato enviado. Obrigado por compartilhar.', { exact: true })).toBeVisible();
  await expect(input).toHaveValue('');
  expect(requests).toBe(1);
  await expect(page.getByRole('heading', { name: 'Relatos recebidos', exact: true })).toHaveCount(0);
  expect(await page.evaluate(value => Object.values(localStorage).every(item => !item.includes(value)), text)).toBe(true);
});

test('admin consulta autor/data e paginação; HTML é texto sem execução', async ({ page }) => {
  await login(page);
  const marker = randomUUID();
  for (let index = 0; index < 21; index++) {
    expect((await post(page.request, `Relato fictício de paginação ${marker} ${index}`)).status()).toBe(201);
  }
  const html = `Relato fictício ${marker} <script>window.__relatoExecutado=1</script><img src=x onerror="window.__relatoExecutado=1">`;
  expect((await post(page.request, html)).status()).toBe(201);
  await page.getByRole('button', { name: 'Sair', exact: true }).click();
  await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  await login(page, 'admin@example.test');
  await expect(page.getByRole('heading', { name: 'Relatos recebidos', exact: true })).toBeVisible();
  await expect(page.getByText('Consultando relatos…', { exact: true })).toHaveCount(0);
  const firstIds = await page.locator('.report-entry').evaluateAll(items => items.map(item => item.getAttribute('data-report-id')));
  expect(firstIds.length).toBe(20);
  await page.getByRole('button', { name: 'Próxima página', exact: true }).click();
  await expect(page.getByText('Página 2', { exact: true })).toBeVisible();
  await expect(page.getByText('Consultando relatos…', { exact: true })).toHaveCount(0);
  const secondIds = await page.locator('.report-entry').evaluateAll(items => items.map(item => item.getAttribute('data-report-id')));
  expect(secondIds.length > 0 && secondIds.every(id => !firstIds.includes(id))).toBe(true);
  await page.getByRole('button', { name: 'Página anterior', exact: true }).click();
  await expect(page.getByText('Consultando relatos…', { exact: true })).toHaveCount(0);
  expect(await page.locator('.report-entry').evaluateAll(items => items.map(item => item.getAttribute('data-report-id')))).toEqual(firstIds);

  let found = false;
  for (let index = 0; index < 20; index++) {
    const item = page.locator('.report-entry').filter({ hasText: '<script>window.__relatoExecutado=1</script>' });
    if (await item.count()) {
      expect((await item.locator('.report-text').textContent()) === html).toBe(true);
      await expect(item.getByText('Autor: aluno.a@example.test', { exact: true })).toBeVisible();
      expect(await item.locator('time').getAttribute('datetime')).toBeTruthy();
      await expect(item.locator('script, img')).toHaveCount(0);
      found = true; break;
    }
    const next = page.getByRole('button', { name: 'Próxima página', exact: true });
    if (!(await next.isEnabled())) break;
    await next.click();
    await expect(page.getByText('Consultando relatos…', { exact: true })).toHaveCount(0);
  }
  expect(found).toBe(true);
  expect(await page.evaluate(() => '__relatoExecutado' in window)).toBe(false);
  await page.getByRole('button', { name: 'Sair', exact: true }).click();
  await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  await expect(page.locator('.report-entry')).toHaveCount(0);
});

test('rotas privadas e CSRF negados; sessão revogada remove formulário e rascunho', async ({ page, browser }) => {
  expect((await page.request.get('/api/admin/relatos')).status()).toBe(401);
  expect((await page.request.post('/api/relatos', { data: { text: 'Fixture anônima' } })).status()).toBe(401);
  await login(page);
  expect((await page.request.get('/api/admin/relatos')).status()).toBe(403);
  expect((await page.request.get('/api/relatos')).status()).toBe(405);
  expect((await page.request.post('/api/relatos', { data: { text: 'Fixture sem CSRF' } })).status()).toBe(400);
  await page.getByLabel('Seu relato', { exact: true }).fill(`Rascunho fictício ${randomUUID()}`);
  const second = await browser.newContext({ ignoreHTTPSErrors: true, baseURL: 'https://localhost:5173' });
  try {
    await login(await second.newPage());
    await page.getByRole('button', { name: 'Enviar relato', exact: true }).click();
    await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
    await expect(page.getByLabel('Seu relato', { exact: true })).toHaveCount(0);
    await expect(page.getByText('Sua sessão terminou. Entre novamente.', { exact: true })).toBeVisible();
  } finally { await second.close(); }
});

test('resposta administrativa atrasada não reaparece após logout', async ({ page }) => {
  let release!: () => void;
  let reached!: () => void;
  const blocked = new Promise<void>(resolve => { release = resolve; });
  const received = new Promise<void>(resolve => { reached = resolve; });
  await page.route('**/api/admin/relatos?*', async route => {
    const response = await route.fetch();
    expect(response.status()).toBe(200);
    reached();
    await blocked;
    try { await route.fulfill({ response }); } catch { /* A consulta é abortada ao sair da conta. */ }
  });
  await login(page, 'admin@example.test');
  await received;
  await page.getByRole('button', { name: 'Sair', exact: true }).click();
  await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  release();
  await page.unrouteAll({ behavior: 'wait' });
  await expect(page.getByRole('heading', { name: 'Relatos recebidos', exact: true })).toHaveCount(0);
  await expect(page.locator('.report-entry')).toHaveCount(0);
});
