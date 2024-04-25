FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ARG URL_EXPOSE_PORT=80
ARG URL_EXPOSE_PORT_HTTPS=443
ARG BUILDER_VERSION=latest
ARG RELEASE_DATE="2020-02-20"

LABEL h2020.ipmdecisions.idp.version=${BUILDER_VERSION} \
    vendor="RSK ADAS Ltd" \
    h2020.ipmdecisions.idprelease-date=${RELEASE_DATE}

WORKDIR /app
EXPOSE ${URL_EXPOSE_PORT}
EXPOSE ${URL_EXPOSE_PORT_HTTPS}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj", "H2020.IPMDecisions.IDP.API/"]
COPY ["H2020.IPMDecisions.IDP.API/appsettingsTemplate.json", "H2020.IPMDecisions.IDP.API/appsettings.json"]
RUN dotnet restore "H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj"

COPY . .
WORKDIR /src/H2020.IPMDecisions.IDP.API
RUN dotnet build "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/build

# WORKDIR /src/H2020.IPMDecisions.IDP.Tests
# RUN dotnet test "H2020.IPMDecisions.IDP.Tests.csproj" --filter Category!=Docker

WORKDIR /src/H2020.IPMDecisions.IDP.API
FROM build AS publish
RUN dotnet publish "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/publish

FROM base AS final
ARG URL_PORT=5000
ARG URL_PORT_HTTPS=443

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "H2020.IPMDecisions.IDP.API.dll"]