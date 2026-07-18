FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY EaaSAPI/*.csproj ./EaaSAPI/
RUN dotnet restore ./EaaSAPI/EaaSAPI.csproj

COPY . .

RUN dotnet publish ./EaaSAPI/EaaSAPI.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app .

ENTRYPOINT ["dotnet", "EaaSAPI.dll"]
