# ADR-001: Adição da Plataforma .NET 10 LTS

## Contexto
O projeto original propôs o uso de C# .NET 8. Contudo, em agosto de 2026, o .NET 8 encontra-se no final da sua fase de suporte (com término em 10 de novembro de 2026). Iniciar um novo middleware em .NET 8 exigiria uma migração imediata logo após a implantação em produção.

## Decisão
Adotar o **.NET 10 LTS** (`net10.0`) como a plataforma padrão do projeto `Jrc.LigoCampaignGateway`. O .NET 10 LTS é ativamente suportado até novembro de 2028, oferecendo estabilidade operacional estendida, melhor desempenho de concorrência e recursos modernos de C#.

## Consequências
- Solução configurada com `<TargetFramework>net10.0</TargetFramework>`.
- Garantia de suporte oficial até final de 2028.
- Eliminação da necessidade de refatoração/upgrade no curto prazo.
