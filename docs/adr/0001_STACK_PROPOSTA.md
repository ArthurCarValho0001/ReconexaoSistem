# ADR-0001 - Stack e execução local

Estado: PROPOSTO, não aprovado.

Contexto: Arthur conhece C#; aplicação precisa de regras no servidor, banco relacional,
interfaces responsivas, integrações e baixo custo operacional.

Proposta: ASP.NET Core .NET 10 + React/TypeScript/Vite + PostgreSQL; monólito modular.
Banco local: decidir instalação nativa OU contêiner, conforme ambiente/licença/familiaridade.
Identidade: discutir mecanismo com sessão revogável e vinculação Google verificada.

Consequências: duas ferramentas de desenvolvimento (.NET/Node), banco local real e
integração de frontend/API. Hospedagem deve suportar a escolha; não é ASP.NET Framework.

Antes de aprovar: conferir `dotnet --info`, Node/npm, Git e método de banco. Registrar
versões principais, política de patches, autenticação, portas e alternativa se ferramenta faltar.

Aprovação: PENDENTE. Data: PENDENTE. Responsável: Arthur.
Alternativa escolhida, se houver: PENDENTE.
