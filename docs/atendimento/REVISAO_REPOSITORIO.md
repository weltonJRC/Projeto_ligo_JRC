# Revisão do repositório — 19/10/2026

## Base examinada

- Repositório: weltonJRC/Projeto_ligo_JRC
- Branch: main
- Commit anterior à atualização documental: `52b5d8d95fda3ec44c0801be79d42c58c3afe054`
- Data do commit: 12/08/2026.
- Visibilidade observada: pública.
- Árvore recursiva retornada sem truncamento pelo conector GitHub.

## Inventário confirmado

| Área | Conteúdo observado |
| --- | --- |
| Solução | Jrc.LigoCampaignGateway.slnx |
| SDK | global.json: 10.0.302 / latestFeature |
| Código | API, Application, Domain, Infrastructure |
| Testes | Unit, Integration, Contract |
| Deploy | Dockerfile, Compose e scripts Sytel |
| Contratos | Auth, HSM, media upload, status callback, Sytel legacy GET |
| Documentação | ADRs, implantação HML, rollback |
| n8n | Nenhum export de workflow Jade/KSYS/BTV encontrado na árvore |
| Ligo Builder | Nenhum export de bot identificado |
| Agentes | Não havia AGENTS.md na árvore |
| CI | Não havia arquivos em .github/workflows na árvore |

A presença de DLLs Sytel não prova procedência, compatibilidade ou publicação. Não foram executadas nem auditadas.

## Leitura direcionada

Foram lidos README, global.json, solução, ADR-000/001, DEPLOY-HML, ROLLBACK, Dockerfile, Compose, Program.cs, HealthController, testes Contract/Integration e contratos auth/HSM. Esta é revisão estrutural e documental, não auditoria integral de código ou segurança.

## Achados verificáveis

| Achado | Evidência | Consequência / próximo passo |
| --- | --- | --- |
| Repositório é gateway de campanhas | README original, solução e src | Não tratar o gateway como implementação da Jade |
| SDK e imagem Docker divergem na referência de versão | global.json fixa 10.0.302; Dockerfile usa 10.0-preview | Validar imagem suportada e reproduzir build antes de mudar; não houve troca nesta atualização |
| Runbook sugere diretório diferente do clone | clone Projeto_ligo_JRC seguido de cd Jrc.LigoCampaignGateway | Usar diretório real; correção documental incluída |
| Compose contém valores literais | environment define Mock e configuração fixa | Um .env isolado não substitui valores sem interpolação; runbook esclarecido |
| Credencial de exemplo no Compose | senha de banco literal já presente | Não reutilizar em produção; parametrização é tarefa separada, sem alterar credenciais nesta revisão |
| Seed não condicionado a Mock no bloco observado | Program.cs executa seed quando não há templates | Verificar comportamento em Real antes de implantação; não executar nesta tarefa |
| Fallback de banco em memória | Program.cs quando connection string ausente ou contém InMemory | Não usar como prova de persistência em produção |
| Health é resposta estática do controller | HealthController retorna HEALTHY | Não comprova banco, autenticação ou disponibilidade Ligo |
| Teste Integration usa EF InMemory | IdempotencyIntegrationTests armazena e recupera registro | Não prova unicidade, concorrência ou efeito externo único |
| Teste Contract usa WireMock | LigoContractTests simula login 200/401 | Não comprova contrato real vigente |
| Contrato HSM mostra alternativas de endpoint | hsm-send.md contém “ou” | Confirmar documentação do fornecedor antes de uso Real |
| Contrato auth contém token de aparência JWT | auth.md original | Substituído por placeholder nesta atualização; validade do conteúdo anterior não foi testada |

## Verificações executadas

- Inspeção de árvore e leitura dos arquivos acima no commit fixado.
- Suites locais externas ao repositório executadas em 19/10/2026:
  - test_correcoes.js: 25 aprovadas, zero falhas;
  - test_regression.js: 25 aprovadas, zero falhas.
- Elas usaram artefatos da correção de 05/10 disponíveis na sessão anterior; não foram publicadas com dumps ou dados reais.
- SDK dotnet não disponível no ambiente desta revisão: build e testes C# NÃO executados.
- Não foram executadas chamadas de criação de chamados, boletos, disparos ou publicação.
- Não foi auditado todo o histórico Git para segredos; placeholder novo não remove conteúdo de commits antigos.

## Parecer

A documentação habilita a retomada organizada no Codex. O bot ainda depende da recuperação dos exports atuais e de homologação. O gateway não recebeu atestado de prontidão operacional.
