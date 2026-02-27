FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AuthSystem.API/AuthSystem.API.csproj", "AuthSystem.API/"]
COPY ["AuthSystem.Application/AuthSystem.Application.csproj", "AuthSystem.Application/"]
COPY ["AuthSystem.Domain/AuthSystem.Domain.csproj", "AuthSystem.Domain/"]
COPY ["AuthSystem.Infrastructure/AuthSystem.Infrastructure.csproj", "AuthSystem.Infrastructure/"]
RUN dotnet restore "AuthSystem.API/AuthSystem.API.csproj"
COPY . .
WORKDIR "/src/AuthSystem.API"
RUN dotnet build "AuthSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthSystem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthSystem.API.dll"]
