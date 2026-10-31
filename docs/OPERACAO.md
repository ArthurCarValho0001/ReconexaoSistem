# Operação, ambientes e segredos

## Desenvolvimento

Dados fictícios, banco local real, configurações ignoradas pelo Git. Cadastrar usuários
de teste por processo controlado. Fake providers explícitos e exclusivos de dev/test.
Portas/bancos próprios por worktree. Nada de secrets em prompts, screenshots ou fixtures.

## Homologação

Dados fictícios ou saneados, chaves sandbox e domínio/endereço distinto. Provar login,
Google, pagamento, vídeos e restauração. Não enviar e-mail real para alunos em testes.
Não reaproveitar credenciais ou cópia não autorizada de produção.

## Produção

Fornecedores aprovados, HTTPS, origem/CORS/cookies corretos, migrations revisadas,
primeiro admin controlado, backups e alerta de erros. Contratar plano não implementa
autorização. Testar que fake providers e endpoints de teste falham fechados.

## Publicação

Build e testes em CI, lockfiles, versão/release e changelog. Rotina de migração separada
e registrada, backup quando necessário, smoke test e plano de rollback compatível com
schema. Nem toda migração de dados tem downgrade seguro: registrar estratégia.
Nunca autorizar produção ou merge apenas por instrução genérica de implementação local.

## Recuperação e monitoramento

Definir retenção, responsável, teste de restore isolado, tempo e perda de dados aceitáveis.
Logs sem respostas/relatos/tokens. Alertar falhas de importação, filas paradas, confirmações
de pagamento e consumo. Cotas gratuitas podem interromper automações; documentar reprocessamento.
Não prometer monitoramento humano contínuo depois da entrega sem acordo.

## Validade e renovação

Verificar validade no servidor em todos os acessos protegidos; ao vencer, bloquear conteúdos e manter caminho de renovação. Checkout deve distinguir compra inicial/renovação e validar confirmação idempotente. Agendar avisos persistentes 60 dias antes e no vencimento por e-mail e plataforma, com retry, reconciliação e alertas. Não depender de visita do aluno para enviar. Guardar preço do pedido e períodos anteriores. P19–P22 bloqueiam os comportamentos ainda indefinidos.
