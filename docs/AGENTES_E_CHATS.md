# Organização de chats e agentes

## Padrão recomendado para este projeto

Um chat por entrega, principal responsável pelo fluxo completo e dois revisores pontuais.
Não criar seis chats permanentes com decisões independentes. Nome: `Reconexao | T03 | Relatos`.
Esta conversa de coordenação decide regras; o repositório registra o que os chats devem seguir.
Chats do navegador não editam automaticamente sua pasta local: use o cliente local conectado.

| Papel | Onde atua | Pode escrever? |
|---|---|---|
| Principal | Contratos, implementação e integração | Sim, na branch da tarefa |
| seguranca | Autorizações, identidade, pagamentos e privacidade | Não por padrão; devolve achados |
| qa | Critérios, casos-limite, evidências e regressão | Não por padrão; executa verificações existentes |
| pesquisador | Documentação oficial de API/versões e viabilidade | Não por padrão |

Os três perfis estão em `.codex/agents/*.toml`, sem modelo fixo. Configuração atual
documentada usa name/description/developer_instructions; confira compatibilidade do
cliente instalado. Instrução de não editar é comportamental, não sandbox separado.
Modelos/cotas vêm da conta e da sessão principal; nenhum perfil concede ferramentas extras.

## Ativação e verificação

1. Abra a raiz do projeto no Codex local e revise/confie na pasta conforme política do cliente.
2. O `.codex/config.toml` do kit limita a dois subagentes simultâneos e não muda permissões.
3. Peça: “Use o agente seguranca para ler AGENTS.md e retornar suas três verificações
   prioritárias, sem editar arquivos”. Confirme a atividade e o nome do agente no cliente.
4. Se não reconhecer, confira versão/documentação oficial. Atualize a configuração num
   commit separado; não tente contornar permissões ou inventar execução de subagente.
5. Fallback: novo chat de revisão ou revisão sequencial com os mesmos critérios.

## Quando dividir escrita

Depois de contrato de API definido, frontend/backend podem ser separados. Reserve arquivos
e use worktrees (ou diretórios realmente isolados). Não permitir dois agentes alterarem
a mesma migração, arquivo de dependências ou ledger ao mesmo tempo. Principal integra
e roda os testes do resultado integrado, não apenas os testes isolados de cada agente.

Exemplo PowerShell, apenas a partir de `main` limpa e atualizada:

```powershell
git worktree add -b feature/t06-catalogo ../ReconexaoApp-t06 main
code ../ReconexaoApp-t06
```

Use outro banco/schema descartável, portas e variáveis locais no worktree. Ao concluir,
commit/PR e revisão; não remover worktree com alterações não salvas. Integração pelo PR
aprovado ou cherry-pick autorizado, nunca copiando pastas manualmente sobre trabalho ativo.

## Consumo e qualidade

Use subagentes para partes independentes; não delegue trivialidades. Principal recebe
resumos com arquivo/símbolo, impacto e passos de reprodução. Não gastar contexto com
logs enormes. Manter as referências em arquivo e checkpoints permite retomar sem repetir.
Configuração de modelos não é necessária para começar; escolha os disponíveis na conta.
