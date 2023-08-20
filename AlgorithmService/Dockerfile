#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Set the ASPNETCORE_ENVIRONMENT build argument
ARG ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AlgorithmService/AlgorithmServiceServer.csproj", "AlgorithmService/"]
RUN dotnet restore "AlgorithmService/AlgorithmServiceServer.csproj"

COPY . .
WORKDIR "/src/AlgorithmService"

COPY ["SharedSettings/appsettings.json", "/src/AlgorithmService/"]
RUN if [ "$ASPNETCORE_ENVIRONMENT" != "Production" ]; then \
        cp "SharedSettings/appsettings.$ASPNETCORE_ENVIRONMENT.json" "/src/AlgorithmService/"; \
    fi

RUN dotnet build "AlgorithmServiceServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AlgorithmServiceServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlgorithmServiceServer.dll"]