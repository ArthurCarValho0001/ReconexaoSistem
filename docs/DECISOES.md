# Decisões consolidadas e substituições

Fonte: ZIP de 18/10/2026 e respostas explícitas de Arthur nesta conversa, até 31/10/2026.
Não houve inspeção do repositório atual nem demonstração de implementação.
APROVADO abaixo significa decisão informada por Arthur; não presume ata separada do cliente.

| ID | Decisão atual | Substitui / observação |
|---|---|---|
| D01 | Compra inicial parcelável e renovação paga a cada dois anos | Não implementar débito recorrente automático sem aprovação |
| D02 | Acesso por dois anos desde a liberação/ativação, com mínimo de seis meses garantidos | Substitui acesso enquanto existir/vitalício; expiração não ocorre aos seis meses; garantia em encerramento é P02 |
| D03 | Marco inicial na ativação da conta | Não usar confirmação do pagamento como início do primeiro período de acesso |
| D04 | Pagar antes; ativação por convite; convite reemitível sem perder compra | Compra com e-mail já existente precisa de fluxo técnico |
| D05 | II libera aos 200 pontos, sem novo pagamento nem outros pré-requisitos específicos de progressão | Substitui RF-061/062/063 e RN-020/021/022 no que exigiam compra/atividades; inicial ainda é obrigatório na entrada |
| D06 | Reversão da compra inicial sem direitos independentes suspende I e II; histórico preservado | Manter distinção entre acesso e saldo |
| D07 | Liberação manual concede mesma duração e progressão, sem checkout | Não autoriza pontos manuais nem II sem pontos |
| D08 | Todos os questionários no Google Forms, inclusive 3FM e Day | Cadastro de atividade e resultados no Reconexão; edição de perguntas no Google |
| D09 | Conta Google verificada vinculada ao aluno; todas as respostas importadas | Não confiar em e-mail digitado ou retorno visual |
| D10 | Formulário respondido muda por nova versão; mesma atividade não pontua de novo | Preservar questões/alternativas e versão original de cada submissão |
| D11 | Novos formulários comuns: 10 pontos por conclusão, uma vez por aluno/atividade | Não deixar valor dos pontos editável pelo admin sem nova aprovação |
| D12 | Day: cinco perguntas variáveis por dia; somente no dia em Brasília; sem bloqueio por ausência | Proposta anterior de Day atrasado bloquear não vigora |
| D13 | Inicial obrigatório na entrada; acompanhamento 30 dias após inicial | Escopo exato das rotas bloqueadas ainda pendente |
| D14 | 3FM: perguntas editáveis no Forms; resultados dentro do Reconexão; configuração de vínculos e parâmetros da fórmula oficial | Substitui exclusão total do editor em RF-049; não é editor de fórmulas livres; material ainda será enviado |
| D15 | Arthur inclui configuração parametrizada do 3FM no orçamento e prazo atuais | Não inventar detalhes de cálculo para cumprir prazo |
| D16 | Todos os administradores consultam relatos, respostas e resultados | Day privado em relação a outros alunos, não oculto dos admins pela decisão atual |
| D17 | Upload de vídeos pelo painel Reconexão | Arquivos enviados ao fornecedor, ainda não escolhido |
| D18 | Vídeo concluído +10 pontos aos 40% de trechos válidos únicos | Substitui 90% em requisitos, regras, arquitetura e aceite antigos |
| D19 | Admin distribui alunos nas turmas; limite de vagas | Substitui proposta de autoinscrição do aluno; cancelamento/realocação pendentes |
| D20 | Novo login encerra sessão anterior | Verificação no servidor nas requisições subsequentes |
| D21 | Saldo próprio/barra na Home; ranking só admin | Não divulgar dados de outros alunos |
| D22 | Relatos de até 1.000 caracteres, sem pontos, histórico do aluno ou resposta admin | Admin vê autor, texto e data/hora |
| D23 | Fornecedores não escolhidos; retenção precisa de orientação | Não tratar sugestões anteriores como contratação |

| D24 | Avisos por e-mail e na plataforma 60 dias antes e no vencimento | Não adicionar outros prazos como requisito aprovado |
| D25 | Vencimento bloqueia conteúdos; renovação confirmada concede mais dois anos | Preservar pontos, respostas, resultados e histórico; acesso ao caminho de renovação permanece |
| D26 | Preço de renovação configurável pelo admin e informado antes do pagamento | Registrar valor do pedido; política de mudança de preço em checkout aberto é P20 |

Arquitetura ASP.NET Core/React/PostgreSQL foi proposta, não aprovada explicitamente.
Prevalência: nova instrução explícita registrada > decisões atuais > requisito consolidado;
conflitos novos devem ser relatados. Material do cliente será referência literal dos cálculos,
não autorização implícita para mudar acesso ou pontuação.
