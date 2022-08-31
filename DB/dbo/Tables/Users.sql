CREATE TABLE [dbo].[Users] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Email]           NVARCHAR (256) NOT NULL,
    [NormalizedEmail] NVARCHAR (256) NOT NULL,
    [PasswordHash]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

