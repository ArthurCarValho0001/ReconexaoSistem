> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Arquitetura — proposta inicial, não aprovada

## Estado
**PROPOSTO.** A stack ainda deve ser selecionada pelo Coordenador Técnico com avaliação de custos e facilidade de manutenção. Não tratar exemplos como decisões finais.

## Componentes lógicos
1. **Frontend web responsivo:** Home, Área do Aluno, Admin, questionários e dashboards.
2. **API/backend:** autenticação, autorização, pagamentos, regras de negócio, pontuação, progressão, rastreamento de aulas e relatórios.
3. **Banco relacional:** usuários, planos, acesso, conteúdo, submissões, resultados, pontos e auditoria.
4. **Serviços externos:** checkout, vídeos incorporados, e-mails transacionais, Meet/Zoom e WhatsApp.
5. **Observabilidade e operações:** logs, monitoramento, backup, migrações e deploy.

```text
Navegador -> Frontend -> API segura -> Banco de dados
                          |-> Gateway de pagamento (webhooks validados)
                          |-> Provedor de e-mail
Navegador -> Player de vídeo incorporado (telemetria validada pela API)
Navegador -> Links externos (Meet/Zoom/WhatsApp)
```

## Decisões arquiteturais a aprovar
- Linguagens/frameworks e versões com suporte; opção de stack adequada à experiência do desenvolvedor e custos.
- Hospedagem de frontend/backend e banco; ambientes DEV/HOM/PROD.
- Autenticação/sessões, recuperação de senha e estratégia de sessão única.
- Mecanismo de monitoramento de 90% de vídeo, com segmentos assistidos e antifraude razoável.
- Modelo de processamento idempotente de webhooks e pontos.
- Rotina de backup/restauração, logs e políticas de retenção.

## Princípios obrigatórios
- Backend determina acesso e pontos; não confiar em flags do navegador.
- Autorização por perfil e por recurso; aluno não consulta dados de outro aluno.
- Pagamentos confirmados somente por fonte verificável do provedor.
- Não usar GitHub Pages como backend nem como repositório de informações sensíveis.
- Usar transações/constraints para evitar duplicidade de pontos e submissões.
- Preservar histórico quando itens forem despublicados ou excluídos logicamente.
- Questionário 3FM com regras versionadas e reprodutíveis.

## Protótipo existente
Repositório `ArthurCarValho0001/Reconexao`: HTML/CSS/JS puro, `index.html`, `styles.css`, `app.js`; GitHub Pages. É demonstração independente da escolha de arquitetura real.

## Restrições funcionais recentes
- Hospedagem de frontend, backend, banco e vídeos ainda será escolhida com os agentes; nenhuma plataforma foi aprovada.
- Turmas com limite de vagas exigem controle de concorrência no backend/banco.
- Descontos e validade de planos/ofertas serão geridos pelo administrador; preservar histórico das condições contratadas.
- A comunidade é somente via WhatsApp externo; expiração de acesso na plataforma não garante retirada automática do grupo.
- Não há encontros presenciais. Lives e gravações compõem a jornada do curso.
