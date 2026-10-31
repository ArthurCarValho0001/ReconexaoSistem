# Execução local

Estado: BASE APROVADA em 31/10/2026; execução da aplicação AINDA NÃO IMPLEMENTADA.
T01 não iniciada. O plano abaixo não é evidência de comandos já disponíveis.

Inventário realizado na T00 em 31/10/2026: [resultados reais](INVENTARIO_T00.md).
Existe SDK .NET/Node/Git; banco ainda não foi iniciado ou validado. `npm.cmd` funciona;
`npm.ps1` está bloqueado por política Restricted, preservada. A T01 deve oferecer
comandos compatíveis com essa política; scripts futuros ainda não foram criados.

## Docker verificado na T00

`docker info` e `docker compose version` pelo PATH falharam com comando não encontrado.
Processos mostraram instalação por usuário; acesso à pasta foi negado no sandbox.
Consulta repetida com escalonamento autorizado e caminho completo funcionou:

```powershell
$taskDockerExe = 'C:\Users\Arthur\AppData\Local\Programs\DockerDesktop\resources\bin\docker.exe'
& $taskDockerExe info
& $taskDockerExe compose version
& $taskDockerExe buildx imagetools inspect docker.io/library/postgres:18.6-bookworm
```

Resultados reais: Engine cliente/servidor 29.9.2, contexto desktop-linux, Linux x86_64,
Compose v5.6.1; imagem linux/amd64 publicada, digest do índice no ADR-0001. Não houve
pull de camadas, criação de Compose, container, volume ou banco nesta T00. Nenhum
listener TCP na porta 55432 foi encontrado; bind efetivo ainda deve ser testado.

## Banco aprovado e plano curto T01

Imagem: `docker.io/library/postgres:18.6-bookworm@sha256:5c4783a8cc3f0074912164f94dfa1be4e043438f0f707cf968d7af9a0d31a2c8`.
Tag/digest confirmados no registro e linha 18 suportada. Revalidar disponibilidade e
avisos de segurança antes de executar; eventual atualização documentada, sem `latest`.
Compatibilidade arquitetural confirmada, funcional EF/driver/migrações ainda não testada.

- Desenvolvimento: Compose `reconexao-dev`, serviço `db`, volume nomeado persistente
  exclusivo em `/var/lib/postgresql`, porta `127.0.0.1:55432:5432`.
- Testes: Compose `reconexao-test`, serviço `db`, banco e credenciais separados, tmpfs
  em `/var/lib/postgresql`, porta proposta `127.0.0.1:55433:5432`. Nunca usar volume dev.
- Arquivos `.env.development` e `.env.test` ignorados, com referências a arquivos de
  senha em `local-data/` para Compose secrets/POSTGRES_PASSWORD_FILE. API em user-secrets
  com usuário próprio sem superadmin; nenhum segredo real em exemplos/logs/saída config.
- Confirmar persistência após parada/recriação dev. Descarte apenas do projeto test;
  sem `down -v` no desenvolvimento e sem prune global.

Sequência T01: conferir ambiente e Git; criar Compose e segredos locais; fixar versões
SDK/NuGet/npm; criar API/health, persistência mínima, migração de teste e tela inicial;
executar verificações e documentar resultados. Somente fundação: autenticação T02,
relatos T03, Forms T04, pontos T05 e demais funcionalidades não serão antecipados.

Comandos previstos, NÃO EXECUTADOS; os arquivos/solução ainda serão criados na T01:

```powershell
git status --short --branch
dotnet --info
node --version
npm.cmd --version
& $taskDockerExe info
& $taskDockerExe compose version
& $taskDockerExe compose --env-file .env.development -p reconexao-dev -f infra/compose.dev.yaml config --quiet
& $taskDockerExe compose --env-file .env.development -p reconexao-dev -f infra/compose.dev.yaml up -d --wait db
& $taskDockerExe compose --env-file .env.development -p reconexao-dev -f infra/compose.dev.yaml exec -T db pg_isready
& $taskDockerExe compose --env-file .env.test -p reconexao-test -f infra/compose.test.yaml config --quiet
& $taskDockerExe compose --env-file .env.test -p reconexao-test -f infra/compose.test.yaml up -d --wait db
dotnet restore
dotnet build --no-restore
dotnet test --no-build
npm.cmd --prefix src/frontend ci
npm.cmd --prefix src/frontend run build
git diff --check
```

Na T01 definir comando EF com projeto/contexto reais, aplicar migração no banco
descartável e verificar leitura/escrita. Subir API/UI, consultar health e demonstrar
UI consumindo backend real; validar falha quando banco/API indisponível. Confirmar
versão via SQL, binding loopback e isolamento/persistência; então completar abaixo
os comandos de start/stop/descarte seletivo e testes conforme arquivos implementados.

## T01 precisa preencher e verificar

- SDK/runtime e ferramentas com versões; lockfiles/global.json conforme stack aprovada.
- Pré-requisitos e comandos PowerShell e, se suportado, alternativa Linux.
- Como configurar segredos locais sem Git e exemplos sem credenciais reais.
- Como iniciar/parar banco, criar schema, aplicar migrações e inserir seeds fictícios.
- Como criar primeiro admin com procedimento controlado, sem senha fixa no repositório.
- Como iniciar frontend/API, portas e URLs reais de health/UI/OpenAPI.
- Como executar testes unitários, integração em PostgreSQL descartável e E2E.
- Como testar nova sessão, aluno A/B e admin com dados fictícios.
- Como desligar tudo sem apagar dados e como recriar banco SOMENTE descartável de testes.
- Onde ficam logs e como garantir que não exibem tokens, relatos ou respostas.

## Convenção proposta de variáveis, nomes finais a definir

Ambiente, conexão de banco, frontend origin, URL pública da API, Google client ID/secret,
identificação de conta/projeto Forms, credencial de integração, provedor e-mail, provedor
checkout e credencial de vídeo. Variável prefixada para frontend é pública: não use para segredo.

## Não fazer

Não colar comandos de migrations/compose que ainda não existem e afirmar que funcionam.
Não usar banco de produção para testes; não executar remoção de volumes para resolver build.
Adaptadores fake só em Development/Test e com teste de falha de inicialização em Production
se ainda configurados. Pagamento ou resposta Forms não devem ter bypass de produção.
