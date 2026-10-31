# Reconexão: kit de desenvolvimento com Codex

Versão 1.1 | Atualizado em 31/10/2026 | Responsável: Arthur Carvalho.

## O que você recebeu

Um pacote de instruções, requisitos consolidados, roteiro, prompts e agentes opcionais.
Não é a aplicação implementada. Não contém credenciais, vídeos, fórmulas oficiais do
3FM ou uma cópia atual do código do protótipo. Nenhum repositório remoto foi criado,
nenhum agente foi instalado no seu computador e nenhum serviço foi contratado.

## Caminho recomendado

1. Extraia esta pasta em `C:\Projetos\ReconexaoApp` (exemplo).
2. Leia [o passo a passo](GUIA_PASSO_A_PASSO.md).
3. Abra a pasta inteira no VS Code; `AGENTS.md` deve estar na raiz aberta.
4. Instale/abra o Codex oficial e entre com sua conta ChatGPT.
5. Envie [o prompt inicial](prompts/00_INICIAR.md).
6. Aprove a arquitetura na entrega T00. ASP.NET Core + React + PostgreSQL ainda é proposta.
7. Execute uma entrega por vez, conforme [a linha de entregas](docs/PLANO_ENTREGAS.md).
8. Retome pelo [estado do projeto](docs/ESTADO_PROJETO.md), e não pela memória do chat.

## Três cuidados que evitam retrabalho

- Não execute todos os prompts simultaneamente: existem dependências entre eles.
- Não use `docs/referencia_historica/` como regras vigentes. Há decisões substituídas.
- Não confunda simulação local aprovada para testes com integração real pronta para produção.

## Navegação

| Preciso de... | Arquivo |
|---|---|
| Instalar, abrir e trabalhar no Codex | [Guia](GUIA_PASSO_A_PASSO.md) |
| Saber o que já foi decidido | [Decisões](docs/DECISOES.md) |
| Conferir o comportamento atual | [Requisitos](docs/REQUISITOS.md) e [regras](docs/REGRAS_NEGOCIO.md) |
| Saber o que falta | [Pendências](docs/PENDENCIAS.md) |
| Dividir tarefas e chats | [Agentes e chats](docs/AGENTES_E_CHATS.md) |
| Começar uma entrega | [Prompts](prompts/README.md) |
| Revisar e entregar ao cliente | [Aceite](docs/CRITERIOS_ACEITE.md) e [handover](docs/ENTREGA_E_CORRETIVAS.md) |
| Conferir versões das ferramentas | [Fontes](docs/FONTES.md) |

O valor e prazo comerciais já acordados não foram recalculados por este kit.
O foco é documentação e execução técnica. Não há cronograma fictício de dias por tarefa.

## Preparação desta versão

Use [PREPARAR_DOCUMENTACAO.md](PREPARAR_DOCUMENTACAO.md) para copiar este pacote para o repositório ReconexaoSistem. A regra atual é acesso por dois anos, renovação paga por dois anos e avisos 60 dias antes/no vencimento.
