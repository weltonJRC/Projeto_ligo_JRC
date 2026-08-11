# ADR-000: Seleção da Versão de Runtime .NET 10 LTS

## Contexto
O projeto foi inicialmente contemplado em .NET 8. Contudo, em agosto de 2026, o .NET 8 encontra-se no final da sua fase de suporte (terminando em 10 de novembro de 2026). O .NET 10 é a versão LTS atual com suporte ativo até novembro de 2028.

## Decisão
1. Adotar o **.NET 10 LTS** (`net10.0`) como o runtime padrão do projeto `Jrc.LigoCampaignGateway`.
2. Caso ocorra alguma limitação de infraestrutura no servidor IIS que obrigue o uso temporário do .NET 8, a migração para .NET 10 deve ser agendada obrigatoriamente antes de novembro de 2026.

## Consequências
- Código preparado com recursos de C# 13 / .NET 10.
- Longevidade técnica garantida até novembro de 2028.
