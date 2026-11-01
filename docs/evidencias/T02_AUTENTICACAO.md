# Evidência — T02 Autenticação e sessões

- Estado: IMPLEMENTADO, VALIDADO_LOCAL e ACEITO por Arthur em 01/11/2026, considerando
  a conferência manual anterior e o diagnóstico/correções da ressalva abaixo.
  Não VALIDADO_INTEGRACAO externa.
- Data: 01/11/2026. Principal, implementação backend delegada, revisões QA/segurança.
- Branch: `task/t02-autenticacao-sessoes`; base main `6c406d4a14423e2b9b8d215a440b8ead4facde2c`.
  Árvore inicialmente limpa, PR #2 integrado. Fechamento autorizado com
  `feat(t02): implementar autenticacao e sessoes revogaveis`; hash consultável no Git.
- Requisitos: R01/D20, C05/C06; sem T03+, Google T04 e gate inicial P03 pendentes.
- Alterações: Auth/Identity, migração AuthenticationSessions, contexto, API, frontend,
  scripts locais, testes e documentos. Banco dev e infraestrutura Compose preservados.
- Ambiente: Windows/PowerShell, SDK 10.0.402, ASP.NET/EF/Identity 10.0.13,
  Npgsql 10.0.3, Node 24.22.2, React 19.3.1, Vite 7.3.7, PostgreSQL real 18.6,
  Docker 29.9.2/Compose 5.6.1; versões anteriores preservadas, Identity fixado no lockfile.

## Implementação e limites

Cookie HttpOnly/Secure/Strict, sessão de oito horas sem sliding, ID substituído no
login e validado com perfil/stamp por requisição. Login, reset e logout usam transação
e `SELECT FOR UPDATE` por usuário. Banco restringe perfil e unicidade de e-mail.
Não há criação pública de usuário/admin; perfil não é recebido como autoridade.
CSRF em todos POSTs, senha >=12 conforme Identity, lockout 5/15min, limite 20/min/IP.
Pedido de recuperação mantém sessão, reset de 30min revoga e é consumido uma vez.

E-mail LOCAL_SIMULADO em arquivos ignorados apenas dev/test, sem entrega externa.
202 não comprova envio. Falhas estáveis de disponibilidade são 503 genéricas; falha
após probe mantém 202 e warning genérico para não enumerar conta. Produção rejeita
adapter Local; sem fornecedor real, recuperação indisponível. Não há vínculo Google
fictício, gate inicial, compra, convite financeiro ou fórmula 3FM implementados.

## Comandos e resultados reais

| Teste/comando | Resultado | Evidência sanitizada |
|---|---|---|
| `git status --short --branch`; `git switch -c task/t02-autenticacao-sessoes` | PASSOU | Main limpa em 6c406d4; branch criada com acesso autorizado ao Git |
| `node --version`; `dotnet --version` | PASSOU | 24.22.2 e 10.0.402 |
| `node scripts/local.mjs docker-info` inicial | FALHOU | Engine parado: `open //./pipe/dockerDesktopLinuxEngine: O sistema não pode encontrar o arquivo especificado.` |
| Iniciar Docker Desktop já instalado; `docker-info`; `test-db-up` | PASSOU | Sem instalação/proteções alteradas; somente test saudável |
| `dotnet restore Reconexao.slnx` pelo backend | PASSOU | Identity 10.0.13 com lockfiles gerados pelo NuGet |
| `dotnet restore Reconexao.slnx --locked-mode` | BLOQUEOU sandbox; PASSOU com acesso de ferramenta | NuGet.Config do usuário inicialmente inacessível; repetição exit0 |
| `dotnet build Reconexao.slnx --no-restore` | PASSOU | 0 avisos/erros após correções; rodada final exit 0 |
| `dotnet ef migrations add AuthenticationSessions ... --no-build` pelo backend | PASSOU | 20261101180858; geração sem aplicar dev |
| `test-db-discard`; `test-db-up`; `migrate-test` | PASSOU | Somente projeto reconexao-test/tmpfs recriado; duas migrações aplicadas |
| `node local-data/verify-t02-bootstrap.cjs` | PASSOU | Primeiro admin por CLI em test vazio; repetição recusada, credenciais transitórias não exibidas |
| `node scripts/local.mjs test` | PASSOU | 24/24 PostgreSQL real, zero ignorados; repetido após recriar test e complementar cenários |
| `node scripts/local.mjs auth-seed-test` | PASSOU | A/B/admin fictícios, senha aleatória em arquivo ignorado, sem valores na saída |
| `node --check scripts/local.mjs`; `node scripts/local.mjs test` após guarda do destino | PASSOU | Sintaxe válida; guarda test e suíte 24/24 confirmadas |
| `npm.cmd --prefix src/frontend run build` | BLOQUEOU sandbox; PASSOU com acesso de ferramenta | Esbuild não lia diretório ancestral; TypeScript/Vite passaram, sem alterar proteções |
| `api-test`; Vite `run dev`; `run test:e2e` | PASSOU após correções | 8/8 Chrome, API/PostgreSQL reais; cinco auth e três regressão T01 |
| `git diff --check`; novos arquivos com `--no-index --check -- NUL` | PASSOU | Sem erro whitespace; avisos Git LF/CRLF não são falha |
| `git check-ignore local-data/auth-test.json local-data/email-test local-data/keys-test` | PASSOU | Senhas, inbox e chaves ignorados; nenhum valor incluído nesta evidência |
| Integração real de e-mail/Google, migração dev, produção, T03+ na primeira etapa | NÃO EXECUTADO | Migração dev posteriormente autorizada/executada na complementação abaixo; demais limites preservados |

## Cobertura C05/C06

Integração: login sequencial e concorrente deixa só uma sessão válida; replay de
cookie após logout negado; logout com cookie antigo não invalida sessão nova;
pedido de recuperação preserva sessão; reset válido revoga; senha antiga negada e
nova aceita; reset concorrente consome uma vez; tokens inválido/expirado/reutilizado
negados. Conta A não muda autor ao fornecer ID real de B; anônimo 401/aluno 403 em
rota admin; perfil forjado ignorado, sem endpoint de registro. Perfil alterado no
banco invalida cookie. CSRF, cookie seguro, lockout/rate, headers, adapter proibido
em produção e falhas locais também cobertos. Seed idempotente e bootstrap recusado
com admin existente; primeira criação comprovada adicionalmente por CLI.

E2E: aluno A entra, segundo navegador revoga primeiro e logout encerra; admin com
perfil do servidor e falha de credenciais; B pede recuperação mantendo sessão e
redefine com token do adapter local, revogando e negando reuso; falha de rede
explicitamente simulada preserva e-mail e apresenta erro, celular360px/teclado.
Regressão T01 usa API real; somente o cenário de falha de rede aborta request no navegador.
Capturas locais `evidencia-t02-admin.png`/`mobile.png` inspecionadas sem dados reais;
não foram incluídos inbox, senhas, códigos ou chaves em artefatos versionados.

## Achados, correções e revisões

- Backend corrigiu nullability CS8602 e analisador xUnit2031; build posterior 0/0.
  Primeira rodada 14/19 falhou por DataProtection/EventLog nas factories antigas;
  caminhos temporários isolados e logging sem providers nos testes corrigiram;
  rodadas 19/19, 21/21 e finais 24/24 passaram. Substituição inicial PowerShell falhou,
  corrigida sem alterar política. Comandos/resultados informados pelo implementador.
- Segurança/médio: falha de IO/ACL só na conta existente podia enumerar pelo503.
  Probe antes de consultar conta e 202 genérico no erro posterior corrigiram; testes
  de path inválido e IOException após probe passaram. Warning sem email/token.
- Segurança/baixo: headers depois da autorização não cobriam respostas antecipadas.
  Movidos antes de auth/rate; teste 429 confirma no-store. Segurança revalidou ambos.
- E2E inicial 6/7 e duas repetições focadas falharam no cenário de sessão: seletor
  “Sua conta” aceitava “Acesse sua conta” e avançava antes do login terminar.
  Conferência temporária apenas de nomes/tamanhos de cookies confirmou a corrida;
  nenhuma credencial exibida. Seletor exato corrigido, diagnósticos removidos, 7/7 passaram.
- Runtime avisou ausência de encryptor XML das chaves locais. Persistência configurada
  fora Git; proteção do diretório e operação/criptografia de produção documentadas.
  CSRF gerava warning ao sobrescrever cache; handler passou a definir os headers
  corretos antes da emissão do token, sem reduzir proteção.

Backend `/root/t02_backend` editou somente backend/testes; principal aguardou handoff
antes de editar frontend/scripts/complementos. QA e segurança em leitura, máximo dois
subagentes simultâneos, sem agentes abrirem outros agentes. Segurança revalidou
código final sem novo bloqueador estático fundamentado; não executou suites.
QA confrontou C05/C06, código e documentos. Encontrou corrida visual de consulta
atrasada sobrescrevendo login/logout; geração de consulta e bloqueio durante mutações
corrigiram. Novo E2E retém respostas reais até após a ação e as libera, sem fabricar
conteúdo; cenário focal passou 1/1. QA revalidou estaticamente, sem novo bloqueador
fundamentado; não executou suites. Resultado completo final registrado na tabela.

Fontes técnicas oficiais consultadas para implementação, sem mudança de stack:
[Identity .NET 10](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-10.0),
[Antiforgery](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-10.0)
e [Data Protection](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-10.0).

## Limitações e reprodução

Aceite explícito de Arthur registrado no fechamento abaixo, sem integração externa.
Fornecedor de e-mail,
entrega/retry T08, Google T04 e definição do gate P03 seguem pendentes. Rate limit
em memória é por processo e não prova operação distribuída; a proteção operacional
das chaves depende do ambiente futuro. Testes de autenticação não homologam módulos
privados ainda inexistentes. Nenhum fornecedor contratado, segredo versionado ou dado real.
Ao concluir a primeira etapa, API test HTTPS e Vite estavam disponíveis; na
complementação abaixo, API passou ao dev. Somente test foi recriado na primeira
etapa; dev nunca foi limpo/recriado nesta tarefa.
Reproduzir pela seção T02 de docs/EXECUCAO_LOCAL.md; contratos em CONTRATOS_API.md.

## Complementação — demonstração manual estável em dev, 01/11/2026

Arthur solicitou duas contas fictícias reutilizáveis no dev persistente, senhas
estáveis em configuração local ignorada e exibição direta na resposta final, sem
senhas em AGENTS/fluxo/documentos. Autorização restrita a essas contas locais;
credenciais reais, chaves e tokens continuam proibidos. Sem acesso comercial real.

Implementado `auth-demo`/`--seed-local-demo` para somente `admin@example.test` e
`aluno.a@example.test`, usando o seed seguro existente. Reutiliza contas compatíveis,
não redefine senhas, não promove perfil divergente, não executa no startup e recusa
produção. Senha dev preexistente preservada; valores não entram nesta evidência.
Guarda dos scripts passou a conferir ambos os destinos dev/test. `test` aceita filtro
para validação focal sem executar toda a suíte.

| Comando/prova | Resultado real | Evidência sanitizada |
|---|---|---|
| `git status --short --branch` | PASSOU | Mesma branch T02 e alterações anteriores sem commit preservadas |
| `node --check scripts/local.mjs`; `docker-info`; `db-start` | PASSOU | Início do container dev existente; sem recriação |
| `node local-data/verify-dev-demo.cjs before/after` | PASSOU | Helper ignorado comparou ID container, volume, digest marcador, usuários preexistentes e SHA256 config da senha; intactos |
| Primeiro build | FALHOU | MSB3027/MSB3021: executável bloqueado por API local anterior; migração/seed ainda não executados nessa tentativa |
| Conferir caminho do processo e encerrar somente API anterior; repetir build | PASSOU | Mesmo executável deste repo; build final 0 avisos/erros |
| `node scripts/local.mjs migrate` | PASSOU | Migrações dev 1→2; apenas alteração aditiva, sem limpeza/recriação |
| `node scripts/local.mjs auth-demo`, duas vezes | PASSOU | Duas contas adicionadas; repetição sem nova conta nem troca da senha local |
| `node scripts/local.mjs test --filter FullyQualifiedName~SeedIsExplicitIdempotentAndBootstrapCannotPromoteExistingAccount` | PASSOU | 1/1 em test: demo cria/reutiliza admin/aluno, preserva IDs/hashes/perfis, não inclui B e recusa produção |
| `node scripts/local.mjs api` | PASSOU | Development no dev persistente; substitui API test na porta 5001 |
| Tentar novo Vite `run dev` | FALHOU sem efeito na demonstração | `Port 5173 is already in use`; Vite existente reutilizado e verificado no smoke real |
| `node local-data/smoke-dev-demo.cjs` | PASSOU | Chrome: dois logins com IDs conferidos no banco dev, perfil correto, admin200/aluno403 e logout401; sem senhas/tokens na saída |
| `node scripts/local.mjs verify-storage` | PASSOU | Dev volume persistente, test tmpfs, portas loopback 55432/55433 e armazenamentos separados |
| `git diff --check` e checagem dos novos arquivos | PASSOU | Sem erro de whitespace; senha fictícia procurada em memória nos arquivos alterados/versionáveis, nenhuma ocorrência; configuração local ignorada |
| Suite completa nesta complementação | NÃO EXECUTADO | Mudança delimitada; teste focal e smoke de duas contas suficientes, resultados anteriores 24/24 e 8/8 preservados como históricos |

Preferência registrada em AGENTS.md e FLUXO_AGENTES.md; execução local/estado/plano
atualizados. A aplicação disponível para conferência manual usa dev; test continua
descartável e separado. Nenhum segredo versionado, migração destrutiva, concessão
comercial, commit, push, merge ou início T03.

## Ressalva da conferência manual — sessão anônima e favicon

Arthur aprovou a conferência manual da T02 com ressalva: dois GET session 401 antes
de login 200 e favicon.ico 404. Nenhuma captura HAR/cookies dos pedidos históricos foi
fornecida; o diagnóstico abaixo reproduz os gatilhos no código atual, sem afirmar a
origem exata de cada pedido histórico.

Diagnóstico anterior à alteração, com Chrome em contexto novo/sem cookie, API real:
helper ignorado `local-data/diagnose-session.cjs` observou uma consulta inicial 401,
segunda após 31s virtuais e terceira após evento de foco: statuses `[401,401,401]`,
zero alertas. A interface tratava 401 como anônimo, sem lançar exceção; o intervalo
periódico de 30s continuava consultando mesmo anônimo. Não havia retry em laço de erro.
Abrir/fechar DevTools ou voltar à janela pode gerar foco, disparando outra consulta.

`main.tsx` usa `createRoot(...).render(<App />)` sem StrictMode, e nenhuma outra
referência StrictMode foi encontrada nas fontes. Assim, duplicação por checagem
extra de Effects do StrictMode não explica esta versão. Nada foi removido para
ocultar consultas. A [documentação oficial React](https://react.dev/reference/react/StrictMode)
confirma o ciclo extra de Effects quando StrictMode está habilitado em desenvolvimento;
essa condição não está presente no entrypoint atual.

Correção limitada: intervalo só consulta enquanto autenticado; primeira consulta e
revalidação por foco permanecem. Não altera backend/cookie/CSRF/perfis nem transforma
401 em 200. Sessão ausente, expirada ou revogada continua 401; autenticado sem permissão
continua 403. A mensagem “Sua sessão terminou” só aparece quando havia sessão antes;
primeira visita anônima mostra normalmente o formulário, sem alerta indevido.

Favicon: busca nas fontes/arquivos do repo não encontrou ícone/logo como asset. A
marca atual é R em círculo no HTML/CSS. Acrescentado `public/favicon.svg` reproduzindo
essa mesma marca/paleta e `<link rel="icon" type="image/svg+xml" href="/favicon.svg">`.
Não houve redesenho de identidade. Navegação passou a usar recurso explícito 200,
sem depender da busca implícita por `/favicon.ico`.

| Verificação focal | Resultado real | Evidência |
|---|---|---|
| Diagnóstico Chrome sem cookie, antes da alteração | PASSOU | 401 no carregamento, intervalo e foco; nenhum alerta |
| Primeiro build dos testes novos | FALHOU | TS2352 no tipo do contador de unhandledrejection; corrigido cast somente na instrumentação de teste |
| `npm.cmd --prefix src/frontend run build` após correção | PASSOU | TypeScript e Vite; favicon incluído no artefato |
| `npm.cmd --prefix src/frontend run test:e2e -- --grep "sessão anônima\|sessão autenticada\|favicon explícito\|consulta atrasada\|aluno entra"` | PASSOU | 5/5 Chrome, API real em test; somente cenários afetados |
| Anônimo: 95s virtuais sem foco | PASSOU | Uma consulta 401; zero alertas, pageerror ou unhandledrejection; nenhum polling anônimo |
| Revalidação de foco anônima | PASSOU | Segunda consulta 401, formulário normal; gatilho explícito, sem laço |
| Login válido e GET session | PASSOU | 200 com próprio email/perfil aluno e admin; polling autenticado 200; anônimo 401, aluno 403 em admin |
| Revogação e consulta atrasada | PASSOU | Dois cenários anteriores mantidos, sem regressão do estado de sessão |
| Favicon explícito | PASSOU | 200, Content-Type image/svg+xml, marca R; nenhum favicon 404 observado na navegação |
| `node local-data/smoke-dev-demo.cjs` após restaurar dev | PASSOU | Duas contas fictícias: login e GET session 200 com ID/perfil do dev, admin 200/aluno 403, logout 401; senhas preservadas |
| `git diff --check` e novos arquivos | PASSOU | Sem erros de whitespace; alterações anteriores preservadas |

Suites .NET/completa E2E não repetidas por ausência de alteração backend/domínio.
Banco dev e senhas preservados; API alternada temporariamente para test na execução
dos cenários, depois devolvida a Development para demonstração manual. Sem commit,
push, merge ou início T03.

## Aceite e fechamento — 01/11/2026

Arthur declarou: “Confirmo o aceite da T02, considerando minha conferência manual
anterior e o diagnóstico e correções registrados para a ressalva.” Aceite restrito
ao escopo T02 e aos ajustes autorizados de demonstração; não aprova integração
externa ou módulos seguintes. E-mail real/T08, Google/T04 e gate inicial/P03 permanecem
pendentes, assim como operação/criptografia de chaves e rate limit distribuído antes
da implantação.

Autorizados commit `feat(t02): implementar autenticacao e sessoes revogaveis`,
`git fetch origin`, comparação com `origin/main`, push da branch atual sem force e
PR para main “Implementar autenticação e sessões revogáveis”. Merge e T03 não
autorizados. Testes aprovados não repetidos neste fechamento; revisão do diff,
arquivos novos, exclusão de dados locais/segredos e whitespace conferidos antes do commit.
Revisão de fechamento: 27 arquivos de T02/demonstração/documentação; nenhuma mudança
em `.codex`, infraestrutura Compose ou regras de negócio. Migração aditiva e modelo
gerado conferidos; snapshot e designer correspondentes. Helper local ignorado
`audit-t02-commit.cjs` comparou os arquivos alterados/novos com valores protegidos da
configuração local em memória, sem exibi-los, e buscou assinaturas de chave privada/
tokens: nenhuma ocorrência encontrada. `git check-ignore` confirmou senhas, chaves
e inbox locais fora do Git. `git diff --check` passou; testes não repetidos.
Commit, link do PR e estado Git finais serão informados na resposta de fechamento.
