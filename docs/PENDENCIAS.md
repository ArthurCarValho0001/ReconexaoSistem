# Pendências por impacto

Nada abaixo impede leitura, organização ou planejamento local. Bloqueie apenas a
implementação/aceite indicado. Responsável por negócio: Arthur/cliente; por técnica: Codex
propõe e Arthur decide. Não marcar pendência resolvida só porque existe um mock.

| ID | Pergunta / decisão | Bloqueia | Ação |
|---|---|---|---|
| P01 | Base técnica e método de banco local | RESOLVIDA para início T01 | Arthur aprovou em 31/10/2026 com Docker Compose; imagem 18.6-bookworm/digest no ADR; patches de pacotes e testes funcionais ficam na T01 |
| P02 | Como cumprir mínimo garantido se plataforma encerrar antes? Comunidade acompanha novo acesso? | Encerramento/produção | Definir processo, sem criar expiração aos seis meses |
| P03 | Quais rotas ficam disponíveis antes do inicial? Vincular Google/conta/ajuda/checkout/admin? | Gate definitivo de entrada | Fechar com Arthur; não bloquear admin ou vinculação por acidente |
| P04 | Acompanhamento: 30 dias de calendário ou 720 horas? | Fronteira de liberação | Definir e testar fuso |
| P05 | Histórico e edição de respostas próprias por instrumento, especialmente Day | Consulta do aluno | Preservar regras antigas de exercício/3FM onde não substituídas |
| P06 | Retomada do Forms, identidade e associação de respostas atrasadas; estratégia para muitos formulários | T04 real | Prova de integração, medir limites, documentar suporte operacional |
| P07 | Reprodução acelerada, reconciliação entre dispositivos, tolerância de telemetria | T09 player | Definir política proporcional e limitações |
| P08 | Fórmula oficial 3FM completa e parâmetros editáveis | T10 cálculo/editor | Cliente ainda vai enviar; não inventar |
| P09 | Provedor checkout e meios habilitados; conta já existente; renovação; estorno parcial | T07 real | Escolher/verificar sandbox; não transformar em assinatura |
| P10 | Vídeo, e-mail, hospedagem e banco gerenciado | Integrações/publicação | Comparativo existe, nenhum contratado |
| P11 | Retenção de relatos/respostas/logs/backups e pedidos de exclusão | Produção com dados reais | Proposta operacional e avaliação do cliente |
| P12 | Limites de arquivos e upload; materiais protegidos; CDN/região | T08/T09 reais | Dimensionar com serviço aprovado |
| P13 | Realocação/cancelamento de turmas; pode alocar em mais de uma? | Operações correspondentes de T06 | Não criar fila ou autoinscrição |
| P14 | Campos de ofertas/descontos e impacto não retroativo | Administração comercial | Detalhar sem cupons |
| P15 | Período/empate do ranking; significado de evasão e presença | Métricas correspondentes | Não usar clique como presença comprovada |
| P16 | Navegadores, pico de carga, retenção/recuperação e alertas | Aceite não funcional/produção | Propor metas mensuráveis antes de prometer |
| P17 | Quem é primeiro admin e como criar com segurança | Primeiro uso real | Bootstrap controlado; sem senha fixa em seed |
| P18 | Versão visual atual do protótipo | Fidelidade visual | Obter snapshot/commit, não presumir merge anterior |

| P19 | Renovação antecipada conta do vencimento e tardia da reativação? Dois anos de calendário, fuso/horário e caso 29/02? | Cálculo definitivo de vigência | Recomendações ainda não confirmadas; decidir e testar fronteiras |
| P20 | Alteração do preço pelo admin afeta checkout de renovação já iniciado? | Checkout de renovação | Definir validade da oferta e registrar preço do pedido |
| P21 | Estorno de renovação, direitos sobrepostos e concessão manual coexistente | Reversões de renovação | Não revogar acesso independente por suposição |
| P22 | Rotas permitidas ao vencido e tratamento de avisos falhos/atrasados | Gate final e operação dos avisos | Manter caminho de renovação; definir processamento, reconciliação e alerta |

## Registro de resolução

### Triagem T00 — 31/10/2026

Nenhuma decisão de negócio foi resolvida nesta etapa. Dependências por módulo:

| Módulo/entrega | Pendências e fronteira bloqueada | Trabalho independente possível após P01/T01 |
|---|---|---|
| Fundação T01 | P01 resolvida; PostgreSQL em Compose ainda não criado/executado | Base aprovada; T01 não iniciada por instrução explícita |
| Identidade T02 e entrada T04 | P03 gate; P17 primeiro admin real | Login/sessão/revogação e bootstrap com conta fictícia |
| Relatos T03 | P03 gate final, P11 retenção real | Envio e consulta admin sem exclusão automática |
| Forms/Day T04/T05 | P03/P04/P05/P06; configuração Google e prova real | Contratos/fixtures identificadas, importação e ledger locais sem alegar integração |
| Conteúdo/vídeo T06/T09 | P07 telemetria, P10/P12 fornecedor e limites | Catálogo e políticas de acesso; player final depende de prova real |
| Turmas T06 | P13 realocação/cancelamento/múltiplas turmas | Alocação admin com capacidade transacional |
| Comercial/acesso T07 | P09/P14, P19–P21 vigência/preço/reversões, P22 gate vencido | Modelar contratos sem escolher regras ou fornecedor |
| E-mail/materiais T08 | P10/P12 e P22 operação dos avisos | Fila persistente e adapter local explícito |
| 3FM T10 | P08 fórmula/vetores oficiais; P06 retomada Google | Contratos/estados bloqueados, sem cálculo inventado |
| Painéis/UX T11 | P05 consulta própria, P15 métricas, P18 protótipo | Métricas aprovadas e saldo individual |
| Homologação/produção T12/T13 | P02/P11/P16 e integrações reais; autorização de publicação | Plano de testes, restauração e handover |

P19 também afeta a fronteira de calendário do primeiro período de dois anos;
P22 não autoriza negar o caminho de renovação. Decisões pendentes devem gerar
cenários BLOQUEADOS específicos, sem dar aceite integral ao módulo por fixtures.

Resolução P01, 31/10/2026, Arthur: base técnica aprovada com PostgreSQL em Docker
Compose no Docker Desktop existente, substituindo método nativo. Afeta ADR-0001,
arquitetura e execução local; desenvolvimento persistente e testes descartáveis isolados,
loopback e segredos fora do Git. Sem mudança de regra de negócio ou teste funcional
alegadamente concluído. Docker e manifesto verificados; não iniciar T01 nesta resposta.

Ao resolver: data, pergunta, resposta literal resumida, responsável, requisitos afetados,
efeito em teste/dados/integração e decisão substituída. Atualizar `DECISOES.md` e critérios.
Não reabrir preço/prazo/pagamento como pré-condição de uma tarefa técnica independente.

### Execução independente T03 — 01/11/2026

P03 (gate inicial) e P11 (retenção/produção com dados reais) continuam pendentes.
T03 implementa envio autenticado e consulta administrativa conforme D16/D22, sem
gate inicial, exclusão automática ou prazo de retenção inventados. Essa execução
local não resolve essas pendências nem autoriza produção com dados reais.
