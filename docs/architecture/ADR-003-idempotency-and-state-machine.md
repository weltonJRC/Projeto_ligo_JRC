# ADR-003: Idempotência Transacional e Máquina de Estados

## Contexto
O Sytel ou a rede podem reexecutar requisições de envio em caso de timeout. Sem um controle transacional de idempotência, o cliente pode receber disparos duplicados no WhatsApp. Além disso, a checagem simples (select antes de insert) é suscetível a condições de corrida (*race conditions*) quando duas chamadas paralelas chegam para o mesmo `recordId`.

## Decisão
1. **Chave Única de Idempotência**:
   - Formato: `grupojrc:{campaign}:{recordId}:{templateId}`.
   - Criar constraint única no PostgreSQL sobre a combinação `(tenant, campaign, record_id, template_id)`.
2. **Máquina de Estados de Disparo (`DispatchState`)**:
   - `Received` -> Registrado na entrada.
   - `Preparing` -> Validando parâmetros e resolvendo mídia.
   - `Submitting` -> Enviando para a API Ligo.
   - `Accepted` -> Ligo confirmou o aceite (HTTP 200 / status ACCEPTED).
   - `Sent` / `Delivered` / `Read` -> Atualizados via Webhook de Status.
   - `FailedTransient` -> Erro de rede/rate limit (elegível a retry com backoff).
   - `FailedFinal` -> Erro funcional (4xx) ou exaustão de tentativas.
   - `Unknown` -> Timeout durante o aceite (exige reconciliação por status).
3. **Comportamento em Repetição**:
   - Se a chave já existe no estado `Accepted`, `Sent`, `Delivered` ou `Read`: Retornar `ALREADY_ACCEPTED` sem re-disparar.
   - Se estiver em `Submitting`: Retornar `PROCESSING` (evita concorrência).
4. **Obrigatoriedade do `recordId`**:
   - O parâmetro `recordId` passa a ser obrigatório no adaptador Sytel (`&recordId=${cam:id}`).

## Consequências
- Zero disparos duplicados no WhatsApp.
- Resiliência contra retentativas do discador Sytel.
- Rastreabilidade ponta a ponta do ciclo de vida da mensagem.
