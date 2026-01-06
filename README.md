# Golden Raspberry Awards API

API RESTful para consultar os produtores de filmes do Golden Raspberry Awards com maior e menor intervalo entre dois prêmios consecutivos.

## 🛠️ Tecnologias

- .NET 9
- ASP.NET Core Minimal APIs
- Entity Framework Core (SQLite In-Memory)
- Scalar (OpenAPI/Swagger UI)
- xUnit (Testes)

## 📁 Estrutura do Projeto

```
src/
└── GoldenRaspberryAwards.Api/
    ├── Modules/
    │   └── Awards/
    │       ├── Application/     # DTOs e Handlers
    │       ├── Domain/          # Entidades e Value Objects
    │       ├── Infrastructure/  # DbContext e Services
    │       └── Presentation/    # Endpoints da API
    └── Program.cs

tests/
└── GoldenRaspberryAwards.Api.Tests/
    ├── Domain/                  # Testes unitários
    └── Integration/             # Testes de integração

docs/
└── Movielist.csv                # Arquivo CSV com dados dos filmes
```

## ⚙️ Configuração

O arquivo `appsettings.json` contém a configuração do caminho do CSV:

```json
{
  "Awards": {
    "MovieListPath": "/mnt/c/repos/GoldenRaspberryAwards/docs/Movielist.csv"
  }
}
```

> ⚠️ **Importante:** Ajuste o caminho `MovieListPath` para o caminho absoluto do arquivo CSV no seu sistema.

## 🚀 Executando a API

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Rodando o projeto

```bash
cd src/GoldenRaspberryAwards.Api
dotnet run
```

A API estará disponível em:
- **HTTPS:** https://localhost:7091
- **HTTP:** http://localhost:5153

## 📖 Documentação da API (OpenAPI)

Acesse a documentação interativa da API (Scalar/Swagger):

```
https://localhost:7091/scalar/v1
```

## 🔗 Endpoints

### GET /api/awards/intervals

Retorna os produtores com **maior** e **menor** intervalo entre vitórias consecutivas.

#### Exemplo de Request

```bash
curl -k https://localhost:7091/api/awards/intervals
```

#### Exemplo de Response

```json
{
  "min": [
    {
      "producer": "Producer Name",
      "interval": 1,
      "previousWin": 2000,
      "followingWin": 2001
    }
  ],
  "max": [
    {
      "producer": "Producer Name",
      "interval": 10,
      "previousWin": 1990,
      "followingWin": 2000
    }
  ]
}
```

## 🧪 Testes

Para executar os testes:

```bash
dotnet test
```

Os testes incluem:
- **Testes Unitários:** Validação das regras de domínio
- **Testes de Integração:** Validação dos endpoints da API

## 📝 Licença

Este projeto foi desenvolvido como parte de um desafio técnico.
