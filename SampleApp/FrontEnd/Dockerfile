FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FrontEnd/FrontEnd.csproj FrontEnd/
RUN dotnet restore FrontEnd/FrontEnd.csproj
COPY . .
WORKDIR /src/FrontEnd
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8081
ENTRYPOINT ["dotnet", "FrontEnd.dll"]
