# ADR-002: Separação de Planos de Mídia e Suporte a Multipart Upload

## Contexto
O componente HTTP do Sytel executa requisições síncronas com timeout restrito. Fazer o upload da mídia na Ligo durante o disparo individual do cliente (caminho crítico de envio) introduz latência desnecessária, risco de timeout no Sytel e uploads duplicados da mesma arte. Além disso, expor cada imagem em um endpoint público HTTPS sem autenticação é indesejável quando a API da Ligo aceita o upload direto por arquivo binário (`multipart/form-data`).

## Decisão
1. **Separação entre Plano de Controle e Plano de Disparo**:
   - A imagem da campanha deve ser cadastrada e preparada previamente através do endpoint de gestão `/api/v1/templates/{templateId}/prepare-media`.
   - O upload na Ligo gera o `idmedia` e a data de validade `validUntil` (gravada em texto e parseada).
   - O plano de disparo (consumido pelo Sytel) realiza apenas a leitura do `idmedia` ativo do template.
   - O upload durante o disparo ocorre estritamente como mecanismo de contingência se a mídia expirou.
2. **Estratégia de Upload (`MediaUploadMode`)**:
   - Suportar dois modos: `Multipart` (preferencial) e `Url` (fallback).
   - No modo `Multipart`, o middleware faz o POST do stream binário diretamente para a Ligo em `multipart/form-data`.
   - No modo `Url`, o middleware utiliza a rota opaca pública `/public/media/{assetId}`. O endpoint `/public/media/{assetId}` é ativado **somente** se `MediaUploadMode=Url`.

## Consequências
- Redução drástica da latência no disparo do Sytel (sem upload de imagem por cliente).
- Proteção da mídia (sem exposição desnecessária em URLs públicas).
- Reutilização eficiente do `idmedia` em milhares de envios.
