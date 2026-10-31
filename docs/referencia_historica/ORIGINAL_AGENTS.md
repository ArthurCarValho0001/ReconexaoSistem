> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# AGENTS — Reconexão

## Objetivo
Orientar agentes de IA e desenvolvedores na implementação da plataforma Reconexão I e II. **Este arquivo não substitui os requisitos.**

## Hierarquia de referência
1. `docs/REQUISITOS.md` e `docs/REGRAS_NEGOCIO.md`: comportamento aprovado.
2. `docs/ESCOPO.md`: limites da entrega.
3. `docs/CRITERIOS_ACEITE.md`: condições de validação.
4. `docs/ARQUITETURA.md`, `docs/MODELO_DADOS.md`, `docs/INTEGRACOES.md`: propostas técnicas, ainda sujeitas a aprovação.
5. Código e testes existentes: evidência do que está implementado, não autorização para mudar requisitos.

Em caso de conflito, **não escolha silenciosamente**: registre a divergência e solicite decisão do Coordenador/cliente. Não confunda protótipo com produto funcional.

## Agentes e limites
- **01 Coordenador/Arquiteto:** backlog, arquitetura, interfaces e integração; não aprova novas regras de negócio por conta própria.
- **02 Frontend/UX:** telas e usabilidade; não implementa segurança apenas no cliente.
- **03 Backend:** regras, APIs, autenticação, pontos e pagamentos; não inventa fórmulas do 3FM.
- **04 Dados:** modelagem, migrações e integridade; não remove histórico por conveniência.
- **05 Segurança/QA:** testes, LGPD, autorização e regressões; não muda escopo sem aprovação.
- **06 Integrações/DevOps:** custos, infraestrutura, provedores e publicação; não contrata serviços sem autorização.
- **07 Protótipo:** mantém GitHub Pages demonstrativo; não apresenta simulação como produção.

## Fluxo de execução
1. Ler documentos relevantes e identificar dependências.
2. Descrever proposta, arquivos afetados, riscos e critérios de aceite.
3. Criar branch de funcionalidade; mudanças pequenas e verificáveis.
4. Implementar e testar; registrar evidências e limitações.
5. Abrir PR e aguardar revisão antes de mesclar/publicar.

## Regras obrigatórias
- Não inserir credenciais, tokens ou dados reais de alunos no repositório ou GitHub Pages.
- Backend é autoridade para pontuação, acesso e autorização.
- Pontos são idempotentes por evento elegível; registrar trilha auditável.
- Respeitar regras distintas de exclusão de aulas, exercícios e lives.
- Não transformar clique em link externo em presença efetivamente comprovada.
- Não modificar textos, perguntas, pesos ou cálculos do 3FM sem especificação do cliente.
- Preservar o protótipo visual aprovado (azul-marinho/preto, logo R, aparência profissional).
- Não fazer alterações invasivas nem modernizações fora do escopo.

## Pendências bloqueantes antes de codificar
- Aprovar stack, provedor de hospedagem, banco, pagamentos, vídeo e e-mail.
- Incorporar o documento **integral** do 3FM (40 perguntas, associação dimensões/pilares, fórmulas, faixas e devolutivas), que não está transcrito neste pacote.
- Confirmar regras ainda marcadas como **PENDENTE** nos documentos quando afetarem o módulo em implementação.

## Decisões adicionais obrigatórias
- Vigência inicial do Reconexão I: seis meses. Após expiração, aluno sem progressão/renovação perde acesso e elegibilidade à comunidade.
- Comunidade exclusivamente pelo WhatsApp; não alegar que bloquear link remove automaticamente membro já presente.
- Não implementar encontros presenciais. Turmas possuem capacidade configurável.
- Publicar aulas gravadas sem bloqueio sequencial; oito aulas por módulo é referência inicial alterável pelo administrador.
- Administrador configura descontos e validade das ofertas/planos; não presumir cupons ou retroatividade.
- Nenhum provedor de hospedagem foi aprovado; agentes devem avaliar opções antes de implementar dependências específicas.
