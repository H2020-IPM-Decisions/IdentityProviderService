-- phpMyAdmin SQL Dump
-- version 5.0.1
-- https://www.phpmyadmin.net/
--
-- Host: db
-- Generation Time: Feb 18, 2020 at 02:41 PM
-- Server version: 8.0.19
-- PHP Version: 7.4.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `H2020.IPMDecisions.IDP`
--

CREATE DATABASE IF NOT EXISTS `H2020.IPMDecisions.IDP` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE `H2020.IPMDecisions.IDP`;

-- --------------------------------------------------------

--
-- Database User: `ipm_idp_user`
--

CREATE USER `ipm_idp_user`@`%` IDENTIFIED BY 'user_password';
GRANT USAGE ON *.* TO `ipm_idp_user`@`%`;
GRANT SELECT, INSERT, UPDATE, DELETE ON `H2020.IPMDecisions.IDP`.* TO `ipm_idp_user`@`%`;

-- --------------------------------------------------------

--
-- Table structure for table `ApplicationClient`
--

CREATE TABLE `ApplicationClient` (
  `Base64Secret` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ApplicationClientType` int NOT NULL,
  `Enabled` tinyint(1) NOT NULL,
  `RefreshTokenLifeTime` int NOT NULL,
  `Id` char(36) NOT NULL,
  `JWTAudienceCategory` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `ApplicationClient`
--

INSERT INTO `ApplicationClient` (`Base64Secret`, `Name`, `ApplicationClientType`, `Enabled`, `RefreshTokenLifeTime`, `Id`, `JWTAudienceCategory`) VALUES
('VdzZzA3lxu-P4krX0n8APfISzujFFKAGn6pbGCd3so8', 'My Test Client', 0, 1, 90, '08d7aa5b-e23c-496e-8946-6d8af6b98dd6', 'https://testclient.com');

-- --------------------------------------------------------

--
-- Table structure for table `RefreshToken`
--

CREATE TABLE `RefreshToken` (
  `Id` char(36) NOT NULL,
  `ApplicationClientId` char(36) NOT NULL,
  `ProtectedTicket` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` char(36) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `ExpiresUtc` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Role`
--

CREATE TABLE `Role` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `Role`
--

INSERT INTO `Role` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('dd2f7616-65b7-4456-a6dd-37b25a2c050d', 'Admin', 'ADMIN', 'd4c227a1-bed8-4cbb-b0af-cabec5b6d8a6');

-- --------------------------------------------------------

--
-- Table structure for table `RoleClaim`
--

CREATE TABLE `RoleClaim` (
  `Id` int NOT NULL,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `User`
--

CREATE TABLE `User` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  `RegistrationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `User`
--

INSERT INTO `User` (`Id`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`, `RegistrationDate`) VALUES
('380f0a69-a009-4c34-8496-9a43c2e069be', 'admin@test.com', 'ADMIN@TEST.COM', 'admin@test.com', 'ADMIN@TEST.COM', 1, 'AQAAAAEAACcQAAAAEJfYkkq/P/d3+GZjsDeGS4HCjukw0vJNN9fg0mdDBzVbKEdNCHMc8bTtUyo/UGVsSw==', 'KYK2EHHFUNXK62Z7E7H7BNCAABMUL5PE', '963515b9-8e57-4a9c-9286-d86dcf9e5fa0', NULL, 0, 0, NULL, 1, 0, '2020-01-01 00:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `UserClaim`
--

CREATE TABLE `UserClaim` (
  `Id` int NOT NULL,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `UserLogin`
--

CREATE TABLE `UserLogin` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `UserRole`
--

CREATE TABLE `UserRole` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `User`
--
INSERT INTO `UserRole` (`UserId`, `RoleId`) VALUES ('380f0a69-a009-4c34-8496-9a43c2e069be', 'dd2f7616-65b7-4456-a6dd-37b25a2c050d');
-- --------------------------------------------------------

--
-- Table structure for table `UserToken`
--

CREATE TABLE `UserToken` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `__EFMigrationsHistory`
--

CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(95) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `__EFMigrationsHistory`
--

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
('20200729155000_AddRegistrationDateToUser', '3.1.1');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `ApplicationClient`
--
ALTER TABLE `ApplicationClient`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `RefreshToken`
--
ALTER TABLE `RefreshToken`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_RefreshToken_ApplicationClientId` (`ApplicationClientId`),
  ADD KEY `IX_RefreshToken_UserId` (`UserId`);

--
-- Indexes for table `Role`
--
ALTER TABLE `Role`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `RoleNameIndex` (`NormalizedName`);

--
-- Indexes for table `RoleClaim`
--
ALTER TABLE `RoleClaim`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_RoleClaim_RoleId` (`RoleId`);

--
-- Indexes for table `User`
--
ALTER TABLE `User`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  ADD KEY `EmailIndex` (`NormalizedEmail`);

--
-- Indexes for table `UserClaim`
--
ALTER TABLE `UserClaim`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_UserClaim_UserId` (`UserId`);

--
-- Indexes for table `UserLogin`
--
ALTER TABLE `UserLogin`
  ADD PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  ADD KEY `IX_UserLogin_UserId` (`UserId`);

--
-- Indexes for table `UserRole`
--
ALTER TABLE `UserRole`
  ADD PRIMARY KEY (`UserId`,`RoleId`),
  ADD KEY `IX_UserRole_RoleId` (`RoleId`);

--
-- Indexes for table `UserToken`
--
ALTER TABLE `UserToken`
  ADD PRIMARY KEY (`UserId`,`LoginProvider`,`Name`);

--
-- Indexes for table `__EFMigrationsHistory`
--
ALTER TABLE `__EFMigrationsHistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `RoleClaim`
--
ALTER TABLE `RoleClaim`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `UserClaim`
--
ALTER TABLE `UserClaim`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `RefreshToken`
--
ALTER TABLE `RefreshToken`
  ADD CONSTRAINT `FK_RefreshToken_ApplicationClient_ApplicationClientId` FOREIGN KEY (`ApplicationClientId`) REFERENCES `ApplicationClient` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `RoleClaim`
--
ALTER TABLE `RoleClaim`
  ADD CONSTRAINT `FK_RoleClaim_Role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `UserClaim`
--
ALTER TABLE `UserClaim`
  ADD CONSTRAINT `FK_UserClaim_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `UserLogin`
--
ALTER TABLE `UserLogin`
  ADD CONSTRAINT `FK_UserLogin_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `UserRole`
--
ALTER TABLE `UserRole`
  ADD CONSTRAINT `FK_UserRole_Role_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Role` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_UserRole_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `UserToken`
--
ALTER TABLE `UserToken`
  ADD CONSTRAINT `FK_UserToken_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
