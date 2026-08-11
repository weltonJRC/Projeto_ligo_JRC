# Guia de Patch para o Script Sytel (`WhatsApp_ativo_manual_MIDIA_HML`)

Este documento orienta a equipe de discador/telefonia JRC sobre como atualizar o script do Sytel na versão de homologação **sem alterar o fluxo de produção existente**.

---

## 1. Alterações no `config.xml`

Alterar o atributo `URL` para apontar para o novo middleware JRC:
```xml
<URL>https://gateway-whatsapp.jrcws.cloud/softdial/Whats_New_API_whatsapp_ativo/SendWhatsAppOutboundTemplate</URL>
```

---

## 2. Alterações no `Start.xml` (Passo `Step3` HTTP Request)

No nó `<URL_Parameters>`, incluir os parâmetros obrigatórios `&recordId=${cam:id}` e `&campaignRunId=${var:campaignRunId}`:

```xml
<Step name="Step3" type="HTTPRequest">
  <URL_Parameters>
    numberchip=${var:numberchip}&amp;template=${var:template}&amp;destination=${var:destination}&amp;field1=${var:field1}&amp;field2=${var:field2}&amp;recordId=${cam:id}&amp;campaignRunId=${var:campaignRunId}
  </URL_Parameters>
</Step>
```

---

## 3. Tratamento dos Ramos de Retorno

| Código / Texto de Retorno | Ramos no Sytel | Ação do Agente Sytel |
|---|---|---|
| `WHATSAPP_ACCEPTED` | **Success** | Registra como enviado com sucesso. |
| `WHATSAPP_FAILED` | **Fail** | Registra como falha e tenta remarcação. |
| `WHATSAPP_TIMEOUT` | **TimeOut** | Registra como tempo esgotado e tenta reagendamento. |
