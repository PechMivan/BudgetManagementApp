CREATE TABLE [dbo].[Transactions] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [UserId]          INT             NOT NULL,
    [TransactionDate] DATETIME        NOT NULL,
    [Amount]          DECIMAL (18, 2) NOT NULL,
    [Note]            NVARCHAR (1000) NULL,
    [AccountId]       INT             NOT NULL,
    [CategoryId]      INT             NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Transactions_Accounts] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id]),
    CONSTRAINT [FK_Transactions_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]),
    CONSTRAINT [FK_Transactions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

