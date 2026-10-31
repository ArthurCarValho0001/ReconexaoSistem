# Pendências por impacto

Nada abaixo impede leitura, organização ou planejamento local. Bloqueie apenas a
implementação/aceite indicado. Responsável por negócio: Arthur/cliente; por técnica: Codex
propõe e Arthur decide. Não marcar pendência resolvida só porque existe um mock.

| ID | Pergunta / decisão | Bloqueia | Ação |
|---|---|---|---|
| P01 | Aprovar stack e versões, método de banco local | T01 | T00 propõe ADR e inspeciona ferramentas |
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

Ao resolver: data, pergunta, resposta literal resumida, responsável, requisitos afetados,
efeito em teste/dados/integração e decisão substituída. Atualizar `DECISOES.md` e critérios.
Não reabrir preço/prazo/pagamento como pré-condição de uma tarefa técnica independente.
