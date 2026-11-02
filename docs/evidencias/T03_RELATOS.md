# Evidência — T03 Relatos privados

- Estado: IMPLEMENTADO, VALIDADO_LOCAL e ACEITO por Arthur em 01/11/2026 após
  conferência manual e confirmação explícita do aceite.
- Data: 01/11/2026; principal, banco/backend/frontend, revisões QA/segurança.
- Branch: `task/t03-relatos`, base main `27c4a865d3dda0748247221d6cfe15fb80d81216`.
  Git inicialmente limpo e sincronizado conforme conferência real. Fechamento
  autorizado com `feat(t03): implementar relatos privados do aluno`; hash no Git.
- Critérios: C06/C07/C08, R26/R27/D16/D22. Gate P03 e retenção P11 permanecem pendentes.
- Alterações: módulo Reports, contexto/migração PrivateReports, interface aluno/admin,
  integração com sessão, testes e documentos. Sem pacote/fornecedor novo, T04 ou pontos.
- Ambiente: base T02 preservada: SDK 10.0.402, ASP.NET/EF/Identity 10.0.13, Npgsql
  10.0.3, Node 24.22.2, React 19.3.1/Vite 7.3.7, PostgreSQL18.6 em Compose.

## Contrato e implementação

POST `/api/relatos`: aluno autenticado, CSRF, somente `{text}`; dados adicionais de
autor/data são recusados. Autor vem da sessão e instante UTC do servidor. Sucesso
201 `{status:"received"}` após persistência, sem texto/ID/URL de consulta no retorno.
GET `/api/admin/relatos`: somente admins, inclusive dois admins distintos; autor,
instante e texto, limit 1–50/default20, cursor e ordem CreatedAt DESC/Id DESC.
Cursor usa instante lido do PostgreSQL, sem perder precisão. Sem GET de histórico
ou detalhe do aluno, pontos, resposta, edição/exclusão, gate ou retenção automática.

Contagem coerente frontend/backend: valores escalares Unicode; 1.000 emojis simples
cabem, 1.001 não. Acentos combinantes e componentes ZWJ contam separadamente; CRLF/CR
viram LF antes de contar/salvar, sem trim/NFC. White_Space-only, NUL e surrogates
isolados recusados. CHECK char_length reforça limite no banco; FK Restrict protege
autor e não apaga relatos em cascata. JSON de entrada limitado a 16 KiB, compatível
com 1.000 valores mesmo com escapes Unicode. Sem corpo privado em logs/erros.

Interface: aluno tem envio opcional, contador, guarda síncrona de envio em andamento,
rascunho preservado em erro e limpo apenas após confirmação201 válida; sem histórico
ou localStorage. Admin vê texto literal via JSX, autor e horário de Brasília,
paginação anterior/próxima e atualização. Logout/perda de sessão/troca de usuário
desmonta dados; cleanup/abort descarta respostas antigas. Rascunho não sobrevive recarga
ou saída; retries após resposta perdida não têm deduplicação prometida.

## Comandos e resultados reais

| Autor / comando ou prova | Resultado real | Evidência sanitizada |
|---|---|---|
| Principal: `git status --short --branch`; `git rev-parse HEAD`; criação da branch | PASSOU | Main limpa em27c4a865, branch de tarefa criada sem descarte |
| Principal: `docker-info`; `test-db-up`; parar API anterior do repo | PASSOU | Docker29.9.2/Compose5.6.1; apenas test preparado e API parada para compilar |
| Principal: `node local-data/t03-dev-proof.cjs before` | PASSOU | Digests dev/contas/configs registrados sem exibir dados/credenciais |
| Backend: build .NET e geração migration PrivateReports | PASSOU | 0 avisos/erros; geração sem aplicar dev |
| Backend: `node scripts/local.mjs test --filter FullyQualifiedName~ReportTests` inicial | PASSOU |24/24 casos iniciais em PostgreSQL test real |
| Backend: suíte .NET após quatro casos adicionais de cursor | PASSOU, reportado antes do handoff |52/52:24 base +28 relatos; build0/0 |
| Backend: ajuste isolado da fixture de paginação para tolerar dados fictícios de outras suites | PASSOU | Build0/0 e cenário focal1/1 após ajuste; não altera código de produção |
| Principal: `auth-seed-test` | PASSOU | Fixtures existentes de test reutilizadas, senha não exibida/trocada |
| Frontend: `npm.cmd --prefix src/frontend run build` | PASSOU | TypeScript/Vite, sem falha intermediária reportada |
| Frontend: `npm.cmd --prefix src/frontend run test:e2e -- reports.spec.ts` | PASSOU |4/4 Chrome, API/PostgreSQL test reais |
| Frontend: `npm.cmd --prefix src/frontend run test:e2e -- auth.spec.ts --grep "aluno entra\|consulta atrasada"` | PASSOU |2/2 regressões de sessão reais |
| Principal: `node local-data/t03-ui-smoke.cjs` | PASSOU |390px, 1.000 emojis enviados201 e campo limpo; aluno/admin sem overflow; capturas locais fictícias |
| Principal: `db-start`; `migrate`; `auth-demo`; `t03-dev-proof.cjs after` | PASSOU | Dev 2→3; mesmo container/volume, marcador/usuários e arquivos de senhas intactos; sem limpar/recriar |
| Principal: `verify-storage`; iniciar `api` no dev; smoke local das duas contas | PASSOU | Dev persistente e test tmpfs separados; login/perfil/GET sessão/autorização/logout confirmados, sem senhas na saída |
| Principal: `node local-data/audit-t03.cjs`; `git diff --check` e novos arquivos | PASSOU |20 arquivos de T03/documentação; nenhum valor local protegido/assinatura de segredo; snapshot/designer consistentes e whitespace válido |
| Gate P03, retenção P11, produção/T04 | NÃO EXECUTADO | Dependências preservadas; nenhuma decisão de negócio inventada |

## Cobertura e revisões

Integração real: limites 1.000/1.001 ASCII/emoji/combining, família ZWJ e quebras de
linha; vazio/Unicode White_Space/NUL/UTF-16 inválido; autor/data forjados; CSRF;
anônimo sem acesso e aluno sem consulta própria/terceiros/admin; admin não envia como
aluno; autor/UTC reais e confirmação sem eco/pontos; dois admins consultam; cursor
inválido/limites/timezone/overflow, empates e inserção entre páginas sem duplicar ou
omitir a sequência; DB CHECK Unicode; persistência falha503 sem conteúdo nos logs/
erros. Logger captura mensagens/exceções somente em memória; asserções não imprimem
o relato. Sem dados reais em testes.

E2E: contador/limites, erro de rede antes do envio preserva texto, confirmação real
limpa, pedidos concorrentes do form suprimidos em voo; paginação/retorno à página
anterior; HTML/script como texto sem nó/execução; anônimo401/aluno403, ausência de
histórico, CSRF400; sessão revogada remove formulário e consulta administrativa
atrasada não reaparece após logout. Regressão login/logout/revogação executada.

Delegações executadas, em no máximo dois subagentes simultâneos:

- `/root/t03_banco_contrato`: leitura e recomendações de contrato/modelo/índice/cursor,
  sem editar ou operar DB. Fontes oficiais PostgreSQL18, Npgsql e EF Core consultadas.
- `/root/t03_backend`: análise inicial e implementação exclusiva backend/testes;
  principal aguardou handoff antes de liberar frontend. Relatou resultados acima e
  encerrou escrita explicitamente. Depois disso, o encerramento do agente retornou
  literalmente: `You’ve hit your usage limit. Upgrade to Pro (https://chatgpt.com/explore/pro), visit https://chatgpt.com/settings/usage to purchase more credits or try again at 8:02 PM.`
  Arquivos e mensagens de handoff/resultados preservados; não houve ampliação de
  permissões, contratação ou substituição silenciosa por agente genérico.
- `/root/t03_frontend`: análise inicial e implementação exclusiva frontend após
  handoff; build/E2E reportados e ownership liberada. Sem editar backend/docs/scripts.
- `/root/t03_qa` e `/root/t03_seguranca`: acionados separadamente após estabilização,
  somente leitura, sem testes operacionais, edição ou acesso a credenciais.
  Ambos concluíram revisão estática sem bloqueador fundamentado. QA pediu explicitar
  limite JSON16KiB e descarte de rascunho ao encerrar sessão; documentos ajustados,
  sem alteração de comportamento/regra de negócio. Ambos acionados novamente e
  aguardados: revalidação documental concluída, observações incorporadas e nenhum
  novo bloqueador fundamentado. Não executaram suites ou operação de banco.

Nenhum teste é atribuído aos revisores. Resultados do backend/frontend são dos
respectivos implementadores, não execuções alegadas pelo principal. O principal
integra e verifica a demonstração/preservação; Arthur concede aceite.

## Dependências e reprodução

P03 e P11 não resolvidas; nenhuma retenção, exclusão ou liberação comercial criada.
Sem integração fictícia de fornecedor neste módulo. Test é descartável/isolado;
dev permanece persistente com contas/senhas anteriores, sem limpeza/recriação.
Reprodução e roteiro manual em EXECUCAO_LOCAL.md; contratos em CONTRATOS_API.md.
Não há publicação ou início T04. Aceite e autorização de fechamento abaixo.

## Aceite e fechamento — 01/11/2026

Arthur declarou: “Minha conferência manual foi aprovada. Confirmo o aceite da T03.”
Aceite da implementação local e demonstração autorizada; P03/gate inicial e
P11/retenção continuam pendentes, sem regra ou autorização de produção implícita.

Esclarecimento preservado: o backend entregou código, migração, testes e handoff
explícito antes do limite do fornecedor; nenhuma implementação/correção conhecida
ficou pendente por esse limite. Faltou apenas o resumo final detalhado. Frontend
concluiu interface/E2E; principal integrou, verificou o diff, fez provas adicionais
de navegador/dev/preservação e finalizou documentos. QA e segurança revisaram o
código final além dos documentos, por leitura estática, sem executar suites. Após
esses pareceres, só observações documentais foram incorporadas e revalidadas;
não houve mudança posterior do código da aplicação. As contagens de teste acima
continuam atribuídas aos seus executores, sem alegar repetição pelo principal.

Autorizados commit `feat(t03): implementar relatos privados do aluno`, `git fetch
origin`, comparação com `origin/main`, push da branch sem force e PR para main
“Implementar relatos privados do aluno”. Não autorizado merge ou início T04.
Neste fechamento: revisão do diff completo e arquivos novos, checagem de segredos/
dados locais e `git diff --check`; testes aprovados não repetidos. Commit, link do PR
e estado Git finais serão informados na resposta de fechamento, sem credenciais.
Revisão de fechamento: 20 arquivos de T03/documentação, sem alteração de regras,
infraestrutura, configuração de agentes ou módulos T04+. Código novo, testes e
migração aditiva conferidos; snapshot/designer correspondentes. Auditoria local
comparou arquivos revisáveis com valores protegidos da configuração local em memória
e assinaturas de segredos, sem exibi-los: nenhuma ocorrência. Nenhum relato real,
captura, inbox, senha, chave ou token incluído. `git diff --check` passou nos arquivos
rastreados e checagem equivalente nos novos; suites aprovadas não repetidas.
