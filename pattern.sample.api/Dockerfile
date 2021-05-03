#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["pattern.sample.api/pattern.sample.api.csproj", "pattern.sample.api/"]
RUN dotnet restore "pattern.sample.api/pattern.sample.api.csproj"
COPY . .
WORKDIR "/src/pattern.sample.api"
RUN dotnet build "pattern.sample.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pattern.sample.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pattern.sample.api.dll"]