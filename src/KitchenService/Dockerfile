FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/KitchenService/KitchenService.csproj", "src/KitchenService/"]
COPY ["src/Common/Common.csproj", "src/Common/"]
RUN dotnet restore "src/KitchenService/KitchenService.csproj"
COPY . .
WORKDIR "/src/src/KitchenService"
RUN dotnet build "KitchenService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KitchenService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KitchenService.dll"]