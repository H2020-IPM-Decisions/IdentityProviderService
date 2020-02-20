## INITIAL DATA

SQL scrip will add the following default data into your database. **Please modify before or after loading the data into your database.**

* A DB User with read/write access to the H2020.IDPDecisions.IDP database with the following 
* An Application client with the following information needed for modify inital information by API calls:
  * ClientID: 08d7aa5b-e23c-496e-8946-6d8af6b98dd6
  * ClientSecret: VdzZzA3lxu-P4krX0n8APfISzujFFKAGn6pbGCd3so8
  * URL: https://testclient.com 
* An `Admin` role needed for manage access secure API end points
* An User with `Admin` rights. The login details for this user are:
  * Username: initialadminuser
  * Password: Password1!

You can modify any of this information changing the scripts or using the postman collection created for this purpose. 

---
**Please note that when creating a new client, the new URL this will need to be added into the `H2020.IPMDecisions.IDP.API\appsettings.json\JwtSettings\ValidAudiencesUrls` strings**

**You can use *`docker-compose up -d`* to achieve the above**