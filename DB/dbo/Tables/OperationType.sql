CREATE TABLE [dbo].[OperationType] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_OperationType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

