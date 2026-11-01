# Linha de entregas

Status em 01/11/2026: T00 ACEITA documentalmente por Arthur, base aprovada com Docker
Compose; T01 ACEITA por Arthur após evidências e conferência manual da tela local;
T02 IMPLEMENTADO, VALIDADO_LOCAL e ACEITO por Arthur; T03–T13 A FAZER.
Não é cronograma em dias; preservar o prazo já acordado
sem prometer durações inventadas.

| ID | Entrega | Dependência | Prompt |
|---|---|---|---|
| T00 | Alinhamento tecnico | Nenhuma | [Abrir](../prompts/00_INICIAR.md) |
| T01 | Fundacao executavel | T00 aprovada | [Abrir](../prompts/01_FUNDACAO.md) |
| T02 | Autenticacao e sessoes | T01 | [Abrir](../prompts/02_AUTENTICACAO.md) |
| T03 | Relatos completos | T02; confirmar P03 para gate final | [Abrir](../prompts/03_RELATOS.md) |
| T04 | Forms: prova e contrato | T02; conta/configuracao Google para parte real | [Abrir](../prompts/04_FORMS_PROVA.md) |
| T05 | Ledger e progressao | T02; contrato T04; P03 para gate final | [Abrir](../prompts/05_PONTOS.md) |
| T06 | Conteudos, lives e turmas | T02/T05; T04 para vinculos de atividades | [Abrir](../prompts/06_CONTEUDOS_TURMAS.md) |
| T07 | Compra, ativacao e reversao | T02/T05; gateway aprovado para parte real | [Abrir](../prompts/07_CHECKOUT.md) |
| T08 | Materiais e emails | T02/T06/T07; fornecedores aprovados para real | [Abrir](../prompts/08_ARQUIVOS_EMAIL.md) |
| T09 | Upload e progresso de video | T05/T06; fornecedor de video aprovado | [Abrir](../prompts/09_VIDEOS.md) |
| T10 | 3FM e editor parametrizado | T04/T05; especificacao oficial P08 | [Abrir](../prompts/10_3FM.md) |
| T11 | Paineis e acabamento | T03/T05/T06; T10 para resultados reais | [Abrir](../prompts/11_PAINÉIS_UX.md) |
| T12 | Seguranca e homologacao | Entregas funcionais anteriores e integracoes reais | [Abrir](../prompts/12_HOMOLOGACAO.md) |
| T13 | Publicacao e handover | T12 aprovada; autorizacao de publicacao | [Abrir](../prompts/13_ENTREGA.md) |

## Sequência e paralelismo

Preparação transversal de agentes em `task/preparar-agentes` (base main `69e8cec`):
configuração IMPLEMENTADA, prova de delegação VALIDADO_LOCAL e preparação ACEITA por
Arthur em 31/10/2026 com base nas delegações reais registradas. QA/segurança/pesquisador
executados na preparação; backend/frontend/banco/devops executados no novo chat de
31/10/2026 pelos tipos exatos, em pares, com retornos somente de leitura aguardados.
Bloqueio anterior `unknown agent_type` resolvido. Sem início T02 ou mudança
de dependências. Fluxo em [FLUXO_AGENTES.md](FLUXO_AGENTES.md), resultados e limitações
na [evidência](evidencias/PREPARAR_AGENTES.md). Commit, push sem force e abertura de PR
para main autorizados; merge e início T02 não autorizados nesta tarefa.

T00 aprova a base; T01 cria aplicação. T02 precede dados privados. T04 deve ocorrer cedo; provas reais podem exigir configuração externa. T03 e investigação T04 podem avançar isoladas após T02. T05 usa o contrato de conclusão. T06/T07 podem se dividir só com contratos e worktrees separados. T10 aguarda fórmula oficial; não deve travar catálogo/relatos. T12/T13 exigem integração real e autorização de publicação.

## Portas de conclusão

- IMPLEMENTADO: código existe.
- VALIDADO_LOCAL: testes locais reais passaram.
- VALIDADO_INTEGRACAO: fornecedor real/sandbox validado quando aplicável.
- ACEITO: critérios e evidências revistos por Arthur/cliente.

Nenhuma entrega de integração vira ACEITA se apenas o adapter fake funciona. Cada tarefa gera evidência sanitizada e atualização do estado. Não avançar sobre falha bloqueante; continuar partes independentes.

## T00 - Alinhamento tecnico
- Estado: ACEITA documentalmente em 31/10/2026; base aprovada por Arthur com ajuste
  Docker Compose. Docker/manifesto conferidos; sem implementação da aplicação ou início T01.
- Artefatos: [ADR-0001](adr/0001_STACK_PROPOSTA.md), [inventário](INVENTARIO_T00.md),
  [evidência](evidencias/T00_ALINHAMENTO.md) e triagem por módulo em PENDENCIAS.md.
- Dependências: Nenhuma.
- Trabalho: Ler documentos consolidados, identificar conflitos e revisar ferramentas locais. Apresentar stack, identidade, banco local e estrutura; registrar ADR-0001 para aprovacao. Nao implementar a aplicacao antes da aprovacao da base. Separar requisitos pendentes de escolhas tecnicas rotineiras.
- Saída: Inventario local e ADR proposto/aprovado por Arthur; pendencias por modulo; plano sem regras antigas.

## T01 - Fundacao executavel
- Estado: ACEITO por Arthur em 31/10/2026, com base nas evidências registradas e na
  conferência manual da tela local. Build, PostgreSQL e testes locais executados;
  revisões QA/segurança concluídas com dois achados baixos corrigidos/retestados.
  Sem implementação T02+; não equivale à homologação de integrações futuras.
- Evidência: [T01](evidencias/T01_FUNDACAO.md). Branch `task/t01-fundacao`, base
  T00 `6e2fe66`; fechamento autorizado com o commit
  `feat(t01): criar fundacao local do Reconexao`, sem push ou merge.
- Dependências: T00 aprovada.
- Trabalho: Criar frontend/backend/banco da stack aprovada, health e primeira migracao de teste. Fixar versoes e lockfiles. Configurar segredos locais, testes e comandos PowerShell reproduziveis. Entregar tela inicial simples com referencia visual, nao todos os modulos. Sem pagamento ou Forms reais obrigatorios nesta etapa.
- Saída: Build e testes executados; PostgreSQL descartavel funciona; frontend chama API; EXECUCAO_LOCAL preenchido e verificavel.

## T02 - Autenticacao e sessoes
- Estado: IMPLEMENTADO, VALIDADO_LOCAL e ACEITO por Arthur em 01/11/2026; não homologação
  de integração externa. Branch `task/t02-autenticacao-sessoes`, base main `6c406d4`.
- Conferência manual aprovada com ressalva por Arthur: consultas 401 anônimas e
  favicon investigados; polling anônimo retirado, favicon corrigido, cinco cenários
  afetados passaram. Arthur confirmou o aceite considerando essas correções;
  401/403 preservados.
- Evidência: [T02](evidencias/T02_AUTENTICACAO.md). Identity/cookie/CSRF, perfis,
  revogação e recuperação local, bootstrap e fixtures explícitas. Build .NET sem
  avisos/erros, 24/24 integração PostgreSQL real e 8/8 Chrome E2E; build frontend passou.
  Migração validada em test descartável e depois aplicada ao dev existente para
  demonstração autorizada, sem limpar/recriar dev. Admin/aluno fictícios estáveis,
  provisionamento explícito; teste focal 1/1 e dois logins Chrome dev confirmados.
  Commit, push sem force e PR de fechamento autorizados. Sem fornecedor real,
  merge, publicação ou início T03. Google T04, e-mail T08 e gate P03 pendentes.
- Dependências: T01.
- Trabalho: Implementar identidade segura, perfis, login/logout, recuperacao com adapter de email local e sessao revogavel; novo login encerra a anterior. Bootstrap admin controlado, contas ficticias A/B/admin. Google vinculado sera completado em T04, sem forjar validacao. Inicial gate so quando P03 definido.
- Saída: C05/C06; falhas de login/recuperacao tratadas; aluno nao atribui perfil; fake email so dev/test.

## T03 - Relatos completos
- Dependências: T02; confirmar P03 para gate final.
- Trabalho: Implementar tela de envio, API e persistencia de relatos ate 1000 caracteres. Sem historico do aluno, pontos ou resposta admin; todos admins consultam autor/data/texto, com paginacao. Preservar texto no erro e limpar no sucesso. Nao bloquear implementacao independente por politica de retencao ainda pendente.
- Saída: C06/C07/C08; validacoes cliente/servidor coerentes; nenhuma listagem privada acessivel a aluno.

## T04 - Forms: prova e contrato
- Dependências: T02; conta/configuracao Google para parte real.
- Trabalho: Definir e provar fluxo de identidade Google verificada, vinculo, atividade/versao e importacao de resposta. Comparar callback Apps Script e consulta API/reconciliacao; documentar cotas e onboarding de novo formulario, inclusive muitos Days. Fazer uma prova real com conta de teste. Validar dia de envio, nova versao e acesso anterior ao inicial. Se credencial faltar, concluir apenas contrato/fixtures e registrar prova real bloqueada.
- Saída: C09/C10/C11/C12/C13/C15/C25; demonstracao real distinguida de mock; gate P03 e fronteira P04 resolvidos para concluir respectivos fluxos.

## T05 - Ledger e progressao
- Dependências: T02; contrato T04; P03 para gate final.
- Trabalho: Implementar ledger e politicas aprovadas com chave unica/transacao. Integrar conclusoes Forms; nao pontuar genérico e especifico juntos. II com 200 sem nova compra/antigos requisitos. Home saldo/barra/explicacao; ranking restrito com criterio pendente separado. Testar 199/200, reenvio, versao, Day e concorrencia. Nao expor endpoint arbitrario para pontos em producao.
- Saída: C04/C11/C14/C16/C17/C26; concorrencia em PostgreSQL; integracao mock identificada se provedor real ainda bloqueado.

## T06 - Conteudos, lives e turmas
- Dependências: T02/T05; T04 para vinculos de atividades.
- Trabalho: CRUD incremental de modulos/aulas/materiais/avisos/lives, referencia de video e gravacao; preservar historico/exclusoes. Turmas alocadas pelo admin com capacidade concorrente. Clique live/WhatsApp aprovado idempotente, sem presencia comprovada. Nao implementar fila/autoinscricao. Integrar Forms cadastrados; upload real vem em T08/T09.
- Saída: C19/C20/C21 e testes de acesso ao nivel I/II; limites/realocacao dependentes de P13 claramente marcados.

## T07 - Compra, ativacao e reversao
- Dependências: T02/T05; gateway aprovado para parte real.
- Trabalho: Implementar compra inicial parcelavel e renovacao paga por dois anos, pedido/confirmacao/eventos idempotentes e ativacao por convite reemitivel. Conta existente, notificacoes fora de ordem e reversao da compra inicial sem direito independente cobertos. Liberação manual com mesmo direito/progressao. Nao usar assinatura recorrente nem checkout do II. Confirmar P09 antes de casos indefinidos. Adapter fake somente dev/test; integrar sandbox real quando configurado.
- Saída: C01/C02/C03/C04 e auditoria; reembolso preserva dados; garantia minima de seis meses; validade de dois anos, renovacao paga por dois anos, bloqueio e historico preservado. Cobrir C30/C32/C33 e resolver P19-P21 antes das fronteiras definitivas.

## T08 - Materiais e emails
- Dependências: T02/T06/T07; fornecedores aprovados para real.
- Trabalho: Integrar envio transacional de ativacao/recuperacao com retry e entrega sanitizada. Implementar avisos de vencimento por e-mail e na plataforma 60 dias antes e no vencimento, com agenda persistente, deduplicacao, recuperacao de falhas e C31; resolver P22. Materiais privados, upload com validacao de tipo/tamanho, acesso por nivel e links limitados. Videos integram no T09; nao armazenar binarios no banco. Confirmar P12 antes dos limites finais.
- Saída: C06/C22; tokens nao aparecem em logs/bundle; testes de falha, repeticao e arquivo de terceiro; adapter real provado quando configurado.

## T09 - Upload e progresso de video
- Dependências: T05/T06; fornecedor de video aprovado.
- Trabalho: Implementar upload admin pelo Reconexao ao fornecedor, status de processamento/erro e player autorizado. Calcular uniao de intervalos assistidos com validacao temporal razoavel, 40% para conclusao e 10 pontos. Confirmar politica P07; testar retomada/repeticao/salto/dispositivos e gravacao separada da live. Nao apresentar iframe generico como telemetria confiavel.
- Saída: C18/C19/C22; provider real e tokens verificados; mocks nunca aprovam streaming final.

## T10 - 3FM e editor parametrizado
- Dependências: T04/T05; especificacao oficial P08.
- Trabalho: Receber fonte oficial, mapear perguntas Google a dimensoes/pilares, parametrizar somente operacoes previstas e versionar configuração/publicacao. Implementar resultados no Reconexao com textos oficiais e testes de vetores. Nova configuracao nao altera resultado antigo. Sem expressao arbitraria/eval; sem inventar formula ou refazer questionario concluido. Se material faltar, limitar ao schema e estados bloqueados, sem resultado ficticio em producao.
- Saída: C12/C23/C24/C25 e retomada de preenchimento validada no Google; limites de editor aprovados.

## T11 - Paineis e acabamento
- Dependências: T03/T05/T06; T10 para resultados reais.
- Trabalho: Consolidar dashboard aluno/admin e navegacao responsiva usando dados reais. Ranking somente admin, saldo proprio e atividade recente; nao fabricar faltas ou presença. Validar P15, teclado/labels/erros/loading e prototipo snapshot. Não redesenhar identidade nem criar telas fora do escopo.
- Saída: C26/C29; evidencias visuais quando ferramenta disponível; resultado 3FM indisponivel se formula ainda bloqueada.

## T12 - Seguranca e homologacao
- Dependências: Entregas funcionais anteriores e integracoes reais.
- Trabalho: Executar regressao e revisao independente, corrigir achados significativos e testar restauracao isolada, concorrencia e metas acordadas. Conferir Production sem fakes, segredos e dados ficticios. Revisar retencao P11 e clausula de encerramento P02 com responsaveis. Não publicar produção automaticamente.
- Saída: C01-C33 aplicaveis com PASSOU/FALHOU/BLOQUEADO/NÃO EXECUTADO; nenhum bloqueio essencial mascarado; aceite funcional do cliente.

## T13 - Publicacao e handover
- Dependências: T12 aprovada; autorizacao de publicacao.
- Trabalho: Preparar release e plano revisavel de deploy/migrations/rollback, publicar so mediante autorizacao especifica. Transferir controle de contas, codigo e documentacao, validar operacao em producao com smoke test sem dados indevidos. Registrar aceite e inicio das corretivas. Não contratar manutencao ou serviço pago por conta propria.
- Saída: C27/C28 e checklist de handover; status de cada limitacao e responsavel documentados.
