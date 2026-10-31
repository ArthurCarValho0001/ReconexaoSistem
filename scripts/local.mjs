import { existsSync, mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { randomBytes } from 'node:crypto';
import { spawnSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const root = fileURLToPath(new URL('../', import.meta.url));
const data = path.join(root, 'local-data');
const project = 'src/backend/Reconexao.Api';
const command = process.argv[2];
const docker = process.env.RECONEXAO_DOCKER ?? (process.platform === 'win32'
  ? path.join(process.env.LOCALAPPDATA ?? '', 'Programs/DockerDesktop/resources/bin/docker.exe')
  : 'docker');

function run(executable, args, env = {}) {
  const result = spawnSync(executable, args, {
    cwd: root, stdio: 'inherit', env: { ...process.env, ...env },
  });
  if (result.error) throw new Error(`Não foi possível executar ${path.basename(executable)}. Verifique instalação/permissões.`);
  if (result.status !== 0) process.exit(result.status ?? 1);
}
function localConfig(kind) {
  return JSON.parse(readFileSync(path.join(data, `${kind}.json`), 'utf8'));
}
function compose(kind, args) {
  run(docker, ['compose', '--env-file', `.env.${kind === 'dev' ? 'development' : 'test'}`,
    '-p', `reconexao-${kind}`, '-f', `infra/compose.${kind}.yaml`, ...args]);
}
function inspectContainer(kind) {
  const result = spawnSync(docker, ['inspect', `reconexao-${kind}-db-1`], { encoding: 'utf8' });
  if (result.status !== 0) throw new Error('Container indisponível.');
  return JSON.parse(result.stdout)[0];
}
function verifyStorage() {
  const dev = inspectContainer('dev');
  const test = inspectContainer('test');
  const devVolume = dev.Mounts.find(mount => mount.Destination === '/var/lib/postgresql');
  if (devVolume?.Type !== 'volume' || devVolume.Name !== 'reconexao-dev_dev_data') throw new Error('Volume dev inesperado.');
  if (!Object.hasOwn(test.HostConfig.Tmpfs ?? {}, '/var/lib/postgresql') &&
      !test.Mounts.some(mount => mount.Type === 'tmpfs' && mount.Destination === '/var/lib/postgresql'))
    throw new Error('Testes sem armazenamento descartável.');
  if (test.Mounts.some(mount => mount.Name === devVolume.Name)) throw new Error('Volume dev compartilhado com testes.');
  for (const [container, port] of [[dev, '55432'], [test, '55433']]) {
    const binding = container.HostConfig.PortBindings['5432/tcp'];
    if (binding.length !== 1 || binding[0].HostIp !== '127.0.0.1' || binding[0].HostPort !== port)
      throw new Error('Publicação inesperada da porta.');
  }
  console.log('PASSOU: volume dev persistente, tmpfs test isolado e ambas as portas somente 127.0.0.1.');
}
function persistenceProof() {
  const before = inspectContainer('dev');
  const sql = 'SELECT "Id", "Name" FROM "FoundationMarkers"; SELECT count(*) FROM "__EFMigrationsHistory";';
  function readProof() {
    const result = spawnSync(docker, ['compose', '--env-file', '.env.development',
      '-p', 'reconexao-dev', '-f', 'infra/compose.dev.yaml', 'exec', '-T', 'db',
      'psql', '-At', '-v', 'ON_ERROR_STOP=1', '-U', 'reconexao_dev_owner', '-d', 'reconexao_dev', '-c', sql],
    { cwd: root, encoding: 'utf8' });
    const proof = result.stdout?.trim().replaceAll('\r\n', '\n');
    if (result.status !== 0 || proof !== '1|Reconexão\n1') throw new Error('Marcador/migração ausentes ou diferentes do esperado.');
    return proof;
  }
  const originalProof = readProof();
  compose('dev', ['up', '-d', '--wait', '--force-recreate', 'db']);
  const after = inspectContainer('dev');
  if (before.Id === after.Id) throw new Error('Container não foi recriado.');
  const volume = container => container.Mounts.find(mount => mount.Destination === '/var/lib/postgresql')?.Name;
  if (volume(before) !== volume(after)) throw new Error('Volume mudou.');
  if (readProof() !== originalProof) throw new Error('Dados alterados após recriação.');
  console.log('PASSOU: container recriado, mesmo volume, marcador 1/Reconexão e uma migração idênticos antes/depois.');
}
function setup() {
  mkdirSync(data, { recursive: true });
  for (const kind of ['dev', 'test']) {
    for (const role of ['owner', 'app']) {
      const file = path.join(data, `${kind}-${role}-password`);
      if (!existsSync(file)) writeFileSync(file, randomBytes(32).toString('hex'), { flag: 'wx', mode: 0o600 });
    }
    const password = readFileSync(path.join(data, `${kind}-app-password`), 'utf8');
    const connection = `Host=127.0.0.1;Port=${kind === 'dev' ? '55432' : '55433'};Database=reconexao_${kind};Username=reconexao_${kind}_app;Password=${password};Timeout=3;Command Timeout=5;Include Error Detail=false`;
    const configFile = path.join(data, `${kind}.json`);
    if (!existsSync(configFile)) writeFileSync(configFile, JSON.stringify({ connection }), { flag: 'wx', mode: 0o600 });
    const envFile = path.join(root, `.env.${kind === 'dev' ? 'development' : 'test'}`);
    if (!existsSync(envFile)) writeFileSync(envFile,
      `DB_OWNER_PASSWORD_FILE=../local-data/${kind}-owner-password\nDB_APP_PASSWORD_FILE=../local-data/${kind}-app-password\n`, { flag: 'wx' });
  }
  console.log('Configuração local criada/preservada. Valores não exibidos.');
}
function apiEnv(kind = 'dev') {
  return {
    ASPNETCORE_ENVIRONMENT: kind === 'test' ? 'Test' : 'Development',
    ConnectionStrings__Database: localConfig(kind).connection,
    ASPNETCORE_URLS: 'https://localhost:5001',
    ASPNETCORE_Kestrel__Certificates__Default__Path: path.join(data, 'https.pem'),
    ASPNETCORE_Kestrel__Certificates__Default__KeyPath: path.join(data, 'https.key'),
  };
}

try {
  switch (command) {
    case 'setup': setup(); break;
    case 'docker-info': run(docker, ['info', '--format', '{{.ServerVersion}} {{.OSType}} {{.Architecture}}']); run(docker, ['compose', 'version']); break;
    case 'db-up': compose('dev', ['config', '--quiet']); compose('dev', ['up', '-d', '--wait', 'db']); break;
    case 'db-stop': compose('dev', ['stop', 'db']); break;
    case 'db-start': compose('dev', ['start', '--wait', 'db']); break;
    case 'db-recreate': compose('dev', ['up', '-d', '--wait', '--force-recreate', 'db']); break;
    case 'test-db-up': compose('test', ['config', '--quiet']); compose('test', ['up', '-d', '--wait', 'db']); break;
    case 'test-db-discard': compose('test', ['down']); break;
    case 'verify-storage': verifyStorage(); break;
    case 'verify-persistence': persistenceProof(); break;
    case 'db-info': compose('dev', ['exec', '-T', 'db', 'psql', '-v', 'ON_ERROR_STOP=1', '-U', 'reconexao_dev_owner', '-d', 'reconexao_dev', '-c', 'SELECT version(); SHOW data_directory; SELECT count(*) FROM "FoundationMarkers";']); break;
    case 'cert-export':
      run('dotnet', ['dev-certs', 'https', '--check']);
      run('dotnet', ['dev-certs', 'https', '--export-path', path.join(data, 'https.pem'), '--format', 'PEM', '--no-password']);
      break;
    case 'migration-add': run('dotnet', ['ef', 'migrations', 'add', 'InitialFoundation', '--project', project], apiEnv()); break;
    case 'migrate': run('dotnet', ['ef', 'database', 'update', '--project', project], apiEnv()); break;
    case 'api': run('dotnet', ['run', '--no-build', '--no-launch-profile', '--project', project], apiEnv()); break;
    case 'api-test': run('dotnet', ['run', '--no-build', '--no-launch-profile', '--project', project], apiEnv('test')); break;
    case 'test': run('dotnet', ['test', '--no-build'], { RECONEXAO_TEST_CONNECTION: localConfig('test').connection }); break;
    default: throw new Error('Comando inválido. Consulte docs/EXECUCAO_LOCAL.md.');
  }
} catch {
  console.error('Falha na operação local. Confira comando, arquivos locais e Docker; nenhum segredo foi exibido.');
  process.exitCode = 1;
}
