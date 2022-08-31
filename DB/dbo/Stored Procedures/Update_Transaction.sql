-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Update_Transaction]
	@Id int,
	@TransactionDate date,
	@Amount decimal,
	@PreviousAmount decimal,
	@Note nvarchar(1000),
	@AccountId int,
	@PreviousAccountId int,
	@CategoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- UPDATE PREVIOUS ACCOUNT BALANCE.

	UPDATE Accounts
	SET Balance -= @PreviousAmount
	WHERE Id = @PreviousAccountId;

	-- UPDATE NEW ACCOUNT BALANCE.

	UPDATE Accounts
	SET Balance += @Amount
	WHERE Id = @AccountId;

	--UPDATE TRANSACTION.

	UPDATE Transactions
	SET TransactionDate = @TransactionDate, Amount = ABS(@Amount),
	AccountId = @AccountId, CategoryId = @CategoryId, Note = @Note
	WHERE Id = @Id;

END
