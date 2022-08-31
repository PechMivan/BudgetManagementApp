CREATE ROLE [db_executor]
    AUTHORIZATION [dbo];


GO
ALTER ROLE [db_executor] ADD MEMBER [BudgetManagementApp];


GO
ALTER ROLE [db_executor] ADD MEMBER [BMDBApp];

