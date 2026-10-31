# Evidência da entrega T01

- Estado: IMPLEMENTADO e VALIDADO_LOCAL, com revisões concluídas e retestes abaixo.
  ACEITO por Arthur em 31/10/2026, com base nestas evidências e na conferência manual
  da tela local. Sem homologação de fornecedor ou produção.
- Data/autor: 31/10/2026, Codex.
- Branch: task/t01-fundacao, criada do commit aprovado
  6e2fe6657a0edb580b01d284995810701162acf7; T00 preservada. Commit de fechamento
  autorizado: `feat(t01): criar fundacao local do Reconexao`; consultar hash no Git.
  Nenhum push, merge ou início T02 autorizado neste fechamento.
- Critérios: T01 do PLANO_ENTREGAS e prompts/01_FUNDACAO. Sem afirmar aceite dos
  critérios C05/C06 de autenticação ou demais funcionalidades futuras.
- Alterações: solução, SDK/manifestos/lockfiles, API/health, frontend, Compose, scripts,
  integração/E2E, documentação. Migração InitialFoundation somente marcador fictício.
- Ambiente: Windows, .NET SDK 10.0.402/runtime ASP.NET 10.0.13, Node 24.22.2/npm
  11.19.1, Docker 29.9.2/Compose 5.6.1, Chrome instalado; versões em EXECUCAO_LOCAL.

| Comando/cenário executado | Resultado real | Evidência sanitizada |
|---|---|---|
| git status/log; git switch -c task/t01-fundacao <commit T00> | PASSOU | Árvore inicialmente limpa, base aprovada e branch T00 preservada |
| Docker info/compose version por executável completo | PASSOU | 29.9.2, Linux x86_64; Compose 5.6.1 |
| Consulta npm versões no sandbox | FALHOU por EACCES/rede | Repetida com escalonamento autorizado, sucesso; sem atualização global |
| npm install --prefix src/frontend --ignore-scripts --no-fund --no-audit | PASSOU | Criou package-lock.json com versões exatas antes do primeiro ci |
| npm ci --prefix src/frontend --ignore-scripts --no-fund | PASSOU | 26 pacotes; auditoria 0 vulnerabilidades na consulta |
| dotnet tool restore | PASSOU | dotnet-ef 10.0.13 |
| Primeiro restore antes do projeto de testes existir | FALHOU MSB3202 | Criado projeto e restore repetido com sucesso |
| Primeiro build .NET | FALHOU CS1705 | Conflito EF 10.0.4/10.0.13 corrigido com referências runtime explícitas |
| dotnet build Reconexao.slnx --no-restore após correção | PASSOU | 0 avisos/0 erros |
| dotnet restore Reconexao.slnx --locked-mode | PASSOU | Dois projetos restaurados a partir dos locks |
| Primeiro build frontend | FALHOU TS2769 | Tipo da CA do proxy corrigido de Buffer para string |
| npm --prefix src/frontend run build após correção | PASSOU | TypeScript + Vite 7.3.7 |
| Certificado no sandbox / exportação PFX sem senha | FALHOU | Certificado existente localizado com autorização; PFX flags incompatíveis |
| cert-export em PEM | PASSOU | Exportou certificado existente sem --trust/instalação; arquivos ignorados |
| node scripts/local.mjs setup; db-up; test-db-up | PASSOU | Secrets aleatórios locais; dois containers saudáveis |
| migration-add; migrate | PASSOU | Migração inicial e marcador no banco dev; usuário API sem superadmin |
| Primeira suíte .NET | 7 PASSARAM, 2 FALHARAM | EF encapsulava falha Npgsql e retornava 500 em vez de 503 |
| Suíte .NET após correção do filtro de exceções | 9 PASSARAM | Migração, escrita/rollback, guards, live/ready/API e falha sem segredos |
| verify-storage inicial | FALHOU | Tmpfs sem opções representado por string vazia; guarda corrigida para presença da chave |
| verify-storage após correção | PASSOU | Volume dev exclusivo; tmpfs test; ambas as portas só 127.0.0.1 |
| verify-persistence | PASSOU | ID container mudou; mesmo volume; marcador Reconexão e uma migração antes/depois |
| test-db-discard; test-db-up; test | PASSOU | Somente projeto test recriado; 9 testes passaram de novo em banco novo |
| Consulta SQL direta pelo PowerShell | FALHOU em identificador com aspas | SHOW/versão retornaram; helper Node db-info corrigiu passagem de aspas |
| node scripts/local.mjs db-info | PASSOU | PostgreSQL 18.6; /var/lib/postgresql/18/docker; um marcador dev preservado |
| Playwright com API realmente parada | 1 PASSOU | Proxy >=500 e mensagem de falha, sem mock |
| Playwright com API HTTPS real | 3 PASSARAM | Sucesso real, recuperação após rede simulada e viewport móvel/teclado |
| Inspeção visual capturas desktop/móvel | CONCLUÍDA | Sem overflow na viewport testada; referência textual azul-escuro/R |
| npm audit --omit=dev | PASSOU | 0 vulnerabilidades reportadas nesta consulta |
| git check-ignore de conexões, chave PEM e .env | PASSOU | Arquivos locais sensíveis ignorados |
| Varredura de senhas locais/chave privada em arquivos rastreados e candidatos não ignorados | PASSOU | Sem valores encontrados em 101 arquivos; scanner só exibiu contagem, sem secrets |
| verify-persistence após revisão QA | PASSOU | Exige conteúdo esperado e compara antes/depois automaticamente; ID alterado e mesmo volume |
| Bootstrap corrigido, test-db-discard; test-db-up; test | PASSOU | Inicialização limpa só test, senha fora argv; 9/9 .NET e dev preservado |
| E2E após recriar dev, sem esperar prontidão | 2 PASSARAM, 1 FALHOU | Primeira conexão antiga no pool retornou 503; não era indisponibilidade persistente |
| E2E com pré-condição explícita de API real pronta | 3 PASSARAM | Sem mock no happy path; limite de 15s, falha se serviço não ficar pronto |
| Nova verify-persistence seguida imediatamente de E2E | PASSOU + 3/3 | Reproduzida a condição anterior após correção da precondição |
| Build frontend e locked restore finais | PASSOU | Sem erros de compilação ou divergência dos locks |
| node --check scripts/local.mjs; git diff --check | PASSOU | Sintaxe e whitespace; aviso inicial de LF/CRLF tratado pela regra explícita .gitattributes |
| Conferência final para commit após aceite de Arthur | PASSOU | Diff/arquivos novos revistos; 101 arquivos candidatos sem senhas locais/chaves privadas ou caminhos de dados/builds/dependências; manifestos/locks JSON válidos e arquivos sensíveis ignorados |

## Achados e revisões

Revisões executadas em paralelo por agentes separados `t01_qa` (perfil qa) e
`t01_seguranca` (perfil seguranca), sem edição, acesso aos valores secretos ou execução
de testes que alterassem ambientes. Principal aguardou ambos e corrigiu:

| Revisor/severidade | Achado e impacto | Correção e reteste |
|---|---|---|
| QA/baixa, scripts/local.mjs persistenceProof | Somente imprimir SELECTs permitia PASSOU sem exigir dados | Captura psql -At, exige marcador/migração e compara antes/depois; persistência/isolamento repetidos com sucesso |
| Segurança/baixa, infra/init-app.sh | Senha expandida em argv do psql durante bootstrap | Variável interna psql lê arquivo montado; sem segredo em argv. Somente banco test recriado e 9 testes repetidos, dev preservado |

Ambos revisores confirmaram por leitura as respectivas correções em uma segunda
consulta. QA executou leituras, git status/branch/log, rg, node --check e diff --check;
segurança executou leituras públicas, rg e git check-ignore, incluindo busca no bundle
sem encontrar padrões de credenciais. Não repetiram suítes: resultados operacionais
acima pertencem ao principal. Nenhum bloqueio alto/médio fundamentado foi identificado
pelo revisor de segurança no escopo, o que não é prova de ausência de falhas.

Não houve testes de autenticação/autorização aluno/admin ou fornecedor, fora da T01.

## Limitações e reprodução

Serviço real local: PostgreSQL no Docker. API/UI reais. Sem fornecedor externo/fake,
conta de aluno, admin, Forms, pagamento ou dados pessoais. A interrupção de rede no
segundo E2E é simulada no browser; o teste offline separado usa API realmente parada.
Playwright ignora confiança HTTPS só no contexto de testes; proxy mantém secure:true
e CA local. Sem instalação global/PATH/alteração de política do Windows.
Não há snapshot oficial do protótipo (P18), homologação multi-browser (P16), OpenAPI,
produção, backup/restore ou controles de T02+. Não confundir fundação com produto pronto.

Capturas: [desktop](media/T01_DESKTOP.png), [móvel](media/T01_MOBILE.png),
[falha real](media/T01_FALHA_REAL.png). Capturas não demonstram fidelidade ao protótipo.

Reprodução integral em [EXECUCAO_LOCAL](../EXECUCAO_LOCAL.md); segredos necessários
são gerados localmente sem valores em documentação. Capturas sanitizadas em media.
Próxima entrega possível após revisão/aceite: T02, somente quando solicitada.
