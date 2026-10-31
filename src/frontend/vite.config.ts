import { defineConfig } from 'vite';
import { readFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';

export default defineConfig(({ command }) => ({
  server: command === 'serve' ? {
    https: {
      cert: readFileSync(fileURLToPath(new URL('../../local-data/https.pem', import.meta.url))),
      key: readFileSync(fileURLToPath(new URL('../../local-data/https.key', import.meta.url))),
    },
    proxy: { '/api': {
      target: 'https://localhost:5001', secure: true,
      ca: readFileSync(fileURLToPath(new URL('../../local-data/https.pem', import.meta.url)), 'utf8'),
    } },
  } : undefined,
}));
