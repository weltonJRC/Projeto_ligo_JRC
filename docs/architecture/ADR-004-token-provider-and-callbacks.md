# ADR-004: Provedor de Token JWT e Separação de Callbacks

## Contexto
A API Ligo Messaging exige autenticação via JWT token obtido no endpoint `POST /auth/login` e injetado no header `x-access-token`. O token possui validade de 24 horas. Além disso, as mensagens enviadas exigem dois callbacks distintos: `callbackStatus` (para eventos de entrega/leitura) e `callbackResponses` (para continuidade da conversa no Ligo Bot quando o cliente responde ao HSM).

## Decisão
1. **Gerenciamento de Token (`ILigoTokenProvider`)**:
   - Manter o token JWT em memória com cache thread-safe (*single-flight refresh*).
   - Inspecionar o vencimento através do retorno do login (`validate`) ou da claim `exp` do JWT.
   - Renovar o token automaticamente minutos antes da expiração.
   - Em caso de resposta `401 Unauthorized` da Ligo, invalidar o cache e executar um único retry transparente.
2. **Separação Rígida de Callbacks**:
   - `callbackStatus`: Aponta obrigatoriamente para o Middleware JRC (`https://gateway-whatsapp.jrcws.cloud/api/v1/webhooks/ligo/status`) para auditoria e atualização de estados do disparo.
   - `callbackResponses`: Aponta para a URL do Ligo Bot / Boteria configurada no cadastro do template, garantindo que o retorno do cliente entre no fluxo conversacional correto sem passar pelo gateway de saída.
3. **Modo de Resposta do Adaptador Legado Sytel**:
   - Suportar a flag `Sytel:LegacyAlwaysHttp200` em configuração.
   - Inicialmente `true` para compatibilidade com o script legadom mas configurável para `false` no script clonado de homologação, permitindo diferenciar HTTP 200 (Accepted), HTTP 400 (Bad Request) e HTTP 500 (Internal Error).

## Consequências
- Comunicação transparente e segura com a Ligo.
- Isolamento total entre o disparo de outbound (Middleware JRC) e a jornada receptiva (Ligo Bot + IMBridge + Sytel Humano).
