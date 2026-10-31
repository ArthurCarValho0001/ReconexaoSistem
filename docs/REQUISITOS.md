# Requisitos consolidados

IDs R abaixo são novos; os RF/RN antigos estão mapeados em `DECISOES.md`.
APROVADO não significa implementado. PENDENTE bloqueia só o trecho afetado.

| ID | Estado | Comportamento |
|---|---|---|
| R01 | APROVADO | Login de aluno/admin, senha segura, recuperação e sessão única; novo login revoga a anterior |
| R02 | APROVADO | Compra única parcelável externa; confirmação confiável antes da ativação |
| R03 | APROVADO | Convite de ativação reemitível; início do período mínimo na ativação |
| R04 | APROVADO | Acesso por dois anos desde liberação/ativação; mínimo garantido de seis meses; bloqueio ao vencer, renovação paga por dois anos e histórico preservado |
| R05 | APROVADO | Compra revertida suspende I e II associados sem apagar histórico |
| R06 | APROVADO | Admin pode liberar acesso equivalente sem compra; registrar autoria/motivo |
| R07 | APROVADO | Admin cria/ordena/publica módulos e aulas; aulas do nível liberado sem sequência obrigatória |
| R08 | APROVADO | Upload pelo Reconexão ao fornecedor de vídeo; player incorporado; processamento e falhas visíveis |
| R09 | APROVADO | 40% únicos efetivamente assistidos concluem e concedem 10 pontos uma vez |
| R10 | APROVADO | Materiais/e-book disponíveis conforme acesso; arquivos privados protegidos |
| R11 | APROVADO | Lives externas, gravações e avisos de alterações; clique registra acesso, não presença |
| R12 | APROVADO | Turmas com vagas e alocação administrativa, sem exceder capacidade concorrente |
| R13 | APROVADO | Comunidade somente link WhatsApp; retirada do grupo não é automática |
| R14 | APROVADO | Admin cria perguntas no Google Forms e cadastra atividade/versão no Reconexão |
| R15 | APROVADO | Conta Google verificada e vinculada; importar todas as respostas para consulta autorizada |
| R16 | APROVADO | Conferir submissão no servidor, tipo, versão, elegibilidade e completude antes de pontuar |
| R17 | APROVADO | Formulário com respostas tem nova versão para mudanças, sem pontuar de novo a mesma atividade |
| R18 | APROVADO | Inicial obrigatório na entrada; acompanhamento 30 dias após sua submissão, sem expiração automática |
| R19 | APROVADO | Day: cinco perguntas variáveis por data, envio só no dia de Brasília, 10 pontos; ausência não bloqueia |
| R20 | APROVADO | 3FM respondido no Google, calculado/exibido no Reconexão; rascunho/retomada deve ser validado no fluxo Google |
| R21 | APROVADO | 3FM edita vínculos/parâmetros previstos na fórmula oficial; versão imutável dos resultados |
| R22 | APROVADO | 3FM concede 10 pontos uma vez; não inventar número variável, fórmulas ou novas tentativas sem decisão |
| R23 | APROVADO | Novos formulários comuns completos concedem 10 pontos uma vez por aluno/atividade |
| R24 | APROVADO | II libera automaticamente aos 200 pontos, sem outra compra ou antiga lista de pré-requisitos |
| R25 | APROVADO | Home mostra saldo próprio, barra e como ganhar pontos; ranking exclusivo do admin |
| R26 | APROVADO | Relatos opcionais até 1.000 caracteres; confirmação, limpar no sucesso, preservar no erro |
| R27 | APROVADO | Relatos sem pontos, histórico do aluno ou respostas; todos admins consultam autor/texto/data |
| R28 | APROVADO | Admin consulta respostas/resultados; aluno nunca consulta os de terceiros |
| R29 | APROVADO | Dashboards: totais, inatividade >=30 dias, entradas, status e progresso, sem fabricar presença/evasão |
| R30 | APROVADO | Descontos/condições administráveis preservam a compra contratada; detalhamento pendente |
| R31 | APROVADO | Identidade visual azul-escuro/preto, logo R, responsivo e navegação simples; protótipo é referência |
| R32 | PENDENTE | Delimitar rotas bloqueadas pelo inicial e fluxo de vinculação anterior a ele |
| R33 | PENDENTE | Receber fórmula, questões, mapeamentos, faixas e vetores de teste oficiais do 3FM |
| R34 | PENDENTE | Retenção, exportação/exclusão, operação em encerramento e dados fora da plataforma |

Não funcionais: autorização por recurso no servidor, segredos fora do frontend, eventos
idempotentes, migrações, logs mínimos, backups e restauração verificáveis, configuração
separada por ambiente. Metas de desempenho/recuperação/compatibilidade ainda serão definidas.

## Complemento aprovado em 31/10/2026

Avisos por e-mail e na plataforma 60 dias antes e no vencimento; preço da renovação configurável pelo admin e apresentado antes de pagar. Ver D24–D26 e P19–P22.
