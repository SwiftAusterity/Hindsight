﻿/*
Deployment script for Data

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Data"
:setvar DefaultFilePrefix "Data"
:setvar DefaultDataPath ""
:setvar DefaultLogPath ""

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [master];


GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Creating $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)] COLLATE SQL_Latin1_General_CP1_CI_AS
GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                RECOVERY FULL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
USE [$(DatabaseName)];


GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
PRINT N'Creating [dbo].[Choice]...';


GO
CREATE TABLE [dbo].[Choice] (
    [Id]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [StanzaID]      BIGINT        NOT NULL,
    [StatusChanges] VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Stanza]...';


GO
CREATE TABLE [dbo].[Stanza] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [MinimumAge]  INT            NOT NULL,
    [MaximumAge]  INT            NOT NULL,
    [StatusLogic] VARCHAR (MAX)  NOT NULL,
    [Text]        VARCHAR (2000) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Pathway]...';


GO
CREATE TABLE [dbo].[Pathway] (
    [Id]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [Key]           UNIQUEIDENTIFIER NOT NULL,
    [Order]         INT              NOT NULL,
    [Created]       DATETIME         NOT NULL,
    [ArcID]         BIGINT           NOT NULL,
    [StanzaID]      BIGINT           NOT NULL,
    [ChoiceID]      BIGINT           NULL,
    [Age]           INT              NOT NULL,
    [CurrentStatus] VARCHAR (MAX)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Arc]...';


GO
CREATE TABLE [dbo].[Arc] (
    [Id]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [StoryID]           BIGINT           NOT NULL,
    [Key]               UNIQUEIDENTIFIER NOT NULL,
    [StartingPathwayID] BIGINT           NOT NULL,
    [AncestorArcID]     BIGINT           NOT NULL,
    [Status]            VARCHAR (MAX)    NOT NULL,
    [Created]           DATETIME         NOT NULL,
    [Archived]          DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Story]...';


GO
CREATE TABLE [dbo].[Story] (
    [Id]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [Created]           DATETIME         NOT NULL,
    [Key]               UNIQUEIDENTIFIER NOT NULL,
    [Name]              VARCHAR (2000)   NOT NULL,
    [StartingArcID]     BIGINT           NULL,
    [Archived]          DATETIME         NULL,
    [ProtagonistName]   VARCHAR (200)    NOT NULL,
    [ProtagonistGender] VARCHAR (50)     NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating unnamed constraint on [dbo].[Stanza]...';


GO
ALTER TABLE [dbo].[Stanza]
    ADD DEFAULT 1 FOR [MinimumAge];


GO
PRINT N'Creating unnamed constraint on [dbo].[Stanza]...';


GO
ALTER TABLE [dbo].[Stanza]
    ADD DEFAULT 99 FOR [MaximumAge];


GO
PRINT N'Creating unnamed constraint on [dbo].[Pathway]...';


GO
ALTER TABLE [dbo].[Pathway]
    ADD DEFAULT 1 FOR [Order];


GO
PRINT N'Creating unnamed constraint on [dbo].[Pathway]...';


GO
ALTER TABLE [dbo].[Pathway]
    ADD DEFAULT GetUTCDate() FOR [Created];


GO
PRINT N'Creating unnamed constraint on [dbo].[Arc]...';


GO
ALTER TABLE [dbo].[Arc]
    ADD DEFAULT GetUTCDate() FOR [Created];


GO
PRINT N'Creating unnamed constraint on [dbo].[Story]...';


GO
ALTER TABLE [dbo].[Story]
    ADD DEFAULT GetUTCDate() FOR [Created];


GO
-- Refactoring step to update target server with deployed transaction logs

IF OBJECT_ID(N'dbo.__RefactorLog') IS NULL
BEGIN
    CREATE TABLE [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
    EXEC sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
END
GO
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '918ab7d6-157d-40d7-8bfa-1ed3218af836')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('918ab7d6-157d-40d7-8bfa-1ed3218af836')

GO

GO
DECLARE @VarDecimalSupported AS BIT;

SELECT @VarDecimalSupported = 0;

IF ((ServerProperty(N'EngineEdition') = 3)
    AND (((@@microsoftversion / power(2, 24) = 9)
          AND (@@microsoftversion & 0xffff >= 3024))
         OR ((@@microsoftversion / power(2, 24) = 10)
             AND (@@microsoftversion & 0xffff >= 1600))))
    SELECT @VarDecimalSupported = 1;

IF (@VarDecimalSupported > 0)
    BEGIN
        EXECUTE sp_db_vardecimal_storage_format N'$(DatabaseName)', 'ON';
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET MULTI_USER 
    WITH ROLLBACK IMMEDIATE;


GO
PRINT N'Update complete.';


GO
