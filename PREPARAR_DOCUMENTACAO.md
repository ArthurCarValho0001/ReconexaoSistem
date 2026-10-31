# Preparar a documentação no ReconexaoSistem

Pacote 1.1 — 31/10/2026. Sem código de aplicação e sem pasta .git.

1. Extraia o ZIP em uma pasta temporária, fora do clone.
2. Abra a pasta interna ReconexaoSistem extraída.
3. Copie todo o conteúdo, incluindo .codex, .gitignore e .gitattributes, para C:\Users\Arthur\Documents\GitHub\ReconexaoSistem.
4. Preserve a pasta .git existente. Não execute git init novamente. Não copie a pasta externa como uma subpasta adicional.
5. Se houver arquivos de trabalho no destino, revise conflitos antes de substituir. Este pacote foi preparado para o estado vazio informado, não para sobrescrever código posterior.
6. No PowerShell dentro do clone, execute:

```powershell
Get-ChildItem -Force | Select-Object Name, Mode
Test-Path .\AGENTS.md
Test-Path .\.codex\config.toml
Test-Path .\docs\DECISOES.md
Test-Path .\prompts\00_INICIAR.md
git status --short
```

Os quatro Test-Path devem retornar True. A pasta .git deve continuar presente.

7. Abra a pasta no VS Code, revise os arquivos e confira se não há credenciais.
8. Prepare o commit e examine a lista:

```powershell
git add .
git diff --cached --stat
git diff --cached --check
```

9. Se a lista contiver somente o kit e nenhum segredo, registre:

```powershell
git commit -m "docs: preparar kit inicial e regras de renovacao"
```

10. Após conferir que o remoto é o repositório privado correto, envie pelo GitHub Desktop ou execute git push -u origin main. Um erro de histórico remoto deve ser investigado; não use force push.
11. Abra o Codex na raiz e use prompts/00_INICIAR.md para a T00. Não inicie T01 antes de aprovar a stack.

A renovação antecipada/tardia ainda precisa de regra de contagem; veja P19–P22. Este pacote não cria repositório, não faz commit e não testa sua máquina automaticamente.
