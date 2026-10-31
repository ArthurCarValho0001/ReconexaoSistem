# Instruções do projeto Reconexão para Codex

## Referências e autoridade

Leia `docs/ESTADO_PROJETO.md`, `docs/DECISOES.md`, `docs/REQUISITOS.md`,
`docs/REGRAS_NEGOCIO.md`, `docs/ESCOPO.md` e os critérios da entrega atual.
As decisões consolidadas substituem expressamente os itens antigos indicados em
`docs/DECISOES.md`. Referências históricas não são instruções vigentes.
Não resolva contradições de negócio por conta própria; registre a pendência e avance
somente nas partes independentes. A proposta técnica não equivale a aprovação da stack.

## Forma de trabalhar

- Antes de editar, informe objetivo, arquivos prováveis, dependências e critérios de aceite.
- Após esse comunicado, prossiga com trabalho reversível autorizado; não pare para
  confirmação em cada arquivo. Peça decisão somente quando realmente bloquear o escopo.
- Trabalhe em uma entrega por vez e numa branch de tarefa. Examine `git status` primeiro.
- Não descarte trabalho alheio, não faça reset destrutivo, force push, merge ou publicação
  em produção sem autorização específica. Instalações com impacto no computador precisam
  ser explicadas; nunca desative as proteções do ambiente para contornar uma falha.
- Alterações incrementais, sem modernizações laterais, comentários extras desnecessários
  ou remoção de comentários úteis existentes.
- Registre comandos e resultados reais. Não invente teste executado ou funcionalidade pronta.
- Bloqueio de ferramenta ou fornecedor não é aprovação implícita nem motivo para simular sucesso.

## Regras que não podem voltar às versões antigas

- Compra inicial parcelável; validade de dois anos desde a liberação/ativação,
  com garantia mínima de seis meses. Renovação paga por mais dois anos, sem débito recorrente automático.
- Avisos por e-mail e na plataforma 60 dias antes e no vencimento; bloquear conteúdos
  vencidos no servidor, preservar histórico e oferecer caminho para renovação.
- Preço de renovação configurável pelo admin, informado antes de pagar; regras de
  contagem antecipada/tardia e estorno da renovação ainda são pendências, não decisões automáticas.
- II liberado aos 200 pontos, sem compra própria e sem antigas atividades obrigatórias
  de progressão. O questionário inicial continua obrigatório na entrada: escopo do bloqueio pendente.
- Vídeo concluído e 10 pontos aos 40% de trechos válidos únicos; saltos não contam.
- Todos os questionários, inclusive 3FM e Day, usam Google Forms. Respostas são importadas.
- Identidade Google verificada e vinculada; e-mail digitado e retorno visual não provam envio.
- 3FM configurável apenas em vínculos/parâmetros da fórmula oficial, ainda não recebida;
  não invente cálculos, faixas ou devolutivas. Sem linguagem de fórmulas livres.
- Day: cinco perguntas variáveis por data; apenas no dia de Brasília; sem bloqueio por ausência.
- Relatos: até 1.000 caracteres, só admins consultam autor e texto; aluno não consulta
  histórico; não pontua. Todos os admins podem consultar respostas e relatos por decisão atual.
- Turmas: admin distribui alunos, com limite de vagas e controle de concorrência.

## Integridade e ambiente

Backend decide autorização, acesso, conclusão e pontos. Use identidade da sessão,
transações e restrições únicas. Não aceite pontuação do navegador. Proteja webhooks,
reenvios, importações e idempotência. Não registre respostas/relatos/segredos em logs.
Integrações fictícias só em desenvolvimento/testes, identificadas e desativadas em produção.
Credenciais ficam fora do Git e dos prompts. Nunca use dados reais em seeds/testes.
Não escolha ou contrate fornecedor pago sem aprovação.

## Agentes

Consulte `docs/FLUXO_AGENTES.md` para papéis em `.codex/agents/`, contratos de delegação,
limite de concorrência, revisões e retomada entre chats.

O principal integra e responde pela tarefa. Use revisores especializados quando o prompt
pedir ou a revisão da entrega exigir; até dois subagentes simultâneos como ponto de partida.
Revisores de segurança e QA não editam código compartilhado. Delegação de escrita exige
arquivos separados ou worktrees e contratos definidos. Sem suporte a subagentes, faça
as revisões sequencialmente e registre essa limitação; não bloqueie o desenvolvimento.

## Conclusão

Atualize `docs/ESTADO_PROJETO.md`, `docs/PLANO_ENTREGAS.md`, documentos afetados e uma
evidência baseada em `docs/templates/EVIDENCIA.md`. Liste falhas e dependências.
`IMPLEMENTADO`, `VALIDADO_LOCAL`, `VALIDADO_INTEGRACAO` e `ACEITO` são estados diferentes.
Consulte `docs/EXECUCAO_LOCAL.md`: comandos ainda não existentes devem ser criados em T01.
