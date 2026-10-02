FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY thomasmack.dev.csproj ./
RUN dotnet restore thomasmack.dev.csproj

COPY . .
RUN dotnet publish thomasmack.dev.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --gid 1000 app \
    && useradd --uid 1000 --gid 1000 --no-create-home --shell /usr/sbin/nologin app

COPY --from=build /app ./

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "thomasmack.dev.dll"]
