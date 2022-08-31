-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Insert_Transaction]
	@UserId int,
	@TransactionDate date,
	@Amount decimal,
	@Note nvarchar(1000),
	@AccountId int,
	@CategoryId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO Transactions("UserId", "TransactionDate", "Amount", "Note", "AccountId", "CategoryId")
	VALUES (@UserId, @TransactionDate, ABS(@Amount), @Note, @AccountId, @CategoryId)

	UPDATE Accounts
	SET Balance += @Amount
	WHERE Id = @AccountId

	SELECT SCOPE_IDENTITY();

END
