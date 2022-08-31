CREATE TABLE [dbo].[Categories] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [OperationTypeId] INT           NOT NULL,
    [UserId]          INT           NOT NULL,
    [Name]            NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categories_OperationType] FOREIGN KEY ([OperationTypeId]) REFERENCES [dbo].[OperationType] ([Id]),
    CONSTRAINT [FK_Categories_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

