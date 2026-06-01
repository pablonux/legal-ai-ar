# MCR only: Docker Hub is rate-limited and public.ecr.aws is blocked in AAAS/Kaniko builds.
FROM mcr.microsoft.com/devcontainers/javascript-node:20-bookworm AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration=development

# Node serve runs correctly as UID 1000 (Knative securityContext). Azure Linux nginx does not.
FROM mcr.microsoft.com/devcontainers/javascript-node:20-bookworm AS runtime
WORKDIR /app
COPY --from=build /app/dist/legal-ai-ar/browser ./dist
RUN npm install -g serve@14.2.4 \
    && echo -n ok > ./dist/health \
    && chown -R node:node /app
USER node
EXPOSE 8081
CMD ["serve", "-s", "dist", "-l", "8081"]
