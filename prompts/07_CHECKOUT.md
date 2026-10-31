# Prompt - T07: Compra, ativacao e reversao

Abra a raiz do projeto no Codex. Cole o bloco abaixo. Dependências: T02/T05; gateway aprovado para parte real.

```text
Execute a entrega T07 - Compra, ativacao e reversao do Reconexão.
Leia AGENTS.md, docs/ESTADO_PROJETO.md, docs/DECISOES.md e o item T07
em docs/PLANO_ENTREGAS.md. Referências adicionais: docs/DECISOES.md, docs/INTEGRACOES.md.

Confira git status e o estado real dos arquivos. Antes de editar, apresente
arquivos envolvidos, dependências, plano curto e critérios de aceite.
Não considere referências históricas como regras atuais.

Escopo desta tarefa:
Implementar compra inicial parcelavel e renovacao paga por dois anos, pedido/confirmacao/eventos idempotentes e ativacao por convite reemitivel. Conta existente, notificacoes fora de ordem e reversao da compra inicial sem direito independente cobertos. Liberação manual com mesmo direito/progressao. Nao usar assinatura recorrente nem checkout do II. Confirmar P09 antes de casos indefinidos. Adapter fake somente dev/test; integrar sandbox real quando configurado.

Critérios específicos:
C01/C02/C03/C04 e auditoria; reembolso preserva dados; garantia minima de seis meses; validade de dois anos, renovacao paga por dois anos, bloqueio e historico preservado. Cobrir C30/C32/C33 e resolver P19-P21 antes das fronteiras definitivas.

Prossiga com implementação local autorizada, execute os testes adequados e corrija falhas. Se algo bloquear apenas uma parte, registre e continue nas partes independentes.
Não invente decisão de negócio, fórmula 3FM ou teste executado.
Não faça merge, contratação de fornecedor ou publicação em produção sem autorização.
Ao concluir, registre evidência conforme docs/templates/EVIDENCIA.md, atualize
docs/ESTADO_PROJETO.md e docs/PLANO_ENTREGAS.md e os documentos afetados.
Informe o que foi implementado, comandos/resultados, o que está simulado,
o que está bloqueado e como eu reproduzo a validação local.
```
