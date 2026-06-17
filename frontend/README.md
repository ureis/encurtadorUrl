# Frontend - Encurtador de URLs

Aplicação Angular 17 standalone para consumir a API de encurtamento de URLs.

## 📋 Estrutura

```
frontend/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── url-shortener/           # Componente para criar URLs
│   │   │   └── url-list/                # Componente para listar URLs
│   │   ├── services/
│   │   │   └── url.service.ts           # Serviço da API
│   │   ├── app.component.*              # Root component
│   │   ├── app.routes.ts                # Configuração de rotas
│   │   └── app.component.scss           # Estilos do root
│   ├── index.html                       # HTML principal
│   ├── main.ts                          # Entry point
│   └── styles.scss                      # Estilos globais
├── public/
│   └── favicon.ico
├── Dockerfile                           # Build multi-stage
├── nginx.conf                           # Configuração do servidor
├── package.json                         # Dependências npm
├── angular.json                         # Configuração Angular CLI
├── tsconfig.json                        # TypeScript config
├── tsconfig.app.json                    # TypeScript app-specific
└── README.md                            # Este arquivo
```

---

## 🚀 Desenvolvimento

### Pré-requisitos

- Node.js 20+
- npm ou yarn

### Setup

```bash
# Instalar dependências
npm install

# Iniciar dev server
npm start

# Abrir no navegador
# http://localhost:4200
```

### Build para Produção

```bash
# Compilar otimizado
npm run build:prod

# Saída em: dist/encurtador-url-frontend/
```

---

## 🧪 Testes

```bash
# Executar testes unitários
npm test

# Executar testes com cobertura
npm test -- --code-coverage
```

---

## 🐳 Docker

### Build Local

```bash
docker build -t encurtador-frontend:latest .
docker run -p 4200:80 encurtador-frontend:latest
```

### Com Docker Compose (do root)

```bash
docker-compose up frontend
```

---

## 📝 Componentes

### UrlShortenerComponent

Componente principal para criar URLs encurtadas.

**Inputs:**
- `originalUrl`: URL que será encurtada
- `alias` (opcional): Alias personalizado

**Features:**
- Validação de URL
- Loading state
- Tratamento de erros
- Copy to clipboard
- Reset do formulário

### UrlListComponent

Componente para visualizar URLs criadas recentemente.

**Features:**
- Listagem de URLs
- Copiar URL curta
- Abrir URL em nova aba
- Formatação de datas

---

## 🔗 API Integration

### UrlService

Serviço que encapsula as chamadas à API.

```typescript
// Criar URL curta
urlService.createShortUrl({
  originalUrl: 'https://exemplo.com',
  alias: 'meu-link'
}).subscribe(response => {
  console.log(response.shortUrl);
});

// Obter URL original
urlService.getOriginalUrl('DQkv2XA').subscribe(url => {
  console.log(url);
});
```

---

## ⚙️ Configuração

### Alterar URL da API

Edite `src/app/services/url.service.ts`:

```typescript
private apiUrl = 'http://localhost:8080'; // Altere aqui
```

Ou use variáveis de ambiente:

```typescript
private apiUrl = environment.apiUrl;
```

---

## 🎨 Styling

O projeto usa **SCSS** com a seguinte organização:

- `src/styles.scss`: Estilos globais (reset, tipografia, scrollbar)
- `src/app/components/**/*.scss`: Estilos componentes individuais

**Convenções:**
- Mobile-first com media queries
- Grid e Flexbox para layout
- Variáveis de cor reutilizáveis
- BEM (Block Element Modifier) para nomenclatura

---

## 🔒 Segurança

- ✅ Validação de URL no frontend
- ✅ CORS habilitado apenas para origens confiáveis
- ✅ Sanitização de inputs (Angular automático)
- ✅ Sem exposição de dados sensíveis

---

## 📱 Responsividade

A aplicação é totalmente responsiva:

- Desktop (1200px+)
- Tablet (768px - 1199px)
- Mobile (<768px)

---

## 🚨 Possíveis Melhorias

1. **Animations**: Adicionar animações com `@angular/animations`
2. **Material Design**: Integrar Angular Material para UI consistente
3. **State Management**: NgRx ou Akita para gerenciamento de estado
4. **PWA**: Tornar uma Progressive Web App (Service Workers)
5. **Analytics**: Integrar Google Analytics ou similar
6. **Internacionalização**: i18n para múltiplos idiomas

---

## 🔄 Atualizar Dependências

```bash
# Verificar atualizações
npm outdated

# Atualizar todas
npm update

# Atualizar para versão major (cuidado!)
npm install -g @angular/cli@latest
ng update @angular/cli @angular/core
```

---

## 📚 Recursos Úteis

- [Angular Docs](https://angular.io/docs)
- [RxJS Docs](https://rxjs.dev)
- [TypeScript Docs](https://www.typescriptlang.org/docs)
- [MDN Web Docs](https://developer.mozilla.org)

---

## 🐛 Debugging

### DevTools

```bash
# Angular DevTools Chrome Extension
# Acesse: chrome://extensions

# Inspect Network Requests
# DevTools → Network → Filter por "api"
```

### Logs

```typescript
// No serviço
console.log('Request:', request);
console.log('Response:', response);
```

---

## 📄 Licença

MIT
