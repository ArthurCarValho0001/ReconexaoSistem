# Fluxo de agentes do Reconexão

O chat principal conduz uma entrega por vez, define escopo, contratos e arquivos,
consolida resultados, registra evidências e responde pela entrega. Os especialistas
não substituem decisões de Arthur nem tornam pendências de negócio aprovadas.

## Definições e compatibilidade

- `.codex/config.toml`: habilita agentes e limita a dois subagentes simultâneos,
  excluindo o principal (`agents.max_concurrent_threads_per_session = 2`).
- `.codex/agents/<nome>.toml`: um agente por arquivo, identificado por `name`, com
  `description` e `developer_instructions`. O nome no TOML é a identidade do papel.
- `AGENTS.md`: regras gerais do repositório; este documento descreve a coordenação.

Formato conferido em 31/10/2026 para `codex-cli 0.164.0-alpha.11.1`, executável da
extensão VS Code `openai.chatgpt-26.1021.22027-win32-x64`. Não há modelo fixado ou
ampliação de permissões nos arquivos do projeto. Os especialistas herdam o contexto
de permissões do principal. A instrução de não editar é comportamental e não cria
sandbox isolado de leitura. Verificação de carregamento é separada da sintaxe.

Referências oficiais consultadas em 31/10/2026:
[Subagents](https://learn.chatgpt.com/docs/agent-configuration/subagents) e
[Configuration reference](https://learn.chatgpt.com/docs/config-file/config-reference).
O formato de TOML autônomo e as opções existentes foram mantidos. Não converter
para um formato antigo com cadastro de papéis por `config_file` sem conferir o cliente.

## Iniciar pelo chat principal

Exemplo de pedido (substituir TXX pela entrega autorizada):

> Execute somente TXX conforme seu prompt. Leia AGENTS.md, docs/FLUXO_AGENTES.md,
> estado, decisões, requisitos, regras, escopo, arquitetura e critérios. Confira Git,
> apresente plano, arquivos e dependências antes de editar e crie a branch de tarefa
> a partir da base conferida. Acione os especialistas necessários, até dois simultâneos,
> com um implementador editando por vez. Aguarde revisões e revalidações. Registre
> resultados reais e pendências. Não faça integração ou publicação sem autorização.

O principal confirma branch, commits, estado e alterações existentes. Aceite informado
em documentos não prova integração: confira o histórico. Se a base divergir, avance
apenas nas partes independentes e relate o bloqueio, sem merge implícito.

## Participação por responsabilidade

| Papel | Quando participa | Limite |
|---|---|---|
| backend | APIs, contratos de erro, validações, acesso e regras no servidor | Não delega autoridade ao navegador; só edita arquivos atribuídos |
| frontend | Telas, contratos de API, acessibilidade e estados de interação | Não comprova Forms ou concede pontos pela UI; só edita arquivos atribuídos |
| banco | Modelo, restrições, índices, transações, concorrência e migrações | Preserva histórico e dados dev; só edita arquivos atribuídos |
| devops | Ambiente, scripts, CI e procedimentos de operação | Não instala/contrata/publica por iniciativa própria; só edita arquivos atribuídos |
| qa | Critérios, cenários, regressão, evidências e revalidação | Revisa sem editar arquivos; informa achados ao principal |
| seguranca | Identidade, autorização, sessões, dados e integrações | Revisa sem editar arquivos; informa achados ao principal |
| pesquisador | Dúvida pontual sobre ferramenta/API e compatibilidade | Consulta fontes oficiais, informa versão/limites, sem editar |

Papéis orientam responsabilidades, não concedem propriedade automática de pastas.
Backend e banco, por exemplo, podem precisar dos mesmos arquivos: o principal define
a atribuição e a ordem antes de autorizar qualquer escrita.

## Planejar, implementar, revisar e revalidar

1. Planejamento: consultar regras aprovadas e critérios da entrega; separar pendências
   e contratos de API/dados. Delegar análise delimitada quando necessária. Não iniciar
   entregas posteriores nem preencher regras ou fórmula 3FM ausentes.
2. Contrato de delegação: informar papel, objetivo, referências, critérios, arquivos
   explicitamente atribuídos, arquivos proibidos, dependências, comandos seguros e
   retorno esperado. Sem lista explícita de escrita, o especialista só analisa.
   Avisar que há outros colaboradores e que deve preservar suas alterações.
3. Implementação: inicialmente apenas um implementador edita por vez. O principal
   aguarda sua conclusão antes de liberar a próxima escrita. Até dois subagentes podem
   participar de análises/revisões; especialistas não abrem outros subagentes.
   Conflito de arquivos volta ao principal; não reverter mudanças alheias.
4. Revisão: QA confronta critérios/evidências; segurança examina riscos pertinentes.
   Ambos retornam severidade, referência, impacto, reprodução e limites sem corrigir
   arquivos. Podem trabalhar juntos respeitando o teto de dois. Não precisam revisar
   funcionalidades inexistentes ou ampliar o escopo para cumprir uma lista genérica.
5. Correção: principal consolida achados e atribui correções a um implementador por
   vez, com arquivos e critérios explícitos. Questões de negócio voltam a Arthur;
   partes independentes podem avançar.
6. Revalidação: o revisor confere correção; testes afetados são repetidos com evidência.
   Coordene com o principal comandos que geram artefatos. Use somente ambiente
   descartável separado de `docs/EXECUCAO_LOCAL.md`, conferindo destino antes de escrever.
   Preserve volume/banco dev; nunca dados reais, limpeza dev ou prune global.
7. Conclusão: principal atualiza estado, plano, documentos afetados e evidência baseada
   em `docs/templates/EVIDENCIA.md`, distinguindo IMPLEMENTADO, VALIDADO_LOCAL,
   VALIDADO_INTEGRACAO e ACEITO. Informe PASSOU/FALHOU/BLOQUEADO/NÃO EXECUTADO por teste.
   Arthur dá o aceite; integração, commit, push, merge e publicação seguem autorização
   específica da tarefa. Aceite não implica permissão para publicar.

## Carregamento e prova de delegação

Não confundir quatro verificações: arquivo existe; TOML e campos são válidos; o
cliente reconhece o nome; um subagente desse papel executou e retornou resultado.
Um papel genérico com instruções copiadas não comprova carregamento do papel customizado.

Na sessão de preparação, QA, segurança e pesquisador já constavam dos tipos disponíveis;
os quatro papéis novos retornaram `unknown agent_type` ao tentar delegar. Ler um TOML
atualizado numa tarefa comprova leitura explícita, não recarga automática da definição.
Resultados e validações exatas estão na [evidência](evidencias/PREPARAR_AGENTES.md).

Na retomada de 31/10/2026, backend, frontend, banco e devops foram reconhecidos e
executaram inspeções reais pelos tipos exatos, em dois pares aguardados, sem edição.
O bloqueio anterior está resolvido; resultados e falhas de leitura constam da evidência.
As instruções seguintes preservam o procedimento de diagnóstico usado na preparação.

Para concluir a prova pendente: salve os arquivos, encerre este chat e abra um novo
chat principal no mesmo workspace, na branch `task/preparar-agentes`. No VS Code,
se os tipos continuarem ausentes, execute **Developer: Reload Window** na paleta de
comandos, reabra o projeto e crie outro chat. Não amplie permissões para tentar carregar.
Peça ao principal que delegue, usando o tipo exato de cada papel, uma análise curta
de documentação/estrutura, sem editar ou iniciar T02. Execute pares, aguarde o retorno
antes do próximo par e registre os nomes, resultados e referências na evidência.
Inclua o pesquisador na conferência de preservação. Se houver erro, registre literalmente.
Revisão sequencial pelo principal não equivale a agentes separados.

## Retomada entre chats

### Demonstração manual ao concluir interfaces

O principal prepara/reutiliza duas contas fictícias no dev persistente:
`admin@example.test` (admin) e `aluno.a@example.test` (aluno). Use migração documentada
e `auth-demo`, sem limpar/recriar dev. O test segue separado e descartável para suites.
Senhas são geradas na primeira preparação, preservadas entre entregas em configuração
local ignorada e nunca redefinidas no startup. Provisionamento permanece explícito,
proibido em produção e sem concessão de direito comercial.

Na resposta final de cada entrega com interface testável, informar URL, e-mail/senha
dessas duas contas fictícias, ambiente e roteiro curto; Arthur autorizou explicitamente
essa exibição local e não deve ser encaminhado a procurar credenciais em arquivos.
Não colocar senhas nos documentos/evidências, nem expor credenciais reais, chaves ou
tokens. Revisores recebem resultados sanitizados, sem precisar das senhas. Essa exceção
restrita prevalece sobre a proibição geral de registrar credenciais em prompts.

Não presuma memória. Releia `AGENTS.md`, este fluxo, `ESTADO_PROJETO.md`, `DECISOES.md`,
`PLANO_ENTREGAS.md`, o prompt/critérios da entrega e sua evidência. Consulte os demais
documentos obrigatórios e Git reais. Cada delegação recebe as referências e os limites
necessários; resultados importantes devem ficar em arquivos antes de encerrar o chat.
Não registre credenciais, respostas ou relatos. Nova sessão deve confirmar novamente
reconhecimento dos papéis e pendências, sem tratar trabalho não validado como aceito.
