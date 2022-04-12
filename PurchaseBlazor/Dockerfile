FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PurchaseBlazor/PurchaseBlazor.csproj", "PurchaseBlazor/"]
COPY ["PurchaseShared/PurchaseShared.csproj", "PurchaseShared/"]
RUN dotnet restore "PurchaseBlazor/PurchaseBlazor.csproj"
COPY . .
WORKDIR "/src/PurchaseBlazor"
RUN dotnet build "PurchaseBlazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PurchaseBlazor.csproj" -c Release -o /app/publish

####FROM base AS final
####WORKDIR /app
####COPY --from=publish /app/publish .
####ENTRYPOINT ["dotnet", "PurchaseBlazor.dll"]

##
# Specify the image that will be used  
# This will be pulled from Docker Image Registry  
FROM nginx:alpine  
# Environment variable set for Blazor.  
# The enviroment variable is used in nginx   
# configuration file to set up the headers.  
ENV BLAZOR_ENVIRONMENT=Production  
# Set the working directory in the image  
WORKDIR /usr/share/nginx/html  
# Copy the publish application to the workdir in image  
#COPY bin/Release/net5.0/browser-wasm/publish/wwwroot  ./  
COPY --from=publish /app/publish/wwwroot  ./ 
# Copy the nginx configuration file  
COPY PurchaseBlazor/nginx.conf /etc/nginx/nginx.conf  
# Copy the start script  
COPY PurchaseBlazor/start.sh ./  
# This command is optional,   
# it is just used to make sure all the EOL unix compliant  
RUN sed -i -e "s/\r$//" start.sh  
# The command that will be executed when the container starts  
CMD /bin/sh ./start.sh
