
# 🎮 FCG - FIAP Cloud Games (Fase 1 - Arquitetura .NET)

Projeto desenvolvido como parte da **FASE 1** da pós-graduação em Arquitetura .NET pela FIAP.

A aplicação representa uma **plataforma de jogos digitais**, permitindo que usuários possam adquirir jogos, acompanhar promoções, e gerenciar sua biblioteca. A estrutura do projeto foi pensada com foco em **boas práticas de arquitetura**, **DDD**, **segurança** e **testabilidade**.

---

## 📂 Estrutura do Projeto

```
.
├── docs/                     # Documentação e instruções
├── scripts/                  # Scripts de desenvolvimento (migrations, coverage, etc)
├── src/                      # Código-fonte da aplicação
│   ├── FCG.API               # Camada de apresentação (Web API)
│   ├── FCG.Application       # Camada de aplicação (AppServices, DTOs)
│   ├── FCG.Domain            # Camada de domínio (Entidades, regras de negócio)
│   ├── FCG.Infra.Data        # Persistência de dados (EF Core, Repositórios)
│   └── FCG.Infra.Security    # Segurança e autenticação (JWT, Identity)
└── tests/
    └── FCG.UnitTests         # Testes unitários
```

---

## 🛠️ Tecnologias Utilizadas

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Entity Framework Core](https://learn.microsoft.com/ef/)
- [ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/web-api)
- [JWT Bearer Authentication](https://jwt.io/)
- [Identity](https://learn.microsoft.com/pt-br/aspnet/identity)
- [FluentValidation](https://fluentvalidation.net/)
- [xUnit](https://xunit.net/)
- [Moq](https://github.com/moq/moq)
- [Bogus (Faker)](https://github.com/bchavez/Bogus)
- [dotnet-reportgenerator-globaltool](https://github.com/danielpalme/ReportGenerator)

---

## 🚀 Como Executar o Projeto

1. **Clone o repositório**
   ```bash
   git clone <repo-url>
   cd nome-do-projeto
   ```

2. **Execute o projeto**
   ```bash
   cd src/FCG.API
   dotnet run
   ```

> ℹ️ O projeto utiliza banco de dados **InMemory** para facilitar a execução local. Caso queira executar com SQL Server, é preciso descomentar o código dentro do arquivo "**src/FCG.API/Extensions/ServiceCollectionExtensions**" do método "**AddDataContexts**" e executar o script o seguinte script da pasta `scripts`

```bash
./migrations.sh
```

---


## 🧪 Executando os Testes e Gerando Relatório de Cobertura

### 1️⃣ Instale a ferramenta de relatório (apenas uma vez)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### 2️⃣ Execute o script de cobertura

Navegue até a pasta `scripts` e execute o script:

```bash
./coverage-tests.sh
```

O script irá:
- Rodar os testes com cobertura
- Gerar os arquivos de cobertura no diretório `coverage/`
- Criar o relatório HTML em `coverage-report/`
- Abrir automaticamente o `index.html` do relatório no navegador padrão

---

## 📌 Notas

- Este projeto foi estruturado pensando na **expansão futura** (fases seguintes da pós-graduação)
- Já inclui **testes unitários** para regras de negócio essenciais
- A autenticação já considera perfis com **autorização baseada em roles**

---
