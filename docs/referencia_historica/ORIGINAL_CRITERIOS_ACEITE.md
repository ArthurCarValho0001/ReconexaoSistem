> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Critérios de aceite — primeira versão

Cada cenário deve ser testado com aluno, administrador e usuário não autenticado quando aplicável. Registrar evidência e status PASSOU/FALHOU/BLOQUEADO.

## Autenticação e acesso
- **CA-001:** Login válido permite acesso ao perfil correto; acesso anônimo a dados privados é negado.
- **CA-002:** Aluno A não consulta respostas, pontos ou dados do aluno B alterando IDs.
- **CA-003:** Pagamento pendente não libera plano; confirmação válida libera vigência correta.
- **CA-004:** Reembolso/chargeback confirmado bloqueia acesso, preservando histórico.
- **CA-005:** Nova sessão aplica política de sessão única aprovada.

## Conteúdo e pontos
- **CA-010:** Aula publicada está acessível sem concluir aula anterior.
- **CA-011:** Pular vídeo até 90% não concede pontos; assistir 90% dos segmentos válidos concede 10 uma vez.
- **CA-012:** Repetir vídeo concluído não concede pontos adicionais.
- **CA-013:** Clicar `Acessar aula` de live concede 10 uma vez, sem afirmar presença externa.
- **CA-014:** Assistir 90% da gravação concede 10 pontos independentes da live.
- **CA-015:** Clicar WhatsApp concede 10 uma vez, sem confirmar ingresso.
- **CA-016:** Reconexão Day só concede 10 se cinco perguntas do dia forem respondidas; edições não duplicam pontos; outro dia pode pontuar.

## Questionários
- **CA-020:** Inicial e 3FM aparecem como instrumentos distintos.
- **CA-021:** 30 dias começa na conclusão do inicial; não expira após liberado.
- **CA-022:** 3FM fica disponível após confirmação do pagamento e não bloqueia outras áreas se incompleto.
- **CA-023:** 3FM salva rascunho, recupera respostas e mostra pendências.
- **CA-024:** 3FM só apresenta resultados e concede 10 pontos após 40 respostas concluídas.
- **CA-025:** Conclusão do 3FM impede novo envio e não gera pontos duplicados.
- **CA-026:** Admin autorizado consulta resultados individuais; aluno não acessa os de terceiros.
- **CA-027 [BLOQUEADO ATÉ ANEXAR FONTE]** Para vetores de teste definidos pelo cliente, dimensões, pilares, empates, faixas e mensagens devem corresponder exatamente à especificação 3FM.

## Exercícios e administração
- **CA-030:** Exercício com todos os itens respondidos concede 10 pontos, independentemente de acertos.
- **CA-031:** Aluno consulta submissão, mas não edita/refaz.
- **CA-032:** Exercício com resposta não pode ser editado, excluído nem arquivado.
- **CA-033:** Módulo com aula concluída ou exercício respondido não pode ser excluído.
- **CA-034:** Aula excluída deixa de ser obrigatória, mas preserva pontos/histórico.
- **CA-035:** Live publicada não pode ser excluída; cancelamento e reativação preservam registro e geram aviso interno.

## Progressão
- **CA-040:** 199 pontos não satisfazem mínimo; 200 satisfazem apenas esse requisito, sem dispensar os demais.
- **CA-041:** Falta de aula obrigatória publicada, Mediação ou questionários inicial/30 dias impede progressão.
- **CA-042:** 3FM incompleto não impede progressão se os demais critérios forem satisfeitos.
- **CA-043:** Pagamento confirmado do II com todos os critérios libera acesso automaticamente.
- **CA-044:** Aluno elegível pode comprar II mesmo após expiração do I.

## Protótipo e publicação
- **CA-050:** GitHub Pages apresenta apenas dados fictícios e informa claramente o caráter demonstrativo.
- **CA-051:** Layout profissional azul-marinho/preto, sem página `Conhecer`, com Home, Área do Aluno e Painel Admin.
- **CA-052:** Nenhum segredo, senha real ou dado pessoal de aluno aparece no código público.

## Vigência, turmas e ofertas
- **CA-060:** Após seis meses de vigência do Reconexão I, aluno sem progressão ou renovação perde acesso ao conteúdo protegido e deixa de ter direito à comunidade; a remoção do WhatsApp depende do procedimento externo aprovado.
- **CA-061:** Aluno com plano ativo acessa todas as videoaulas gravadas publicadas imediatamente, sem desbloqueio por sequência.
- **CA-062:** Administrador pode configurar quantidade de aulas por módulo, incluindo valor diferente de oito.
- **CA-063:** Turma não admite inscrições acima do limite configurado, inclusive em tentativas simultâneas.
- **CA-064:** Não existem encontros presenciais no fluxo demonstrado ou na implementação.
- **CA-065:** Administrador pode alterar descontos e validade das ofertas; condições já contratadas permanecem auditáveis e não são modificadas retroativamente sem regra aprovada.
- **CA-066:** Links da comunidade levam apenas ao WhatsApp; não há chat/comunidade interna.
- **CA-067:** Nenhum fornecedor de hospedagem é tratado como contratado antes da aprovação formal da arquitetura e integrações.
