# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["babbly-like-service.csproj", "./"]
RUN dotnet restore "babbly-like-service.csproj"
COPY . .
RUN dotnet build "babbly-like-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "babbly-like-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "babbly-like-service.dll"]