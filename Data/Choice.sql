﻿CREATE TABLE [dbo].[Choice]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [StanzaID] BIGINT NOT NULL, 
    [StatusChanges] VARCHAR(MAX) NOT NULL
)
