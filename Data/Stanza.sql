﻿CREATE TABLE [dbo].[Stanza]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [MinimumAge] INT NOT NULL DEFAULT 1, 
    [MaximumAge] INT NOT NULL DEFAULT 99, 
    [StatusLogic] VARCHAR(MAX) NOT NULL, 
    [Text] VARCHAR(2000) NOT NULL
)
