FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

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

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj", "H2020.IPMDecisions.IDP.API/"]
RUN dotnet restore "H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj"

COPY . .
WORKDIR /src/H2020.IPMDecisions.IDP.API
RUN dotnet build "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/build

WORKDIR /src/H2020.IPMDecisions.IDP.Tests
RUN dotnet test "H2020.IPMDecisions.IDP.Tests.csproj"

WORKDIR /src/H2020.IPMDecisions.IDP.API
FROM build AS publish
RUN dotnet publish "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/publish

FROM base AS final
ARG URL_PORT=5000
ARG URL_PORT_HTTPS=443

WORKDIR /app
COPY --from=publish /app/publish .

# Uncomment if you have SSL Certificate
#ENV ASPNETCORE_URLS=https://*:${URL_PORT_HTTPS};http://*:${URL_PORT}
ENV ASPNETCORE_URLS=http://*:${URL_PORT}
ENV ConnectionStrings:MySqlDbConnection=""
ENV JwtSettings:TokenLifetimeMinutes="80"
ENV JwtSettings:SecretKey=""
ENV JwtSettings:IssuerServerUrl=""
ENV JwtSettings:ValidAudiencesUrls=""
ENV AllowedHosts="*"
ENV AccessClaims:ClaimTypeName="UserAccessType"
ENV AccessClaims:DefaultUserAccessLevel="Level0"

ENTRYPOINT ["dotnet", "H2020.IPMDecisions.IDP.API.dll"]