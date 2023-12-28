#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameInterface.CrPati/Tgs.GameInterface.CrPati.csproj", "Tgs.GameInterface.CrPati/"]
COPY ["Tgs.GameInterface.CrPati.Core/Tgs.GameInterface.CrPati.Core.csproj", "Tgs.GameInterface.CrPati.Core/"]
RUN dotnet restore "./Tgs.GameInterface.CrPati/./Tgs.GameInterface.CrPati.csproj"
COPY . .
WORKDIR "/src/Tgs.GameInterface.CrPati"
RUN dotnet build "./Tgs.GameInterface.CrPati.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameInterface.CrPati.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.CrPati.dll"]