﻿CREATE TABLE [dbo].[Story]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Created] DATETIME NOT NULL DEFAULT GetUTCDate(), 
	[Key] UNIQUEIDENTIFIER NOT NULL,
    [Name] VARCHAR(2000) NOT NULL, 
    [StartingArcID] BIGINT NULL, 
    [Archived] DATETIME NULL, 
    [ProtagonistName] VARCHAR(200) NOT NULL, 
    [ProtagonistGender] VARCHAR(50) NOT NULL
)
