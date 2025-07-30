FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MenuService/MenuService.csproj", "src/MenuService/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
RUN dotnet restore "src/MenuService/MenuService.csproj"
COPY . .
WORKDIR "/src/src/MenuService"
RUN dotnet build "MenuService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MenuService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MenuService.dll"]

