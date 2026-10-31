# Modelo lógico proposto, não DDL aprovado

| Entidade | Finalidade / integridade |
|---|---|
| Usuario, Perfil | Identidade local e autorização; e-mail normalizado/validado |
| IdentidadeGoogle | Identificador estável do provedor e vínculo verificado; não só string de e-mail |
| Sessao | Sessão ativa/revogação concorrente; novas requisições verificam estado |
| Compra, EventoPagamento | Pedido externo, modalidade única, IDs de transação/notificação únicos |
| DireitoAcesso | Compra ou concessão manual, ativação, garantia mínima, início/fim de validade de dois anos, suspensão e origem; não expira em seis meses |
| ConviteAtivacao | Token protegido/expirável/uso único e reemissão sem perder compra |
| Modulo, Aula, Material | Nível I/II, ordem, publicação, histórico e exclusão controlada |
| Video, Upload, Progresso | Fornecedor/processamento e intervalos efetivamente assistidos |
| Live, Gravacao | Clique separado da gravação, cancelamento e janela de disponibilidade |
| Atividade | Identidade lógica e tipo (comum/inicial/30dias/3FM/Day), associação a aula/módulo/data |
| FormularioVersao | Google form ID, snapshot de questões/alternativas, estrutura e versão |
| Submissao, Resposta | Aluno, ID de resposta externo, instante de envio confiável, versão e estado |
| Configuracao3FM, Resultado3FM | Mapeamentos/parâmetros oficiais versionados; resultado imutável por versão |
| EventoPontuacao | Ledger por aluno/tipo/recurso; constraint única; origem auditável |
| Relato | Autor, texto, instante; sem rota de histórico do aluno |
| Turma, Alocacao | Admin aloca, capacidade protegida em concorrência |
| Aviso, Auditoria | Notificações internas e ações relevantes sem copiar conteúdo privado |
| InboxIntegracao, TentativaProcessamento | Recebimento, retry, erro sanitizado e reconciliação |

Não implementar soma de pontos como campo editável pelo cliente. Saldo deriva do ledger
ou projeção consistente. Reserva para II não é uma compra adicional. Versões de formulários
não geram atividades novas automaticamente. Preservar histórico nas exclusões/despublicações.

## Revisão antes das migrações

Definir timezone (instantes UTC e data Day derivada em Brasília), tipos/limites, FKs,
unicidade, índices para consultas reais, política de soft delete e transações. Crescimento
de JSON de respostas/intervalos deve ser avaliado; não usar banco para armazenar vídeos.
Retenção e exclusão de dados pessoais são pendentes; não configurar purge irreversível.

## Renovação e avisos — proposta técnica

Modelar pedidos inicial/renovação, valor e moeda registrados, períodos de acesso e origem de pagamento, configuração auditada do preço e notificações por aluno/período/tipo/canal com deduplicação e retry. Não sobrescrever o período anterior nem apagar histórico. Definir a regra de extensão em P19 antes das migrações definitivas.
