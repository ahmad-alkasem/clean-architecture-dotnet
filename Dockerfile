FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/WebApi/WebApi.csproj src/WebApi/
RUN dotnet restore src/WebApi/WebApi.csproj

COPY src/ src/
RUN dotnet publish src/WebApi/WebApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll"]
