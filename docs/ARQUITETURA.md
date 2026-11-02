# Arquitetura técnica aprovada na T00

STATUS: BASE APROVADA por Arthur em 31/10/2026, com PostgreSQL local em Docker Compose.
Implementação e testes funcionais ainda não executados. Não autoriza contratar fornecedor.

## Base recomendada

- Backend: ASP.NET Core em .NET 10 LTS, versão suportada fixada em `global.json` na T01.
- Frontend: React + TypeScript + Vite; fixar dependências e lockfile, sem usar `latest`
  silenciosamente como contrato de compatibilidade.
- Banco: PostgreSQL em versão suportada escolhida em T00; mesmo motor local/testes/produção.
- Escolha aprovada: PostgreSQL via Docker Compose, imagem `postgres:18.6-bookworm`
  fixada também por digest no ADR. Volume dev persistente em `/var/lib/postgresql`,
  testes em projeto/banco separado descartável, sem compartilhar volume dev.
  Portas apenas loopback (dev 55432, testes proposta 55433); credenciais fora do Git.
  Docker Engine/Compose consultados com sucesso, sem iniciar containers; detalhes no
  [ADR-0001](adr/0001_STACK_PROPOSTA.md) e [inventário](INVENTARIO_T00.md).
- EF Core e driver PostgreSQL compatíveis com o SDK; confirmar versões oficiais em T01.
- Monólito modular: um backend, um banco e módulos claros. Não iniciar com microsserviços,
  mensageria externa ou múltiplos bancos sem necessidade demonstrada.
- Identidade local aprovada: ASP.NET Core Identity e cookies HttpOnly, com estado de
  sessão revogável no servidor, CSRF para operações de navegador e política CORS restrita.
  Vinculação Google é fluxo separado que não concede acesso financeiro por si só.

A opção cookie exige revisar domínio, proxy e SameSite no fluxo Google. Não persistir
tokens de sessão em localStorage como atalho. Segredos locais em user-secrets/configuração
ignorada; nunca no frontend ou numa variável VITE pública. Persistir chaves de proteção
de dados conforme ambiente; não perder sessões/reset tokens por configuração acidental.

## Estrutura de aplicação a criar após aprovação

| Pasta | Conteúdo pretendido |
|---|---|
| src/backend | API, aplicação/domínio, persistência, adapters e migrações |
| src/frontend | Telas e componentes |
| tests | Testes de domínio, integração e E2E |
| infra | Configuração local e, depois, implantação aprovada |
| docs | Decisões, evidências e instruções |

Preferir estrutura simples antes de multiplicar projetos/camadas. Contratos REST e erros
documentados antes de dividir frontend/backend entre agentes. API usa IDs opacos, paginação,
validação, auditoria mínima e política de acesso a recursos. Health não expõe segredos.

## Adapters e tarefas assíncronas

Interfaces para pagamentos, vídeos, Forms e e-mail. Implementações reais dependem do
fornecedor; implementações de teste são explicitamente locais. Inbox de notificações,
processamento idempotente e recuperação/reconciliação evitam perda de eventos. Uma fila
no banco/worker do backend pode bastar; avaliar antes de contratar infraestrutura extra.

Importação Forms e pontos: confirmar versão e completude, registrar resposta e ledger de
modo consistente. Timestamp de envio e de recebimento separados. Banco deve proteger
unicidade, não só `if` antes do insert. Contenção em turmas precisa de transação/lock adequado.

## O que podemos validar localmente

Persistência, login local, perfis, relatos, catálogo, ledger, turmas e importações com fixtures.
Google OAuth/Forms reais precisam de conta/configuração/conectividade, mesmo com aplicação
local. Callbacks do servidor Google exigem endpoint HTTPS alcançável ou alternativa de
consulta pela API; localhost não é endpoint público. Não contratar túnel sem aprovação.

## Versões

T00/T01 devem verificar versões no momento da instalação. .NET 10 e Node 24 LTS foram
consultados em 26/10/2026 (links em `FONTES.md`); não congelamos patch neste kit.

Em 31/10/2026, ferramentas locais foram inspecionadas e linhas de suporte consultadas
novamente. A proposta detalhada e o mecanismo de identidade estão no ADR-0001;
patches e compatibilidade funcional do conjunto serão fixados/testados na T01.
PostgreSQL 18.6-bookworm teve manifesto linux/amd64 confirmado via Docker buildx;
Engine Linux x86_64 29.9.2 e Compose v5.6.1 responderam. Docker fora do PATH foi
localizado na instalação por usuário; nenhum fallback para banco nativo/SQLite.

## Fundação implementada na T01

API em src/backend/Reconexao.Api, frontend em src/frontend, suíte de integração em
tests/Reconexao.IntegrationTests e Compose separados em infra. Migração InitialFoundation
cria somente marcador estático; não implementa entidades ou fluxos dos módulos futuros.
API só expõe health e GET /api/foundation; UI verifica API real, com estado de falha/retry.
Não há autenticação, bootstrap admin ou adapters de fornecedores nesta entrega.

EF Core/Relational/Design 10.0.13, provider/driver Npgsql 10.0.3, React 19.3.1,
TypeScript 5.9.3 e Vite 7.3.7 foram fixados e compilados com o SDK 10.0.402 e Node
24.22.2. Dependências completas em lockfiles. API HTTPS 5001 e Vite HTTPS 5173
com CA local explícita; nenhuma mudança global de confiança/PATH/política PowerShell.
PostgreSQL 18.6 em volume dev persistente e tmpfs de testes, dados/credenciais isolados.
Comandos reais e limitações em EXECUCAO_LOCAL e evidência T01.

## Identidade implementada na T02

ASP.NET Core Identity 10.0.13 no contexto existente, com tabelas Identity e campos
de perfil/sessão. Cookie seguro com validação de sessão/perfil/security stamp por
requisição; login/reset/logout em transações PostgreSQL com bloqueio de linha por
usuário. Perfis permitidos e e-mail normalizado único protegidos no banco.
Não há cadastro público nem identidade/pontuação confiada ao navegador.

CSRF nas escritas, lockout e rate limit local. Bootstrap admin e fixtures explícitos
por CLI, sem seed automático. `IRecoveryEmail` separa o adapter local de arquivos,
identificado e proibido em produção. Nenhum fornecedor real foi integrado.
Chaves Data Protection persistidas em caminho configurado fora do Git; localmente
dependem de proteção do diretório do usuário, com aviso de ausência de criptografia
XML em repouso. Configuração operacional de chaves/criptografia/replicação e rate
limit distribuído devem ser revisados antes da implantação, sem alegar produção pronta.
Contratos e políticas técnicas em CONTRATOS_API; testes/evidência T02.

## Relatos privados implementados na T03

Módulo `Reports.cs`, persistência no contexto existente e componente `ReportsPanel`.
POST aluno autenticado com CSRF, leitura JSON limitada a 16 KiB, campos adicionais
recusados, autor/data do servidor, validação por escalares Unicode e erros genéricos.
GET paginado somente admin, incluindo todos os admins pela decisão D16. Sem rotas
de histórico do aluno, edição/exclusão, resposta, pontos ou regras comerciais.
Texto administrativo renderizado literalmente pelo React, sem HTML interpretado.
Formulário em memória preserva erros de envio e limpa apenas após confirmação;
logout/perda de sessão desmonta conteúdo, consultas antigas são abortadas/descartadas.
P03/P11 não foram resolvidas. Sem serviço externo, armazenamento de rascunho ou logs
de conteúdo. Contratos, contagem Unicode e resultados em CONTRATOS_API/evidência T03.
