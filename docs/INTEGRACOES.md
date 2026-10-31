# Integrações: estratégia e fronteiras

## Google Forms (aprovado como abordagem, ainda não homologado)

O admin edita perguntas no Google; a plataforma cadastra atividade e versão. Ela importa
todas as respostas, associa aluno pela identidade Google verificada e registra conclusão
e pontos só após validação de servidor. Voltar à página ou um iframe carregar não é prova.

T04 compara Apps Script com gatilho e comunicação autenticada versus leitura autenticada
da API com consulta/reconciliação. Não configurar um gatilho/projeto por formulário
indefinidamente: verificar cotas, número de formulários/dias e operação da conta proprietária.
O cadastro de um formulário novo deve ter onboarding claro e estado de integração validado.

Proposta de webhook: segredo somente servidor/script, assinatura da mensagem e controle
de repetição, com ID externo único, timestamp e esquema validado. Uma URL secreta sem
validação não basta. Um ID preenchido no Forms não autentica aluno. E-mail coletado
precisa ser verificado, não resposta textual. Nunca enviar access token ao frontend.

Separar data de submissão, chegada e processamento. Retry/reconciliação devem recuperar
envio após falha de rede sem duplicar pontos. Atualização de respostas deve preservar
versões; limitar edição no Google não substitui validação do servidor. Item alterado na
estrutura exige nova versão e nova validação de configuração.

Para desenvolver localmente, uma fixture autenticada de teste valida domínio, mas não a
integração. Para prova real usar conta de teste, formulário sem dados pessoais reais e
HTTPS alcançável para callback OU consulta API sem callback; escolher após avaliação.
Retomada do Google, múltiplos logins, e-mail diferente e formulário despublicado são testes.

## Pagamentos

Um checkout externo a escolher, compra única parcelável, confirmação confiável, ativação
e reversão. Adapter fake local não libera compras reais. Não escolher assinatura para
representar parcelamento. Consultar estado confiável, conferir valor/moeda/pedido,
guardar idempotência e processar notificações fora de ordem. Estorno parcial é pendência.

## Vídeos e e-mail

Admin envia vídeo pelo Reconexão ao fornecedor, acompanha processamento e erros. Aluno
reproduz com acesso autorizado; medir 40% por intervalos e tempo plausível, sem alegar
antifraude absoluto. E-mail: ativação, reemissão e recuperação com fila/retry e tokens
protegidos. Mensagens de erro não enumeram contas. Fornecedores ainda não escolhidos.

## Links

Meet/Zoom/WhatsApp são links externos. Clique pode gerar evento aprovado, não comprova
participação real nem remove membro. Reuniões não dependem de API do Meet/Zoom neste escopo.

## Validade e renovação

Verificar validade no servidor em todos os acessos protegidos; ao vencer, bloquear conteúdos e manter caminho de renovação. Checkout deve distinguir compra inicial/renovação e validar confirmação idempotente. Agendar avisos persistentes 60 dias antes e no vencimento por e-mail e plataforma, com retry, reconciliação e alertas. Não depender de visita do aluno para enviar. Guardar preço do pedido e períodos anteriores. P19–P22 bloqueiam os comportamentos ainda indefinidos.
