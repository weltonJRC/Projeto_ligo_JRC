# Instruções para agentes — Projeto Ligo JRC

## Ordem de leitura

1. README.md
2. docs/atendimento/README.md
3. docs/atendimento/REVISAO_REPOSITORIO.md
4. docs/atendimento/CONTEXTO_E_HISTORICO.md
5. docs/atendimento/REGRAS_DE_NEGOCIO.md
6. docs/atendimento/CONTRATO_LIGO.md
7. docs/atendimento/PLANO_E_BACKLOG.md
8. docs/atendimento/TESTES_E_ACEITE.md
9. docs/atendimento/IMPLANTACAO_E_ROLLBACK.md
10. workflows/README.md

## Escopo e fontes

Há duas frentes: gateway C# de campanhas e bot n8n/Ligo. Não confundir contratos HSM do gateway com payload do webhook da Jade.

Diferencie sempre:
- observado em código versionado;
- observado em arquivo local externo ao repositório;
- relato histórico de execução;
- hipótese;
- requisito;
- implantação comprovada.

Workflows atuais não estavam no repositório na revisão de 19/10/2026. Não inventar exports nem reconstruir o sistema a partir deste texto. Recuperar a base para editar com fidelidade.

## Regras de execução

- Preservar nomes, IDs, posições, conexões, credenciais referenciadas, webhooks e referências entre workflows.
- Fazer diff mínimo e justificar cada nó alterado.
- Não modificar CORE KSYS/BTV se o defeito está somente no orquestrador.
- Áudio está adiado. Não implementar captura/transcrição.
- Manter baseline do modelo existente; não trocar por preferência.
- Não alterar gateway/Sytel/IMBridge para resolver bug n8n sem evidência.
- Nunca presumir que caminho HML usa dados de homologação.
- Não testar abertura de chamados, boletos ou mensagens reais sem autorização de escopo e ambiente.
- Não publicar/ativar em n8n ou Ligo só porque um commit foi aprovado.
- Avançar autonomamente em inspeção, correções locais e testes isolados.
- Respeitar instruções explícitas posteriores do usuário.

## Estado e negócio

Ronald 3327 é a decisão histórica de usuário de abertura; confirmar configuração atual. Empresa operadora não equivale a empresa cliente. Não prometer abertura direta por qualquer empresa sem comprovação.

Nunca afirmar sucesso por HTTP isolado. Separar criação, protocolo, verificação do registro, roteamento e SLA. Preservar o registro já criado quando houver falha posterior e evitar duplicação.

Preservar identidade confirmada entre jornadas, ledger e proteções de retry. Invalidar autorização financeira pendente ao mudar de jornada/empresa. Respeitar atendimento humano ativo.

## Segurança do repositório público

Não publicar segredos ou dados identificáveis de clientes. Não copiar credenciais do histórico. Exports podem conter segredos em parâmetros, headers, code nodes e pinData, não apenas na seção credentials.

Fixtures devem ser sintéticas e identificadas. Não inserir valores reais de telefones, CNPJs, linhas digitáveis, URLs autenticadas ou conversation_id.

## Verificação e entrega

Executar testes pertinentes à alteração. Registrar comando, artefato/hash, data, resultado e limite. Testes mock não provam integração real; banco InMemory não prova concorrência PostgreSQL.

Para gateway, usar solução e SDK definidos no repositório. Para n8n, só usar comandos associados aos arquivos realmente importados; não inventar suites.

Atualizar documentação, backlog e evidências ao concluir uma etapa. Entregar diff, pendências e rollback. Não declarar pronto para produção sem os critérios de aceite.
