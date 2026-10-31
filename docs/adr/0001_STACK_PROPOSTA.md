# ADR-0001 - Stack e execução local

Estado: APROVADO por Arthur em 31/10/2026, com ajuste para Docker Compose.

Contexto: Arthur conhece C#; aplicação precisa de regras no servidor, banco relacional,
interfaces responsivas, integrações e baixo custo operacional.

Proposta: ASP.NET Core .NET 10 + React/TypeScript/Vite + PostgreSQL; monólito modular.
Banco: PostgreSQL 18, mesmo motor em desenvolvimento, testes e produção.
Local aprovado: Docker Compose no Docker Desktop existente, substituindo a proposta
anterior de distribuição nativa e gerenciamento manual de binários. Sem fallback
silencioso para PostgreSQL nativo ou SQLite; falha Docker exige diagnóstico explícito.
Imagem fixada: `docker.io/library/postgres:18.6-bookworm`, índice OCI
`sha256:5c4783a8cc3f0074912164f94dfa1be4e043438f0f707cf968d7af9a0d31a2c8`.
Na T01 o Compose usará tag e digest, sem `latest`. Manifesto linux/amd64 confirmado
no registro, compatível com o Engine Linux x86_64 consultado; sem pull de camadas
ou execução PostgreSQL nesta T00. Linha 18 em suporte até 14/11/2030 conforme
[política oficial](https://www.postgresql.org/support/versioning/); tag confirmada no
[catálogo oficial](https://hub.docker.com/_/postgres). Provider Npgsql EF 10 documenta
[suporte ao PostgreSQL 18](https://www.npgsql.org/efcore/release-notes/10.0.html).
Compatibilidade funcional de EF/driver/migrações será provada na T01, com patches fixados.

Desenvolvimento: projeto Compose `reconexao-dev`, volume nomeado persistente exclusivo
montado em `/var/lib/postgresql` (PGDATA padrão 18: `/var/lib/postgresql/18/docker`).
Publicação `127.0.0.1:55432:5432`; nenhum listener encontrado na consulta T00,
mas o bind efetivo será testado na T01. Testes: projeto separado `reconexao-test`,
serviço/credenciais/banco próprios, armazenamento temporário tmpfs nesse mesmo caminho,
sem montar o volume dev; porta proposta `127.0.0.1:55433:5432`, a verificar na T01.
Descarte/recriação só do projeto de testes; parar desenvolvimento preserva o volume.
Credenciais em arquivos locais ignorados (`.env`/`local-data/`) ou user-secrets;
Compose secret com `POSTGRES_PASSWORD_FILE`, sem valores no Git, logs ou prompts.
Não usar `trust` para conexões de host nem usuário superadmin como credencial da API.

Identidade: ASP.NET Core Identity, senha com hash pelo framework e cookie HttpOnly,
Secure em HTTPS, SameSite compatível com o callback Google validado. Sessão opaca
revogável no banco, conferida em cada requisição autenticada. Novo login substitui a
sessão ativa em transação; login concorrente deve deixar só uma sessão válida. Logout,
conclusão válida da redefinição de senha e alteração de privilégios revogam sessões
conforme contrato T02. Solicitar e-mail de recuperação não altera a sessão vigente.
Convites e recuperação usam tokens expirantes de uso único; reemissão invalida token
anterior sem apagar compra. Bootstrap admin explícito, sem senha fixa ou perfil do cliente.
Proteção CSRF nas mutações por cookie, inclusive login/vínculo; CORS só origem autorizada.
Limitar tentativas e devolver erros de autenticação/recuperação sem enumerar contas.

Google é vínculo adicional, sem conceder perfil ou acesso financeiro. Validar no servidor
emissor, audiência, expiração, assinatura, state/nonce e PKCE conforme fluxo. Vínculo exige
sessão local e prova Google recente; chave única por provedor/subject, sem associação
automática por e-mail. T04 deve provar correspondência entre identidade vinculada e
respondente verificado do Forms; se a evidência disponível não bastar, bloquear conclusão
e registrar a limitação. E-mail digitado, retorno de tela e ID oculto não autenticam envio.

## Versões, estrutura e execução propostas

- .NET 10 LTS, EF Core 10 e Npgsql compatível; React 19, TypeScript 5 e Vite 7,
  Node 24 LTS. São linhas aprovadas, não dependências já resolvidas: na T01 confirmar
  suporte/matriz oficial, fixar patches em global.json/manifestos/lockfiles e executar
  restore/build antes de declarar compatibilidade. Mudança de linha por incompatibilidade
  deve voltar ao ADR; patch suportado é escolha técnica documentada.
- Inventário real: SDK 10.0.402, runtime ASP.NET 10.0.13, Node 24.22.2, npm.cmd 11.19.1,
  Git 2.56.0.windows.4; ver [inventário](../INVENTARIO_T00.md).
- Docker Engine 29.9.2, Compose v5.6.1 e contexto desktop-linux acessíveis pelo
  executável da instalação por usuário, fora do PATH desta sessão. Nenhuma reinstalação.
- `src/backend`: um backend com módulos Identidade, Acesso/Comercial, Conteúdo/Turmas,
  Forms/3FM, Progresso e Relatos; dependências explícitas, sem microsserviços.
  `src/frontend`: React; `tests`: unidade/integração/E2E; `infra`: configuração;
  `docs`: decisões/evidências. Essas pastas de aplicação ainda não serão criadas.
- Propor UI HTTPS localhost:5173 e API HTTPS localhost:5001 com proxy `/api` do Vite;
  produção sob mesma origem. Portas/certificado/confiança local serão verificados na T01,
  com explicação e autorização caso seja necessária instalação de certificado.
- Segredos locais fora do Git por user-secrets/configuração ignorada; nenhuma variável
  VITE contém segredo. Dados, backups e chaves Data Protection separados por ambiente;
  produção precisa de persistência, acesso mínimo e proteção das chaves em repouso.

## Fronteiras de segurança e integridade propostas

Backend combina perfil, propriedade do recurso, conta ativa, direito válido, nível e
gate aprovado. Inicial (P03) e vencido (P22) precisam de matriz de rotas antes dos gates
finais; admin e vinculação não devem ser bloqueados por suposição. Dois anos iniciam
na ativação/liberação, nunca no pagamento; seis meses não são prazo de expiração.
Relógio do servidor, instantes UTC e data Day em America/Sao_Paulo; P19 define calendário.
Arquivos/player exigem autorização também na emissão de links temporários. Links já
emitidos precisam de validade limitada e estratégia de revogação validada com fornecedor;
não prometer bloqueio instantâneo de URL externa sem essa capacidade.
Expiração de URL/token privado deve ser menor ou igual ao vencimento do direito,
com checagem no emissor. T08/T09 testam URL emitida imediatamente antes do vencimento
e reutilizada depois, além de suspensão/revogação durante sua validade; fornecedor
incapaz de cumprir o bloqueio impede o aceite correspondente de C30.
Usar relógio injetável (TimeProvider) em políticas e workers para testar fronteiras
sem mudar o relógio da máquina: avisos de 60 dias, vencimento, Day, acompanhamento
e calendário após resolução de P04/P19.

Ledger e importação usam transações e restrições únicas por aluno/evento lógico;
nova versão não repontua atividade. Turmas usam transação/lock e teste concorrente
em PostgreSQL. Conteúdo de relatos/respostas nunca entra nos logs; auditoria guarda
ator/ação/recurso/instante, com acesso restrito. Sem purge antes de P11.

Adapters de Forms, checkout, vídeo e e-mail isolam fornecedores ainda não escolhidos.
Inbox/outbox persistentes no PostgreSQL, worker com retry limitado, deduplicação,
reconciliação e erro sanitizado; não contratar fila externa nesta fase. Webhooks exigem
autenticidade específica do provedor, assinatura/anti-replay quando suportados, limites
de payload, correlação e idempotência; nunca usar só URL secreta. Conferir pagamento
no servidor, pedido/valor/moeda e eventos fora de ordem. Segredos somente backend.
Forms conserva instante confiável de envio, esquema/versão e respostas; callback
autenticado ou consulta API será escolhido por prova real em T04. Rejeitar origem,
identidade ou versão não comprovadas sem pontuar. Fakes somente Development/Test,
com falha de inicialização em Production e sem endpoint arbitrário de pontos.
T04 deve provar restrição de envio Day antes/depois do dia de Brasília, inclusive
por link Google direto. Apenas negar pontos não cumpre a restrição de envio de R19;
se o Google não permitir cumpri-la, registrar bloqueio e obter decisão de Arthur.

## Dependências, aceite e alternativas

P01 é a única aprovação necessária para iniciar a fundação T01; não resolve regras de
negócio ou fornecedores. [Pendências](../PENDENCIAS.md) delimitam cada módulo.
3FM não terá cálculo/devolutiva inventados; falta P08. P19–P22 bloqueiam fronteiras
de renovação/gates/avisos, não leitura ou estrutura independente. Regras vigentes:
40% únicos/+10 por vídeo, II aos 200, Google Forms em todos instrumentos, Day no dia
de Brasília, relatos privados até 1.000 e turmas alocadas pelo admin.

Aceite T00: inventário verificável, alternativas e riscos registrados, revisões QA e
segurança consolidadas e decisão de Arthur registrada. Proposta revisada não significa
stack aprovada. Aceite T01: restore/build/testes, PostgreSQL descartável, migração,
UI chamando API e comandos PowerShell reais; nada disso foi executado nesta T00.
Integrações reais e E2E pertencem às entregas posteriores, sem aprovação por mocks.

Alternativa avaliada de PostgreSQL nativo foi substituída expressamente por Docker
Compose na aprovação. Frontend Razor foi considerado para reduzir toolchain e não
foi escolhido. Base aprovada permanece monólito modular, React e PostgreSQL via Compose.

## Revisões independentes consolidadas — 31/10/2026

Agentes separados `qa` e `seguranca` executados após preparar a proposta, sem edições
ou testes de aplicação. QA: cobertura T12 C01–C33/C32, janela real Day e relógio
injetável. Segurança: validade de URLs privadas e momento da revogação após reset.
Os cinco achados foram incorporados nos documentos e cenários; a efetividade depende
das implementações/provas futuras. Ver [evidência T00](../evidencias/T00_ALINHAMENTO.md).

Consequências: duas ferramentas de desenvolvimento (.NET/Node), banco local real e
integração de frontend/API. Hospedagem deve suportar a escolha; não é ASP.NET Framework.

Verificações T00: inventário, Docker info/Compose e manifesto concluídos; build,
migrações, banco e persistência aguardam T01. Não confundir aprovação com validação funcional.

Aprovação: Arthur, mensagem explícita nesta sessão em 31/10/2026: "Aprovo a base técnica
da T00 com este ajuste" — PostgreSQL em Docker Compose no Docker Desktop já preparado.
O ajuste substitui somente o método de banco local; não resolve pendências de negócio,
não aprova fornecedor pago/publicação e não autoriza iniciar T01 nesta resposta.
