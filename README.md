# Projeto Ligo JRC

Este repositório reúne o gateway de campanhas Ligo/Sytel e a documentação de continuidade do atendimento Jade/Help/BemTeVi.

## Comece aqui no Codex

Leia [AGENTS.md](AGENTS.md) e [o índice do atendimento](docs/atendimento/README.md). O [prompt de continuidade](docs/atendimento/PROMPT_CODEX.md) inicia o trabalho sem depender do chat anterior.

**Estado em 19/10/2026:** documentação de continuidade adicionada; workflows n8n e exportação Ligo ainda não versionados aqui. Nenhuma implantação foi realizada por esta atualização. Não considerar produção homologada.

## Dois componentes distintos

| Componente | Conteúdo | Estado da evidência |
| --- | --- | --- |
| Gateway C# | `src/`, `tests/`, `deploy/`, `Jrc.LigoCampaignGateway.slnx` | Código existente inspecionado parcialmente; build não executado nesta revisão |
| Atendimento n8n/Ligo | `docs/atendimento/`, `workflows/README.md` | Contexto, contratos históricos e plano; exports atuais ausentes |

O gateway de disparos HSM não implementa automaticamente a recepção Jade, o suporte KSYS ou o financeiro BTV. Não migrar as jornadas para C# nem alterar o gateway por associação de nomes.

## Gateway existente

Solução C# com projetos API, Application, Domain e Infrastructure e testes Unit, Integration e Contract. `global.json` fixa SDK 10.0.302 com rollForward latestFeature.

Com SDK compatível, para validar localmente:

```bash
dotnet restore Jrc.LigoCampaignGateway.slnx
dotnet build Jrc.LigoCampaignGateway.slnx --no-restore
dotnet test Jrc.LigoCampaignGateway.slnx --no-build
```

Esses comandos são instruções de reprodução, não resultados desta revisão. Não iniciar o gateway em modo Real para validar documentação.

Documentação original preservada:
- [Arquitetura](docs/architecture/ADR-001-net10-lts.md)
- [Implantação HML do gateway](docs/runbooks/DEPLOY-HML.md)
- [Rollback do gateway](docs/runbooks/ROLLBACK.md)

Antes de aplicar esses runbooks, leia as [ressalvas da revisão](docs/atendimento/REVISAO_REPOSITORIO.md). O rollback do gateway não substitui o rollback do bot n8n/Ligo.

## Publicação de arquivos

O repositório é público. Nunca adicionar credenciais, dumps de execução, conversas identificáveis, boletos reais ou exports sem sanitização. Consulte [importação de artefatos](workflows/README.md).
