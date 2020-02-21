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

## Creating a Docker Image locally

A Docker file has been created to allow to build and run the image in your preferred server. Before building your image ensure that your 'appsettings.json' is configured correctly.

***
**NOTE**
This docker build image doesn't include the "MySQL" database with a user with `Admin` role and an Application Client that can consume the API.
***

Remember to change the **EXPOSE** ports in the `Dockerfile` if the default ports are taken (80 and 443).
The following commands assumes that you are in the root directory of the application. Don't forget the **"."** at the end of the first command!
* The image created will be called: `h2020.ipmdecisions.identityproviderservice`
* The container created will be called `IDP` and will be running in the port `80`
* The command bellow assumes that the URL port `H2020.IPMDecisions.IDP.API\Properties\launchSettings.json` is 5000
```Console
docker build --rm --pull -f ".\H2020.IPMDecisions.IDP.API\Docker\Dockerfile" -t "h2020ipmdecisions\identityproviderservice:latest" --build-arg URL_PORT=5000 --build-arg BUILDER_VERSION=0.0.1 .

docker run  -d -p 443:443/tcp -p 80:5000/tcp --name IDP h2020.ipmdecisions.identityproviderservice:latest 
```
Now you should be able to user your API in the docker container. Try to navigate to: `http://localhost/swagger/index.html`

## Deployment with Docker Compose

You can deploy the Identity Provider Service API, including a MySQL database with test data and a phpMyAdmin UI to manage the database, using a docker compose.
A file called `docker-compose.yml` is located in the following folder `H2020.IPMDecisions.IDP.API\Docker`. 
To run the docker compose, navigate to the following folder `H2020.IPMDecisions.IDP.API\Docker` and run the following command:

```console
dotnet-compose up -d
```

If no data have been modified in the `docker-compose.yml` the solution will be working in the URL `localhos:8086`, so you can check that the API works navigating to `http://localhost:8086/swagger/index.html`

The docker compose file will also load data into the database. Please read more about this in the [ReadMe.md](H2020.IPMDecisions.IDP.API\Docker\MySQL_Init_Script\ReadMe.md) file located in `H2020.IPMDecisions.IDP.API\Docker\MySQL_Init_Script`.

To help modifying the default data, a postman collection has been created with the calls needed. Also, please note that if a new Client is added into the database, this one will be needed added into the `H2020.IPMDecisions.IDP.API\appsettings.Development.json`. You can achieve this modifying the `docker-compose.yml` file and running `dotnet-compose up -d` again. 


## Versioning

For the versions available, see the [tags on this repository](https://github.com/H2020-IPM-Decisions/IdentityProviderService/tags). 

## Authors

* **ADAS Modelling and Informatics** - *Initial work* - [ADAS](https://www.adas.uk/)