# Execução local — fundação T01

Base: ADR-0001 aprovado na T00, commit `6e2fe6657a0edb580b01d284995810701162acf7`.
T01 implementa API/health, persistência mínima e tela inicial. Não há login, admin,
sessões, Forms, pontos, pagamentos ou demais módulos. Primeiro admin e contas A/B
só serão criados na T02; instruções anteriores sobre esses testes eram planejamento.

## Ambiente verificado em 31/10/2026

Windows x64; SDK .NET 10.0.402 (`global.json`), ASP.NET runtime 10.0.13, Node 24.22.2,
npm 11.19.1, Git 2.56.0. EF Core/Relational/Design e dotnet-ef 10.0.13, provider
Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3 e driver Npgsql 10.0.3. React/DOM 19.3.1,
TypeScript 5.9.3, Vite 7.3.7, Playwright 1.64.1. Pacotes transitivos em lockfiles.
O provider aceita EF 10.0.4 até antes de 11; usamos 10.0.13 em todos os projetos.
Build e integração provaram esse conjunto localmente, sem afirmar homologação produção.

Docker Engine 29.9.2, Compose v5.6.1, desktop-linux/WSL2, Linux x86_64. CLI localizada:
`C:\Users\Arthur\AppData\Local\Programs\DockerDesktop\resources\bin\docker.exe`.
`scripts/local.mjs` usa essa instalação via LOCALAPPDATA, sem alterar PATH global.
Pode-se definir RECONEXAO_DOCKER com outro executável existente. Se houver falha,
diagnosticar acesso/Engine/contexto; nunca trocar por banco nativo/SQLite ou reinstalar.

Imagem tag/digest fixados nos dois Compose:
`postgres:18.6-bookworm@sha256:5c4783a8cc3f0074912164f94dfa1be4e043438f0f707cf968d7af9a0d31a2c8`.
[Documentação oficial](https://hub.docker.com/_/postgres): PostgreSQL 18 monta volume
em `/var/lib/postgresql`, com PGDATA `/var/lib/postgresql/18/docker`. Conferido no
container real, com PostgreSQL 18.6 e mesmo diretório. Nunca montar apenas o caminho
antigo `/var/lib/postgresql/data` presumindo persistência em 18.

## Preparação na raiz do repositório

Os comandos Node funcionam no PowerShell Restricted sem executar `.ps1` ou alterar
proteções. Docker/cache NuGet/npm e certificado do usuário podem exigir autorização
do sandbox Codex; são limitações de acesso, não falhas da aplicação.

```powershell
node scripts/local.mjs docker-info
node scripts/local.mjs setup
dotnet tool restore
dotnet restore Reconexao.slnx --locked-mode
npm.cmd ci --prefix src/frontend --ignore-scripts --no-fund
dotnet dev-certs https --check
node scripts/local.mjs cert-export
node scripts/local.mjs db-up
node scripts/local.mjs test-db-up
dotnet build Reconexao.slnx --no-restore
node scripts/local.mjs migrate
npm.cmd --prefix src/frontend run build
```

O primeiro `package-lock.json` foi gerado nesta T01 com `npm.cmd install --prefix
src/frontend --ignore-scripts --no-fund --no-audit`, usando versões exatas previamente
consultadas. Só depois foi executado `npm ci`. No clone com lockfile, usar `ci`.
NuGet usa nuget.org e packages.lock.json nos dois projetos; não editar locks manualmente.

`setup` gera senhas aleatórias locais, preserva arquivos já existentes e não imprime
valores. `.env.development`/`.env.test` contêm só referências aos secrets em local-data;
exemplos versionados não contêm credenciais. Conexões da API/testes em
`local-data/dev.json` e `test.json`, carregadas apenas no ambiente do processo.
Arquivos/segredos/chaves/capturas temporárias nessa pasta são ignorados pelo Git;
restringir seu acesso à conta local. Não copiar seu conteúdo para prompts ou logs.
Não apagar/trocar senhas de banco já inicializado: POSTGRES_PASSWORD_FILE e scripts
de inicialização só atuam com armazenamento vazio. Rotação precisa de procedimento próprio.

`cert-export` exporta em PEM o certificado de desenvolvimento existente, sem executar
`--trust`, `--clean` ou instalar certificado. Nesta máquina um certificado válido já
existia (consulta autorizada); exportação em PFX sem senha falhou e foi substituída
pela exportação PEM suportada. Em outra máquina sem certificado, parar e explicar
a preparação necessária antes de alterar armazenamento/confiança de certificados.
Vite valida o certificado da API com a CA local explícita (`secure: true`).

## Executar API e frontend

Terminal 1, na raiz:

```powershell
node scripts/local.mjs api
```

Terminal 2, na raiz:

```powershell
npm.cmd --prefix src/frontend run dev
```

UI: `https://localhost:5173`; API: `https://localhost:5001`.
Proxy Vite `/api` para API HTTPS. Endpoints somente leitura:

| Endpoint | Resposta |
|---|---|
| `/health/live` | 200 alive; processo está vivo |
| `/health/ready` | 200 ready com marcador no banco, 503 genérico se indisponível |
| `/api/foundation` | Nome Reconexão e ready lidos do banco; 503 genérico na falha de conexão |

A migração `InitialFoundation` cria só FoundationMarkers e um marcador estático,
sem pessoas/dados reais. Não há endpoint de escrita, migração automática no startup,
OpenAPI ou integrações fake. O próprio EF registra __EFMigrationsHistory.
API não recebe ConnectionStrings de variável VITE; nenhuma credencial vai para o bundle.
Logs da API só warning/error, EF desabilitado e sem sensitive data logging; falhas de
conexão esperadas são tratadas sem registrar exceção/conexão. Não usar logging de corpo.

## Testar sem tocar nos dados dev

```powershell
node scripts/local.mjs test-db-up
node scripts/local.mjs test
node scripts/local.mjs verify-storage
npm.cmd --prefix src/frontend run test:e2e
```

`test` executa `dotnet test --no-build` com conexão de teste carregada localmente.
Antes de migrar, a suíte exige exatamente host 127.0.0.1, porta 55433, banco
reconexao_test e usuário reconexao_test_app. Não usa EnsureDeleted, DROP DATABASE
ou limpeza do banco dev. Escrita de teste usa transação revertida.
O cenário de conexão indisponível usa porta 55434 sem gravar/migrar nela.

Chrome instalado é usado pelo Playwright, sem instalar navegador extra. API/UI precisam
estar em execução. Testes cobrem sucesso com API real, interrupção de rede simulada
somente no navegador seguida de recuperação real, viewport 360px e foco por teclado.
Playwright ignora erro de confiança HTTPS apenas no contexto local de testes; não
modifica o navegador normal nem valida a confiança do certificado pelo Windows.
Os cenários de sucesso aguardam /api/foundation real pronto por até 15s, antes de
abrir a tela. Após recriar banco com API ainda ligada, uma conexão antiga do pool
pode produzir 503 na primeira consulta; isso é tratado como falha e a UI permite retry.

Para provar falha real do proxy: parar a API no seu terminal (Ctrl+C), manter Vite:

```powershell
$env:RECONEXAO_API_OFFLINE='1'
npm.cmd --prefix src/frontend run test:e2e
Remove-Item Env:RECONEXAO_API_OFFLINE
```

Esse modo exige resposta HTTP >=500 e mensagem de indisponibilidade sem mock.
Depois reiniciar API e rodar os três cenários normais. Testes geram capturas em
local-data; evidências sanitizadas selecionadas em docs/evidencias/media.

## Persistência, isolamento e parada

Dev: projeto `reconexao-dev`, volume `reconexao-dev_dev_data`, porta
`127.0.0.1:55432:5432`. Testes: projeto `reconexao-test`, tmpfs no caminho PG18,
porta `127.0.0.1:55433:5432`, usuários/secrets/banco independentes. Credencial da API
não é superadmin; dono do cluster é usado somente pela inicialização/operação local.

```powershell
node scripts/local.mjs db-info
node scripts/local.mjs verify-persistence
node scripts/local.mjs verify-storage
node scripts/local.mjs test-db-discard
node scripts/local.mjs test-db-up
node scripts/local.mjs test
node scripts/local.mjs db-stop
node scripts/local.mjs db-start
git diff --check
```

`verify-persistence` consulta marcador/migração, força recriação só do container dev,
confere troca de ID e conservação do volume, exige exatamente marcador 1/Reconexão e
uma migração e compara dados antes/depois. Não apaga volume/dados. Essa prova é específica
da migração mínima T01; o verificador deve evoluir quando o schema receber novas migrações.
`test-db-discard` executa down apenas no projeto/arquivo test: remove container/rede e
perde o tmpfs, sem volumes dev. Recriação limpa test e nova migração foram verificadas.
Não usar down -v dev, prune global ou banco de produção para resolver falhas de build.
Para parar API/Vite, Ctrl+C nos respectivos terminais; db-stop preserva desenvolvimento.

## Limitações e próximas entregas

Fundação local não é produto pronto ou implantação. Sem testes de login/aluno/admin,
que pertencem à T02. Metas não funcionais, homologação de navegadores e fidelidade ao
protótipo oficial dependem de P16/P18; tela inicial usa apenas azul-escuro, preto e R
como referência textual, sem alegar reprodução do protótipo. Contratos das demais
funcionalidades e fornecedores permanecem pendentes conforme o plano.
Resultados, falhas anteriores e revisões em [evidência T01](evidencias/T01_FUNDACAO.md).
Revisões em agentes QA e segurança distintos, em paralelo, sem edição. Os dois achados
baixos foram corrigidos e conferidos pelos respectivos revisores; principal repetiu os
testes afetados. Essas revisões, por si só, não concediam aceite: Arthur aceitou depois
a T01, conforme ESTADO_PROJETO.md. Não houve revisão de funções ainda inexistentes.
