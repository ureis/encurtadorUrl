# URL Shortener Challenge - .NET Web API

Esta é uma solução robusta e estruturada para um sistema encurtador de URLs, desenvolvida em **.NET 8** utilizando os princípios da **Clean Architecture**, **SOLID** e **Clean Code**.

## 📌 Funcionalidades
* **Encurtamento de URL:** Recebe uma URL original e gera um código curto ofuscado utilizando Base62 e Hashids.
* **Alias Personalizado:** Permite definir um nome curto específico, validando a sua disponibilidade.
* **Redirecionamento:** Redireciona o utilizador da URL curta para a URL original com os status HTTP adequados.
* **Motor Sincronizado:** Mecanismo rigoroso para processar apenas uma geração por vez, garantindo a integridade dos dados sob alta concorrência.

---

## 🏗 Arquitetura do Projeto

A solução foi dividida em camadas para garantir a separação de responsabilidades, testabilidade e facilidade de manutenção:

* **`UrlShortener.Api`**: Camada de entrada (Controllers, DTOs e configuração da Injeção de Dependência).
* **`UrlShortener.Application`**: Casos de uso e interfaces de serviços do sistema.
* **`UrlShortener.Domain`**: Entidades de negócio e interfaces dos repositórios (Core da aplicação, livre de dependências externas).
* **`UrlShortener.Infrastructure`**: Implementação do acesso a dados (Entity Framework Core com SQLite) e serviços utilitários (Hashids).
* **`UrlShortener.Tests`**: Testes unitários e de concorrência com xUnit.

---

## 🛠️ Escolhas de Design e Trade-offs

### 1. Persistência com SQLite
* **Escolha:** Utilização do Entity Framework Core configurado com um banco de dados SQLite local (`encurtadorurl.db`).
* **Justificativa:** Para facilitar a avaliação do desafio, eliminou-se a necessidade de o avaliador configurar instâncias externas de base de dados (como SQL Server ou Docker). O padrão *Repository* foi aplicado rigidamente, o que permite alterar o provedor de dados para qualquer banco relacional ou NoSQL alterando apenas uma linha no arquivo `Program.cs`.

## Como executar

**Executar localmente (dotnet)**

1. Abra um terminal na raiz do repositório.
2. Restaurar dependências e executar o projeto API:

```powershell
dotnet restore
dotnet run --project EncurtadorURL.Api/EncurtadorURL.Api.csproj
```

Por padrão em ambiente de desenvolvimento o Swagger estará disponível em `http://localhost:5156/swagger` (veja `EncurtadorURL.Api/Properties/launchSettings.json`).

**Executar com Docker**

1. Construir a imagem (Development para habilitar Swagger no container):

```powershell
docker build --build-arg ENVIRONMENT=Development -t encurtadorurl:latest -f EncurtadorURL.Api/Dockerfile .
```

2. Executar o container:

```powershell
docker run -p 8080:8080 encurtadorurl:latest
```

Abra `http://localhost:8080/swagger` para acessar o Swagger quando o ambiente for `Development`.

Se quiser habilitar o `Development` sem rebuildar a imagem, passe a variável de ambiente ao executar:

```powershell
docker run -e ASPNETCORE_ENVIRONMENT=Development -p 8080:8080 encurtadorurl:latest
```

**Persistência do SQLite**

O projeto usa um arquivo SQLite chamado `encurtadorurl.db` criado no diretório de trabalho da aplicação (`/app` dentro do container). Para persistir os dados no host, monte um volume:

```powershell
# no PowerShell (Windows)
docker run -v ${PWD}\\data:/app -p 8080:8080 encurtadorurl:latest
# ou em Linux/macOS
# docker run -v $(pwd)/data:/app -p 8080:8080 encurtadorurl:latest
```

**Notas rápidas**
- O Swagger é habilitado apenas quando o ambiente é `Development` (ver `Program.cs`).
- Endpoint para criar URL curta: `POST /api/urls` (body: `originalUrl`, `alias`).
- Endpoint para redirecionamento: `GET /{shortCode}`.


### 2. Motor de Geração Sincronizado (Concorrência)
* **Escolha:** O serviço `ShortCodeGenerator` foi registado como `Singleton` e utiliza um mecanismo de `lock` interno na geração de códigos.
* **Justificativa:** O requisito determinava que o motor processasse apenas uma requisição por vez de forma sincronizada. O `lock` garante exclusão mútua ao nível de *thread*, impedindo que colisões ocorram na geração concorrente baseada em identificadores numéricos sequenciais.

### 3. Ofuscação com Base62 + Hashids
* **Escolha:** Utilização da biblioteca `Hashids.net` configurada com o alfabeto Base62.
* **Justificativa:** Evita a exposição de IDs sequenciais do banco de dados na URL pública, adicionando uma camada de segurança por ofuscação através de algoritmos matemáticos reversíveis e performáticos.

---

## 🔮 O Que Faria Diferente com Mais Tempo?

1.  **Cache com Redis:** Encurtadores de URL possuem uma carga de leitura (redirecionamento) infinitamente superior à de escrita. Adicionar um cache distribuído para resolver os códigos curtos pouparia idas desnecessárias à base de dados.
2.  **Motor de Identificadores Distribuídos:** O uso de `lock` local funciona perfeitamente para ambientes de uma única instância. Num cenário real de escala global com múltiplas instâncias da API, o `lock` em memória causaria gargalos. Substituiria por uma abordagem descentralizada como o algoritmo *Snowflake ID* ou pré-alocação de blocos de IDs por instância.
3.  **Métricas e Mensageria:** Implementaria o envio de eventos de clique de forma assíncrona utilizando **RabbitMQ**. Isso permitiria contabilizar estatísticas de acesso sem bloquear a resposta de redirecionamento do utilizador.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

### Passo a Passo

1. Clonar o repositório.
2. Na raiz do projeto, execute o comando para restaurar as dependências e compilar a solução:
   ```bash
   dotnet build