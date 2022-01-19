FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
COPY . ./
RUN dotnet publish "PurchaseBlazor.csproj" -c Release -o output

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=base /app/output/wwwroot .
