FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
COPY . .

ENTRYPOINT ["dotnet", "demoapi.dll"]
