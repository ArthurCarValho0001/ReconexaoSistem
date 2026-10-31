# Regras de negócio atuais

## Acesso

Compra inicial parcelável cria direito após confirmação, conta ativa após convite.
Reemitir convite não cancela compra. Liberação/ativação inicia dois anos de acesso e
a garantia mínima de seis meses. Vencimento em dois anos, nunca em seis meses.
Operação da garantia em encerramento é P02. Vencimento bloqueia conteúdos no servidor,
preservando conta, caminho para renovação, pontos, respostas, resultados e histórico.
Avisar por e-mail e na plataforma 60 dias antes e no vencimento.
Renovação paga concede mais dois anos, com preço configurável pelo admin e mostrado
antes do pagamento. Não é débito recorrente automático. Exibir validade antes da compra
e na área do aluno. Não calcular vigência de renovação antecipada/tardia sem resolver P19.
Compra inicial revertida suspende I e II quando não existir direito independente.
Estorno de renovação e múltiplos direitos exigem P21; não revogue concessão independente sem regra.
Concessão manual equivale ao acesso comprado, não a aprovação manual do II.

II: saldo >=200 pontos libera, com conta e direito de acesso válidos. Não exigir
Mediação, conclusão de todas as aulas, acompanhamento ou pagamento extra como travas
de progressão. Inicial obrigatório na entrada não é requisito adicional exclusivo do II;
a aplicação do bloqueio é P03. A suspensão de acesso não apaga o saldo.

## Pontos (10 por evento)

| Evento | Chave lógica por aluno |
|---|---|
| Aula gravada com 40% válidos | Aula |
| Clique em live | Live |
| Gravação de live com 40% válidos | Gravação; independente da live |
| Exercício/formulário comum enviado completo | Atividade lógica, não cada versão |
| Inicial | Instrumento inicial |
| Acompanhamento | Instrumento de 30 dias |
| 3FM completo com estrutura válida | Instrumento 3FM |
| Day completo no dia | Data de Brasília |
| Clique no WhatsApp | Aluno/comunidade, uma única vez |

Não há concessão manual nem acerto como requisito para pontuar exercício. Não conceder
dois eventos genérico/específico pela mesma submissão. A chave de pontuação não muda
com reenvio, edição ou nova versão da mesma atividade. Nova atividade comum publicada
pode pontuar; não há teto global aprovado além da unicidade por evento.

Vídeos: união de intervalos válidos, não soma de repetições. Saltar não é assistir;
recarregar/trocar dispositivo não duplica. Velocidade permitida e limites de telemetria
são P07, sem alegar proteção absoluta contra fraude no navegador.

## Formulários

Questões administradas no Google. Reconexão registra tipo, data, vínculo, versão e
respostas importadas. Google verificado precisa estar vinculado com prova de posse.
Não aceitar texto de e-mail/ID de aluno como autenticação. Voltar à tela não pontua.
Inicial libera acompanhamento após 30 dias da submissão, não do 3FM. Se dia/horas exatas
afetarem implementação, fechar P04 antes de automatizar essa fronteira.

Day aceita apenas a data correspondente em Brasília. Usar instante de envio confiável,
não instante de processamento da fila: envio às 23h59 processado depois não perde direito.
Cinco perguntas por dia; não respondido não bloqueia outras atividades. Edição/reenvio
no Google não gera pontos extras; visibilidade de histórico próprio do Day é P05.

3FM segue documento do cliente, com 40 perguntas/escala 0-4/cinco dimensões na referência
antiga preservada até especificação oficial. Editar perguntas exige atualizar vínculos e
validar nova versão antes de publicar. Não alterar resultado antigo nem avaliar expressões
arbitrárias. Nova tentativa após conclusão não foi aprovada. Retomada/rascunho depende
do comportamento real do Google e deve ser testada; não prometer rascunho na API própria
sem implementação.

## Conteúdo e exclusões preservadas

- Aulas publicadas do nível acessível disponíveis sem sequência.
- Aulas despublicadas/excluídas preservam pontos e histórico.
- Módulo com aula concluída ou exercício respondido não é excluível.
- Exercício respondido não é editado/excluído/arquivado; nova versão não remove original.
- Live publicada não é excluída; pode cancelar/despublicar/reativar o mesmo registro,
  com avisos e preservação do clique já pontuado.
- Mediação e janela de gravação permanecem como conteúdo administrável, sem trava do II.
- Reservas/fila de espera não foram aprovadas para substituir alocação administrativa.

## Privacidade

Todos os admins podem consultar relatos/respostas/resultados. Outros alunos não.
Relato não tem consulta do próprio aluno após envio, não tem pontuação nem resposta admin.
Critério de contagem de 1.000 caracteres (ex.: emojis) deve ser coerente no cliente/servidor
e documentado como decisão técnica, sem reduzir silenciosamente o limite aprovado.
