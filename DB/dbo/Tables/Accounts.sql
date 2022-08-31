CREATE TABLE [dbo].[Accounts] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50)   NOT NULL,
    [AccountTypeId] INT             NOT NULL,
    [Balance]       DECIMAL (18, 2) NOT NULL,
    [Description]   NVARCHAR (1000) NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Accounts_AccountTypes] FOREIGN KEY ([AccountTypeId]) REFERENCES [dbo].[AccountType] ([Id])
);

