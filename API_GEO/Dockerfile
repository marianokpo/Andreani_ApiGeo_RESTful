FROM mcr.microsoft.com/dotnet/core/sdk:3.1
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80
EXPOSE 5001
EXPOSE 5672
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh