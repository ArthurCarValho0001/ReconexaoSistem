# Contratos antes do paralelismo

Estado: orientação de especificação; rotas finais ainda não implementadas.

Para cada endpoint registrar: método/rota, ator, autorização por recurso, entrada validada,
retorno, erros, idempotência, paginação, efeitos no banco e teste correspondente. OpenAPI
deve refletir implementação real. Nunca confiar em `alunoId` enviado para atuar como outro aluno.

| Operação | Regra de contrato |
|---|---|
| Entrar/sair/consultar sessão | Sessão revogável; saída invalida servidor; sem conteúdo privado no status |
| Vincular Google | State/nonce e tokens verificados no servidor; não vincular pela igualdade não comprovada de e-mail |
| Enviar relato | Autor da sessão; 1..1000 caracteres válidos; confirmação sem expor listagem do aluno |
| Consultar relatos | Admin, paginado; aluno/anônimo negados |
| Consultar atividade | Acesso/nivel/initial gate/janela verificados; não inclui chaves Google |
| Importar submissão | Canal autenticado, identidade vinculada, form/version, timestamp e completude verificados |
| Conceder pontos | Operação interna de domínio; não expor endpoint de produção com valor arbitrário |
| Consultar progresso | Próprio usuário; admin conforme permissão; ledger auditável |
| Notificar pagamento | Assinatura/autenticidade específica do provedor, correlação de pedido, deduplicação |
| Solicitar upload | Admin; URL/credencial limitada; não expor chave mestre |
| Alocar em turma | Admin; transação de vagas e deduplicação |

## Erros

Adotar formato consistente de validação/autenticação/proibição/conflito/indisponibilidade.
Mensagens públicas não contêm stack trace, respostas privadas ou segredos. A aplicação
trata integração pendente sem afirmar sucesso. Fixar contratos aprovados aqui durante T01-T04.

## Validade e renovação

Verificar validade no servidor em todos os acessos protegidos; ao vencer, bloquear conteúdos e manter caminho de renovação. Checkout deve distinguir compra inicial/renovação e validar confirmação idempotente. Agendar avisos persistentes 60 dias antes e no vencimento por e-mail e plataforma, com retry, reconciliação e alertas. Não depender de visita do aluno para enviar. Guardar preço do pedido e períodos anteriores. P19–P22 bloqueiam os comportamentos ainda indefinidos.
