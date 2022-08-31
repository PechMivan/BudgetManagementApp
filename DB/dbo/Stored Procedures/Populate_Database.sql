-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Populate_Database 
	-- Add the parameters for the stored procedure here
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO AccountType(Name, UserId, [Order])
	VALUES ('Cash', @UserId, 1),
		   ('Credit Card', @UserId, 2);

	INSERT INTO Accounts(Name, AccountTypeId, Balance)
	SELECT Name, Id, 0
	FROM AccountType
	WHERE UserId = @UserId;

	INSERT INTO Categories(Name, OperationTypeId, UserId)
	VALUES ('Food', 2, @UserId),
			('Transportation', 2, @UserId),
			('Rent', 2, @UserId),
			('Salary', 1, @UserId),
			('Goverment Support', 1, @UserId),
			('Investments', 1, @UserId);

END
