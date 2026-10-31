# Execução local

Estado: AINDA NÃO IMPLEMENTADA. Este kit contém documentos e configurações opcionais.

## T01 precisa preencher e verificar

- SDK/runtime e ferramentas com versões; lockfiles/global.json conforme stack aprovada.
- Pré-requisitos e comandos PowerShell e, se suportado, alternativa Linux.
- Como configurar segredos locais sem Git e exemplos sem credenciais reais.
- Como iniciar/parar banco, criar schema, aplicar migrações e inserir seeds fictícios.
- Como criar primeiro admin com procedimento controlado, sem senha fixa no repositório.
- Como iniciar frontend/API, portas e URLs reais de health/UI/OpenAPI.
- Como executar testes unitários, integração em PostgreSQL descartável e E2E.
- Como testar nova sessão, aluno A/B e admin com dados fictícios.
- Como desligar tudo sem apagar dados e como recriar banco SOMENTE descartável de testes.
- Onde ficam logs e como garantir que não exibem tokens, relatos ou respostas.

## Convenção proposta de variáveis, nomes finais a definir

Ambiente, conexão de banco, frontend origin, URL pública da API, Google client ID/secret,
identificação de conta/projeto Forms, credencial de integração, provedor e-mail, provedor
checkout e credencial de vídeo. Variável prefixada para frontend é pública: não use para segredo.

## Não fazer

Não colar comandos de migrations/compose que ainda não existem e afirmar que funcionam.
Não usar banco de produção para testes; não executar remoção de volumes para resolver build.
Adaptadores fake só em Development/Test e com teste de falha de inicialização em Production
se ainda configurados. Pagamento ou resposta Forms não devem ter bypass de produção.
