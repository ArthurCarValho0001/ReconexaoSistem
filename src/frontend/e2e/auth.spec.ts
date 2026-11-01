import { test, expect, type Page } from '@playwright/test';
import { readFileSync, readdirSync, statSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const data = fileURLToPath(new URL('../../../local-data/', import.meta.url));
const password: string = JSON.parse(readFileSync(path.join(data, 'auth-test.json'), 'utf8')).password;

async function login(page: Page, email: string) {
  await page.goto('/');
  await page.getByLabel('E-mail', { exact: true }).fill(email);
  await page.getByLabel('Senha', { exact: true }).fill(password);
  await page.getByRole('button', { name: 'Entrar', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Sua conta', exact: true })).toBeVisible();
}

test('aluno entra, nova sessão revoga a anterior e logout encerra', async ({ page, browser }) => {
  await login(page, 'aluno.a@example.test');
  await expect(page.getByText('Perfil: Aluno', { exact: true })).toBeVisible();
  const other = await browser.newContext({ ignoreHTTPSErrors: true, baseURL: 'https://localhost:5173' });
  try {
    const second = await other.newPage();
    await login(second, 'aluno.a@example.test');
    expect(await page.evaluate(async () => (await fetch('/api/auth/session')).status)).toBe(401);
    await page.reload();
    await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
    await second.getByRole('button', { name: 'Sair', exact: true }).click();
    await expect(second.getByText('Você saiu da sua conta.')).toBeVisible();
    expect((await other.request.get('/api/auth/session')).status()).toBe(401);
  } finally { await other.close(); }
});

test('admin usa perfil do servidor e credenciais inválidas são tratadas', async ({ page }) => {
  await page.goto('/');
  await page.getByLabel('E-mail', { exact: true }).fill('admin@example.test');
  await page.getByLabel('Senha', { exact: true }).fill('senha-incorreta');
  await page.getByRole('button', { name: 'Entrar', exact: true }).click();
  await expect(page.getByRole('alert')).toContainText('E-mail ou senha inválidos');
  await page.getByLabel('Senha', { exact: true }).fill(password);
  await page.getByRole('button', { name: 'Entrar', exact: true }).click();
  await expect(page.getByText('Perfil: Administrador', { exact: true })).toBeVisible();
  expect((await page.request.get('/api/admin/session')).status()).toBe(200);
  await page.screenshot({ path: path.join(data, 'evidencia-t02-admin.png'), fullPage: true });
  await page.getByRole('button', { name: 'Sair', exact: true }).click();
});

test('recuperação local redefine senha, revoga sessão e rejeita reuso', async ({ page, browser }) => {
  const authenticated = await browser.newContext({ ignoreHTTPSErrors: true, baseURL: 'https://localhost:5173' });
  try {
    const account = await authenticated.newPage();
    await login(account, 'aluno.b@example.test');
    await page.goto('/');
    await page.getByRole('button', { name: 'Esqueci minha senha' }).click();
    await page.getByLabel('E-mail', { exact: true }).fill('aluno.b@example.test');
    const since = Date.now();
    await page.getByRole('button', { name: 'Enviar instruções' }).click();
    await expect(page.getByRole('status').filter({ hasText: 'Pedido recebido' })).toBeVisible();
    expect((await authenticated.request.get('/api/auth/session')).status()).toBe(200);
    const messages = readdirSync(path.join(data, 'email-test')).filter(name => name.endsWith('.json'))
      .map(name => path.join(data, 'email-test', name)).filter(file => statSync(file).mtimeMs >= since - 1000)
      .map(file => JSON.parse(readFileSync(file, 'utf8'))).filter(message => message.email === 'aluno.b@example.test');
    const token: string = messages.at(-1)?.token;
    if (!token) throw new Error('Adapter local não produziu instruções de recuperação.');
    await page.getByRole('button', { name: 'Já tenho um código' }).click();
    await page.getByLabel('Código recebido por e-mail').fill(token);
    await page.getByLabel('Nova senha', { exact: true }).fill(password);
    await page.getByRole('button', { name: 'Redefinir senha', exact: true }).click();
    await expect(page.getByText('Senha redefinida. Entre novamente com sua nova senha.')).toBeVisible();
    expect((await authenticated.request.get('/api/auth/session')).status()).toBe(401);
    await page.getByRole('button', { name: 'Já tenho um código' }).click();
    await page.getByLabel('Código recebido por e-mail').fill(token);
    await page.getByLabel('Nova senha', { exact: true }).fill(password);
    await page.getByRole('button', { name: 'Redefinir senha', exact: true }).click();
    await expect(page.getByRole('alert')).toContainText('Código inválido, expirado ou já utilizado');
  } finally { await authenticated.close(); }
});

test('falha de recuperação é apresentada e formulário cabe no celular', async ({ page }) => {
  await page.setViewportSize({ width: 360, height: 780 });
  await page.goto('/');
  await page.getByRole('button', { name: 'Esqueci minha senha' }).click();
  await page.getByLabel('E-mail', { exact: true }).fill('aluno.a@example.test');
  await page.route('**/api/auth/forgot-password', route => route.abort('failed'));
  await page.getByRole('button', { name: 'Enviar instruções' }).click();
  await expect(page.getByRole('alert')).toContainText('Verifique a conexão');
  await expect(page.getByLabel('E-mail', { exact: true })).toHaveValue('aluno.a@example.test');
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
  await page.getByLabel('E-mail', { exact: true }).focus();
  await page.keyboard.press('Tab');
  await expect(page.getByRole('button', { name: 'Enviar instruções' })).toBeFocused();
  await page.screenshot({ path: path.join(data, 'evidencia-t02-mobile.png'), fullPage: true });
});

test('consulta atrasada da sessão não desfaz login ou logout', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  for (const action of ['login', 'logout']) {
    let release!: () => void;
    let captured!: () => void;
    const blocked = new Promise<void>(resolve => { release = resolve; });
    const received = new Promise<void>(resolve => { captured = resolve; });
    await page.route('**/api/auth/session', async route => {
      const response = await route.fetch();
      captured();
      await blocked;
      await route.fulfill({ response });
    });
    await page.evaluate(() => window.dispatchEvent(new Event('focus')));
    await received;
    if (action === 'login') {
      await page.getByLabel('E-mail', { exact: true }).fill('aluno.a@example.test');
      await page.getByLabel('Senha', { exact: true }).fill(password);
      await page.getByRole('button', { name: 'Entrar', exact: true }).click();
      await expect(page.getByRole('heading', { name: 'Sua conta', exact: true })).toBeVisible();
    } else {
      await page.getByRole('button', { name: 'Sair', exact: true }).click();
      await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
    }
    const responseCompleted = page.waitForResponse(response => response.url().endsWith('/api/auth/session'));
    release();
    await responseCompleted;
    await page.unroute('**/api/auth/session');
    if (action === 'login') await expect(page.getByRole('heading', { name: 'Sua conta', exact: true })).toBeVisible();
    else await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  }
});

test('sessão anônima usa 401 sem alerta, polling ou rejeição não tratada; foco revalida', async ({ page }) => {
  const pageErrors: string[] = [];
  const statuses: number[] = [];
  page.on('pageerror', error => pageErrors.push(error.name));
  page.on('response', response => { if (response.url().endsWith('/api/auth/session')) statuses.push(response.status()); });
  await page.addInitScript(() => {
    (window as unknown as { unhandledRejections: number }).unhandledRejections = 0;
    window.addEventListener('unhandledrejection', () => {
      (window as unknown as { unhandledRejections: number }).unhandledRejections++;
    });
  });
  await page.clock.install();
  await page.goto('/');
  await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  expect(statuses).toEqual([401]);
  await expect(page.getByRole('alert')).toHaveCount(0);
  await expect(page.getByText('Sua sessão terminou. Entre novamente.')).toHaveCount(0);
  await page.clock.runFor(95000);
  expect(statuses).toEqual([401]);
  const focused = page.waitForResponse(response => response.url().endsWith('/api/auth/session'));
  await page.evaluate(() => window.dispatchEvent(new Event('focus')));
  expect((await focused).status()).toBe(401);
  expect(statuses).toEqual([401, 401]);
  await expect(page.getByRole('alert')).toHaveCount(0);
  expect(pageErrors).toEqual([]);
  expect(await page.evaluate(() => (window as unknown as { unhandledRejections: number }).unhandledRejections)).toBe(0);
});

test('sessão autenticada retorna 200 e perfil próprio; anônimo e aluno mantêm 401/403', async ({ page }) => {
  await page.clock.install();
  expect((await page.request.get('/api/auth/session')).status()).toBe(401);
  expect((await page.request.get('/api/admin/session')).status()).toBe(401);
  await login(page, 'aluno.a@example.test');
  const session = await page.request.get('/api/auth/session');
  expect(session.status()).toBe(200);
  expect(await session.json()).toMatchObject({ email: 'aluno.a@example.test', role: 'aluno' });
  const polled = page.waitForResponse(response => response.url().endsWith('/api/auth/session'));
  await page.clock.runFor(31000);
  expect((await polled).status()).toBe(200);
  expect((await page.request.get('/api/admin/session')).status()).toBe(403);
  await page.getByRole('button', { name: 'Sair', exact: true }).click();
  await expect(page.getByRole('button', { name: 'Entrar', exact: true })).toBeVisible();
  expect((await page.request.get('/api/auth/session')).status()).toBe(401);
  await login(page, 'admin@example.test');
  const admin = await page.request.get('/api/auth/session');
  expect(admin.status()).toBe(200);
  expect(await admin.json()).toMatchObject({ email: 'admin@example.test', role: 'admin' });
  expect((await page.request.get('/api/admin/session')).status()).toBe(200);
});

test('favicon explícito responde SVG e mantém a marca R', async ({ page }) => {
  const missingIcons: string[] = [];
  page.on('response', response => { if (/favicon\./.test(response.url()) && response.status() >= 400) missingIcons.push(response.url()); });
  await page.goto('/');
  await expect(page.locator('link[rel="icon"]')).toHaveAttribute('href', '/favicon.svg');
  const response = await page.request.get('/favicon.svg');
  expect(response.status()).toBe(200);
  expect(response.headers()['content-type']).toContain('image/svg+xml');
  expect(await response.text()).toContain('>R</text>');
  expect(missingIcons).toEqual([]);
});
