# Reconexão I e II

Comece por [LEIA_PRIMEIRO.md](LEIA_PRIMEIRO.md).

Estado: fundação T01 implementada localmente, com API, frontend e PostgreSQL em Docker
Compose. Execução e evidências em [EXECUCAO_LOCAL](docs/EXECUCAO_LOCAL.md).
Não há funcionalidades de login ou dos módulos posteriores nesta etapa.
O repositório público do protótipo é referência visual, não a aplicação de produção.

## Referências vigentes

- [Escopo](docs/ESCOPO.md)
- [Requisitos](docs/REQUISITOS.md)
- [Regras](docs/REGRAS_NEGOCIO.md)
- [Decisões e substituições](docs/DECISOES.md)
- [Arquitetura aprovada](docs/ARQUITETURA.md)
- [Plano incremental](docs/PLANO_ENTREGAS.md)
- [Como executar localmente](docs/EXECUCAO_LOCAL.md)
- [Estado atual](docs/ESTADO_PROJETO.md)

Builds: `dotnet build Reconexao.slnx --no-restore` e
`npm.cmd --prefix src/frontend run build`, após a preparação documentada.
Não inserir credenciais nos arquivos versionados nem usar banco dev para testes.

Preparação do clone: [PREPARAR_DOCUMENTACAO.md](PREPARAR_DOCUMENTACAO.md). Kit atualizado em 31/10/2026 para validade e renovação por dois anos.
