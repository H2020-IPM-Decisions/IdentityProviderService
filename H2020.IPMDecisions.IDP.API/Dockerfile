FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj", "H2020.IPMDecisions.IDP.API/"]
RUN dotnet restore "H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj"
COPY . .
WORKDIR "/src/H2020.IPMDecisions.IDP.API"
RUN dotnet build "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "H2020.IPMDecisions.IDP.API.dll"]
