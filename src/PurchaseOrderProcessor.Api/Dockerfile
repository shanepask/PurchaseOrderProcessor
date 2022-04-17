#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PurchaseOrderProcessor.Api/PurchaseOrderProcessor.Api.csproj", "src/PurchaseOrderProcessor.Api/"]
COPY ["src/PurchaseOrderProcessor.Domain/PurchaseOrderProcessor.Domain.csproj", "src/PurchaseOrderProcessor.Domain/"]
COPY ["src/PurchaseOrderProcessor.Infrastructure/PurchaseOrderProcessor.Infrastructure.csproj", "src/PurchaseOrderProcessor.Infrastructure/"]
RUN dotnet restore "src/PurchaseOrderProcessor.Api/PurchaseOrderProcessor.Api.csproj"
COPY . .
WORKDIR "/src/src/PurchaseOrderProcessor.Api"
RUN dotnet build "PurchaseOrderProcessor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PurchaseOrderProcessor.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PurchaseOrderProcessor.Api.dll"]