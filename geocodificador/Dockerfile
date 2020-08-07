FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
EXPOSE 5672
COPY bin/Release/netcoreapp3.1/publish/ app/
WORKDIR /app
ENTRYPOINT ["dotnet", "geocodificador.dll"]