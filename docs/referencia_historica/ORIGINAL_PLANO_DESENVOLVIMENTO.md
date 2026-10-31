> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Plano de desenvolvimento — proposta incremental

**Status:** PROPOSTO. Não constitui cronograma fechado; estimativas dependem da arquitetura, fornecedores e disponibilidade de materiais.

## Fase 0 — Preparação documental
**Responsável:** Coordenador.
- Ler todos os documentos; identificar conflitos e pendências por módulo.
- Anexar especificação original integral do 3FM.
- Aprovar arquitetura, stack, ambientes e critérios de aceite.
**Saída:** decisões técnicas registradas, backlog priorizado.

## Fase 1 — Protótipo GitHub Pages
**Responsável:** Agente 07 + Frontend, com revisão do Coordenador.
- Auditar `index.html`, `styles.css`, `app.js` e README.
- Preservar aparência e dados simulados.
- Demonstrar 3FM, três questionários, Reconexão Day, pontos, progressão, exercícios e Mediação de Conflito.
- Trabalhar em `feature/atualizacao-prototipo`, revisar PR e publicar após aprovação.
**Saída:** demonstração atualizada, sem backend nem dados reais.

## Fase 2 — Fundação da aplicação real
**Responsáveis:** Backend, Dados, Frontend, DevOps.
- Estrutura de projetos, CI, ambientes, banco, migrações.
- Autenticação, perfis, recuperação de senha, sessão única.
- Logs e observabilidade mínimos.
**Saída:** aplicação autenticada com testes básicos.

## Fase 3 — Planos, pagamentos e acesso
- Modelar planos, pagamentos, vigência e estornos.
- Integrar gateway aprovado; validar webhooks e idempotência.
- Testar acesso e expiração de Reconexão I, incluindo perda de elegibilidade à comunidade após seis meses sem progressão/renovação.
- Implementar gestão administrativa de descontos e validade das ofertas, preservando condições contratadas.
**Saída:** controle de acesso financeiro verificável.

## Fase 4 — Conteúdos, lives e exercícios
- CRUD de módulos/aulas, publicação, vídeo incorporado e progresso 90%.
- Lives, avisos, gravações e Mediação de Conflito.
- Exercícios e regras de imutabilidade/exclusão.
**Saída:** fluxo educacional funcional.

## Fase 5 — Questionários e pontos
- Inicial, 30 dias, Reconexão Day.
- 3FM (somente após receber texto integral das fórmulas e mensagens).
- Ledger de pontos com idempotência e dashboards.
**Saída:** resultados corretos e trilha auditável.

## Fase 6 — Reconexão II
- Verificador de progressão, checkout próprio, liberação automática.
- Turmas com limite de vagas, reservas e encontros exclusivamente on-line; testar concorrência de inscrições.
**Saída:** progressão testada de ponta a ponta.

## Fase 7 — Segurança, homologação e produção
- Testes funcionais, autorização/IDOR, privacidade, pagamentos, regressões e carga proporcional.
- Backups, monitoramento, domínio, HTTPS, publicação e plano de rollback.
**Saída:** checklist de aceite aprovado e implantação controlada.

## Responsabilidades
| Agente | Entrega principal |
|---|---|
| 01 Coordenador | Arquitetura, backlog, dependências, revisão final |
| 02 Frontend | Interfaces e fluxos de aluno/admin |
| 03 Backend | APIs e regras de negócio |
| 04 Dados | Modelo, migrações, integridade |
| 05 Segurança/QA | Cenários, evidências e revisão |
| 06 Integrações/DevOps | Provedores, ambientes e publicação |
| 07 Protótipo | GitHub Pages demonstrativo |

## Regra de conclusão de tarefa
Uma tarefa só é concluída com: requisito vinculado, implementação revisada, testes registrados, documentação atualizada e ausência de pendência bloqueante.

## Pendências a encaminhar aos agentes
- Comparar provedores de hospedagem (web/API, banco e vídeos); apresentar custos e riscos antes de escolher.
- Definir operação de entrada e saída dos grupos WhatsApp quando a vigência expirar.
- Especificar descontos e validade administráveis (tipo, aplicação, retroatividade e auditoria).
- Refletir no GitHub Pages as turmas limitadas, aulas liberadas imediatamente e ausência de encontros presenciais.
