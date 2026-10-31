# Delegação de trabalho independente

Use somente após definir arquivos e contratos. Substitua os marcadores antes de enviar.

```text
Na entrega TXX, delegue duas partes independentes que descrevo abaixo.
Parte A: [objetivo, arquivos permitidos, contrato e testes].
Parte B: [objetivo, arquivos permitidos, contrato e testes].
Se forem alterações de código concorrentes, isole em worktrees/branches e bancos/portas
próprios, sem alterar a branch da pasta de outro agente. Não paralelize migrações ou
arquivos compartilhados sem divisão explícita. Mantenha como principal a integração.
Se as partes dependerem uma da outra, explique e execute sequencialmente.
Após receber os resultados, revise o diff, integre em branch revisável, rode testes
do conjunto e atualize estado/evidências. Não mescle na main nem publique produção.
```
