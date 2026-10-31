# Estado do projeto para retomada

Atualização: 31/10/2026. T00 concluída documentalmente com base aprovada por Arthur.
Não há aplicação implementada; T01 não iniciada por instrução explícita.

- Entrega atual: T00 ACEITA quanto à base técnica, com ajuste Docker Compose registrado no ADR-0001.
- Branch: `task/t00-alinhamento-tecnico`; base `8d01736`; commit desta entrega identificado por `git log -1` após o fechamento documental (hash informado na resposta final).
- Stack: ASP.NET Core/.NET 10, React/TypeScript/Vite, PostgreSQL, Identity e monólito modular aprovados; patches de dependências e validação funcional ficam na T01.
- Fornecedores: não escolhidos.
- Fórmula oficial 3FM: aguardando envio do cliente.
- Testes da aplicação: NÃO EXECUTADOS.
- Verificação anterior do kit: links locais, sintaxe TOML e integridade do ZIP informados na geração; não repetidos nesta T00.
- Revisões T00: agentes separados QA e segurança, sem edição; cinco achados incorporados. Sem validação de aplicação/fornecedor.
- Próxima ação: quando solicitada, executar T01 conforme prompts/01_FUNDACAO.md e plano de EXECUCAO_LOCAL; não iniciar T01 nesta resposta nem T13 pelo arquivo aberto.
- Artefatos: ADR-0001, inventário T00, evidência T00, arquitetura, pendências, critérios, integrações, execução local e plano atualizados.
- Ambiente: .NET SDK 10.0.402, Node 24.22.2, npm.cmd 11.19.1, Git 2.56.0.windows.4; Docker Engine 29.9.2, Compose v5.6.1, desktop-linux/x86_64 verificados por caminho completo.
- Banco aprovado: Docker Compose, postgres:18.6-bookworm fixado por digest no ADR; dev persistente e testes descartáveis isolados, portas só loopback. Nenhum PostgreSQL iniciado nesta T00.
- Falhas reais: npm.ps1 bloqueado (npm.cmd validado); Docker fora do PATH e instalação por usuário inacessível no sandbox; consulta autorizada por caminho completo confirmou Engine/Compose. Branch inicialmente bloqueada pelo sandbox e criada com escalonamento.
- Integrações reais/fake: nenhuma executada. Credenciais necessárias nesta T00: nenhuma; configuração Google e fornecedores permanece para as entregas correspondentes.
- Bloqueios: P01 resolvida pela aprovação com Compose; P03/P04/P06/P08/P19–P22 e demais pendências por módulo preservadas, sem decisões de negócio inferidas.
- Comandos/resultados e reprodução: docs/INVENTARIO_T00.md e docs/evidencias/T00_ALINHAMENTO.md.

## Atualizar a cada entrega

Data; branch e commit; requisito/entrega; arquivos alterados; comandos reais e resultados;
integrações reais versus fake; pendências bloqueantes; próximo passo exato; alterações
locais não commitadas; credenciais necessárias (somente nomes, nunca valores).

Não marcar T04/T07/T09 prontos com base apenas em mocks. Não apagar falhas anteriores
sem registrar a correção/evidência. Um novo chat deve ler este arquivo antes de agir.

## Preparação informada por Arthur

Máquina declarada preparada por Arthur; não houve execução remota dos testes neste kit. Repositório informado: ReconexaoSistem, pasta Windows C:\Users\Arthur\Documents\GitHub\ReconexaoSistem. Nenhuma alteração remota feita por este pacote. Regra de dois anos consolidada; P19–P22 continuam pendentes.
