import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  testMatch: process.env.RECONEXAO_API_OFFLINE === '1' ? 'offline.spec.ts' : /(?:foundation|auth)\.spec\.ts/,
  workers: 1,
  reporter: 'list',
  use: {
    baseURL: 'https://localhost:5173',
    channel: 'chrome',
    ignoreHTTPSErrors: true,
    screenshot: 'only-on-failure',
  },
});
