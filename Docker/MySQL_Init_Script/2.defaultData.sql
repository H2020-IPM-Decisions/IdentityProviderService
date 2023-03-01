USE `H2020.IPMDecisions.IDP`;

INSERT INTO `ApplicationClient` (`Base64Secret`, `Name`, `ApplicationClientType`, `Enabled`, `RefreshTokenLifeTime`, `Id`, `JWTAudienceCategory`) VALUES
('VdzZzA3lxu-P4krX0n8APfISzujFFKAGn6pbGCd3so8', 'My Test Client', 0, 1, 90, '08d7aa5b-e23c-496e-8946-6d8af6b98dd6', 'https://testclient.com');

INSERT INTO `Role` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('dd2f7616-65b7-4456-a6dd-37b25a2c050d', 'Admin', 'ADMIN', 'd4c227a1-bed8-4cbb-b0af-cabec5b6d8a6');

INSERT INTO `User` (`Id`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`, `RegistrationDate`) VALUES
('380f0a69-a009-4c34-8496-9a43c2e069be', 'admin@test.com', 'ADMIN@TEST.COM', 'admin@test.com', 'ADMIN@TEST.COM', 1, 'AQAAAAEAACcQAAAAEJfYkkq/P/d3+GZjsDeGS4HCjukw0vJNN9fg0mdDBzVbKEdNCHMc8bTtUyo/UGVsSw==', 'KYK2EHHFUNXK62Z7E7H7BNCAABMUL5PE', '963515b9-8e57-4a9c-9286-d86dcf9e5fa0', NULL, 0, 0, NULL, 1, 0, '2020-01-01 00:00:00');

INSERT INTO `UserRole` (`UserId`, `RoleId`) VALUES ('380f0a69-a009-4c34-8496-9a43c2e069be', 'dd2f7616-65b7-4456-a6dd-37b25a2c050d');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20200128154224_Initial', '3.1.1'),
('20200131164105_ApplicationClients', '3.1.1'),
('20200203124353_ChangeIdToGuid', '3.1.1'),
('20200203142730_ChangeToInteger', '3.1.1'),
('20200205164837_AddUrlToClient', '3.1.1'),
('20200212164231_RefreshTokens', '3.1.1'),
('20200213102040_ChangeFKRefreshToken', '3.1.1'),
('20200213102409_fixingMissingColum', '3.1.1'),
('20200213132720_addExpireRefreshToken', '3.1.1'),
('20200604151512_ChangeColumnName', '3.1.1'),
('20200729155000_AddRegistrationDateToUser', '3.1.1'),
('20210521101912_AddLastAccessColumn', '3.1.1'),
('20210524140407_InactiveEmailsColumn', '3.1.14');