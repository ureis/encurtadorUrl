# Backend - Encurtador de URLs

API REST para encurtamento de URLs, desenvolvida em **.NET 8** com **Clean Architecture**, **SOLID** e **Entity Framework Core**.

Suporta **PostgreSQL** (Docker) e **SQLite** (desenvolvimento local), com documentação via **Swagger** em ambiente Development.

---

## 📌 Funcionalidades

- Encurtamento de URL com código Base62 ofuscado (Hashids)
- Alias personalizado com validação de duplicidade
- Redirecionamento HTTP 302 para a URL original
- Listagem de todas as URLs cadastradas
- Consulta da URL original por código curto
- Motor sincronizado para geração concorrente de códigos
- Health check para orquestração Docker
- CORS configurável por ambiente

---

## 📋 Estrutura

```
backend/
├── EncurtadorURL.Api/            # Controllers, Program.cs, Swagger
├── EncurtadorURL.Application/    # Interfaces (IShortCodeGenerator)
├── EncurtadorURL.Domain/         # Entidades e contratos de repositório
├── EncurtadorURL.Infrastructure/ # EF Core, repositórios, Hashids
├── EncurtadorUrl.Tests/          # Testes unitários (xUnit)
├── Dockerfile
└── EncurtadorUrl.sln
```

### Camadas

| Projeto | Responsabilidade |
|---------|------------------|
| **Domain** | `UrlRecord`, `IUrlRepository` - sem dependências externas |
| **Application** | `IShortCodeGenerator` - contratos de serviços |
| **Infrastructure** | `AppDbContext`, `UrlRepository`, `ShortCodeGenerator` |
| **Api** | `UrlController`, injeção de dependência, CORS, Swagger |

---

## 📝 Endpoints da API

Documentação interativa: http://localhost:8080/swagger (Docker) ou http://localhost:5156/swagger (local).

### Criar URL curta

```http
POST /api/urls
Content-Type: application/json

{
  "originalUrl": "https://exemplo.com/caminho/longo",
  "alias": "meu-link"
}
```

- `alias` é opcional
- **201 Created** - `{ "shortUrl": "http://localhost:8080/meu-link" }`
- **400** - URL original inválida
- **409** - Alias já em uso

### Listar todas as URLs

```http
GET /api/urls
```

**200 OK** - array ordenado por data (mais recente primeiro):

```json
[
  {
    "shortCode": "meu-link",
    "originalUrl": "https://exemplo.com/caminho/longo",
    "createdAt": "2026-06-17T12:00:00Z",
    "shortUrl": "http://localhost:8080/meu-link"
  }
]
```

### Obter URL original por código

```http
GET /api/urls/{shortCode}
```

**200 OK** - retorna a URL original em texto plano.

**404** - Código não encontrado.

### Redirecionar

```http
GET /{shortCode}
```

**302 Found** - redireciona para a URL original.

### Health check

```http
GET /health
```

**200 OK** - `{ "status": "Healthy" }`

---

## 🛢️ Banco de Dados

A API seleciona o provedor automaticamente em `Program.cs`:

- Connection string com `Host=` → **PostgreSQL** (Npgsql)
- Caso contrário → **SQLite** (`encurtadorurl.db`)

### SQLite (padrão local)

Definido em `EncurtadorURL.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=encurtadorurl.db"
  }
}
```

O banco é criado automaticamente na inicialização (`EnsureCreated`).

### PostgreSQL (Docker)

No `docker-compose.yml` da raiz do monorepo, a API recebe:

```bash
ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=encurtadorurl;Username=encurtador;Password=encurtador123
```

Para usar PostgreSQL localmente sem Docker Compose:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5433;Database=encurtadorurl;Username=encurtador;Password=encurtador123"
dotnet run --project EncurtadorURL.Api/EncurtadorURL.Api.csproj
```

### Tabela

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | int | Chave primária |
| `OriginalUrl` | string | URL completa |
| `ShortCode` | string | Código curto (índice único) |
| `CreatedAt` | DateTime | Data de criação (UTC) |

---

## 🚀 Desenvolvimento

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opcional) Docker para execução containerizada
- (Opcional) PostgreSQL via Docker Compose (raiz do monorepo)

### Setup

```powershell
cd backend

dotnet restore
dotnet run --project EncurtadorURL.Api/EncurtadorURL.Api.csproj
```

Swagger: http://localhost:5156/swagger

### Build

```powershell
cd backend
dotnet build -c Release
```

---

## 🧪 Testes

```powershell
cd backend
dotnet test
```

---

## 🐳 Docker

### Build Local

```powershell
cd backend
docker build --build-arg ENVIRONMENT=Development -t encurtadorurl-api:latest .
docker run -p 8080:8080 encurtadorurl-api:latest
```

Swagger: http://localhost:8080/swagger

Para habilitar Development sem rebuild:

```powershell
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8080:8080 encurtadorurl-api:latest
```

### Com Docker Compose (do root)

```powershell
cd ..
docker compose up --build
```

Sobe API + Frontend + PostgreSQL. API em http://localhost:8080.

---

## 🔐 CORS

Configurado em `Program.cs`:

| Ambiente | Política | Origens |
|----------|----------|---------|
| Development | `AllowAll` | Qualquer origem |
| Production | `AllowFrontend` | Domínios configurados |

Origens padrão em Development: `http://localhost:4200`, `http://localhost:3000`, `http://localhost:5156`.

---

## 🏗 Escolhas de Design

### 1. Persistência flexível (SQLite + PostgreSQL)

O padrão **Repository** isola o acesso a dados. Trocar de SQLite para PostgreSQL exige apenas alterar a connection string — sem mudanças nos controllers ou no domínio.

### 2. Motor de geração sincronizado

`ShortCodeGenerator` é registrado como **Singleton** com `lock` interno, garantindo que apenas uma geração ocorra por vez em ambientes de instância única.

### 3. Ofuscação Base62 + Hashids

A biblioteca **Hashids.net** evita expor IDs sequenciais do banco nas URLs públicas.

---

## 📦 Dependências

| Pacote | Uso |
|--------|-----|
| Microsoft.EntityFrameworkCore.Sqlite | Banco local |
| Npgsql.EntityFrameworkCore.PostgreSQL | Banco Docker/produção |
| Hashids.net | Geração de códigos curtos |
| Swashbuckle.AspNetCore | Swagger/OpenAPI |

---

## 🚨 Possíveis Melhorias

1. **Cache com Redis** — reduzir leituras no banco para redirecionamentos
2. **IDs distribuídos** — substituir `lock` local por Snowflake ID em múltiplas instâncias
3. **Mensageria** — registrar cliques de forma assíncrona (ex.: RabbitMQ)

---

## 🐛 Troubleshooting

### Swagger não abre

- Confirme `ASPNETCORE_ENVIRONMENT=Development`
- Swagger só é habilitado em Development (`Program.cs`)

### Erro de conexão com PostgreSQL

- Verifique se o container `encurtadorurl-db` está healthy
- Connection string correta: `Host=db` dentro do Docker, `Host=localhost;Port=5433` no host

### Porta em uso

Altere em `launchSettings.json` (local) ou no `docker-compose.yml` (Docker).

---

## 📚 Recursos Úteis

- [.NET 8 Docs](https://learn.microsoft.com/dotnet/)
- [ASP.NET Core Docs](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Swagger / OpenAPI](https://swagger.io/docs/)

---

## 📄 Licença

MIT — parte do monorepo **EncurtadorUrl**. Veja o [README principal](../README.md) para instruções completas.
