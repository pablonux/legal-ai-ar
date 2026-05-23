# Build stage — context: backend/
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["LegalAiAr.sln", "LegalAiAr.sln"]
COPY ["global.json", "global.json"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["src/shared/LegalAiAr.Core/LegalAiAr.Core.csproj", "src/shared/LegalAiAr.Core/"]
COPY ["src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj", "src/shared/LegalAiAr.Infrastructure/"]
COPY ["src/workers/LegalAiAr.Worker.Indexer/LegalAiAr.Worker.Indexer.csproj", "src/workers/LegalAiAr.Worker.Indexer/"]

COPY . .

RUN for i in 1 2 3; do dotnet publish "src/workers/LegalAiAr.Worker.Indexer/LegalAiAr.Worker.Indexer.csproj" -c Release -o /app/publish && break || (echo "Publish attempt $i failed, retrying in 15s..." && sleep 15); done

# Runtime stage — SQL, Blob, AI Search, Storage Queues, Azure OpenAI
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY certificates/cacert.pem /usr/local/share/ca-certificates/corporate-ca.crt
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "LegalAiAr.Worker.Indexer.dll"]
