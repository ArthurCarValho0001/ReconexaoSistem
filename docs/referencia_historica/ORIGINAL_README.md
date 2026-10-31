> REFERÊNCIA HISTÓRICA DE 18/10/2026. Não usar como instrução vigente.
> Consulte ../DECISOES.md para alterações posteriores.

# Reconexão — Fonte documental do projeto

Este pacote consolida o escopo funcional e as regras de negócio conhecidas para **Reconexão I e Reconexão II**. Reconexão III não faz parte da primeira entrega.

## Status
- **Aprovado:** requisitos explicitamente consolidados nas conversas com o cliente/desenvolvedor.
- **Proposto:** desenho técnico recomendado, ainda não aprovado.
- **Pendente:** detalhe que não deve ser inventado pelo agente.

## Como começar
1. Criar um novo Projeto no ChatGPT chamado `Reconexão — Desenvolvimento`.
2. Inserir as instruções gerais do Projeto e anexar os dez arquivos Markdown como fontes.
3. Abrir conversas especializadas (Coordenador, Frontend, Backend, Dados, Segurança/QA, Integrações/DevOps e Protótipo).
4. Começar pelo Coordenador: revisar documentos, confirmar lacunas, propor arquitetura e backlog.
5. Atualizar o protótipo GitHub Pages **em branch separada**, sem incluir dados reais.

## Índice
- `AGENTS.md`: instruções comuns para agentes.
- `docs/REQUISITOS.md`: requisitos funcionais e não funcionais.
- `docs/REGRAS_NEGOCIO.md`: regras aprovadas e eventos de pontuação.
- `docs/ESCOPO.md`: dentro/fora do escopo e pendências.
- `docs/ARQUITETURA.md`: proposta de componentes e decisões em aberto.
- `docs/MODELO_DADOS.md`: modelo lógico preliminar.
- `docs/INTEGRACOES.md`: integrações externas e restrições.
- `docs/CRITERIOS_ACEITE.md`: cenários verificáveis.
- `docs/PLANO_DESENVOLVIMENTO.md`: fases, dependências e entregas.

## Repositório demonstrativo existente
- GitHub: https://github.com/ArthurCarValho0001/Reconexao
- Pages: https://arthurcarvalho0001.github.io/Reconexao/
- Estrutura conhecida: `index.html`, `styles.css`, `app.js`, `README.md`, `.nojekyll`.
- É um **protótipo estático**, com dados fictícios e ações não persistidas após recarregar; não há login, pagamentos, banco ou vídeo reais.

## Aviso sobre o 3FM
O cliente informou que a associação das cinco dimensões aos quatro pilares e as fórmulas **já foram definidas**. Entretanto, o texto integral dessas fórmulas e das 40 perguntas **não está reproduzido neste pacote**. Anexar a especificação original do 3FM como fonte complementar **antes de implementar os cálculos**. Não extrapolar.

## Atualização de regras — outubro de 2026
- Reconexão I: seis meses de acesso inicial; quem não progrediu ao II e não renovou perde acesso ao programa e o direito à comunidade.
- Comunidade exclusivamente via WhatsApp externo; remoção de membros após expiração exige processo a definir.
- Sem encontros presenciais; turmas com vagas limitadas e aulas ao vivo/gravações.
- Aulas gravadas publicadas disponíveis desde o início; referência de oito aulas por módulo, quantidade editável pelo administrador.
- Descontos e validade de planos/ofertas editáveis pelo administrador; regras comerciais detalhadas pendentes.
- Hospedagem ainda não escolhida: os agentes avaliarão alternativas e o responsável aprovará a escolha.
