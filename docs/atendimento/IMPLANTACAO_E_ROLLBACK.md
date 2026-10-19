# Implantação e rollback — bot n8n/Ligo

Este runbook é diferente do deploy/rollback do gateway C# existente.

## Antes da implantação

1. Obter export atual de Jade, KSYS, BTV e configuração Ligo.
2. Registrar hashes, IDs, referências e estado de publicação.
3. Guardar backup privado completo; repositório público recebe somente conteúdo sanitizado.
4. Comparar nós alterados e confirmar credenciais no ambiente, sem exportar segredos.
5. Validar webhook real e mapeamento Ligo. Caminho HML não prova ambiente isolado.
6. Confirmar modelo/gateway, Redis, filas e integrações.
7. Testar em cópia isolada com escritas desabilitadas quando viável.
8. Fazer homologação real somente com escopo autorizado.
9. Preparar rollback antes de publicar.

## Atualização

- Não gerar novos IDs por importação desnecessária.
- Se copiar workflows, ajustar referências somente após conferir o mapa.
- CORE inativo não implica falha: pode ser chamado por Execute Workflow.
- Publicar/ativar somente artefatos aprovados.
- Atualizar Ligo apenas nas condições/blocos documentados.
- Conferir URL de produção versus webhook-test.
- Confirmar como tratar conversas já em andamento; não apagar Redis em massa.
- Não migrar estado sem estratégia de compatibilidade.

## Smoke test controlado

Executar o menor conjunto autorizado que confirme:
recepção, identificação, troca de área, encerramento, retorno financeiro,
protocolo e handoff. Antes de criar novo chamado/segunda via, conferir
necessidade e autorização. Guardar resultado privado e resumo sanitizado.

## Gatilhos de rollback

- chamados duplicados;
- protocolo incorreto ou omitido após criação;
- exposição/mistura de empresas;
- financeiro com seleção ou autorização errada;
- loop de agentes;
- humano sem bloqueio;
- falha generalizada de webhook/modelo;
- regressão crítica do atendimento textual.

## Procedimento

1. Suspender a rota nova conforme procedimento operacional autorizado.
2. Restaurar workflow anterior e referências compatíveis.
3. Restaurar versão anterior dos blocos/publicação Ligo.
4. Preservar ledger, histórico e IDs de efeitos já concluídos.
5. Reconciliar efeitos incertos antes de retry.
6. Validar texto e transferência humana.
7. Registrar incidente e versão restaurada.

Rollback de código NÃO desfaz chamado criado nem documento financeiro.
Não excluir registros reais para “limpar teste” sem procedimento autorizado.

## Registro de release

Data/fuso; responsável; commit/hash; workflows/IDs; versão Ligo;
mudanças; testes; limites; aprovação; resultado inicial; backup;
procedimento de reversão; pendências.
