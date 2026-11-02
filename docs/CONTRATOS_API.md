# Contratos antes do paralelismo

Estado: contratos T02/T03 abaixo implementados; demais operações continuam orientação
de especificação, sem alegação de implementação.

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

## Autenticação — T02

Cookie `__Host-Reconexao`, Secure/HttpOnly/SameSite=Strict, caminho `/`, sem Domain,
oito horas sem renovação deslizante. Identidade, perfil e revogação conferidos no
banco a cada requisição; o navegador não escolhe perfil ou autor. Sem cadastro público.
Todos os POSTs abaixo validam cookie CSRF e header `X-CSRF-TOKEN`; obter token novo
por GET após mudar de identidade. O frontend mantém tokens somente em memória.

| Método/rota | Entrada / ator | Retorno e efeito |
|---|---|---|
| GET `/api/auth/csrf` | Público | 200 `{token}` e cookie CSRF; não concede autenticação |
| POST `/api/auth/login` | `{email,password}` | 200 `{id,email,role}`; substitui sessão anterior na transação; 401 genérico para credenciais/bloqueio |
| GET `/api/auth/session` | Sessão atual | 200 `{id,email,role}` próprios; 401 ausente/revogada; query/ID não muda o autor |
| POST `/api/auth/logout` | Sessão atual, sem dados de autor | 204; revoga no servidor, remove cookie; cookie antigo recebe 401 e não revoga sessão nova |
| POST `/api/auth/forgot-password` | `{email}` | 202 genérico; não revoga; não confirma conta ou entrega; adapter indisponível antes da consulta produz 503 para qualquer conta |
| POST `/api/auth/reset-password` | `{email,token,password}` | 204; senha/stamp/revogação atômicos; 400 genérico para token inválido/expirado/reusado ou senha recusada |
| GET `/api/admin/session` | Admin | 200 própria sessão; aluno 403, anônimo 401; prova de autorização T02, sem dados privados de terceiros |

Perfis `aluno` e `admin`, restritos no banco. Login, redefinição e logout serializam
por usuário (`SELECT FOR UPDATE`); dois logins deixam exatamente uma sessão válida;
dois resets com o mesmo token têm apenas um sucesso. Repetir reset válido falha,
sem efeito adicional. Cookie de sessão não é token de redefinição.

Política técnica: senha mínima 12 caracteres com maiúscula/minúscula/número/símbolo;
cinco falhas bloqueiam login por 15 minutos; login/recuperação/reset compartilham
limite local em memória de 20 pedidos por minuto por IP (429). Reset expira em 30
minutos. Corpo de erros públicos é genérico, sem conexão, token ou dados privados.
CSRF inválido retorna 400; indisponibilidade inesperada retorna erro genérico.
Respostas protegidas usam `no-store`; CSRF usa `no-cache, no-store`.

Adapter `Local` grava arquivos identificados `LOCAL_SIMULADO` fora do Git, somente
Development/Test. Falha posterior ao probe mantém 202 para não revelar existência
de conta; registra apenas aviso operacional genérico. 202 significa pedido recebido,
não entrega de e-mail. Produção rejeita `Local` na inicialização; sem adapter real,
recuperação retorna 503. Entrega/retry com fornecedor real pertence à T08.
Bootstrap e fixtures são comandos CLI explícitos, não endpoints. Google verificado
permanece T04 e nenhuma claim fictícia é emitida; gate inicial aguarda P03.

## Relatos privados — contrato T03

Implementado e validado localmente na T03; resultados em evidencias/T03_RELATOS.md.
Aluno autenticado pode enviar opcionalmente; somente admins consultam qualquer relato.
Sem histórico do aluno, pontos, resposta, edição/exclusão ou regra de retenção automática.
Gate inicial depende de P03; retenção com dados reais depende de P11. Não são gates
inventados nesta entrega.

| Método/rota | Ator/entrada | Resposta/efeito |
|---|---|---|
| POST `/api/relatos` | Aluno autenticado, `{text}` e header `X-CSRF-TOKEN` | 201 `{status:"received"}` após persistência; sem eco, ID ou URL de consulta para aluno |
| GET `/api/admin/relatos?limit=20&cursor=...` | Admin; limit de 1 a 50, cursor opcional | 200 `{items:[{id,author:{id,email},createdAt,text}],nextCursor}`; IDs de relato como strings; nextCursor null no final |

Autor vem da sessão e data/hora UTC do servidor. Campos adicionais de autor/data são
rejeitados com 400; não existe GET de histórico/detalhe para aluno. Anônimo recebe
401 nas rotas protegidas, aluno recebe 403 na consulta administrativa; admin não
envia relato pela rota de aluno. Sem métodos de edição/exclusão/resposta.
Erros de validação/CSRF/cursor são 400 genéricos sem texto/autor recebido; falha de
persistência retorna erro genérico sem corpo privado nos logs/mensagens.
Corpo JSON do POST limitado a 16 KiB; exceder ou enviar JSON malformado gera 400
genérico. Esse limite comporta 1.000 escalares inclusive com escapes Unicode.

Contagem: valores escalares Unicode (pontos de código válidos), não bytes nem unidades
UTF-16. Emoji simples conta 1; `e` + acento combinante conta 2; sequências de família
contam seus componentes. CRLF e CR viram LF antes de contar/salvar; sem trim ou NFC.
Texto deve ter 1 a 1.000 valores e algum caractere fora de Unicode White_Space.
Surrogates isolados e NUL são recusados. Frontend aplica a mesma política; não usa
maxlength UTF-16 que reduziria silenciosamente a capacidade de emojis.

Consulta ordenada por `createdAt DESC, id DESC`, com cursor do último registro lido
do PostgreSQL e devolvido na página; seek pelas duas chaves, sem offset/contagem total.
O cursor contém somente posição, sem texto/autor. Novos relatos mais recentes não
deslocam as páginas já percorridas; atualizar a lista reinicia a consulta. Um envio
confirmado é um relato independente, sem deduplicar textos iguais. O frontend impede
clique duplo enquanto o pedido está em andamento, sem prometer deduplicação de retry
de uma resposta perdida. Nenhum rascunho em localStorage.
Rascunho permanece só na memória da sessão/tela: erros de rede/validação/persistência
mantêm o texto; logout, perda de sessão, troca de usuário ou recarga desmontam o
formulário, sem restaurar dados para outro usuário. Relato enviado não retorna à UI
do aluno. Admin mostra instantes UTC convertidos para Brasília na tela.

## Validade e renovação

Verificar validade no servidor em todos os acessos protegidos; ao vencer, bloquear conteúdos e manter caminho de renovação. Checkout deve distinguir compra inicial/renovação e validar confirmação idempotente. Agendar avisos persistentes 60 dias antes e no vencimento por e-mail e plataforma, com retry, reconciliação e alertas. Não depender de visita do aluno para enviar. Guardar preço do pedido e períodos anteriores. P19–P22 bloqueiam os comportamentos ainda indefinidos.
