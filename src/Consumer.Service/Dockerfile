FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Consumer.Service/Consumer.Service.csproj", "src/Consumer.Service/"]
COPY ["src/Common/Common.csproj", "src/Common/"]

RUN dotnet restore "src/Consumer.Service/Consumer.Service.csproj"

COPY . .

WORKDIR "/src/src/Consumer.Service"

RUN dotnet build "Consumer.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.Service.dll"]