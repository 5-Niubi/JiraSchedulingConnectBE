#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "AlgorithmService.csproj"
RUN dotnet build "AlgorithmService.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "AlgorithmService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=publish /app/publish .
EXPOSE 5000


ENTRYPOINT ["dotnet", "AlgorithmService.dll"]