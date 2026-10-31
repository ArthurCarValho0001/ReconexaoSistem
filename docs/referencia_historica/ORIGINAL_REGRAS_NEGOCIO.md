> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Regras de negócio — Reconexão

## Acesso e pagamento
- **RN-001:** Reconexão I tem vigência inicial de seis meses após pagamento confirmado; pendência de pagamento não libera acesso. Após o vencimento, quem não progrediu ao II e não possui novo acesso válido fica sem acesso ao Reconexão I e à comunidade.
- **RN-002:** Reembolso/chargeback confirmado revoga o acesso associado, preservando histórico.
- **RN-003:** O aluno pode renovar Reconexão I; preço e mecânica de renovação ainda pendentes.
- **RN-004:** Uma sessão ativa por conta; política de encerramento da sessão anterior deve ser detalhada tecnicamente.

## Pontuação — 10 pontos por evento elegível
| Evento | Condição | Frequência |
|---|---|---|
| Videoaula gravada | Assistir >=90% dos segmentos efetivamente reproduzidos, sem contar saltos | Uma vez por aula |
| Live | Clicar `Acessar aula` | Uma vez por live |
| Gravação de live | Assistir >=90% efetivamente | Uma vez por gravação |
| Exercício | Responder todos os itens e enviar; acerto não importa | Uma vez por exercício |
| Questionário inicial | Submeter completo | Uma vez |
| Questionário de 30 dias | Submeter completo | Uma vez |
| 3FM com Propósito | Submeter as 40 respostas | Uma vez |
| Reconexão Day | Responder cinco perguntas do dia | Uma vez por dia |
| WhatsApp | Clicar link/botão de acesso ao grupo | Uma vez por aluno |

- **RN-010:** Pontos são concedidos automaticamente e de forma idempotente; sem concessão manual pelo admin.
- **RN-011:** Edição de respostas do Reconexão Day não gera pontos extras.
- **RN-012:** Clicar em live ou WhatsApp é apenas evidência do clique, **não** confirmação de participação efetiva ou ingresso no grupo.
- **RN-013:** Assistir live e assistir sua gravação são eventos de pontos distintos.
- **RN-014:** Iniciar a gravação da Mediação de Conflito pode cumprir o requisito obrigatório, mas não gera automaticamente os pontos por assistir 90%.
- **RN-015:** Pontos já concedidos permanecem registrados após despublicação/exclusão de aula, conforme política de histórico.

## Progressão para Reconexão II
Todas as condições a seguir são necessárias:
1. Pelo menos **200 pontos acumulados**.
2. Todas as videoaulas obrigatórias **atualmente publicadas** concluídas.
3. Mediação de Conflito concluída (clique na live ou início da gravação, conforme disponibilidade).
4. Questionário inicial concluído.
5. Questionário de 30 dias concluído.
6. Novo pagamento do Reconexão II confirmado.

- **RN-020:** O 3FM **não** é condição obrigatória de progressão; seus 10 pontos contam para o saldo.
- **RN-021:** Não há aprovação manual de progressão; liberação ocorre quando pagamento e pré-requisitos forem satisfeitos.
- **RN-022:** Expiração do I não impede aquisição posterior do II se os pré-requisitos já foram atendidos.

## Conteúdo e exclusões
- **RN-030:** Todas as aulas gravadas publicadas do plano ficam acessíveis imediatamente, sem bloqueio sequencial. Referência inicial de oito aulas por módulo, alterável pelo administrador.
- **RN-031:** Admin pode despublicar aula/módulo sem remover pontos ou histórico; apenas aulas obrigatórias publicadas entram na verificação atual.
- **RN-032:** Admin pode excluir videoaula mesmo assistida, preservando histórico/pontos; excluída deixa de ser requisito.
- **RN-033:** Módulo não pode ser excluído se alguma aula nele foi concluída **ou** algum exercício associado ao módulo possui respostas.
- **RN-034:** Exercício com resposta enviada não pode ser editado, excluído nem arquivado. Sem respostas, edição e exclusão são permitidas; política de arquivamento de exercícios sem resposta ainda não definida.
- **RN-035:** Live publicada não pode ser excluída; pode ser editada, cancelada/despublicada e reativada, mantendo registro.
- **RN-036:** Reativação de live cancelada utiliza o mesmo registro e pode mudar data/hora/link.
- **RN-037:** Janela da gravação de Mediação de Conflito é configurada pelo admin e pode ser reaberta globalmente.

## Questionários
- **RN-040:** Inicial, 3FM e acompanhamento de 30 dias são **três instrumentos separados**.
- **RN-041:** O prazo de 30 dias conta da submissão do inicial, **não** do 3FM.
- **RN-042:** 3FM fica disponível após pagamento confirmado; respostas parciais são retomáveis; só conclusão mostra resultado e concede pontos.
- **RN-043:** 3FM não pode ser respondido novamente após conclusão; não bloqueia outras áreas durante preenchimento.
- **RN-044:** Fórmulas, mapeamentos, classificações e textos do 3FM devem ser reproduzidos **literalmente** do documento do cliente; o fato de estarem definidos pelo cliente não significa que estão transcritos neste pacote.
- **RN-045:** Resultados e respostas individuais do 3FM podem ser consultados por administrador autorizado.
- **RN-046:** Reconexão Day é reflexão educacional, privada, sem diagnóstico clínico.

## Pagamentos e oferta
- Reconexão I: cobrança conforme plano contratado; detalhes comerciais vigentes precisam ser confirmados antes do checkout real.
- Reconexão II: pagamento adicional. O administrador pode definir descontos e validade de ofertas/planos; mecanismo de cupons/códigos promocionais não está aprovado.
- Pix e cartão previstos; boleto apenas se o provedor aprovado oferecer.

## Pontos a confirmar antes de módulos específicos
- Preços, descontos (tipo, aplicação, período), vigências específicas por oferta, política de renovação e efeito de alterações sobre contratos já ativos.
- Política de cancelamento, remarcação e fila de espera para reservas de grupos no II.
- Conteúdo integral e regras numéricas do 3FM em anexo original.
- Tratamento de indisponibilidade do vídeo e forma de contabilizar segmentos assistidos em múltiplos dispositivos.

## Turmas, lives e comunidade
- **RN-050:** Não haverá encontros presenciais. A programação inclui aulas ao vivo e acesso às respectivas gravações conforme janelas definidas.
- **RN-051:** Turmas têm limite de vagas configurável pelo administrador; inscrições não podem exceder a capacidade. Regras de fila de espera, cancelamento e realocação continuam pendentes.
- **RN-052:** A comunidade é exclusivamente via WhatsApp externo. Após expiração do acesso, o aluno sem progressão ou renovação deixa de ter direito à comunidade; a remoção efetiva exige processo operacional ainda a definir.
- **RN-053:** Administrador pode configurar descontos e validade de ofertas/planos. Alterações não devem retroagir sobre pagamentos e vigências já confirmados sem regra comercial expressa.
- **RN-054:** Não há provedor de hospedagem definido; seleção ocorrerá posteriormente com avaliação dos agentes e aprovação.
