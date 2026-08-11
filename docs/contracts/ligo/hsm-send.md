# Contrato Congelado: Ligo Template HSM Send

## Endpoint
`POST https://apiwhatsapp.messaging.digitalcontact.cloud/v1/message/send` (ou `/messages/template`)

## Headers
```http
Content-Type: application/json
x-access-token: {{JWT_TOKEN}}
```

## Request Body (Array Batch)
```json
[
  {
    "id": "JRC_DISPATCH_CORRELATION_1001",
    "numberchip": "551148004100",
    "telephone": "5511999999999",
    "template": "62dd9e8329f7b06969d58d47",
    "idmedia": "630634f689573dbee01c84c5",
    "field01": "João Silva",
    "field02": "31/08/2026",
    "callbackStatus": "https://gateway-whatsapp.jrcws.cloud/api/v1/webhooks/ligo/status",
    "callbackResponses": "https://www.ligo.cloud/whatsapp/response"
  }
]
```

## Response Body (HTTP 200)
```json
[
  {
    "id": "JRC_DISPATCH_CORRELATION_1001",
    "telephone": "5511999999999",
    "status": "ACCEPTED",
    "providerMessageId": "wamid.HBgLNTUxMTk5OTk5OTk5O..."
  }
]
```
