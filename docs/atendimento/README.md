# Continuidade do atendimento JRC

Referência consolidada em 19/10/2026. Este material permite retomar o projeto no Codex sem transportar conversas privadas.

## Ordem de consulta

| Documento | Finalidade |
| --- | --- |
| [Revisão do repositório](REVISAO_REPOSITORIO.md) | O que existe de fato e limites da auditoria |
| [Contexto e histórico](CONTEXTO_E_HISTORICO.md) | Fases, incidentes e correção anterior |
| [Regras de negócio](REGRAS_DE_NEGOCIO.md) | Comportamento esperado e decisões |
| [Contrato Ligo](CONTRATO_LIGO.md) | Entrada, retorno, estado, blocos e mapeamentos |
| [Plano e backlog](PLANO_E_BACKLOG.md) | Sequência de execução e pendências |
| [Testes e aceite](TESTES_E_ACEITE.md) | Matriz de regressão e homologação |
| [Implantação e rollback](IMPLANTACAO_E_ROLLBACK.md) | Preparar e reverter publicação do bot |
| [Prompt Codex](PROMPT_CODEX.md) | Instrução de retomada |
| [Importação dos workflows](../../workflows/README.md) | Arquivos faltantes e sanitização |

## Estado

- Documentação preparada; não é implementação das correções pendentes.
- Gateway C# existente preservado.
- Exports n8n e Ligo atuais ainda precisam ser fornecidos e comparados.
- Correção local de 05/10 teve 50 verificações aprovadas nesta sessão, fora deste repositório.
- Instalação em produção e homologação ponta a ponta não confirmadas.
- Áudio adiado por decisão do usuário.

Não existe afirmação comprovada de que o bot abra chamados diretamente no cadastro de qualquer empresa cliente. O caso histórico confirmado utilizou empresa operadora JRC e usuário Ronald.

## Dados omitidos

Clientes são designados Empresa A e Empresa B; identificadores de tickets, cobranças, conversas e execuções foram omitidos desta versão pública. Evidências brutas devem permanecer em armazenamento privado autorizado.
