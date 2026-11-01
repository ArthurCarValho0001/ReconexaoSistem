# Execução local — fundação T01 e autenticação T02

Base: ADR-0001 aprovado na T00, commit `6e2fe6657a0edb580b01d284995810701162acf7`.
T01 implementou API/health, persistência mínima e tela inicial. T02 acrescenta Identity,
login/logout, perfis, sessões, recuperação local e comandos explícitos de bootstrap/
fixtures descritos ao final. Forms, pontos, pagamentos e demais módulos seguem ausentes.

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
Proxy Vite `/api` para API HTTPS. Endpoints da fundação:

| Endpoint | Resposta |
|---|---|
| `/health/live` | 200 alive; processo está vivo |
| `/health/ready` | 200 ready com marcador no banco, 503 genérico se indisponível |
| `/api/foundation` | Nome Reconexão e ready lidos do banco; 503 genérico na falha de conexão |

A migração `InitialFoundation` cria só FoundationMarkers e um marcador estático,
sem pessoas/dados reais. A T02 acrescenta migração de identidade e endpoints de
autenticação. Não há migração automática no startup ou OpenAPI publicado.
O próprio EF registra __EFMigrationsHistory.
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
ou limpeza do banco dev. Escrita da fundação usa transação revertida; testes T02
criam contas fictícias únicas somente nesse banco. As fixtures explícitas A/B/admin
ficam disponíveis para testes de navegador; testes .NET preservam fixtures anteriores.
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
confere troca de ID e conservação do volume, exige marcador 1/Reconexão e ao menos
uma migração e compara marcador/quantidade de migrações antes/depois. Não apaga volume/
dados. Essa prova não verifica toda a persistência de identidade e deve evoluir conforme o schema.
`test-db-discard` executa down apenas no projeto/arquivo test: remove container/rede e
perde o tmpfs, sem volumes dev. Recriação limpa test e nova migração foram verificadas.
Não usar down -v dev, prune global ou banco de produção para resolver falhas de build.
Para parar API/Vite, Ctrl+C nos respectivos terminais; db-stop preserva desenvolvimento.

## Limitações e próximas entregas

Fundação local não é produto pronto ou implantação. A prova de login/aluno/admin foi
acrescentada na T02 abaixo. Metas não funcionais, homologação de navegadores e fidelidade ao
protótipo oficial dependem de P16/P18; tela inicial usa apenas azul-escuro, preto e R
como referência textual, sem alegar reprodução do protótipo. Contratos das demais
funcionalidades e fornecedores permanecem pendentes conforme o plano.
Resultados, falhas anteriores e revisões em [evidência T01](evidencias/T01_FUNDACAO.md).
Revisões em agentes QA e segurança distintos, em paralelo, sem edição. Os dois achados
baixos foram corrigidos e conferidos pelos respectivos revisores; principal repetiu os
testes afetados. Essas revisões, por si só, não concediam aceite: Arthur aceitou depois
a T01, conforme ESTADO_PROJETO.md. Não houve revisão de funções ainda inexistentes.

## Autenticação T02 — validação isolada

Versão adicional fixada: Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.13;
demais versões da base preservadas. Em 01/11/2026 foram executados build .NET sem
avisos/erros, 24 testes de integração e oito cenários Chrome com API/PostgreSQL reais.
Apenas falhas de rede explicitamente simuladas nos respectivos cenários de navegador.

Com Docker Desktop já em execução, após preparação/restore anteriores:

```powershell
dotnet restore Reconexao.slnx --locked-mode
dotnet build Reconexao.slnx --no-restore
node scripts/local.mjs test-db-up
node scripts/local.mjs test
node scripts/local.mjs auth-seed-test
npm.cmd --prefix src/frontend run build
```

`test` confere destino test antes de migrar. Migração nova
`20261101180858_AuthenticationSessions` cria tabelas Identity, perfil restrito e índice
único de e-mail normalizado, sem pessoas ou senhas em migration/seed SQL.
`migrate-test` também está disponível para atualizar explicitamente só esse ambiente.
Scripts test/API test/seed test conferem host, porta, banco e usuário antes de executar,
recusando parâmetros duplicados, destino divergente ou opções de conexão inesperadas.
Não rodar testes apontando dev. Para provar aplicação desde zero, `test-db-discard`
e `test-db-up` recriam somente tmpfs test; não execute isso com E2E/API test ativos.

Terminais separados para a prova no navegador:

```powershell
node scripts/local.mjs api-test
```

```powershell
npm.cmd --prefix src/frontend run dev
```

```powershell
npm.cmd --prefix src/frontend run test:e2e
```

UI https://localhost:5173; API https://localhost:5001. `api-test` usa exclusivamente
banco test. Não iniciar `api` e `api-test` juntos na mesma porta. A UI contém login,
logout, pedido de recuperação e redefinição pelo código recebido. A consulta de
sessão é consultada ao carregar e no foco; enquanto autenticado, também a cada 30
segundos. Anônimo recebe 401 esperado, sem alerta ou polling periódico. A revogação é imediata no servidor
na próxima requisição protegida, independentemente desse intervalo visual.

Fixtures automatizadas em test: `aluno.a@example.test`, `aluno.b@example.test`, `admin@example.test`.
`auth-seed-test` cria senha aleatória em `local-data/auth-test.json` e transmite-a ao
processo por ambiente, sem exibir. A demonstração manual usa as contas dev abaixo,
com credenciais fictícias apresentadas diretamente pelo principal na resposta final.
Não coloque valores em evidências ou Git. O comando preserva contas/senhas já
existentes e recusa perfil divergente, sem promover alunos. Se uma senha fictícia
for alterada manualmente, use a senha atual ou recrie apenas test antes de novas provas.

`auth-setup` cria/preserva também `local-data/auth-dev.json`, sem semear usuários.
Para preparar a demonstração manual no dev existente, depois de validar a migração
em test, pare a API anterior antes de compilar e execute deliberadamente:

```powershell
dotnet build Reconexao.slnx --no-restore
node scripts/local.mjs db-start
node scripts/local.mjs migrate
node scripts/local.mjs auth-demo
node scripts/local.mjs api
```

Em outro terminal, mantenha/inicie `npm.cmd --prefix src/frontend run dev`.
URL: https://localhost:5173; API: https://localhost:5001. O mesmo endereço é usado nos
dois modos locais: encerre `api-test` antes de iniciar `api`. A apresentação manual
deve usar Development, banco `reconexao_dev`, porta 55432, volume persistente;
test automatizado permanece `reconexao_test`, porta 55433, tmpfs descartável.

`auth-demo` provisiona somente `admin@example.test` (admin) e `aluno.a@example.test`
(aluno), reutilizando contas compatíveis existentes. Compartilha a configuração de
senha fictícia já existente em `local-data/auth-dev.json`, distinta de test, sem
gerar nova senha se o arquivo existir. Não reseta senha, não promove conta divergente,
não apaga contas e não concede acesso comercial. Não é executado no startup: apenas
a chamada explícita usa `--seed-local-demo`, recusado fora de Development/Test.
O script confere conexão dev local antes de migrar/provisionar; não aceita produção.
`auth-seed` continua disponível para as três fixtures locais, mas não é necessário
para a demonstração de duas contas.

Arthur autorizou exclusivamente a exibição de e-mail e senha dessas duas contas
fictícias locais na resposta final de cada entrega com interface testável. O principal
deve validá-las e informar URL, ambiente e roteiro curto diretamente, sem encaminhar
Arthur a procurar credenciais em arquivos. Senhas ficam fora desta documentação e
do Git; credenciais reais, chaves e tokens nunca entram nessa exceção.

Na complementação T02 de 01/11/2026, migração dev e `auth-demo` foram executados:
mesmo container/volume, marcador e usuários anteriores preservados; duas contas
adicionadas, segunda chamada idempotente e senha local inalterada. Não houve limpeza
ou recriação dev. O teste focal de provisionamento passou 1/1 em test e os dois
logins/perfis/logout foram validados no Chrome em dev, sem repetir toda a suíte.
Não semear fixtures em produção. Não apagar `auth-*.json` como forma de trocar senhas:
o seed não redefine senha de conta existente.

## Recuperação local e bootstrap

Os comandos API configuram `Auth__KeysPath=local-data/keys-dev` ou `keys-test`,
`Auth__EmailAdapter=Local` e `Auth__LocalEmailPath=local-data/email-dev` ou `email-test`.
São diretórios ignorados, separados por ambiente. O adapter NÃO envia e-mail externo:
salva `{adapter: "LOCAL_SIMULADO", email, token}`. Use o token do arquivo correspondente
ao pedido na tela “Já tenho um código”; não exponha arquivos/chaves por rota HTTP.
Nenhum token fica em URL, localStorage ou logs. Capturas de testes ocultam campo de código.

202 no pedido não confirma existência de conta nem entrega. Adapter indisponível
antes do lookup retorna 503 genericamente; falha posterior registra warning sem dados
e mantém resposta genérica para evitar enumeração. Teste injeta essa falha após probe.
Produção recusa `Local` no startup; sem fornecedor real a recuperação fica indisponível.
Google permanece T04, sem claim fictícia; gate inicial aguarda P03.

Bootstrap de primeiro admin é explícito por CLI, com credenciais fornecidas em
`local-data/bootstrap-admin.json` contendo as propriedades `email` e `password`,
criadas por você fora do Git. Execute `node scripts/local.mjs auth-bootstrap-admin`
apenas no banco dev já migrado e antes de qualquer outro admin. Recusa se já existir
admin ou se a conta fornecida já existir; não promove conta existente. O script passa
valores por ambiente, nunca argv. Para operação futura, a aplicação aceita
`--bootstrap-admin` com `Bootstrap__Email`/`Bootstrap__Password` e conexão/chaves do
ambiente; produção não foi configurada nem executada nesta tarefa.

Foi provado em test vazio: primeira criação por CLI aceita, repetição recusada. O
helper local ignorado `local-data/verify-t02-bootstrap.cjs` capturou saídas em memória
e exibiu apenas PASSOU/FALHOU, com credenciais fictícias transitórias aleatórias.

Chaves locais Data Protection persistem entre reinícios. O runtime avisou que os XML
locais não têm encryptor em repouso; restrinja diretórios à conta do usuário e não
versione nem compartilhe chaves/inbox. Criptografia e operação de chaves de produção
dependem do ambiente de implantação. Não apagar chaves para resolver um erro de login.
Políticas de senha, sessão, token, lockout e rate limit: CONTRATOS_API.md.
Evidência completa: [T02_AUTENTICACAO](evidencias/T02_AUTENTICACAO.md).
