FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["LegalAiAr.sln", "LegalAiAr.sln"]
COPY ["global.json", "global.json"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["src/shared/LegalAiAr.Core/LegalAiAr.Core.csproj", "src/shared/LegalAiAr.Core/"]
COPY ["src/shared/LegalAiAr.Infrastructure/LegalAiAr.Infrastructure.csproj", "src/shared/LegalAiAr.Infrastructure/"]
COPY ["src/api/LegalAiAr.Application/LegalAiAr.Application.csproj", "src/api/LegalAiAr.Application/"]
COPY ["src/api/LegalAiAr.Api/LegalAiAr.Api.csproj", "src/api/LegalAiAr.Api/"]

RUN dotnet restore "src/api/LegalAiAr.Api/LegalAiAr.Api.csproj"

COPY . .

RUN dotnet publish "src/api/LegalAiAr.Api/LegalAiAr.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LegalAiAr.Api.dll"]
