# H2020 IPM Decisions Identity Provider Service

APS.NET Core service in charge to authenticate and authorise users and clients that can access the H2020 IPM Decisions API Gateway.

## Branch structure

The project development will follow [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) branching model where active development will happen in the develop branch. Master branch will only have release ans stable versions of the service.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites

The Identity Provider Service uses the following technologies:

```
ASP.NET Core 3.1
MySql 8
```
1. [Install](https://dotnet.microsoft.com/download/dotnet-core/3.1) the required .NET Core SDK.

2. [Install](https://dev.mysql.com/downloads/installer/) the required MySql database.

### Getting the solution

The easiest way to get the sample is by cloning the samples repository with [git](https://git-scm.com/downloads), using the following instructions. As explained above, the develop branch is an active development branch, so checkout this branch to get the latest version.

```console
git clone https://github.com/H2020-IPM-Decisions/IdentityProviderService.git

cd IdentityProviderService

git checkout develop
```

You can also [download the repository as a zip](https://github.com/H2020-IPM-Decisions/IdentityProviderService/archive/master.zip).

### How to build and download dependencies

You can build the tool using the following commands. The instructions assume that you are in the root of the repository. You will need to run the commands from the API project.

```console
cd H2020.IPMDecisions.IDP.API

cp appsettingsTemplate.json appsettings.json

dotnet build
```

### How to connect and start the database

Open file `H2020.IPMDecisions.IDP.API\appsettings.json` and change the json object `ConnectionStrings\MySqlDbConnection` with your MySql instance.
The following command will create a database and add all the tables necessary to run the solution.
The instructions assume that you are in the **API project** of the repository.

```console
dotnet ef database update
```

Open your MySQL instance and check that the database has been created and tables added.

If new tables are added to the project, to add new migrations to the database run this commands:

```console
dotnet ef migrations add YourMessage --project ..\H2020.IPMDecisions.IDP.Data\H2020.IPMDecisions.IDP.Data.csproj
dotnet ef database update
```

### How to set-up the JsonWebToken (JWT) provider

Open file `H2020.IPMDecisions.IDP.API\appsettings.json` and change the json section `JwtSettings` with the your server information.
1. TokenLifetimeMinutes: Valid lifetime of the JWT in minutes.
2. SecretKey: This parameter holds a secret key to sign the JWT. Your resource API should have the same secret in the JWT properties.
3. IssuerServerUrl: This parameter holds who is issuing the certificate, usually will be this server. Your resource API should have the same issuer url in the JWT properties.
4. ValidAudiencesUrls: This parameter holds which clients URLs can use this IDP service. The different URLS should be separated by a semicolon **";"**. At least one of the client URL should be added into your resource API JWT properties.

### How to run the project

You can build the tool using the following commands. The instructions assume that you are in the root of the repository.
As explained above, the develop branch is an active development branch.

```console
dotnet run
```
The solution will start in the default ports (https://localhost:5001;http://localhost:5000) defined in the file. `H2020.IPMDecisions.IDP.API\Properties\launchSettings.json`

### How to set-up CORS policy

Open file `H2020.IPMDecisions.IDP.API\appsettings.json` and change the json section `AllowedHosts` with the host that you would like to allow to consume the API.
The different URLS should be separated by a semicolon **";"**. If you would like to allow any origin, write an asterisk on the string **"*"**

### API Discovery
A [Swagger UI](https://swagger.io/) has been added to help the discovery of the API endpoints and the data transfer models (DTOs) needed for the use of the API. To access the User Interface, navigate to https://localhost:5001/swagger

### How to interact with the project

A [postman](https://www.getpostman.com/) collection has been created with the current end points available. 
Once the project is running, open the file `H2020.IPMDecisions.IDP.Postman.json` using postman and you will be able to test the application.

## Versioning

For the versions available, see the [tags on this repository](https://github.com/H2020-IPM-Decisions/IdentityProviderService/tags). 

## Authors

* **ADAS Modelling and Informatics** - *Initial work* - [ADAS](https://www.adas.uk/)