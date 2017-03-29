USE [Production Log]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[AddProductionRecordFlex]
		@Columns = N'[Sequence Number], [Device], [Plant Style], [Degrade Status]',
		@Data = N'1234, ''RC_1'',''33'',1'

SELECT	'Return Value' = @return_value

GO
