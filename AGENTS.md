# AGENTS.md - JRC Ligo Campaign Gateway

## Identidade do repositorio
Este repositorio contem o middleware `Jrc.LigoCampaignGateway`, em C#/.NET 10, que integra Sytel Omnichannel com Ligo Messaging / Meta WhatsApp Business Platform.

Este repositorio NAO e o repositorio da Jade Recepcionista nem do Mocchi SDR. Nao mover regras conversacionais de agentes para este middleware sem uma decisao arquitetural explicita.

## Arquitetura
- Preservar Clean Architecture e os limites existentes entre projetos/camadas.
- Consultar os ADRs em `docs/architecture/` antes de alterar runtime, idempotencia, callbacks, media ou versao do .NET.
- Contratos externos ficam documentados em `docs/contracts/` e procedimentos operacionais em `docs/runbooks/`.

## Invariantes de integracao
- Tratar idempotencia e maquina de estados como requisitos de integridade, nao como otimizacoes opcionais.
- Retry tecnico deve ser limitado e nao pode gerar mensagem duplicada.
- Aceite HTTP/provedor nao equivale automaticamente a entrega final; distinguir os estados disponiveis.
- Callbacks devem ser autenticados/validados conforme o contrato atual.
- Upload/lease de midia deve respeitar ciclo de vida e contratos registrados nos ADRs.
- Nao realizar envio real de WhatsApp, campanha, callback destrutivo ou outra acao de producao durante testes sem ambiente e autorizacao explicitos.

## Seguranca
- Nunca commitar tokens Ligo/Meta, credenciais Sytel, senhas, chaves privadas ou payloads com PII desnecessaria.
- Usar configuracao/secret store apropriado ao runtime.
- Sanitizar logs; telefone, conteudo de mensagem e identificadores externos devem ser minimizados quando nao forem necessarios.

## Workflow AI-first
1. Ler `AGENTS.md`, `ARCHITECTURE.md`, README e ADRs relevantes antes de alterar codigo.
2. Consultar codigo e contratos CURRENT; nao assumir comportamento a partir de historico de conversa.
3. Mudancas arquiteturais/contratuais exigem design/ADR antes da implementacao.
4. Trabalhar em branch/worktree isolado; `main` nao e area inicial de trabalho.
5. Preservar Clean Architecture e implementar a menor mudanca necessaria.
6. Executar `dotnet test` e verificacoes adicionais aplicaveis quando houver mudanca de runtime.
7. Revisar diff para idempotencia, retries, callbacks, secrets e side effects.
8. Abrir Pull Request antes de merge.

## Estados tecnicos
Diferenciar `CURRENT`, `HML`, `PRODUCAO`, `BASELINE_VALIDADO`, `EXPERIMENTAL` e `LEGADO/DEPRECATED`. Uma versao mais recente nao e automaticamente uma baseline validada.
