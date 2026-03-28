FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BackEnd/BackEnd.csproj BackEnd/
RUN dotnet restore BackEnd/BackEnd.csproj
COPY . .
WORKDIR /src/BackEnd
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 5000
ENTRYPOINT ["dotnet", "BackEnd.dll"]
