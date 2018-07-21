FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY MessageService/MessageService.csproj MessageService/
RUN dotnet restore MessageService/MessageService.csproj
COPY . .
WORKDIR /src/MessageService
RUN dotnet build MessageService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MessageService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MessageService.dll"]
