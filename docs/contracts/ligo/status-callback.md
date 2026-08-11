# Contrato Congelado: Ligo Status Webhook Callback

## Endpoint no Middleware JRC
`POST /api/v1/webhooks/ligo/status`

## Incoming Request Body (JSON)
```json
{
  "id": "JRC_DISPATCH_CORRELATION_1001",
  "numberchip": "551148004100",
  "telephone": "5511999999999",
  "date": "2026-08-04T12:00:00.000Z",
  "status": "3 - READ",
  "type": "WhatsappStatus",
  "conversationCategory": "MARKETING",
  "errors": [
    {
      "code": 45003,
      "title": "O número de telefone não é válido ou não está registrado no WhatsApp Business"
    }
  ]
}
```

## Normalização de Status no Middleware
- `"1 - QUEUED"` / `"SENT"` -> `Sent`
- `"2 - DELIVERED"` -> `Delivered`
- `"3 - READ"` -> `Read`
- Com erro ou `"FAILED"` -> `FailedFinal`
