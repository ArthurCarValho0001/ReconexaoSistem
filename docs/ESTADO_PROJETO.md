# Estado do projeto para retomada

Atualização: 31/10/2026. T01 ACEITA por Arthur com base nas evidências registradas e
na conferência manual da tela local, conforme mensagem explícita nesta sessão.
Revisões QA/segurança concluídas; sem produção ou funções T02+.

- Branch da implementação T01: `task/t01-fundacao`; criada exatamente de
  `6e2fe6657a0edb580b01d284995810701162acf7`, commit aprovado da T00.
- Branch T00 `task/t00-alinhamento-tecnico` preservada. Fechamento T01 autorizado por
  Arthur com a mensagem `feat(t01): criar fundacao local do Reconexao`; hash consultável
  no histórico Git e informado na resposta final. Sem push ou merge.
- T00: ACEITA documentalmente; base e ajuste Compose registrados no ADR-0001.
- T01: IMPLEMENTADO, VALIDADO_LOCAL e ACEITO em 31/10/2026; aceite restrito à fundação.
- Stack executada: SDK 10.0.402, ASP.NET 10.0.13, EF Core 10.0.13, provider/driver
  Npgsql 10.0.3, React 19.3.1, TypeScript 5.9.3, Vite 7.3.7, Node 24.22.2/npm 11.19.1.
- Docker 29.9.2, Compose 5.6.1, desktop-linux. CLI por caminho completo da instalação
  em LOCALAPPDATA/Programs/DockerDesktop/resources/bin; sem reinstalação ou PATH global.
- PostgreSQL real 18.6-bookworm no digest aprovado, PGDATA /var/lib/postgresql/18/docker.
  Dev: volume persistente reconexao-dev_dev_data e loopback 55432. Test: tmpfs/banco/
  usuários/secrets separados e loopback 55433. Isolamento e persistência verificados.
- Código: API/health e GET foundation, contexto/migração marcador estático, tela inicial
  com sucesso/falha/retry. Sem login, contas, perfis, sessões, Forms, pontos ou pagamentos.
- Testes reais: .NET 9/9 em PostgreSQL descartável, repetidos após recriar só testes;
  Chrome E2E 3/3 com API real e 1/1 com API realmente parada. Build .NET 0 avisos/erros,
  frontend TypeScript/Vite passou, NuGet locked restore passou, npm audit produção 0.
- Revisões: agentes QA e segurança separados, em paralelo, somente leitura. Dois achados
  baixos corrigidos: assert de dados na persistência e senha fora argv do bootstrap psql.
  Ambos conferiram as correções; retestes operacionais foram executados pelo principal.
- Falhas anteriores e correções: npm/cache/certificado bloqueados no sandbox, acessos
  autorizados; EF patches divergentes, tipo CA, flags PFX, exceção EF encapsulada,
  inspeção tmpfs e aspas SQL corrigidos com retestes. Não apagar essas evidências:
  docs/evidencias/T01_FUNDACAO.md contém resultados e limitações.
- Credenciais: geradas só em local-data e ignoradas; .env só referências a secrets;
  API recebe conexão no ambiente do processo. Nenhum valor em Git/prompts/docs.
- Integrações: somente banco local real. Sem fornecedor externo ou adapter fake.
- Fórmula oficial 3FM/fornecedores continuam pendentes. P03/P04/P06/P08/P19–P22 e
  demais pendências por módulo preservadas; P01 resolvida na T00.
- Próximo passo: T02 somente quando solicitada; não iniciada neste fechamento.
  API/Vite e bancos locais disponíveis; parada documentada.
- Reprodução: docs/EXECUCAO_LOCAL.md; evidência T01 e capturas sanitizadas em
  docs/evidencias/media. Não marcar T04/T07/T09 prontos com mocks.

## Estados e histórico

### Preparação de agentes — 31/10/2026

- Branch atual: `task/preparar-agentes`, criada de `main` em `69e8cec`.
  `git fetch origin` confirmou merge remoto da T01 (PR #1), com T00 `6e2fe66`
  e T01 `73437ad` no histórico. A main local estava em `8d01736`; foi avançada
  por referência até a main remota conferida, sem criar merge/commit nesta tarefa.
  O diretório de trabalho estava limpo antes da preparação.
- Configuração IMPLEMENTADA: backend, frontend, banco, devops, QA e segurança;
  pesquisador preservado. Limite de dois subagentes e um implementador editando por vez.
  Fluxo: [FLUXO_AGENTES.md](FLUXO_AGENTES.md).
- Cliente: `codex-cli 0.164.0-alpha.11.1`, da extensão VS Code
  `openai.chatgpt-26.1021.22027-win32-x64`. Configuração nativa estrita passou;
  novo processo local reconheceu os sete papéis no catálogo do prompt.
- Delegações reais da sessão anterior: QA, segurança e pesquisador, em grupos de no máximo
  dois, somente leitura. Definições atualizadas foram lidas explicitamente;
  isso não prova recarga automática no chat anterior ou no atual.
- Backend/frontend/banco/devops: tentativas na sessão anterior retornaram
  `unknown agent_type`. Na retomada de 31/10/2026, os quatro tipos exatos foram
  reconhecidos e executados: `/root/validar_backend`, `/root/validar_frontend`,
  `/root/validar_banco` e `/root/validar_devops`, em dois pares, com respostas finais
  aguardadas. Somente inspeção de autenticação futura, telas/API, contexto/migrações/
  isolamento e scripts/Compose; nenhuma edição pelos agentes ou início T02.
- Preparação IMPLEMENTADA, VALIDADO_LOCAL e ACEITO por Arthur em 31/10/2026,
  com a mensagem: “Aceito a preparação da equipe com base nas delegações reais
  registradas.” Bloqueio dos quatro resolvido. Não é VALIDADO_INTEGRACAO. QA, segurança
  e pesquisador mantêm provas históricas, sem nova execução neste chat. Sete TOML e
  config preservados por SHA256; sem mudança de permissões ou fallback genérico.
  Erros de leitura de caminhos Compose `.yml` foram corrigidos para `.yaml`; consulta
  opcional a `.github` inexistente falhou. Mensagens exatas e limites na evidência.
- Evidência: [PREPARAR_AGENTES](evidencias/PREPARAR_AGENTES.md). Fechamento autorizado
  com `chore: configurar e validar equipe de agentes`, fetch, push sem force e PR
  para main. Sem alteração funcional, teste da aplicação, início T02, merge ou produção.

IMPLEMENTADO, VALIDADO_LOCAL, VALIDADO_INTEGRACAO e ACEITO são estados distintos.
Não houve testes de autenticação/admin/aluno, fornecedores, backup/restore ou produção.
Inspeção visual inicial usa referência textual, sem snapshot oficial P18. Não resolver
pendências de negócio para avançar módulos. Novo chat deve ler este arquivo antes de agir.

A máquina foi informada preparada por Arthur; T00 inicialmente encontrou Docker fora
do PATH e depois confirmou instalação por usuário/Engine com escalonamento autorizado.
Política Restricted do PowerShell e confiança global de certificados foram preservadas.
Após recriar o banco dev, uma conexão antiga do pool pode retornar 503 na primeira
consulta; UI permite repetir. E2E de sucesso aguarda prontidão real com limite de 15s,
e passou também imediatamente após nova recriação. Falha inicial e reteste registrados.
