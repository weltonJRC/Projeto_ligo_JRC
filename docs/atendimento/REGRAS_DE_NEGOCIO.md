# Regras de negócio e invariantes

Regras consolidadas do pedido. Confirmar sua implementação nos exports; não assumir que já existem.

## Identidade

1. Solicitar CNPJ ou nome quando necessário.
2. Consultar base.
3. Apresentar cadastro candidato.
4. Pedir confirmação.
5. Só então usar identidade confirmada na jornada.

Tratar CNPJ inválido, nenhum resultado, vários resultados e rejeição.
Preservar identidade confirmada ao mudar de área na mesma conversa.
Ao mudar de empresa, invalidar confirmação e seleções dependentes do cadastro anterior.
Não misturar conversas ou empresas. Confirmar CNPJ não equivale a autenticação forte; manter controles de acesso existentes.

## Suporte

- Usuário de abertura escolhido: Ronald 3327; substituir Lucas onde for realmente o campo de solicitante.
- Separar solicitante, contato, empresa operadora, cliente, analista e grupo.
- Modelo histórico usava empresa operadora 3. Não prometer associação direta a qualquer cliente.
- Se API não permitir a vinculação direta desejada, preservar modelo aceito e identificar cliente no contexto/descrição.
- Coletar sintoma, ramal/linha se aplicável, impacto e testes relatados.
- Não repetir perguntas já respondidas nem inventar diagnóstico.
- Ramal curto só deve ser interpretado como ramal quando houver contexto apropriado.
- “Sim” após pergunta sobre persistência significa que o problema continua, não autorização universal.
- Respeitar recusa de abertura.
- Preservar limite de triagem existente (relato: 3 rodadas); confirmar no export.
- Protocolo só deve vir do resultado real verificado.
- Se ticket existe e roteamento falhou, preservar o ID e tratar falha operacional sem criar outro.
- Timeout após POST exige reconciliação antes de retry.

Configuração histórica a conferir, não copiar cegamente:
tipo 18, item 61, serviço 105, status 1; prioridade BAIXO=1, MEDIO=2, ALTO=3, CRITICO=4.
A correspondência desses códigos com a API atual não foi revalidada.

## Financeiro

- Consultar cobranças da empresa confirmada.
- Apresentar opções que identifiquem valor, vencimento e cobrança.
- Permitir uma, várias ou todas as elegíveis, conforme capacidade real da API.
- “Todas” refere-se ao conjunto consultado/elegível; verificar paginação.
- Confirmar o conjunto antes da operação.
- Não reutilizar um “sim” de suporte como confirmação financeira.
- Invalidar autorização pendente ao sair da jornada, sem apagar resultados concluídos.
- Não inventar links, PDFs ou linhas digitáveis.
- Relatar sucesso por cobrança, inclusive falhas parciais.
- Não declarar “gerei todas” se uma falhou.
- Prevenir reprocessamento das cobranças já concluídas.
- Se necessário dividir resposta, respeitar contrato e limites reais da Ligo.
- Ausência de cobranças deve receber resposta clara, sem criação fictícia.

## Roteamento global

Permitir Financeiro ↔ Suporte e saída de ambos para Comercial/Mocchi.
Avaliar nova intenção antes de o agente atual consumir a mensagem, depois dos guards de humano ativo e estados impeditivos.

Preservar identidade, ledger, protocolos e histórico útil. Limpar apenas pendências incompatíveis.
Negação e ambiguidade exigem cuidado:
- “quero suporte”: suporte;
- “não quero suporte, quero boleto”: financeiro;
- “sim”: depende da última pergunta;
- intenções mistas: esclarecer quando necessário.

Pedido genérico de atendente pode preservar setor; pedido explícito de outra área deve considerar essa área. Não encaminhar ao financeiro apenas porque ele foi o agente anterior.

Comercial histórico: RETURN / TRANSFER_BOT / destino MOCCHI; a Ligo transfere ao bot Agencia JRC. Confirmar mapeamento atual e evitar loop.

## Menu e encerramento

Reconhecer menu, sair, encerrar, encerra e pode encerrar; “voltar” conforme contrato existente.
Normalizar caixa, espaços, acentos e pontuação.
Não encerrar em “não quero encerrar”.
Menu não deve apagar operações concluídas ou causar reabertura.
Respeitar humano ativo; definir o comportamento por estado com evidência.

## Humano

Filas históricas:
- suporte24hs;
- fila_finaceiro (grafia literal: não “corrigir” sem verificar).

Resumo deve conter identidade disponível, motivo, problema, testes relatados e protocolo existente. Preservar request ID/status de handoff.
Distinguir solicitado, aceito, humano ativo e erro. Não declarar conclusão apenas por classificação da intenção.
Bot não pode disputar atendimento com humano nem transferir repetidamente.

## Idempotência e infraestrutura

- conversation_id estável por sessão; message_id por evento lógico.
- Reentrega não deve produzir efeitos duplicados.
- Redis GET/SET separados não provam exclusão mútua.
- Manter ledger e guard de retry.
- Falha do modelo após efeito externo não autoriza repetir operação.
- HTTP 200 isolado não comprova criação ou entrega.
- HML no nome não define destino seguro: configuração histórica tinha environment PROD e escritas habilitadas.

## Fora de escopo

Áudio, transcrição, troca de modelo sem benchmark, migração das jornadas para C# e reestruturação geral dos COREs.
