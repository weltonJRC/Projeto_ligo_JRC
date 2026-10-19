# Prompt para iniciar no Codex

Copie o bloco abaixo no chat do Codex associado a este repositório.

```text
Continue o projeto JRC deste repositório. Leia AGENTS.md e todos os
documentos listados em docs/atendimento/README.md antes de editar.

Vou enviar os JSONs diretamente neste chat. Eles são a base a inspecionar,
não presuma que já estejam no repositório nem que sejam a versão publicada.

O objetivo é concluir Jade/Help/BemTeVi na Ligo com alterações pontuais:
protocolo KSYS e Ronald 3327; triagem sem repetição; troca Financeiro↔Suporte
e saída para Comercial/Mocchi; encerramento incluindo “Encerra”; seleção
individual/múltipla/todas de cobranças; contexto e bloqueio de humano;
idempotência e contingência.

Áudio está adiado. Preserve o que funciona. O gateway C# de campanhas
existente é uma frente diferente: não migrar jornadas para C# nem modificar
Sytel/IMBridge por associação de nomes.

Comece com tabela dos anexos: versão, hash, função, IDs/referências,
diferenças e evidência de instalação. Compare com a correção de 05/10
se eu a fornecer. Não reconstruir workflows de memória.

Use o histórico como evidência relatada, não como prova do estado atual.
Há 50 checks locais aprovados numa sessão anterior, mas sem comprovação
de instalação ou homologação ponta a ponta. Não declarar produção pronta.

Faça diagnóstico, diff mínimo por nó, testes isolados, plano de Ligo,
implantação e rollback. Avance no trabalho local autorizado. Não chame
webhooks reais nem crie chamados/boletos/mensagens sem autorização.

O repositório é público. Não adicionar exports brutos, credenciais,
pinData, dados de clientes, telefones, boletos ou conversas. Preparar
versão sanitizada separada quando for necessário versionar.

Entregue somente JSONs que precisem mudar, preservando nomes, IDs,
posições, credenciais referenciadas, conexões, webhook e subworkflow refs.
Não alterar os COREs se a correção for apenas na Jade.

Ao concluir, atualize backlog/evidências com o que foi realmente feito,
o que foi testado e o que ainda depende do ambiente. Não peça que eu
explique novamente o projeto: consulte a documentação.
```
