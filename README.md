# Encurtador de URLs

Aplicação full-stack para encurtar URLs, com backend em **.NET 8**, frontend em **Angular 17**, banco **PostgreSQL** e orquestração via **Docker Compose**.

## Estrutura do Projeto

```
EncurtadorUrl/
├── backend/                          # Aplicação .NET 8
│   ├── EncurtadorURL.Api/            # API REST + Swagger
│   ├── EncurtadorURL.Domain/         # Entidades e contratos
│   ├── EncurtadorURL.Infrastructure/ # EF Core, repositórios
│   ├── EncurtadorUrl.Tests/          # Testes unitários (xUnit)
│   ├── Dockerfile
│   ├── EncurtadorUrl.sln
│   └── EncurtadorURL.Application/    # Interfaces de aplicação
│
├── frontend/                         # Aplicação Angular 17
│   ├── src/app/
│   │   ├── components/
│   │   │   ├── url-shortener/        # Criar URL curta
│   │   │   ├── url-lookup/           # Consultar por código
│   │   │   └── url-list/             # Listar URLs do banco
│   │   └── services/url.service.ts   # Cliente HTTP da API
│   ├── Dockerfile
│   └── nginx.conf
│
├── database/                         # PostgreSQL (Docker)
│   └── Dockerfile
│
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## Quick Start

### Pré-requisitos

- Docker e Docker Compose
- (Opcional) Node.js 20+ e .NET 8 SDK para desenvolvimento local
- (Opcional) DBeaver ou outro cliente PostgreSQL

### Executar com Docker Compose

```bash
git clone <seu-repositório>
cd EncurtadorUrl

cp .env.example .env

docker compose up --build
```

Aguarde os três containers ficarem **healthy** (cerca de 1 a 2 minutos).

### Acesso

| Serviço    | URL                              |
|------------|----------------------------------|
| Frontend   | http://localhost:4200            |
| API        | http://localhost:8080            |
| Swagger    | http://localhost:8080/swagger    |
| PostgreSQL | `localhost:5433` (via DBeaver)   |

---

## Frontend

A interface possui três áreas principais:

| Componente       | Função                                                                 |
|------------------|------------------------------------------------------------------------|
| **Criar URL Curta** | Encurta uma URL com código automático ou alias personalizado        |
| **Consultar URL por Código** | Busca uma URL específica via `GET /api/urls/{shortCode}`   |
| **URLs Encurtadas Recentes** | Lista todas as URLs do banco, ordenadas da mais recente     |

A lista é atualizada automaticamente após criar uma nova URL.

### Desenvolvimento local

```bash
cd frontend
npm install
npm start
```

Aplicação em: http://localhost:4200

O serviço `UrlService` aponta por padrão para `http://localhost:8080`.

---

## API

Documentação interativa disponível em http://localhost:8080/swagger (ambiente Development).

### Endpoints

#### Criar URL curta

```http
POST /api/urls
Content-Type: application/json

{
  "originalUrl": "https://exemplo.com/caminho/muito/longo",
  "alias": "meu-link"
}
```

`alias` é opcional. Retorna **201 Created**:

```json
{
  "shortUrl": "http://localhost:8080/meu-link"
}
```

#### Listar todas as URLs

```http
GET /api/urls
```

Retorna **200 OK** com array ordenado por data de criação (mais recente primeiro):

```json
[
  {
    "shortCode": "meu-link",
    "originalUrl": "https://exemplo.com/caminho/muito/longo",
    "createdAt": "2026-06-17T12:00:00Z",
    "shortUrl": "http://localhost:8080/meu-link"
  }
]
```

#### Obter URL original por código

```http
GET /api/urls/{shortCode}
```

Retorna **200 OK** com a URL original em texto plano:

```
https://exemplo.com/caminho/muito/longo
```

#### Redirecionar para URL original

```http
GET /{shortCode}
```

Retorna **302 Found** com redirecionamento para a URL original.

#### Health check

```http
GET /health
```

Retorna **200 OK**: `{"status":"Healthy"}`

### Códigos de erro comuns

| Status | Situação                          |
|--------|-----------------------------------|
| 400    | URL original inválida             |
| 404    | Código curto não encontrado       |
| 409    | Alias já em uso                   |

### Desenvolvimento local

```bash
cd backend
dotnet restore
dotnet run --project EncurtadorURL.Api/EncurtadorURL.Api.csproj
dotnet test
```

Swagger local: http://localhost:5156/swagger

Por padrão, o desenvolvimento local usa **SQLite** (`encurtadorurl.db`). No Docker, a API usa **PostgreSQL** automaticamente via variável de ambiente.

---

## Banco de Dados

### Docker (produção local)

O `docker-compose.yml` sobe um container PostgreSQL 16 com:

| Parâmetro  | Valor            |
|------------|------------------|
| Host       | `localhost`      |
| Porta      | `5433`           |
| Database   | `encurtadorurl`  |
| Usuário    | `encurtador`     |
| Senha      | `encurtador123`  |

> A porta **5433** é usada no host para evitar conflito com outras instâncias PostgreSQL na máquina. Dentro da rede Docker, a API conecta em `db:5432`.

A tabela `Urls` é criada automaticamente na inicialização da API (`EnsureCreated`).

### Conectar com DBeaver

1. Abra o DBeaver → **Nova Conexão** → **PostgreSQL**
2. Preencha:
   - **Host:** `localhost`
   - **Port:** `5433`
   - **Database:** `encurtadorurl`
   - **Username:** `encurtador`
   - **Password:** `encurtador123`
3. Clique em **Test Connection** → **Finish**
4. Navegue até: `Schemas → public → Tables → Urls`

Consulta de exemplo:

```sql
SELECT "Id", "ShortCode", "OriginalUrl", "CreatedAt"
FROM "Urls"
ORDER BY "CreatedAt" DESC;
```

### Desenvolvimento local (SQLite)

Sem Docker, o backend usa SQLite por padrão (`appsettings.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=encurtadorurl.db"
  }
}
```

Para usar PostgreSQL localmente, altere a connection string ou defina a variável:

```bash
ConnectionStrings__DefaultConnection=Host=localhost;Port=5433;Database=encurtadorurl;Username=encurtador;Password=encurtador123
```

---

## Docker

### Serviços

| Container              | Imagem              | Porta host |
|------------------------|---------------------|------------|
| `encurtadorurl-db`     | `encurtadorurl-db`  | 5433       |
| `encurtadorurl-api`    | `encurtadorurl-api` | 8080       |
| `encurtadorurl-frontend` | `encurtadorurl-frontend` | 4200  |

### Comandos úteis

```bash
# Iniciar em background
docker compose up -d

# Parar
docker compose down

# Parar e remover volumes (apaga dados do banco)
docker compose down -v

# Ver logs
docker compose logs -f api
docker compose logs -f frontend
docker compose logs -f db

# Rebuild
docker compose up --build
```

### Build individual

**API:**
```bash
cd backend
docker build --build-arg ENVIRONMENT=Development -t encurtadorurl-api:latest .
docker run -p 8080:8080 encurtadorurl-api:latest
```

**Frontend:**
```bash
cd frontend
docker build -t encurtadorurl-frontend:latest .
docker run -p 4200:80 encurtadorurl-frontend:latest
```

**Banco:**
```bash
cd database
docker build -t encurtadorurl-db:latest .
docker run -p 5433:5432 \
  -e POSTGRES_DB=encurtadorurl \
  -e POSTGRES_USER=encurtador \
  -e POSTGRES_PASSWORD=encurtador123 \
  encurtadorurl-db:latest
```

---

## Arquitetura

### Backend — Clean Architecture

```
Domain         → Entidades, interfaces de repositório
Application    → Interfaces de serviços (IShortCodeGenerator)
Infrastructure → EF Core, repositórios, Hashids
API            → Controllers, Swagger, DI, CORS
```

### Frontend — Standalone Components (Angular 17+)

- Componentes standalone sem NgModules
- `provideHttpClient()` para chamadas à API
- Estilos SCSS por componente
- Serviço centralizado `UrlService` para todos os endpoints

### CORS

- **Development:** `http://localhost:4200`, `http://localhost:3000`, `http://localhost:5156`
- **Production:** domínios configurados em `Program.cs`

---

## Funcionalidades

- Encurtamento de URL com código Base62 (Hashids)
- Alias personalizado com validação de duplicidade
- Redirecionamento HTTP 302
- Listagem de URLs com dados reais do PostgreSQL
- Consulta individual por código curto
- Motor sincronizado para geração concorrente de códigos
- Containerização completa (API + Frontend + Banco)
- Swagger/OpenAPI em ambiente Development
- Conexão externa ao banco via DBeaver (porta 5433)
- Testes unitários com xUnit

---

## Troubleshooting

### "URL não encontrada" no frontend

- Confirme que a API está rodando: `curl http://localhost:8080/health`
- Verifique se `UrlService.apiUrl` aponta para `http://localhost:8080`
- Confira os logs de CORS: `docker compose logs api`

### Containers não iniciam

```bash
docker compose logs api
docker compose logs db
docker compose down -v
docker compose up --build
```

### Erro de conexão no DBeaver

- Container `encurtadorurl-db` deve estar **Up (healthy)**
- Use a porta **5433**, não 5432
- Credenciais: `encurtador` / `encurtador123`

### Porta já em uso

Altere as portas no `docker-compose.yml`:

```yaml
ports:
  - "8081:8080"   # API
  - "4201:80"     # Frontend
  - "5434:5432"   # PostgreSQL
```

---

## Dependências Principais

**Backend:**
- .NET 8, ASP.NET Core
- Entity Framework Core
- PostgreSQL (Npgsql) e SQLite
- Hashids.net
- Swashbuckle (Swagger)

**Frontend:**
- Angular 17
- RxJS 7.8
- TypeScript 5.2
- SCSS

---

## Contribuindo

1. Crie uma branch: `git checkout -b feature/sua-feature`
2. Commit suas mudanças: `git commit -am 'Add nova feature'`
3. Push: `git push origin feature/sua-feature`
4. Abra um Pull Request

---

## Licença

Este projeto está sob licença MIT.

---

**Desenvolvido com .NET 8 + Angular 17 + PostgreSQL**