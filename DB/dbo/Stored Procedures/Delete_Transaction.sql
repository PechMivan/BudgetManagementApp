-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Delete_Transaction]
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @Amount decimal(18,2);
	DECLARE @OperationTypeId int;
	DECLARE @AccountId int;

	SELECT @Amount = Transactions.Amount, @OperationTypeId = cat.OperationTypeId,
	@AccountId = Transactions.AccountId
	FROM Transactions
	INNER JOIN Categories cat
	ON cat.Id = Transactions.CategoryId
	WHERE Transactions.Id = @Id;

	DECLARE @OperationType int = 1;
	IF (@OperationTypeId = 2)
		SET @OperationType = -1;

	SET @Amount = @Amount * @OperationType;

	UPDATE Accounts
	SET Balance -= @Amount
	WHERE Accounts.Id = @AccountId;

	DELETE Transactions
	WHERE Transactions.Id = @Id;
END
