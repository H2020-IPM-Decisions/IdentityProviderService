FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj", "H2020.IPMDecisions.IDP.API/"]
COPY ["H2020.IPMDecisions.IDP.Core/H2020.IPMDecisions.IDP.Core.csproj", "H2020.IPMDecisions.IDP.Core/"]
COPY ["H2020.IPMDecisions.IDP.Data/H2020.IPMDecisions.IDP.Data.csproj", "H2020.IPMDecisions.IDP.Data/"]
COPY ["H2020.IPMDecisions.IDP.Tests/H2020.IPMDecisions.IDP.Tests.csproj", "H2020.IPMDecisions.IDP.Tests/"]

RUN dotnet restore "H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj"
COPY . .
WORKDIR "/src/H2020.IPMDecisions.IDP.API"
RUN dotnet build "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/build

WORKDIR "/src/H2020.IPMDecisions.IDP.Tests"
RUN dotnet test "H2020.IPMDecisions.IDP.Tests.csproj"

WORKDIR "/src/H2020.IPMDecisions.IDP.API"
FROM build AS publish
RUN dotnet publish "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS="http://0.0.0.0:5000"

ENTRYPOINT ["dotnet", "H2020.IPMDecisions.IDP.API.dll"]