# Contrato Congelado: Sytel Legacy HTTP GET

## Endpoint no Middleware JRC
`GET /softdial/Whats_New_API_whatsapp_ativo/SendWhatsAppOutboundTemplate`

## Query Parameters
```http
GET /softdial/Whats_New_API_whatsapp_ativo/SendWhatsAppOutboundTemplate
    ?numberchip=551148004100
    &template=65f9dbce1fb4e9ac773bd386
    &destination=5511999999999
    &field1=João
    &field2=31/08/2026
    &recordId=1001
```

| Parâmetro | Tipo | Descrição |
|---|---|---|
| `numberchip` | string | Número oficial WhatsApp remetente (`551148004100`) |
| `template` | string | ID do template na Ligo |
| `destination` | string | Telefone de destino do cliente |
| `field1` | string | Variável posicional 1 (`{{1}}`) |
| `field2` | string | Variável posicional 2 (`{{2}}`) |
| `recordId` | string | **Obrigatório** — ID do registro da campanha (`whats_ativo.id`) |

## Resposta (HTTP 200 Plain Text)
```text
WHATSAPP_ACCEPTED|correlationId=JRC_DISPATCH_CORRELATION_1001
```

## Catálogo de Templates (XML)
`GET /softdial/Whats_New_API_whatsapp_ativo/GetWhatsAppOutboundTemplatesCollection?numberchip=551148004100`

### Resposta (HTTP 200 XML)
```xml
<collection>
  <entries>
    <entry key="65f9dbce1fb4e9ac773bd386">JRC Marketing Com Imagem</entry>
  </entries>
</collection>
```
