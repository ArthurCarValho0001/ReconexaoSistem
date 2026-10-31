# Evidência da entrega T00

- Estado: T00 ACEITA documentalmente; base aprovada por Arthur em 31/10/2026 com
  ajuste Docker Compose. Aplicação não IMPLEMENTADA,
  não VALIDADA_LOCAL, não VALIDADA_INTEGRACAO e não ACEITA.
- Data/autor: 31/10/2026, Codex coordenador técnico; decisão da stack cabe a Arthur.
- Branch/commit: `task/t00-alinhamento-tecnico`, base `8d01736`; commit de fechamento
  consultável por `git log -1` após esta evidência; hash comunicado na resposta final.
- Requisitos/critérios: T00 de PLANO_ENTREGAS e prompts/00_INICIAR; P01 e ADR-0001.
- Alterações: proposta de stack/identidade/banco/estrutura, inventário, triagem por módulo,
  estado/plano, critérios e integrações. Sem código de aplicação, migrações ou instalação.
- Ambiente/versões: [inventário completo e comandos reais](../INVENTARIO_T00.md).

| Teste/comando ou passos | Resultado real | Evidência sanitizada |
|---|---|---|
| Git status/listagem/log iniciais | PASSOU: documentação, árvore limpa em main | Base 8d01736 |
| git switch -c task/t00-alinhamento-tecnico | FALHOU no sandbox; repetido com escalonamento, PASSOU | Branch criada sem merge/reset |
| dotnet --info / --list-sdks; node --version; npm.cmd --version; git --version | PASSOU: ferramentas respondem | SDK 10.0.402; Node 24.22.2; npm 11.19.1; Git 2.56.0 |
| npm --version; docker --version | FALHOU | Política bloqueia npm.ps1; Docker fora do PATH |
| docker info; docker compose version pelo PATH | FALHOU | Comando não encontrado; não comprova ausência de Docker Desktop |
| Descoberta de processos/pasta Docker | DIAGNOSTICADO | Instalação por usuário em AppData; pasta negada pelo sandbox |
| info e compose version por executável completo, com escalonamento autorizado | PASSOU | Engine 29.9.2, desktop-linux, Linux x86_64, Compose v5.6.1 |
| buildx imagetools inspect docker.io/library/postgres:18.6-bookworm | PASSOU | linux/amd64; digest do índice fixado no ADR, sem baixar camadas |
| Consulta listener TCP 55432 | Sem listener encontrado | Bind efetivo pendente T01 |
| Descoberta delimitada PostgreSQL/serviços | NÃO LOCALIZADO nos locais consultados | Não equivale a ausência global |
| Leitura de documentos consolidados e critérios T00 | CONCLUÍDA | Sem uso de regras históricas como vigentes |
| Consulta de suporte em fontes oficiais | CONCLUÍDA | Links no inventário; compatibilidade final fica para T01 |
| Revisão QA por agente separado revisao_qa | CONCLUÍDA, documental | Três achados abaixo |
| Revisão segurança por agente separado revisao_seguranca | CONCLUÍDA, documental | Dois achados abaixo |
| Build/testes/migrações/banco/E2E/integrações | NÃO EXECUTADOS | Não há aplicação nesta etapa |
| git diff --check; git status --short --branch; git diff --stat; rg de trechos consolidados | PASSOU: sem erros de whitespace; branch correta, apenas documentos alterados/novos | Oito arquivos rastreados modificados e dois documentos novos; cinco recomendações localizadas |

## Aprovação e ajuste do banco

Arthur aprovou a base nesta sessão em 31/10/2026: PostgreSQL local será Docker Compose
com Docker Desktop existente, em lugar de binários nativos manuais. P01 resolvida;
imagem oficial `postgres:18.6-bookworm` suportada, índice fixado por digest no ADR.
Engine/Compose e manifesto compatíveis linux/amd64 verificados; não houve execução
de PostgreSQL nem comprovação funcional de EF/driver. Plano dev persistente em volume
nomeado, testes em banco/credenciais/armazenamento descartável separados, loopback,
segredos fora do Git. ADR/arquitetura/execução/estado/plano/decisões/pendências e
inventário refletem a substituição. Nenhuma implementação da T01 nesta resposta.

## Achados consolidados

| Origem/severidade | Referência e impacto | Tratamento documental / reteste |
|---|---|---|
| QA/média | PLANO_ENTREGAS T12 limitava C01–C29; CRITERIOS C32 só T07: renovação poderia ficar fora da regressão | T12 agora C01–C33 e C32 T07/T12; execução futura |
| QA/média | C13 apenas negava pontos fora do dia, insuficiente para R19 envio só no dia | ADR/INTEGRACOES/C13 exigem prova real por link direto; bloqueio se Google incapaz, decisão Arthur |
| QA/baixa | ADR sem relógio controlável: fronteiras difíceis de reproduzir | TimeProvider proposto; testes de data na entrega correspondente |
| Segurança/média | ADR não limitava expressamente link ao fim do direito | Expiração <= vencimento e prova de links já emitidos em C30/integrações/ADR; fornecedor deve demonstrar capacidade |
| Segurança/baixa | Revogação na recuperação ambígua: pedido de e-mail poderia interromper sessão | Só redefinição válida revoga; C05 testa pedido isolado e conclusão |

Os revisores não editaram arquivos, não criaram outros agentes nem executaram testes
de aplicação. Principal incorporou os cinco achados e conferiu os trechos alterados;
não houve segundo parecer independente após a incorporação. São riscos/requisitos
da proposta, não vulnerabilidades demonstradas em código existente.

## Limitações e reprodução

Nenhuma integração funcional, fake, banco ou conta Google foi executada. Somente
Engine Docker existente e registro de imagens consultados, sem pull de camadas ou up.
Nenhum segredo necessário nesta T00. Não há fórmula 3FM ou fornecedor contratado.
Não houve publicação, merge ou alteração de política do computador. Pendências e
responsáveis em PENDENCIAS; P01 resolvida, demais bloqueiam seus trechos.

Arthur pode reproduzir os comandos de inventário no PowerShell na raiz; usar npm.cmd.
Revisar diff com `git diff --check`, `git diff --stat` e `git diff`; arquivos novos
aparecem em `git status --short` e devem ser lidos separadamente enquanto não rastreados.
Comandos de subir aplicação/banco ainda devem ser criados e verificados na T01.

Próximo passo: T01 somente em resposta posterior, conforme instrução de Arthur.
Aprovação da base não resolve regra de negócio pendente nem autoriza contratação/publicação.
