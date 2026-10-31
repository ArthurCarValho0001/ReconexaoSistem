> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Integrações externas — catálogo e decisões

**Regra:** nenhuma escolha de fornecedor ou serviço pago está aprovada apenas por aparecer neste documento. Priorizar custo reduzido, segurança e viabilidade para escala estimada de 50–600 alunos.

| Integração | Necessidade | Regra/limite | Situação |
|---|---|---|---|
| Gateway de pagamentos | Checkout Pix/cartão; boleto se disponível | Confirmar por webhook/API autenticada; não liberar por retorno visual do checkout | Provedor pendente |
| Hospedagem de vídeo | Player incorporado e progresso | Não redirecionar obrigatoriamente para YouTube; rastrear 90% real | Provedor pendente |
| Meet/Zoom ou similar | Lives e Mediação de Conflito | Clique `Acessar aula` registra pontos, não presença comprovada | Links externos previstos |
| WhatsApp | Única comunidade, em grupo externo | Clique rende 10 pontos uma vez; não comprova ingresso. Definir controle de membros ao vencer acesso | Link externo previsto; operação de remoção pendente |
| E-mail transacional | Recuperação de senha e mensagens essenciais | Evitar expor credenciais e dados pessoais | Provedor pendente |
| Hospedagem web/API | Disponibilizar plataforma real | HTTPS, secrets, deploy e monitoramento | Pendente |
| Banco gerenciado | Persistência de dados | Backups, segurança e limites do plano | Pendente |
| Domínio e DNS | Endereço próprio | Custo e renovação sujeitos à aprovação | Pendente |
| Monitoramento | Disponibilidade/erros | Sem expor respostas privadas ou credenciais | Pendente |

## Pagamentos
- Estados mínimos: pendente, confirmado, falhou/cancelado, reembolsado/chargeback.
- Idempotência e verificação de autenticidade dos eventos.
- Liberação de acesso apenas após confirmação confiável; estorno revoga acesso correspondente.
- Reconexão II exige pagamento próprio e pré-requisitos. O administrador define descontos e validade das ofertas; avaliar suporte do gateway a essas configurações e manter histórico das condições contratadas.

## Hospedagem e escolha de provedores
- Nenhum fornecedor de hospedagem da plataforma, banco ou vídeo foi escolhido. A decisão será tomada depois, com participação dos agentes, comparação de custos e aprovação.
- Não tratar serviços citados como contratados.

## Vídeo
- O player deve permitir detectar segmentos efetivamente assistidos; pular diretamente ao fim não equivale a 90%.
- Testar retomada, repetição, troca de navegador/dispositivo e disponibilidade de gravações.
- A proteção de conteúdo e custos por visualização dependem do provedor.

## E-mails e notificações
- Avisos de alteração/cancelamento de live são **internos**, não dependem de e-mail.
- Recuperação de senha requer canal de e-mail real.
- Política de e-mails de marketing não faz parte da definição técnica aprovada.

## Avaliação de fornecedores
Comparar: preço mensal e por uso, limites gratuitos, segurança, suporte, exportação de dados, webhook/API, SLA, dependência do fornecedor e adequação ao volume previsto. Aprovação explícita antes de contratação.

## WhatsApp e expiração
- Somente WhatsApp será utilizado como comunidade; não integrar chat próprio ou comunidades alternativas.
- A plataforma pode bloquear o botão/link ao fim da vigência, mas isso **não remove automaticamente** quem já ingressou em um grupo externo.
- Definir responsável, processo e periodicidade de verificação/remoção dos membros sem acesso válido; automação somente se tecnicamente viável e aprovada.
