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
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYyZGQ5YmJjMjlmN2IwNjk2OWQ1OGQzZSIsImlhdCI6MTY2MTI2NDg2NSwiZXhwIjoxNjYxMzUxMjY1fQ.qE0m72GZTEcMKPcAbvTMeAmrVWY0CnGREvqA2NcmFWU",
  "validate": "10-12-70T09:41:00.123Z"
}
```

## Header de Autenticação para Demais Endpoints
```http
x-access-token: {{token}}
```
