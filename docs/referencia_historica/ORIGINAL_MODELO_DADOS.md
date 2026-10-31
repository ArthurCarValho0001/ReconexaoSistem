> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Modelo de dados — proposta lógica preliminar

**Status:** PROPOSTO, não é DDL nem esquema aprovado. Nomes e tipos finais dependem da stack escolhida. As entidades abaixo devem ser validadas contra regras de negócio e privacidade.

| Entidade | Dados e relações principais |
|---|---|
| Usuario | id, nome, e-mail único, credenciais/identidade, status, datas |
| Perfil / UsuarioPerfil | Aluno/Admin, atribuição e autorização |
| Sessao | usuário, identificador, criação, expiração, revogação |
| Plano | Reconexão I/II, configuração comercial versionável |
| Pagamento | usuário, plano, provedor, id externo único, status, valor, datas |
| AcessoPlano | usuário, plano, pagamento, início/fim, estado |
| Modulo | ordem, título, descrição, estado de publicação |
| Aula | módulo, vídeo externo, obrigatoriedade, ordem, estado, exclusão lógica |
| ProgressoAula | aluno, aula, segmentos assistidos, percentual válido, conclusão |
| Live | título, data/hora, link, estado, histórico de alterações |
| ParticipacaoLive | aluno, live, data/hora do clique de acesso |
| GravacaoLive | live, vídeo, janela de disponibilidade |
| ProgressoGravacao | aluno, gravação, segmentos/percentual/conclusão |
| Exercicio / QuestaoExercicio / Opcao | tipos, associação, publicação, conteúdo |
| SubmissaoExercicio / RespostaExercicio | aluno, exercício, respostas, data, imutabilidade após envio |
| Questionario | tipo: inicial, 30dias, 3FM; versão e publicação |
| QuestaoQuestionario / OpcaoResposta | enunciado, ordem, escala, dimensão/pilar quando aplicável |
| TentativaQuestionario / RespostaQuestionario | aluno, status parcial/concluído, respostas e datas |
| ResultadoQuestionario | médias, classificações, mensagens, versão de cálculo |
| RespostaReconexaoDay | aluno, data local, cinco respostas, conclusão |
| EventoPontuacao | usuário, tipo de evento, identificador do evento, pontos, data |
| RequisitoProgressao / Elegibilidade | regra/versionamento, evidências e situação calculada |
| GrupoEncontro / ReservaEncontro | vagas, local/link, data, aluno, status |
| Aviso / NotificacaoInterna | destinatários, motivo, conteúdo, data, estado de leitura |
| Auditoria | ator, ação, recurso, data e metadados mínimos |

## Restrições fundamentais
- `EventoPontuacao`: índice único por `(usuario_id, tipo_evento, identificador_evento)`; Reconexão Day usa a data local como identificador.
- Submissões finais de inicial, 30 dias e 3FM: uma por aluno por tipo/versão de programa, conforme regra aprovada; rascunhos não pontuam.
- Exercício submetido fica imutável; exclusão/edição de exercício com submissões é bloqueada.
- Exclusões de aulas devem preservar histórico/pontos; preferir soft delete ou tombstone auditável.
- Webhooks de pagamento com chave idempotente do evento do provedor.
- Admin e aluno só acessam dados conforme permissões.
- Registrar versão das fórmulas/mensagens do 3FM associada ao resultado.

## Cuidados
- Evitar armazenar senha em texto; utilizar mecanismo de identidade seguro.
- Definir política de retenção, anonimização e exportação/exclusão conforme obrigações legais e regras de histórico.
- Não armazenar dados sensíveis desnecessários nos logs.
- A definição final de campos depende da especificação integral do 3FM e da arquitetura aprovada.

## Complementos aprovados de modelagem
- **Turma:** identificador, plano, capacidade máxima configurável, status e relação com alunos inscritos; restrição transacional para impedir vagas acima do limite.
- **Modulo/Aula:** quantidade de aulas por módulo administrável, com oito aulas como referência inicial, sem restrição fixa no banco.
- **Oferta/Plano:** desconto e validade configuráveis, com registro da configuração efetivamente contratada no pagamento/acesso para preservar histórico.
- **AcessoComunidade:** elegibilidade de acesso ao WhatsApp vinculada à vigência do plano; registro de convite/clique e, se adotado, controle operacional de remoção, sem presumir acesso à lista real de membros.
- **Hospedagem:** fornecedor não selecionado; evitar campos e dependências exclusivos de um provedor até decisão arquitetural.
