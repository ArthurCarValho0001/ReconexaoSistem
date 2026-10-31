# Arquitetura técnica proposta para T00

STATUS: PROPOSTA. Arthur ainda não confirmou a stack. T00 termina com aprovação registrada
ou alternativas para decidir. Este kit não autoriza contratar fornecedor.

## Base recomendada

- Backend: ASP.NET Core em .NET 10 LTS, versão suportada fixada em `global.json` na T01.
- Frontend: React + TypeScript + Vite; fixar dependências e lockfile, sem usar `latest`
  silenciosamente como contrato de compatibilidade.
- Banco: PostgreSQL em versão suportada escolhida em T00; mesmo motor local/testes/produção.
- EF Core e driver PostgreSQL compatíveis com o SDK; confirmar versões oficiais em T01.
- Monólito modular: um backend, um banco e módulos claros. Não iniciar com microsserviços,
  mensageria externa ou múltiplos bancos sem necessidade demonstrada.
- Proposta de identidade local: ASP.NET Core Identity e cookies HttpOnly, com estado de
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
