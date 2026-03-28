# SampleApp (BackEnd + FrontEnd)

Projeto básico .NET 10 com API e Blazor Server para demonstração de CRUD.

## Estrutura

- `BackEnd` - API minimal com endpoints:
  - `GET /weatherforecast`
  - `GET /products`
  - `GET /products/{id}`
  - `POST /products`
  - `PUT /products/{id}`
  - `DELETE /products/{id}`

- `FrontEnd` - Blazor Server com página principal (`/`) para gerenciar produtos.

## Como rodar

1. Abra dois terminais
2. Terminal 1 (API):
   - `dotnet run --project SampleApp/BackEnd/BackEnd.csproj`
3. Terminal 2 (Frontend):
   - `dotnet run --project SampleApp/FrontEnd/FrontEnd.csproj`

> Se a API estiver em outra URL, configure `WEATHER_URL` ou `BACKEND_URL` no appsettings ou variáveis de ambiente do projeto `FrontEnd`.

## Testes manuais

- Acesse a UI: `http://localhost:8081` (ou URL de saída do frontend)
- Adicione/edite/exclua produtos direto na interface.

## Testes automáticos

- No diretório `SampleApp`, execute:
  - `dotnet test SampleApp.Tests/SampleApp.Tests.csproj`

## Endpoints (API)

### Listar
GET `http://localhost:5000/products`

## Docker

- Build e start com docker-compose:
  - `cd SampleApp`
  - `docker compose build`
  - `docker compose up -d`
  - `docker compose down`

## GitHub Actions

- Workflow: `.github/workflows/ci.yml`
- Roda `dotnet build`, `dotnet test` e valida `docker compose` na pipeline.

### Dica de setup local antes

1. `dotnet restore SampleApp/SampleApp.sln`
2. `dotnet build SampleApp/SampleApp.sln`
3. `dotnet test SampleApp/SampleApp.Tests/SampleApp.Tests.csproj`

### Obter por id
GET `http://localhost:5000/products/{id}`

### Criar
POST `http://localhost:5000/products`
Body JSON:
```json
{ "name": "Caneta", "price": 7.5 }
```

### Atualizar
PUT `http://localhost:5000/products/{id}`
Body JSON:
```json
{ "id": 1, "name": "Caneta Azul", "price": 9.9 }
```

### Excluir
DELETE `http://localhost:5000/products/{id}`
Projeto fullstack .NET com CRUD, Docker e CI
Atualização para gerar PR
