# Passo a passo: do ZIP à primeira entrega no Codex

## 1. Prepare a pasta correta

Use uma pasta nova para a aplicação real, por exemplo `C:\Projetos\ReconexaoApp`.
Extraia o conteúdo de `Reconexao_Kit_Codex` diretamente nela. Não deixe uma segunda
camada de pasta escondendo `AGENTS.md`. Não copie por cima do protótipo sem comparar
os arquivos: ele é um projeto distinto e não verificamos seu estado atual.

Se já houver código da aplicação, não sobrescreva os documentos/configurações existentes.
Peça primeiro ao Codex para comparar e integrar o kit numa branch. Preserve o histórico.

## 2. Prepare as ferramentas no Windows

| Ferramenta | Uso | Ação |
|---|---|---|
| Git | Histórico/branches | Instalar pela distribuição oficial, se não existir |
| VS Code | Editor e Codex | Instalar; Visual Studio pode continuar sendo usado para backend |
| Codex oficial OpenAI | Execução das tarefas | Instalar extensão oficial no VS Code e autenticar |
| SDK .NET 10 | Backend proposto | Instalar só após aprovar T00; não confundir SDK com runtime |
| Node.js 24 LTS e npm | Frontend proposto | Instalar após T00 e fixar versões em T01 |
| PostgreSQL local | Persistência real | Instalação local OU contêiner, conforme T00 |
| Docker | Opção para banco/testes | Opcional; verificar requisitos e licença aplicável ao seu uso |

Não é necessário instalar tudo para ler o projeto e executar T00. Links oficiais em
[FONTES.md](docs/FONTES.md). Não usar .NET Framework clássico ou a biblioteca corporativa
FuncefEngine neste projeto. Não traga componentes privados do seu emprego.

No PowerShell, confira ferramentas já instaladas:

```powershell
git --version
code --version
dotnet --info
node --version
npm --version
```

Comando não encontrado indica instalação/PATH pendente. Reinicie o terminal após instalar.
Se `npm.ps1` for bloqueado pela política local, tente `npm.cmd --version`; não desative a
política da máquina indiscriminadamente. Não misture ferramentas do Windows e WSL no
mesmo ambiente sem uma decisão explícita. Use Windows nativo como caminho inicial.

## 3. Abra o projeto e configure o Codex

1. VS Code > Arquivo > Abrir Pasta > selecione `ReconexaoApp`.
2. Abra Extensões e procure Codex, verificando que o fornecedor é OpenAI.
3. Abra o painel Codex; entre com a conta ChatGPT que você utiliza.
4. Escolha execução local na pasta. Não confunda com tarefa em nuvem sem seu banco local.
5. Escolha um modelo disponível na sua conta; este kit não fixa nome nem promete cota.
6. Use permissões compatíveis com edição da pasta e execução de testes; revise pedidos
   de instalação, rede e acesso externo. Não habilite acesso irrestrito por conveniência.
7. Faça o teste: peça que o Codex leia `AGENTS.md` e resuma três decisões recentes,
   sem editar arquivos. A resposta deve dizer acesso vitalício, 200 pontos e Google Forms.

O caminho principal é a extensão. O CLI é opcional: instale pelo método atual da página
oficial [Codex CLI](https://learn.chatgpt.com/docs/cli), depois entre na pasta e execute
`codex`. Confira `codex --version` e `codex --help`. Não precisa instalar ambos.
Autenticar pelo ChatGPT é diferente de configurar uma chave de API com cobrança separada.
O produto Reconexão não precisa consumir a API OpenAI para usar Codex no desenvolvimento.

## 4. Crie o checkpoint inicial

Em uma pasta NOVA sem repositório, abra o terminal nela e execute:

```powershell
git init -b main
git status
```

Se Git pedir nome/e-mail, configure a identidade adequada somente neste repositório.
Revise os arquivos, confirme que não há segredos e então:

```powershell
git add .
git diff --cached --stat
git commit -m "docs: prepara base consolidada do Reconexao"
git switch -c chore/t00-alinhamento
```

Se o repositório já existir, use `git status`, preserve mudanças e crie uma branch a
partir da base correta; não rode `git init` para resolver problemas existentes.
O repositório remoto deve ser privado. Crie-o na conta/organização acordada com o cliente,
sem duplicar arquivos iniciais se já houver commit local. Depois de copiar a URL real:

```powershell
git remote add origin URL_REAL_DO_REPOSITORIO_PRIVADO
git push -u origin main
```

Substitua o marcador da URL; não execute literalmente. Não faça esse passo se já houver
`origin`: examine `git remote -v` primeiro. Publicar código não publica a aplicação.

## 5. Execute T00: alinhamento técnico

Cole [prompts/00_INICIAR.md](prompts/00_INICIAR.md) no Codex.
Ele deve levantar pendências, inspecionar seu ambiente e apresentar a arquitetura proposta,
sem criar toda a aplicação. Você aprova ou ajusta a stack; registre a decisão no ADR.
Não precisa voltar a discutir pagamento ou prazo para seguir com requisitos técnicos.

## 6. Execute T01: fundação executável

Após T00 aprovada, cole [prompts/01_FUNDACAO.md](prompts/01_FUNDACAO.md).
O Codex cria os projetos, fixa dependências, banco e configuração local, executa build/testes
e preenche `docs/EXECUCAO_LOCAL.md`. Antes disso, `src/` e comandos de aplicação não existem.
Abra o endereço local informado, confira health e uma migração em banco descartável.

## 7. Trabalhe parte a parte

Use [PLANO_ENTREGAS.md](docs/PLANO_ENTREGAS.md). Uma nova conversa por entrega, com
nomes como `Reconexao | T02 | Autenticacao`. No início, peça leitura do estado e das
referências; não cole o ZIP inteiro em todos os chats. Durante a mesma entrega,
continue no mesmo chat para correções. Ao mudar de entrega, registre o checkpoint.

Para T03/T04, defina antes o escopo do bloqueio pelo inicial. Enquanto pendente, o Codex
pode desenvolver partes independentes; não deve inventar quais rotas o bloqueio restringe.
Teste a integração Forms cedo; não deixe para depois de todas as telas.

## 8. Revisão, aprovação e próxima etapa

1. Peça revisão de segurança e QA com [o prompt de revisão](prompts/90_REVISAO.md).
2. Confira `git diff`, comandos/resultados e telas; reprodução manual complementa testes.
3. Corrija achados; uma revisão por IA não é certificado de segurança.
4. Com testes válidos, faça commit/PR. Merge só após sua aprovação.
5. Atualize o estado e inicie a próxima etapa elegível.

## 9. Aproveite paralelismo sem conflitos

Comece com principal implementando e revisores lendo. Veja
[AGENTES_E_CHATS.md](docs/AGENTES_E_CHATS.md). Se dois trabalhos alterarem código,
use worktrees e bancos/portas distintos; não alterne branches na pasta em que outro
agente está executando. Cada tarefa independente ainda precisa de revisão e integração.

## 10. Quando retomar após uma pausa ou limite de uso

Faça checkpoint, registre a tarefa e o último teste em `ESTADO_PROJETO.md` e use
[99_RETOMAR.md](prompts/99_RETOMAR.md). Não refaça a fundação nem inicie outra arquitetura.
Limites da conta podem variar; mais subagentes não aumentam sua franquia.

## 11. Antes de publicar

Use [OPERACAO.md](docs/OPERACAO.md) e [ENTREGA_E_CORRETIVAS.md](docs/ENTREGA_E_CORRETIVAS.md).
Integrações reais, restauração, autorização e ausência de adaptadores fictícios são critérios
de saída. Homologação deve ter dados e credenciais separados da produção.
