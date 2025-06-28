# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Claims.csproj", "."]
RUN dotnet restore "./Claims.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Claims.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Claims.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Claims.dll"]
