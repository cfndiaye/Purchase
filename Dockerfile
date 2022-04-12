﻿FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Purchase/Purchase.csproj", "Purchase/"]
COPY ["PurchaseShared/PurchaseShared.csproj", "PurchaseShared/"]
RUN dotnet restore "Purchase/Purchase.csproj"
COPY . .
WORKDIR "/src/Purchase"
RUN dotnet build "Purchase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Purchase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Purchase.dll"]
