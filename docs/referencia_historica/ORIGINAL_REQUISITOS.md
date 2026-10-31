> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Requisitos — Reconexão

**Legenda:** APROVADO = decisão consolidada; PENDENTE = requer confirmação; PROPOSTO = sugestão técnica.

## Perfis e acesso
- **RF-001 [APROVADO]** Cadastro/login de aluno com nome, e-mail e senha; recuperação de senha por e-mail.
- **RF-002 [APROVADO]** Perfis de Administrador e Aluno com permissões segregadas.
- **RF-003 [APROVADO]** Acesso ao plano adquirido somente após confirmação do pagamento.
- **RF-004 [APROVADO]** Uma sessão ativa por usuário para desestimular compartilhamento de credenciais.
- **RF-005 [APROVADO]** Reconexão I tem vigência inicial de seis meses após pagamento confirmado. Ao vencer, o aluno que não avançou ao Reconexão II perde acesso ao Reconexão I e à comunidade do programa; eventual continuidade depende de nova contratação/renovação conforme oferta vigente. O administrador poderá configurar a validade das ofertas/planos.
- **RF-006 [APROVADO]** Estorno/chargeback confirmado bloqueia o acesso correspondente sem apagar histórico.

## Conteúdos e aprendizagem
- **RF-010 [APROVADO]** Administrador cria, edita, ordena, salva rascunhos e publica módulos e videoaulas.
- **RF-011 [APROVADO]** Aluno tem acesso imediato a todas as videoaulas gravadas publicadas do plano adquirido, sem desbloqueio sequencial; o planejamento inicial é de oito aulas por módulo, quantidade ajustável pelo administrador.
- **RF-012 [APROVADO]** Vídeo hospedado externamente e reproduzido dentro da plataforma, com rastreamento de segmentos assistidos.
- **RF-013 [APROVADO]** Área do Aluno exibe progresso por módulo, últimas aulas, próximos encontros e avisos.
- **RF-014 [APROVADO]** Materiais de apoio e e-book para leitura na plataforma.
- **RF-015 [APROVADO]** Administrador pode despublicar módulos/aulas sem apagar pontos ou histórico; apenas aulas atualmente publicadas são exigidas para progressão.

## Exercícios
- **RF-020 [APROVADO]** Admin cria exercícios de escolha única, múltipla escolha e texto aberto.
- **RF-021 [APROVADO]** Exercícios podem vincular-se a aula, módulo ou ser independentes; todos os publicados ficam disponíveis sem pré-requisitos.
- **RF-022 [APROVADO]** Aluno envia todas as respostas, vê o que enviou e não pode editar/refazer.
- **RF-023 [APROVADO]** Admin consulta respostas individuais com controle de acesso.
- **RF-024 [APROVADO]** Exercício com pelo menos uma submissão não pode ser editado, excluído ou arquivado; deve ser criado novo exercício do zero.

## Lives, encontros e avisos
- **RF-030 [APROVADO]** Admin cadastra lives com título, descrição, data, horário e link externo (Meet/Zoom ou equivalente aprovado).
- **RF-031 [APROVADO]** Aluno acessa live pelo botão `Acessar aula`; o clique registra participação para pontuação, sem comprovar ingresso na ferramenta externa.
- **RF-032 [APROVADO]** Admin registra gravação de live; aluno assiste com rastreamento de 90% para pontos próprios.
- **RF-033 [APROVADO]** Live publicada pode ser editada, despublicada/cancelada e reativada; não pode ser excluída.
- **RF-034 [APROVADO]** Cancelamento, reagendamento, troca de link e reativação geram aviso dentro da plataforma; não exigem e-mail.
- **RF-035 [APROVADO]** Mediação de Conflito é atividade obrigatória para progressão; na ausência da live, iniciar a gravação satisfaz o requisito de participação, sem dispensar 90% para pontos de gravação.
- **RF-036 [APROVADO]** Admin controla janela de disponibilidade da gravação da Mediação de Conflito e pode reabri-la para todos.
- **RF-037 [APROVADO]** Reconexão II permite turmas com limite de vagas configurável pelo administrador e encontros exclusivamente on-line, com data, horário, link e controle de inscrições/reservas conforme regras a detalhar. Não haverá encontros presenciais.
- **RF-038 [APROVADO]** Comunidade exclusivamente por grupos externos de WhatsApp, sem chat interno ou outra comunidade. A elegibilidade de acesso acompanha a vigência do plano e a situação de progressão; o mecanismo de retirada do grupo externo ainda depende de definição operacional.
- **RF-039 [PENDENTE]** Local exato dos avisos (sino, dashboard, tela de aulas) e política de leitura.

## Questionários
- **RF-040 [APROVADO]** Questionário inicial separado, com 10 pontos na conclusão.
- **RF-041 [APROVADO]** Questionário de 30 dias liberado 30 dias após submissão do inicial, sem prazo de expiração; 10 pontos na conclusão.
- **RF-042 [APROVADO]** Reconexão Day: cinco perguntas diárias, voluntárias, privadas, de reflexão não diagnóstica; 10 pontos ao responder todas as cinco no mesmo dia.
- **RF-043 [APROVADO]** 3FM com Propósito é questionário **adicional**, não substitui inicial nem acompanhamento de 30 dias.
- **RF-044 [APROVADO]** 3FM disponível com a confirmação do pagamento; 40 perguntas em cinco dimensões, escala 0–4.
- **RF-045 [APROVADO]** 3FM permite salvar respostas parciais e indicar pendências; resultado somente após conclusão; não bloqueia aulas e outras funcionalidades.
- **RF-046 [APROVADO]** 3FM calcula resultados por dimensões e pilares Fé, Forma, Força e Mudança e exibe devolutivas personalizadas segundo documento original do cliente.
- **RF-047 [APROVADO]** 3FM pode ser concluído uma única vez por aluno; concede 10 pontos uma única vez.
- **RF-048 [APROVADO]** Admin pode visualizar respostas e resultados individuais do 3FM, com autorização e privacidade.
- **RF-049 [APROVADO]** Perguntas, fórmulas e classificações do 3FM são fixas na primeira versão, sem editor administrativo.
- **RF-050 [APROVADO]** Resultados do 3FM seguem o mesmo padrão de apresentação dos demais questionários.
- **RF-051 [PENDENTE TÉCNICO]** Transcrever fielmente perguntas, mapeamentos, faixas, fórmulas e textos de devolutiva da especificação integral 3FM antes de codificar.

## Pontos, progressão e planos
- **RF-060 [APROVADO]** Registrar pontos por evento elegível, sem atribuição manual, uma vez por evento.
- **RF-061 [APROVADO]** Progressão para Reconexão II exige 200 pontos, todas as aulas obrigatórias atualmente publicadas concluídas, Mediação de Conflito concluída, questionário inicial e de 30 dias concluídos e novo pagamento.
- **RF-062 [APROVADO]** Liberação automática do Reconexão II após confirmação do pagamento **e atendimento dos pré-requisitos**, sem aprovação manual.
- **RF-063 [APROVADO]** Compra de Reconexão II permitida após expiração do I quando os pré-requisitos já tiverem sido cumpridos.
- **RF-064 [APROVADO]** 3FM concede pontos, mas não é requisito obrigatório adicional para progressão ao II.
- **RF-065 [APROVADO PARCIALMENTE]** Administrador poderá configurar descontos e validade das ofertas/planos. Valores concretos, regras de aplicação de desconto, vigência efetiva do Reconexão II e condições comerciais de renovação do I ainda precisam ser definidos.

## Administração e relatórios
- **RF-070 [APROVADO]** Dashboard admin: total de alunos, acessos, alunos inativos por 30 dias ou mais, entradas recentes, evasão/expiração, faltas, presença/ausência na última reunião, distribuição de planos/status e elegibilidade.
- **RF-071 [APROVADO]** Admin gerencia módulos, quantidade de aulas por módulo, aulas, lives, turmas e seus limites de vagas, avisos, exercícios, materiais, alunos, descontos e validade das ofertas/planos, além de consultas de respostas.
- **RF-072 [APROVADO]** Dados demonstrativos devem ser claramente marcados no protótipo.

## Não funcionais
- **RNF-001 [APROVADO]** Proteção de dados pessoais e respostas sensíveis, autorização no servidor e mínimo privilégio.
- **RNF-002 [APROVADO]** Não expor segredos no frontend; confirmação de pagamento validada no servidor.
- **RNF-003 [PROPOSTO]** Backups, logs auditáveis, monitoramento e migrações versionadas.
- **RNF-004 [PROPOSTO]** Interface responsiva, acessível e com feedback de carregamento/erro.
- **RNF-005 [PENDENTE]** Metas quantificadas de disponibilidade, tempo de resposta e retenção de dados.

- **RF-073 [APROVADO]** A jornada de aprendizagem inclui aulas ao vivo e gravações das aulas ao vivo; as videoaulas gravadas publicadas ficam liberadas desde o início da vigência.
- **RF-074 [PENDENTE OPERACIONAL]** Definir como o fim do acesso é comunicado e aplicado nos grupos de WhatsApp externos, pois um clique de convite não comprova participação e a plataforma não controla automaticamente a remoção de membros.
