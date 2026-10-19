# Contexto e histórico

## Papéis

| Parte | Responsabilidade |
| --- | --- |
| Jade | Recepção, intenção e direcionamento |
| Help | Triagem técnica e integração KSYS |
| BemTeVi / BTV | Cadastro, cobranças e segunda via |
| Mocchi | Comercial, por outro bot |
| Ligo | Canal, formulários, respostas, rotas e atendimento humano |
| n8n | Orquestração, regras, integrações e persistência |
| Redis | Contexto e controles de turno/ação relatados |
| Gateway C# | Campanhas HSM Ligo/Sytel; frente distinta |

## Fases da conversa anterior

1. Investigar consulta por CNPJ e abertura de chamado em nome de qualquer cliente.
2. Confirmar consulta de duas empresas cadastradas; isso não valida a base inteira.
3. Decidir manter o modelo existente se vinculação direta ao cliente não for possível; substituir Lucas por Ronald, código 3327.
4. Validar segunda via individual e solicitar seleção múltipla / todas.
5. Identificar aprisionamento no financeiro após novo pedido de suporte.
6. Identificar chamado criado no KSYS sem protocolo apresentado pelo bot.
7. Preparar correção local de quatro nós em 05/10; instalação não comprovada.
8. Investigar áudio sem concluir captura/transcrição; usuário adia funcionalidade.
9. Em 19/10, revisar material local e tentar auditoria online. Login n8n falhou; inspeção Ligo não foi concluída. Nenhuma configuração foi alterada nessa tentativa.
10. Transferir continuidade ao Codex e documentar este repositório.

O usuário lembrava de uma base funcional entre 21 e 22/09. Essa lembrança não identifica um arquivo ou commit; procurar evidência antes de escolher baseline.

## Incidentes históricos

### E-SUP-01 — criado no KSYS, resposta de falha na Jade

Retorno relatado:
- POST 201 e consulta 200;
- created, verified, record_verified e operational_verified verdadeiros;
- status CREATED_AND_OPERATIONALLY_VERIFIED;
- empresa operadora 3;
- usuário 3327;
- grupo null;
- analista 3269;
- routing ANALYST;
- group_verified=false;
- analyst_verified=true;
- routing_verified=true;
- sla_verified=true;
- confirmação POST_RESPONSE_CONFIRMED.

A Jade indicou TICKET_GROUP_NOT_VERIFIED e omitiu protocolo. A análise anterior encontrou exigência de grupo apesar de analista verificado. Confirmar esse caminho no export atual.

Contagens before/after eram ambas 200, mas havia ID novo identificado. Uma listagem limitada/paginada não pode ser usada isoladamente para decidir se houve criação.

Protocolo do atendimento Ligo é diferente do ID do chamado KSYS.

### E-SUP-02 — triagem repetida

Cliente informou um ramal; bot perguntou novamente; após outro número, o bot voltou ao primeiro. Em outro teste, cliente descreveu falha em ligações e testes com aparelhos/internet, confirmou persistência e recebeu novamente a pergunta sobre impacto e testes.

Preservar relato, interpretar respostas no contexto e esclarecer conflito real; não inventar testes.

### E-FIN-01 — segunda via individual

Uma cobrança de teste foi consultada, confirmada e recebeu linha digitável. Não comprova todas as cobranças, PDF, seleção múltipla ou elegibilidade de toda a base.

### E-ROTA-01 — financeiro não reconhece suporte

Depois da segunda via, “estou com problema no ramal” e “quero falar com suporte” receberam repetidamente “Posso ajudar com mais alguma coisa?”. Pedido genérico de atendente foi encaminhado ao financeiro.

### E-END-01 — “Encerra”

Mensagem classificada como OUTRO, gerando pedido de esclarecimento. Forma curta de encerramento não reconhecida no caminho testado.

### E-IA-01 — indisponibilidade

Foi observado “No deployments available for selected model, Try again in 10 seconds”. Resolução não confirmada. Baseline relatado GPT-4.1 em gateway próprio; não trocar modelo sem avaliação.

### E-HUM-01 — transferência

Atendimentos chegaram a humanos. Isso não comprova todos os campos do contexto nem bloqueio completo do bot.

## Artefatos históricos

| Artefato | Versão relatada | Situação neste repositório |
| --- | --- | --- |
| Orquestrador Jade + Help + Financeiro | v1.1.13 HML CONTROLADO MULTIEMPRESA | Export ausente |
| CORE BTV cliente/financeiro | v3.9 PRODUCAO MULTIEMPRESA FAIL CLOSED | Export ausente |
| CORE KSYS suporte | v3.13 PRODUCAO MULTIEMPRESA FAIL CLOSED | Export ausente |
| JADE_v1_1_13_CORRIGIDA_20261005.json | Correção de 05/10 | Disponível apenas na sessão local; não importado aqui |
| JRC_CORRECOES_20261005.zip | JSON, instruções, validação e fontes | Não importado aqui |

Nomes semelhantes com “(1)” não identificam versão. Comparar hash e conteúdo.
Quantidades relatadas: Jade 62 nós, KSYS 52 e BTV 38; não são requisitos.

## Correção de 05/10 relatada e testada localmente

| Nó | Ajuste |
| --- | --- |
| CORE - Validar KSYS CREATE_TICKET Help | Grupo OU analista verificado com código positivo; mantém sucesso, criação, protocolo, empresa, verificação e SLA; erro de roteamento TICKET_ROUTING_NOT_VERIFIED |
| CORE - Policy Gate Help | Captura ramal contextual, preserva relato em support.tests.relato_cliente, reconhece persistência e evita repetição |
| CORE - Preparar KSYS CHECK_DUPLICATE Help | Inclui ramal/linha afetada na descrição usada no fluxo |
| CORE - Controlar takeover humano | Troca clara Help/Financeiro após guards; preserva identidade/ledger; limpa autorização financeira pendente |

Limites:
- Comercial a partir de Help/Financeiro não estava comprovadamente coberto.
- “Encerra” ainda precisava de correção.
- Export corrigido tinha active=false; isso não informa estado do ambiente.
- Duas suites de 25 checks aprovadas não validam integração real.
