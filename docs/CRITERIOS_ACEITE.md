# Critérios de aceite por fluxo

Registrar PASSOU/FALHOU/BLOQUEADO/NÃO EXECUTADO, comando ou passos, data, commit e
ambiente. Este arquivo não afirma que testes já foram realizados na aplicação.

| ID | Cenário obrigatório | Entrega |
|---|---|---|
| C01 | Checkout pendente/retorno visual não ativa conta nem concede direito | T07 |
| C02 | Confirmação repetida não duplica compra/direito; convite reemitido preserva compra | T07 |
| C03 | Revogação da compra única suspende I/II sem apagar saldo/respostas | T07 |
| C04 | Passar seis meses não encerra acesso ativo automaticamente | T05/T07 |
| C05 | Sessão anterior perde acesso após novo login, inclusive concorrente; logout e redefinição válida da senha revogam; só solicitar recuperação não revoga | T02 |
| C06 | Aluno A/anônimo não consulta recursos privados de B; nenhum admin criado por request de aluno | T02/T03 |
| C07 | Relato vazio/acima de 1000 rejeitado; sucesso limpa, erro preserva; sem pontos | T03 |
| C08 | Aluno não lista os próprios relatos; admins veem autor/data/texto | T03 |
| C09 | Retorno, iframe load ou mensagem forjada não concede conclusão/pontos | T04 |
| C10 | Google verificado e vinculado associa resposta ao aluno correto; e-mail digitado não basta | T04 |
| C11 | Mesma resposta/atividade reprocessada ou nova versão não duplica pontos | T04/T05 |
| C12 | Formulário editado incompatível não usa configuração antiga silenciosamente | T04/T10 |
| C13 | Day usa instante de envio e dia de Brasília; 23h59 válido processado depois; dia errado não pontua; provar restrição de envio antes/depois do dia por link direto Google, registrando bloqueio se inviável | T04/T05 |
| C14 | Dois Forms Day na mesma data não concedem duas pontuações | T05 |
| C15 | Inicial incompleto aplica gate aprovado; acompanhamento não libera antes do prazo definido | T04 |
| C16 | 199 pontos não libera II; 200 libera sem compra/Mediação/acompanhamento extras | T05 |
| C17 | Duas transações simultâneas do mesmo evento geram um só lançamento | T05 |
| C18 | Saltos/repetição não aumentam área assistida; 39,9% não conclui, 40% válido conclui uma vez | T09 |
| C19 | Clique live e gravação são eventos separados; clique não chamado de presença real | T06/T09 |
| C20 | Despublicar/excluir aula preserva pontos/histórico; módulo com histórico protegido | T06 |
| C21 | Admin aloca respeitando vagas, inclusive simultaneamente; aluno não se autoaloca | T06 |
| C22 | Upload exige admin e limites; processamento/erro sem expor credencial mestre | T08/T09 |
| C23 | Vetores oficiais do 3FM produzem exatamente faixas, arredondamentos, empates e textos oficiais | T10 |
| C24 | Nova configuração 3FM não altera resultado histórico; parâmetros inválidos não publicam | T10 |
| C25 | Todas respostas importadas só são consultadas por perfis permitidos | T04/T10 |
| C26 | Home exibe só saldo próprio e explicação; ranking restrito ao admin | T05/T11 |
| C27 | Production rejeita fake/bypass; segredos e dados privados ausentes do bundle/logs/repositório | T12 |
| C28 | Restauração de backup testada em ambiente isolado; publicação/rollback documentados | T12/T13 |
| C29 | Fluxos responsivos, teclado, labels e mensagens de falha avaliados em navegadores acordados | T11/T12 |

| C30 | Primeiro acesso vence após dois anos da ativação, bloqueado também no backend; URL/token de conteúdo não ultrapassa vencimento; testar links já emitidos no vencimento/suspensão; preservar histórico e caminho de renovação | T07/T08/T09/T12 |
| C31 | Avisos 60 dias antes e no vencimento, por e-mail e plataforma; retry não duplica; reinício não perde tarefas | T08/T12 |
| C32 | Preço só admin autorizado altera; aluno vê valor antes de pagar; confirmação repetida não duplica renovação | T07/T12 |
| C33 | Renovação concede dois anos conforme regra P19 aprovada; testar antecipada/tardia, fuso e limites do calendário | T07/T12 |

Teste de limite numérico usa política definida de precisão/contagem; não depender apenas de
arredondamento do frontend. E2E deve cobrir pelo menos fluxo aluno e admin com backend real.
Integração Forms/pagamento fake só valida contrato/domínio, não prova o serviço real.
