# Contrato Congelado: Ligo Media Upload

## Endpoint
`POST https://api.messaging.digitalcontact.cloud/media/upload`

## Headers
```http
x-access-token: {{JWT_TOKEN}}
```

## Opção A: JSON Payload (via URL HTTPS Pública)
```http
Content-Type: application/json
```
```json
{
  "file": "https://midia.jrcws.cloud/public/media/78a58d57-f384-48fa-9abc-7b0ef5ff0cde"
}
```

## Opção B: Multipart Form-Data (Upload Direto - Preferencial)
```http
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary...
```
- Campo do form: `file` (stream de arquivo binário `.png` ou `.jpeg`, limite de 5MB)

## Response Body (HTTP 200)
```json
{
  "idmedia": "630634f689573dbee01c84c5",
  "validUntil": "10/12/2070"
}
```

## Notas de Implementação
- O campo `validUntil` é retornado como string de data (`DD/MM/YYYY` ou ISO 8601).
- O middleware armazena tanto o valor bruto (`validUntilRaw`) quanto o valor interpretado (`validUntilParsed`).
