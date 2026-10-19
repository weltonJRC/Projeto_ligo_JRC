# Contrato Congelado: Ligo Authentication

## Endpoint
`POST https://api.messaging.digitalcontact.cloud/auth/login`

## Headers
```http
Content-Type: application/json
```

## Request Body
```json
{
  "login": "EMPRESARIAL_LOGIN",
  "password": "EMPRESARIAL_PASSWORD"
}
```

## Response Body (HTTP 200)
```json
{
  "token": "<JWT_RETORNADO_PELA_LIGO>",
  "validate": "10-12-70T09:41:00.123Z"
}
```

## Header de Autenticação para Demais Endpoints
```http
x-access-token: {{token}}
```

## Limite de validação

Este exemplo não comprova o contrato vigente do fornecedor. Confirmar endpoint e campos antes de modo Real. Token ilustrativo anterior foi substituído por placeholder em 19/10/2026; o histórico Git não foi reescrito.
