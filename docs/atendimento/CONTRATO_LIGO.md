# Contrato histórico Ligo ↔ n8n

Referência para comparação com configuração real; não é schema homologado atual. Não alterar campos ou tipos sem revisar os dois lados.

## Entrada observada

```json
{
  "event_type": "$$jrc_event_type$$",
  "conversation_id": "$$jrc_conversation_id$$",
  "message_id": "$$jrc_message_id$$",
  "mensagem": "$$jrc_entrada_usuario$$",
  "telefone": "$$global.contactnumber$$",
  "origem": "LIGO",
  "canal": "WHATSAPP",
  "contexto_fluxo": "RECEPCAO_JADE",
  "current_agent_ligo": "$$jrc_current_agent$$",
  "estado_jornada_ligo": "$$jrc_estado_jornada$$"
}
```

Validar a serialização de aspas/quebras de linha nos blocos reais. O exemplo não é uma instrução para concatenar texto sem escape.

Não havia campos de mídia. Variável criada manualmente não significa que exista evento nativo correspondente.

## Geração do turno

Script histórico:
```javascript
const entrada = String("$$jrc_entrada_usuario$$" || "").trim();
const messageId = "msg-" + Date.now() + "-" +
  Math.random().toString(36).slice(2, 10);
const eventType = entrada ? "MESSAGE" : "START";
```

O bloco executava done({ updateVariables: [...] }) para gravar ID/tipo e limpar retornos.
Não usar esse fragmento como substituto do script completo. Conferir:
- entrada antiga e placeholder não resolvido;
- START indevido;
- conversation_id estável;
- retry gerando outro message_id;
- preservação dos dados necessários à rota seguinte.

## Saída e mapeamento

| Variável Ligo | Campo retornado |
| --- | --- |
| jrc_ok_ligo | ok_ligo |
| jrc_mensagem | mensagem |
| jrc_mensagem_2 | mensagem_2 |
| jrc_tem_mensagem_2 | tem_mensagem_2 |
| jrc_acao | acao |
| jrc_destino | destino |
| jrc_fila_destino | destination_ligo |
| jrc_transferir | transferir_ligo |
| jrc_ignorar | ignorar_ligo |
| jrc_encerrar | encerrar_ligo |
| jrc_estado_conversa | estado_conversa |
| jrc_estado_jornada | estado_jornada |
| jrc_current_agent | current_agent |
| jrc_intencao | intencao |
| jrc_produto | produto |
| jrc_resumo | resumo_handoff |
| jrc_confianca | confianca |
| jrc_message_id_retorno | message_id |
| jrc_conversation_id_retorno | conversation_id |
| jrc_erro | erro |
| jrc_retorno_raw | resposta completa |
| jrc_handoff_nome | handoff.nome_cliente |
| jrc_handoff_empresa | handoff.empresa_cliente |
| jrc_handoff_cnpj | handoff.cnpj |
| jrc_handoff_telefone | handoff.telefone |
| jrc_handoff_data | handoff_data_json |
| jrc_handoff_request_id | handoff_request_id |
| jrc_handoff_status | handoff_status |
| jrc_ticket_protocol | ticket_protocol |
| jrc_ticket_status | ticket_status |

Campos adicionais observados: context, skip_persistence, persistence_route, handoff.
Flags de integração usam SIM/NAO, enquanto skip_persistence pode ser booleano.
Não converter todos os campos ao mesmo tipo.

Ação observada RESPONDER; comercial usa TRANSFER_BOT no caminho relatado. Levantar enumerações completas no export; não inventar ações.

## Limpeza do retorno antes de cada chamada

O script limpava mensagens, IDs de retorno, flags, ação/destino/fila, intenção/produto/resumo/confiança, handoff, protocolo, erro e retorno bruto. Valores padrão das flags: NAO.

Também limpava:
- jrc_resumo_formatado;
- jrc_screenpop_payload_json;
- CustomerAndDestination;
- jrc_crossbot_route;
- jrc_crossbot_origin;
- jrc_crossbot_mode.

Inspecionar ordem das rotas comerciais antes de julgar se a limpeza de crossbot é correta.

## Contexto histórico — schema_version 1.1

| Grupo | Campos |
| --- | --- |
| raiz | conversation_id, state, current_agent, intent, summary, last_user_message |
| reception | original_issue, welcomed |
| routing | intent, product, subintent, confidence |
| identity | identifier_type, identifier_value, cnpj, codcliente, company, contact_name, company_confirmed, candidate, search |
| support | issue_summary, symptom, product, affected_number, impact, diagnosis_status, tests, customer_wants_ticket, ticket_status, protocol, handoff_reason |
| finance | subintent, charges, selected_charge, explicit_second_copy_request, user_confirmed, handoff_reason |
| actions | last_action, ledger |
| handoff | status, destination, destination_ligo, reason, summary |

Estados observados incluem AI_ACTIVE no contexto, BOT_ACTIVE no retorno, JADE/HELP/FINANCEIRO e handoff NONE. Não tratar a lista como exaustiva.
Preservar compatibilidade com campos adicionais, histórico e contextos antigos.

## Sequência de blocos relatada

1. JADE — Inicializar sessão.
2. COND — Horário de atendimento.
3. JADE — Gerar ID do turno.
4. N8N — Executar Jade (API v2).
5. ROTA — Resultado Jade (nove condições em uma versão).

Resposta:
- JADE — Mensagem 1;
- COND — Tem mensagem 2;
- JADE — Conversar Mensagem 2 ou JADE — Conversar;
- salvar resposta em jrc_entrada_usuario;
- retornar à geração do turno.

Comercial:
- JADE — Direcionar ao Mocchi;
- JADE — Preparar rota Mocchi;
- JADE — Transferir para Agência → Agencia JRC.

Humano:
montagem de resumo → aviso → fila → screenpop → integração IMBridge → atendimento.
Validar os nomes exatos e as conexões na configuração fornecida.

Outros caminhos: ERRO — Integração Jade, encerramento, privacidade, Trabalhe Conosco e JRC Conversas.

## Pontos pendentes na Ligo

- Precedência das condições: erro, ignorar, encerrar, humano, crossbot e resposta.
- Exibição duplicada entre texto e formulário.
- Captura da próxima mensagem em ambos os ramos.
- Atualização do agente/estado e correlação dos IDs.
- Aguardar API habilitado; timeout histórico 90 segundos.
- Validação dos formulários era “Qualquer texto”; houve teste com desativação para áudio. Estado final desconhecido.
- Bloco JADE — Coletar áudio pode estar incompleto/desconectado e afetar publicação.
- Rotas e filas devem ser verificadas em configuração real, não apenas no JSON n8n.

## Endpoint

Caminho histórico: /webhook/jrc-atendimento-ia-hybrid-v1-1-13-hml.
Variante de teste: /webhook-test/jrc-atendimento-ia-hybrid-v1-1-13-hml.

Host e URL efetiva devem ser obtidos no ambiente autorizado e no nó Webhook. Não chamar endpoints a partir desta documentação. “hml” no caminho não prova isolamento.
