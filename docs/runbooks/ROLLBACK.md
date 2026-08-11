# Runbook: Procedimento de Rollback

## 1. Escopo de Impacto
O novo middleware opera de forma independente do backend antigo e do IMBridge existente. O rollback não afeta o fluxo de atendimento humano receptivo.

## 2. Procedimento de Rollback no Sytel
Em caso de falha durante o teste de homologação:

1. No painel do Sytel CallGem / Campaign Manager, desativar a campanha de teste `WhatappJRC_Ativo_HML`.
2. Alterar a associação do script da campanha para apontar novamente para o script original `WhatsApp_ativo_manual`.
3. Caso necessário, paralisar o contêiner do gateway:
   ```bash
   docker-compose -f deploy/docker/docker-compose.yml stop
   ```

## 3. Integridade de Dados
- Os registros de disparos no PostgreSQL permanecem intactos para auditoria post-mortem.
- Nenhuma alteração é efetuada no banco de dados principal do Sytel (`base_grupojrc`).
