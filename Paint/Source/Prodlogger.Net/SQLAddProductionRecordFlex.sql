set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ADD PROCEDURE [dbo].[AddProductionRecordFlex] 
	-- Add the parameters for the stored procedure here
	       @Columns nvarchar(500), 
           @Data    nvarchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SQL nvarchar(max)

SET @SQL = 'INSERT INTO [Production Log].[dbo].[ProdLog]('
SET @SQL = @SQL + @Columns + ') VALUES('  + @Data + ')';

-- this is for debug
print @SQL;

EXEC (@SQL)

END



