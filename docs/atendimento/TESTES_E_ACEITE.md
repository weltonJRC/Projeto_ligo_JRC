# Testes, evidências e critérios de aceite

## Níveis de evidência

1. Estática: JSON válido, referências/conexões e contratos.
2. Local: execução isolada de lógica com fixtures.
3. Integração: resultado real do sistema externo em ambiente autorizado.
4. Ponta a ponta: entrada Ligo, n8n, efeito externo, retorno e estado.
5. Produção: versão publicada comprovada e teste controlado.

Um nível não substitui os seguintes. Registre separadamente.

## Resultado observado em 19/10/2026

Duas suites locais da correção anterior foram reexecutadas fora deste repositório:
- test_correcoes.js: 25/25;
- test_regression.js: 25/25.

Entre os casos estavam validação de analista/grupo, rejeição sem protocolo/SLA/empresa consistente, troca para suporte, guards humanos, triagem e confirmação de todas as cobranças.
Não foram testadas APIs reais nesta revisão. Os scripts e exports não estão neste repositório; não há comando n8n reproduzível aqui até importar artefatos sanitizados.

Gateway: testes C# não executados; dotnet indisponível no ambiente de revisão.
Comandos de reprodução estão no README, sem promessa de aprovação.

## Matriz mínima

| IDs | Casos | Resultado esperado |
| --- | --- | --- |
| REC-01/02 | START vazio; saudação | Boas-vindas uma vez; sessão estável |
| REC-03/04/05 | Suporte; financeiro; comercial | Agente/destino correto |
| ID-01 | Empresa encontrada e confirmada | Identidade confirmada |
| ID-02/03/04 | Inválido; não encontrada; múltiplas | Nenhuma identidade inventada |
| ID-05/06 | Rejeitar cadastro; trocar empresa | Confirmação/seleção anterior inválida |
| ID-07 | Duas conversas simultâneas | Sem vazamento de contexto |
| SUP-01/02 | Ramal inicial; resposta numérica contextual | Ramal preservado sem repetir |
| SUP-03 | Impacto/testes já descritos | Não perguntar o mesmo; sem invenção |
| SUP-04 | Sim/não à persistência | Ramo coerente com resposta |
| SUP-05/06 | Grupo válido; analista válido | Protocolo confirmado conforme regra |
| SUP-07 | Empresa divergente/protocolo ausente | Não afirmar confirmação indevida |
| SUP-08 | Registro criado, roteamento falho | Preservar ID; não criar outro |
| SUP-09/10 | Timeout após POST; mensagem repetida | Reconciliar e não duplicar |
| FIN-01 | Uma cobrança | Confirmar e entregar resultado real |
| FIN-02/03 | Várias; todas | Conjunto exato e paginação considerada |
| FIN-04/05 | Nenhuma elegível; seleção inválida | Explicar sem operação fictícia |
| FIN-06 | Cancelamento | Nenhuma escrita |
| FIN-07 | Sucesso parcial | Identificar concluídas/falhas |
| FIN-08 | Repetir confirmação | Não repetir operação concluída |
| FIN-09 | Trocar empresa | Não usar cobrança anterior |
| ROT-01/02 | Financeiro→Suporte; Suporte→Financeiro | Identidade preservada; pendências limpas |
| ROT-03/04 | Financeiro→Comercial; Suporte→Comercial | Mocchi sem loop |
| ROT-05/06/07 | Negação; ambiguidade; “sim” | Interpretação contextual |
| END-01/02 | Menu; sair/encerrar/Encerra/pode encerrar | Estado correto |
| END-03 | “não quero encerrar” | Não encerrar |
| HUM-01/02 | Suporte/financeiro humano | Fila e resumo corretos |
| HUM-03/04 | Humano ativo; retry handoff | Bot bloqueado; sem duplicação |
| INF-01/02/03 | IA, BTV ou KSYS indisponível | Contingência sem sucesso fictício |
| INF-04 | Falha de persistência após efeito | Preservar/reconciliar resultado |
| LIG-01/02 | Uma/duas mensagens | Sem duplicação e próxima entrada capturada |
| LIG-03 | IDs/flags/erros | Mapeamento consistente |
| LIG-04 | Áudio adiado | Bloco incompleto não interfere na entrega textual |

## Roteiro manual por caso

Registrar em ambiente privado autorizado:
- identificador do caso;
- versão/hash n8n e versão publicada Ligo;
- horário/fuso e ambiente;
- texto sintético enviado;
- resposta esperada e obtida;
- agente, estado e identidade antes/depois;
- IDs técnicos de execução/efeito;
- request/response sanitizados;
- número de efeitos externos;
- aprovado/reprovado e motivo.

Não publicar conversa bruta, documentos financeiros ou contatos.

## Critérios para produção

- Base e referências identificadas.
- BOT-02/04/07/08/09/11/12/13 sem falha crítica.
- Nenhum falso protocolo ou repetição de criação.
- Nenhuma mistura de clientes.
- Troca de áreas e encerramento comprovados.
- Todas as opções financeiras ofertadas realmente funcionam.
- Contingência humana funcional.
- Áudio incompleto fora do caminho.
- Rollback e backups prontos.
- Aprovação de implantação e teste controlado registrados.

Se alguma função não for homologada, não a anunciar ao cliente. Redução de escopo deve ser decisão explícita do responsável.
