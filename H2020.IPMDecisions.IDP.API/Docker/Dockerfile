FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

ARG URL_PORT
ARG BUILDER_VERSION=0.0.1

LABEL h2020.ipmdecisions.idp.version=${BUILDER_VERSION} \
    vendor="RSK ADAS Ltd" \
    h2020.ipmdecisions.idprelease-date="2020-02-20"

WORKDIR /app
EXPOSE ${URL_PORT}
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj", "H2020.IPMDecisions.IDP.API/"]
COPY ["H2020.IPMDecisions.IDP.Core/H2020.IPMDecisions.IDP.Core.csproj", "H2020.IPMDecisions.IDP.Core/"]
COPY ["H2020.IPMDecisions.IDP.Data/H2020.IPMDecisions.IDP.Data.csproj", "H2020.IPMDecisions.IDP.Data/"]
COPY ["H2020.IPMDecisions.IDP.Tests/H2020.IPMDecisions.IDP.Tests.csproj", "H2020.IPMDecisions.IDP.Tests/"]

RUN dotnet restore "H2020.IPMDecisions.IDP.API/H2020.IPMDecisions.IDP.API.csproj"
COPY . .
WORKDIR /src/H2020.IPMDecisions.IDP.API
RUN dotnet build "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/build

WORKDIR /src/H2020.IPMDecisions.IDP.Tests
#RUN dotnet test "H2020.IPMDecisions.IDP.Tests.csproj"

WORKDIR /src/H2020.IPMDecisions.IDP.API
FROM build AS publish
RUN dotnet publish "H2020.IPMDecisions.IDP.API.csproj" -c Release -o /app/publish

FROM base AS final
ARG URL_PORT
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://*:${URL_PORT}
ENV ConnectionStrings:MySqlDbConnection=""
ENV JwtSettings:TokenLifetimeMinutes="80"
ENV JwtSettings:SecretKey=""
ENV JwtSettings:IssuerServerUrl=""
ENV JwtSettings:ValidAudiencesUrls=""
ENV AllowedHosts="*"

ENTRYPOINT ["dotnet", "H2020.IPMDecisions.IDP.API.dll"]