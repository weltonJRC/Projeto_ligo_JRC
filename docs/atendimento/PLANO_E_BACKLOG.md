# Plano de continuidade e backlog

## Decisão de fornecimento

Em 19/10/2026, o usuário informou que enviará os JSONs diretamente no chat do Codex. Não bloquear a preparação documental nem procurar credenciais para obter os arquivos. Ao recebê-los, identificar versão/origem e comparar antes de editar.

## Fases e saídas

| Fase | Trabalho | Evidência de conclusão |
| --- | --- | --- |
| A — Base | Inventário dos anexos, hashes, refs, diferenças e backup | Manifesto local sem segredos |
| B — Diagnóstico | Rastrear entrada → estado → regra → API → retorno → Ligo | Causa demonstrada ou hipótese explicitamente limitada |
| C — Correção | Diff mínimo por nó; preservar contratos | JSONs válidos, mudanças justificadas |
| D — Regressão | Casos isolados e fixtures sintéticas | Comandos, resultados e cobertura |
| E — Homologação | Preview/canal autorizado com coleta de evidências | Matriz preenchida sem lacunas críticas |
| F — Produção | Backup, referências, credenciais, webhook, publicação e smoke test | Aprovação operacional e rollback preparado |

Não esperar a publicação para organizar rollback. Não promover automaticamente ao concluir testes locais.

## Backlog do atendimento

| ID | Prioridade | Item | Critério de conclusão | Estado |
| --- | --- | --- | --- | --- |
| BOT-01 | P0 | Recuperar exports atuais e patch 05/10 | Base identificada por hash; sem adivinhação de versão | Aguardando anexos no Codex |
| BOT-02 | P0 | Protocolo KSYS e roteamento por analista | Ticket confirmado apresentado; sem duplicação em falha posterior | Patch local anterior; integração pendente |
| BOT-03 | P1 | Repetição de triagem | Ramal, impacto e relatos preservados | Patch local anterior; integração pendente |
| BOT-04 | P0 | Financeiro ↔ Suporte | Intenção muda sem loop e sem perder identidade/ledger | Patch local anterior; integração pendente |
| BOT-05 | P1 | Saída para Comercial/Mocchi | Funciona de Jade, Help e Financeiro, sem retorno circular | Pendente |
| BOT-06 | P1 | “Encerra” e variantes | Encerramento contextual, respeita negação/humano | Pendente |
| BOT-07 | P0 | Uma/várias/todas cobranças | Conjunto confirmado, paginação e falhas parciais | Parcial local; integração pendente |
| BOT-08 | P0 | Handoff e humano ativo | Fila/contexto corretos; bot não disputa atendimento | Parcial histórico |
| BOT-09 | P0 | Modelo/gateway disponível | Erro de deployment resolvido ou contingência validada | Pendente |
| BOT-10 | P1 | Restos de áudio/formulários | Áudio fora do caminho; publicação sem bloqueios relacionados | Inspeção pendente |
| BOT-11 | P0 | Contrato Ligo e endpoint | Payload, tipos, IDs, flags e condições conferidos | Inspeção pendente |
| BOT-12 | P0 | Retry/idempotência | Reentrega e timeout não repetem efeito externo | Inspeção e regressão pendentes |
| BOT-13 | P0 | Isolamento de empresas | Cadastro confirmado e seleções não vazam entre sessões | Homologação pendente |

P0 significa bloqueio de homologação/produção da função afetada, não autorização para operação real.

## Backlog separado do gateway

Não incorporar essas tarefas automaticamente à correção da Jade.

- GW-01: reproduzir build/testes com SDK definido.
- GW-02: alinhar estratégia Docker preview/SDK após validação de imagens.
- GW-03: revisar parametrização Compose e persistência.
- GW-04: condicionar/validar seed para ambientes reais.
- GW-05: teste realista de concorrência/idempotência em PostgreSQL isolado.
- GW-06: confirmar contratos fornecedores e distinguir health de readiness.
- GW-07: revisar necessidade de saneamento do histórico de exemplos de token/credenciais; não reescrever histórico sem autorização específica.

## Registro de progresso

Em cada etapa, registrar:
data; commit/hash do artefato; tarefa; evidência; teste; resultado;
limitação; próximo passo. Nunca marcar “concluído” somente por relato de intenção.
