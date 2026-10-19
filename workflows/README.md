# Recebimento dos workflows n8n e configuração Ligo

## Fornecimento combinado

O usuário informou em 19/10/2026 que enviará os JSONs no chat do Codex.
Esta pasta documenta o recebimento; não contém workflows executáveis.

Esperados:
1. Jade + Help + Financeiro v1.1.13 ou export atual.
2. CORE KSYS v3.13 ou export atual.
3. CORE BTV v3.9 ou export atual.
4. JADE_v1_1_13_CORRIGIDA_20261005.json, se disponível.
5. Export/configuração Ligo ou capturas legíveis, conforme disponibilidade.

Os nomes indicam histórico, não obrigação de usar versões antigas.

## Recepção

- Manter original em área privada autorizada.
- Calcular hash e registrar origem/data.
- Comparar nomes, IDs, nós, refs, conexões, webhook e credenciais referenciadas.
- Identificar diferenças entre base, patch e produção.
- Não escolher versão por sufixo “(1)” ou pelo nome HML.
- Se faltar arquivo, solicitar apenas o necessário para a etapa.
- Nunca criar JSON vazio com nome de workflow real como se fosse implementação.

## Sanitização antes de publicar

Revisar:
- credentials e parâmetros;
- auth headers e tokens em URLs;
- code nodes e valores hardcoded;
- pinData e dados de execução;
- prompts com exemplos identificáveis;
- contatos/CNPJs/boletos reais;
- callbacks e URLs assinadas;
- arquivos auxiliares e logs.

Usar placeholders documentados, sem transformar o original privado.
Registrar no manifesto sanitizações e se o artefato exige reconfiguração.
Não marcar export sanitizado como pronto para importação em produção.

## Manifesto sugerido

Para cada arquivo registrar:
nome; papel; versão declarada; hash original privado; hash sanitizado;
data/origem; workflow ID; referências; alterações; status de homologação;
campos que precisam ser configurados no ambiente.

IDs ou informações operacionais só devem ser publicados quando apropriado
ao repositório público. Não versionar o manifesto privado por engano.

## Testes

Os scripts test_correcoes.js e test_regression.js mencionados no histórico
não estão nesta pasta. Se fornecidos, revisar antes de executar e adaptar
caminhos para este checkout, sem chamadas externas ou dados reais.
