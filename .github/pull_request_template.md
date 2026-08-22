## Objetivo

Descreva a mudanca e o contrato/componente afetado.

## Classificacao

- Estado alvo: CURRENT / HML / PRODUCAO / EXPERIMENTAL / LEGADO
- [ ] Altera contrato externo Sytel/Ligo/Meta.
- [ ] Altera idempotencia/maquina de estados/retry.
- [ ] Altera callback.
- [ ] Altera midia/upload/lease.
- [ ] Altera deploy/configuracao.

## Seguranca e side effects

- [ ] Revisei risco de duplicidade de mensagens/operacoes.
- [ ] Retry continua limitado e idempotente quando aplicavel.
- [ ] Nao introduzi tokens, credenciais, PII ou payloads sensiveis desnecessarios.
- [ ] Testes nao executam side effects reais de producao sem ambiente/autorizacao explicitos.

## Verificacoes executadas

Marque apenas o que foi realmente executado:
- [ ] `dotnet test`
- [ ] Testes de contrato
- [ ] Testes de idempotencia/state machine
- [ ] Teste HML com provedor
- [ ] Nao aplicavel — somente documentacao

Resultados:
```text
NAO_EXECUTADO ou resultados reais aqui.
```

## Evidencias

Informe evidencias e status do provedor sem incluir secrets/PII desnecessaria.

## Rollback

Descreva como reverter codigo/configuracao e como tratar side effects externos ja aceitos.

## Checklist final

- [ ] Revisei o diff completo.
- [ ] Consultei ADRs/contratos relevantes.
- [ ] Nao alterei `main` diretamente como area inicial de trabalho.
- [ ] Nao classifiquei como `BASELINE_VALIDADO` sem evidencia.
