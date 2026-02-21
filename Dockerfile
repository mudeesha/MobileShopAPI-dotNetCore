FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj file from the MobileShopAPI subfolder
COPY MobileShopAPI/*.csproj ./MobileShopAPI/
WORKDIR /src/MobileShopAPI
RUN dotnet restore

# Copy everything else and build
COPY MobileShopAPI/. .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MobileShopAPI.dll"]