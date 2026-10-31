# Inventário local — T00

Data: 31/10/2026. Inspeção somente leitura; nenhum pacote/serviço instalado.
Repositório inicialmente limpo em `main`, commit `8d01736`.
Branch de trabalho: `task/t00-alinhamento-tecnico`.

| Verificação | Resultado real |
|---|---|
| `git status --short --branch` inicial | `main...origin/main`, sem alterações |
| `git ls-files` | Documentação/configuração; sem aplicação, projetos ou suítes |
| `git log -1 --format="%h %s"` | `8d01736 docs: adicionar documentacao inicial do Reconexao` |
| `dotnet --list-sdks`; `dotnet --info` | SDK 10.0.402, ASP.NET runtime 10.0.13, Windows x64; sem global.json |
| `node --version` | v24.22.2 |
| `npm --version` | FALHOU: npm.ps1 bloqueado por política do PowerShell |
| `Get-ExecutionPolicy` | Restricted; preservada |
| `npm.cmd --version` | 11.19.1; executável Windows funciona sem mudar política |
| `git --version` | 2.56.0.windows.4 |
| `Get-Command dotnet,node,npm,git,docker,psql,pg_ctl -ErrorAction SilentlyContinue` | dotnet/node/npm/git encontrados; demais não localizados no PATH |
| `docker --version` | FALHOU: comando não encontrado |
| `Get-ChildItem 'C:\Program Files\PostgreSQL' -ErrorAction SilentlyContinue` | Sem resultado; não comprova ausência em outros caminhos |
| `Get-Service -Name '*postgres*','*docker*' -ErrorAction SilentlyContinue` | Sem serviços encontrados por esses nomes |
| `git switch -c task/t00-alinhamento-tecnico` | Primeira tentativa bloqueada pelo sandbox na referência Git; repetida com escalonamento, sucesso |

Os comandos agrupados de descoberta tiveram falhas parciais; ausência de saída nas
buscas com ErrorAction SilentlyContinue é inconclusiva fora dos caminhos consultados.
Não foram testadas conexão de banco, portas livres, certificados, restore, build,
migrações, E2E ou integração externa. Não há serviço fake nem real executado.

Proposta inicial nativa substituída pela aprovação de Arthur: usar ferramentas já
presentes e PostgreSQL via Docker Compose. Arquivos de ciclo de vida ainda faltam.
Scripts PowerShell da T01 devem considerar a política Restricted; não presumir que
`.ps1` executa nem reduzir proteções. Preferir comandos `.cmd`/executáveis e instruções
compatíveis; se script exigir mudança de política, explicar e obter decisão de Arthur.

## Fontes oficiais consultadas

Consulta web nesta T00 para linhas suportadas; versões locais acima vêm do computador,
não da web. Revalidar patches e compatibilidade na T01 antes de fixar dependências.

- [.NET: política de suporte](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
- [Node: calendário oficial](https://github.com/nodejs/Release).
- [PostgreSQL: política de versões](https://www.postgresql.org/support/versioning/).

Essas fontes sustentam a escolha de linhas com suporte; não atestam instalação do
PostgreSQL nem resolução do conjunto React/TypeScript/Vite/EF/Npgsql.

## Complemento após aprovação com Docker Compose — 31/10/2026

Diagnóstico corrigiu a descoberta inicial: Docker Desktop existe e está em execução,
mas a CLI está fora do PATH desta sessão. `docker info` e `docker compose version`
iniciais falharam por comando não encontrado; caminho Program Files também ausente.
Get-Process encontrou instalação em AppData/Local/Programs/DockerDesktop; consulta
desse diretório no sandbox retornou Acesso negado, não ausência comprovada.

Com escalonamento autorizado, execução por caminho completo
`C:\Users\Arthur\AppData\Local\Programs\DockerDesktop\resources\bin\docker.exe`:

| Consulta real | Resultado |
|---|---|
| `info` | Cliente/servidor 29.9.2, desktop-linux, Linux x86_64, Docker Desktop/WSL2 |
| `compose version` | Docker Compose v5.6.1 |
| `buildx imagetools inspect docker.io/library/postgres:18.6-bookworm` | PASSOU, linux/amd64 disponível, índice sha256:5c4783a8cc3f0074912164f94dfa1be4e043438f0f707cf968d7af9a0d31a2c8 |
| `Get-NetTCPConnection -LocalPort 55432 -State Listen -ErrorAction SilentlyContinue` | Sem listener encontrado; confirmar bind na T01 |

Nenhuma política/PATH/instalação alterada, nenhuma camada baixada ou container iniciado.
Não confundir acesso ao Engine/registro com banco validado. Imagem oficial e suporte
18.6 confirmados em https://hub.docker.com/_/postgres e política de versões acima.
Volume PostgreSQL 18 deve montar `/var/lib/postgresql`; integração EF 10/driver será
testada na T01, conforme https://www.npgsql.org/efcore/release-notes/10.0.html.
