# Fontes e proveniência

Consulta técnica em 26/10/2026. Não fixar disponibilidade de modelo, cota ou menus futuros.

## Documentação oficial OpenAI

- [Extensão IDE](https://learn.chatgpt.com/docs/codex/ide): abrir pasta e painel, autenticação e revisão.
- [CLI](https://learn.chatgpt.com/docs/cli): instalação conforme sistema, execução `codex` e comandos de sessão.
- [AGENTS.md](https://learn.chatgpt.com/docs/agent-configuration/agents-md): instruções persistentes do repositório.
- [Subagentes](https://learn.chatgpt.com/docs/agent-configuration/subagents): perfis TOML e delegação.
- [Configuração](https://learn.chatgpt.com/docs/config-file/config-reference): validar opções do cliente instalado.
- [Boas práticas](https://learn.chatgpt.com/docs/learn/best-practices): tarefas delimitadas, checkpoints e isolamento.

Configuração incluída segue o formato consultado de agentes standalone em `.codex/agents`.
Perfis herdam configurações da sessão; ausência de modelo fixo evita dependência de acesso
não confirmado. Subagentes não constituem franquia adicional; usar apenas quando útil.

## Versões da proposta técnica

- [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy): .NET 10 LTS em suporte na consulta.
- [Node.js](https://nodejs.org/en/download): ramo Node 24 LTS na consulta.

## Integrações a verificar na prova real

- [Forms: compartilhar/incorporar](https://support.google.com/docs/answer/2839588)
- [Apps Script: gatilhos instaláveis](https://developers.google.com/apps-script/guides/triggers/installable)
- [Apps Script: cotas](https://developers.google.com/apps-script/guides/services/quotas)
- [Forms API](https://developers.google.com/workspace/forms/api/guides)

Esses links de integração são referências para T04 e não atestam teste executado.

## Base do projeto

ZIP anexado `Reconexao_Documentacao_Atualizada.zip`, documentação de 18/10/2026,
preservado em `referencia_historica`. Mudanças posteriores são as respostas explícitas
de Arthur registradas em `DECISOES.md`. Não consultamos outra conversa nem verificamos
o estado atual do GitHub. As configurações foram verificadas sintaticamente, não executadas
num cliente Codex instalado no computador do usuário.
