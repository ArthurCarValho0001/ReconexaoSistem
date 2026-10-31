# Prompt - T08: Materiais e emails

Abra a raiz do projeto no Codex. Cole o bloco abaixo. Dependências: T02/T06/T07; fornecedores aprovados para real.

```text
Execute a entrega T08 - Materiais e emails do Reconexão.
Leia AGENTS.md, docs/ESTADO_PROJETO.md, docs/DECISOES.md e o item T08
em docs/PLANO_ENTREGAS.md. Referências adicionais: docs/INTEGRACOES.md, docs/OPERACAO.md.

Confira git status e o estado real dos arquivos. Antes de editar, apresente
arquivos envolvidos, dependências, plano curto e critérios de aceite.
Não considere referências históricas como regras atuais.

Escopo desta tarefa:
Integrar envio transacional de ativacao/recuperacao com retry e entrega sanitizada. Implementar avisos de vencimento por e-mail e na plataforma 60 dias antes e no vencimento, com agenda persistente, deduplicacao, recuperacao de falhas e C31; resolver P22. Materiais privados, upload com validacao de tipo/tamanho, acesso por nivel e links limitados. Videos integram no T09; nao armazenar binarios no banco. Confirmar P12 antes dos limites finais.

Critérios específicos:
C06/C22; tokens nao aparecem em logs/bundle; testes de falha, repeticao e arquivo de terceiro; adapter real provado quando configurado.

Prossiga com implementação local autorizada, execute os testes adequados e corrija falhas. Se algo bloquear apenas uma parte, registre e continue nas partes independentes.
Não invente decisão de negócio, fórmula 3FM ou teste executado.
Não faça merge, contratação de fornecedor ou publicação em produção sem autorização.
Ao concluir, registre evidência conforme docs/templates/EVIDENCIA.md, atualize
docs/ESTADO_PROJETO.md e docs/PLANO_ENTREGAS.md e os documentos afetados.
Informe o que foi implementado, comandos/resultados, o que está simulado,
o que está bloqueado e como eu reproduzo a validação local.
```
