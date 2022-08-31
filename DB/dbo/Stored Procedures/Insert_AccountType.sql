-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Insert_AccountType
	@UserId int,
	@Name  nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @Order int;

	SELECT @Order = COALESCE(MAX([Order]), 0) + 1
	FROM AccountType
	WHERE UserId = @UserId

	INSERT INTO AccountType(Name, UserId, [Order])
	VALUES (@Name, @UserId, @Order)

	SELECT SCOPE_IDENTITY();
END
