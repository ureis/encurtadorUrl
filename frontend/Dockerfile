# Estágio 1: Build do Angular
FROM node:20-alpine AS build
WORKDIR /app

# Copiar package.json e instalar dependências
COPY package.json package-lock.json* ./
RUN npm install

# Copiar código-fonte e compilar
COPY . .
RUN npm run build:prod

# Estágio 2: Servir com Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Install wget for Docker health checks
RUN apk add --no-cache wget

# Copiar configuração do Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Remover arquivos padrão e copiar build
RUN rm -rf ./*
COPY --from=build /app/dist/encurtador-url-frontend .

# Expor porta
EXPOSE 80

# Comando de inicialização
CMD ["nginx", "-g", "daemon off;"]
