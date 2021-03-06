set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Speedy
-- Create date: 4/25/08
-- Description:	Get the Production records - this only works with
--				with a single choice for each parameter
-- =============================================
ALTER PROCEDURE [dbo].[GetProductionRecords] 
	-- Add the parameters for the stored procedure here
           @Sequence_Number nvarchar(50) = null,
           @Carrier_Number nvarchar(50) = null,
           @VIN_Number nvarchar(20) = null,  --just "all" for now
           @Device nvarchar(100) = null,
           @Plant_Style nvarchar(100) = null,
           @Plant_Color nvarchar(100) = null,
           @Plant_Option nvarchar(100) = null,
           @Plant_Tutone nvarchar(100) = null,
           @Plant_Repair nvarchar(20) = null,  --just "all" for now
           @Robot_Repair nvarchar(20) = null,  --just "all" for now
           @Purge_Status nvarchar(20) = null,  --just "all" for now
           @Completion_Status nvarchar(100) = null,
           @Completion_Status_Numeric nvarchar(20) = null,
           @Degrade_Status nvarchar(20) = null,  --just "all" for now
           @Ghost_Status nvarchar(20) = null,  --just "all" for now
           @StartQuery_Time datetime = null,
           @EndQuery_Time datetime = null,
           @Cycle_Time nvarchar(20) = null,  --just "all" for now
           @Index_Time nvarchar(20) = null,  
           @Paint_Total_1 nvarchar(20) = null,  --just "all" for now
           @Paint_Total_2 nvarchar(20) = null,  --just "all" for now
           @Material_Learned_1 nvarchar(20) = null,  --just "all" for now
           @Material_Learned_2 nvarchar(20) = null,  --just "all" for now
           @Material_Temperature_1 nvarchar(20) = null,  --just "all" for now
           @Material_Temperature_2 nvarchar(20) = null,  --just "all" for now
           @Init_String nvarchar(20) = null,  --just "all" for now
           @Color_Change_Time nvarchar(20) = null,  --just "all" for now
           @Cannister_TPSU nvarchar(20) = null,  --just "all" for now
           @Zone nvarchar(100) = null,
           @Ratio nvarchar(20) = null,  --just "all" for now
		---- column names
           @Sequence_Number_Cap nvarchar(30) = null,
           @Carrier_Number_Cap nvarchar(30) = null,
           @VIN_Number_Cap nvarchar(30) = null,
           @Device_Cap nvarchar(30) = null,
           @Plant_Style_Cap nvarchar(30) = null,
           @Plant_Color_Cap nvarchar(30) = null,
           @Plant_Option_Cap nvarchar(30) = null,
           @Plant_Tutone_Cap nvarchar(30) = null,
           @Plant_Repair_Cap nvarchar(30) = null,
           @Robot_Repair_Cap nvarchar(30) = null,
           @Purge_Status_Cap nvarchar(30) = null,
           @Completion_Status_Cap nvarchar(30) = null,
           @Completion_Status_Numeric_Cap nvarchar(30) = null,
           @Degrade_Status_Cap nvarchar(30) = null,
		   @Ghost_Status_Cap nvarchar(30) = null,
           @Cycle_Time_Cap nvarchar(30) = null,
           @Index_Time_Cap nvarchar(30) = null,
           @Paint_Total_1_Cap nvarchar(30) = null,
           @Paint_Total_2_Cap nvarchar(30) = null,
           @Material_Learned_1_Cap nvarchar(30) = null,
           @Material_Learned_2_Cap nvarchar(30) = null,
           @Material_Temperature_1_Cap nvarchar(30) = null,
           @Material_Temperature_2_Cap nvarchar(30) = null,
           @Init_String_Cap nvarchar(30) = null,
           @Color_Change_Time_Cap nvarchar(30) = null,
           @Cannister_TPSU_Cap nvarchar(30) = null,
		   @Complete_Time_Cap nvarchar(30) = null,
		   @Ratio_Cap nvarchar(30) = null,
		   @Zone_Cap nvarchar(30) = null
	

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    SET CONCAT_NULL_YIELDS_NULL OFF;
	
	DECLARE @FieldString nvarchar(max),
			@QueryString nvarchar(max),
			@TimeString nvarchar(max),
			@SortString nvarchar(30),
			@SQL nvarchar(max),
			@@sALL nvarchar(10),
			---- temp stuff
		    @TempField nvarchar(50),
		    @TempCriteria nvarchar(100);
	

	-- quick check
if @StartQuery_Time IS NULL
	RETURN (0);
If @EndQuery_Time IS NULL
	RETURN (0);
--
---- build field string based on non null fields
--IF NOT(@Completion_Status_Cap IS NULL)
--	SET @FieldString = @FieldString + 
--		--dbo.GetFieldNamePart(N'[Completion Status]',@Completion_Status_Cap,@FieldString);
--		dbo.GetFieldNamePart(N'[Job Status]',@Completion_Status_Cap,@FieldString);
--		--	+ ' FROM [Production Log].[dbo].Production Job Status';

IF NOT(@Completion_Status_Numeric_Cap IS NULL)
	SET @FieldString = @FieldString + 
		dbo.GetFieldNamePart(N'[Completion Status]',@Completion_Status_Numeric_Cap,@FieldString);


IF NOT(@Sequence_Number_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Sequence Number]',@Sequence_Number_Cap,@FieldString);

IF NOT(@Carrier_Number_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Carrier Number]',@Carrier_Number_Cap,@FieldString);

IF NOT(@VIN_Number_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[VIN Number]',@VIN_Number_Cap,@FieldString);

IF NOT(@Device_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Device]',@Device_Cap,@FieldString);

IF NOT(@Plant_Style_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Plant Style]',@Plant_Style_Cap,@FieldString);

IF NOT(@Plant_Color_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Plant Color]',@Plant_Color_Cap,@FieldString);

IF NOT(@Plant_Option_Cap IS NULL)
SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Plant Option]',@Plant_Option_Cap,@FieldString);

IF NOT(@Plant_Tutone_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Plant Tutone]',@Plant_Tutone_Cap,@FieldString);

IF NOT(@Plant_Repair_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Plant Repair]',@Plant_Repair_Cap,@FieldString);

IF NOT(@Robot_Repair_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Robot Repair]',@Robot_Repair_Cap,@FieldString);

IF NOT(@Purge_Status_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Purge Status]',@Purge_Status_Cap,@FieldString);

IF NOT(@Degrade_Status_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Degrade Status]',@Degrade_Status_Cap,@FieldString);

IF NOT(@Ghost_Status_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Ghost]',@Ghost_Status_Cap,@FieldString);

IF NOT(@Cycle_Time_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Cycle Time]',@Cycle_Time_Cap,@FieldString);

IF NOT(@Index_Time_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Index Time]',@Index_Time_Cap,@FieldString);

IF NOT(@Paint_Total_1_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Paint Total 1]',@Paint_Total_1_Cap,@FieldString);

IF NOT(@Paint_Total_2_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Paint Total 2]',@Paint_Total_2_Cap,@FieldString);

IF NOT(@Material_Learned_1_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Material Learned 1]',@Material_Learned_1_Cap,@FieldString);

IF NOT(@Material_Learned_2_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Material Learned 2]',@Material_Learned_2_Cap,@FieldString);

IF NOT(@Material_Temperature_1_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Material Temperature 1]',@Material_Temperature_1_Cap,@FieldString);

IF NOT(@Material_Temperature_2_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Material Temperature 2]',@Material_Temperature_2_Cap,@FieldString);

IF NOT(@Ratio_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Ratio]',@Ratio_Cap,@FieldString);

IF NOT(@Init_String_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Init String]',@Init_String_Cap,@FieldString);

IF NOT(@Color_Change_Time_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Color Change Time]',@Color_Change_Time_Cap,@FieldString);

IF NOT(@Cannister_TPSU_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Cannister TPSU]',@Cannister_TPSU_Cap,@FieldString);

IF NOT(@Zone_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Zone]',@Zone_Cap,@FieldString);

-- add time as last column
IF NOT(@Complete_Time_Cap IS NULL)
	SET @FieldString = @FieldString + 
			dbo.GetFieldNamePart(N'[Complete Time]',@Complete_Time_Cap,@FieldString);
	
SET @SQL = 'SELECT ' + @FieldString + ' FROM [Production Log].[dbo].[ProdLog]';

--- END of select section


--- Inner joins section
SET @@sALL = N'ALL';
--SET @SQL = @SQL + N' INNER JOIN [Production Log].[dbo].[Production Job Status]  ON ' +
--	N'[Production Log].[dbo].[ProdLog].[Completion Status] = [Production Log].[dbo].[Production Job Status].[Value] ' 


-- build where clause
SET @@sALL = N'ALL';

	-- build field string based on non null fields

/*  Sequence Number Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Sequence Number]';
SET @TempCriteria=@Sequence_Number;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/*  VIN Number Field ***********************************************************
		 wildcards															
*/
SET @TempField= N'[VIN Number]';
SET @TempCriteria=@VIN_Number;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	BEGIN

		IF charindex(N'%',@TempCriteria,0) = 0 
			SET @QueryString = @QueryString + 
				dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);
		ELSE
			SET @QueryString = @QueryString + 
				dbo.GenLikeSQLPart(@TempField,@TempCriteria,@QueryString);
	END

/*  Carrier Number Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Carrier Number]';
SET @TempCriteria=@Carrier_Number;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/*  Device Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Device]';
SET @TempCriteria=@Device;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

	
/*  [Plant Style] Number Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Plant Style]';
SET @TempCriteria=@Plant_Style;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);
	

/*  [Plant Color]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Plant Color]';
SET @TempCriteria=@Plant_Color;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);
	

/*  [Plant Option]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Plant Option]';
SET @TempCriteria=@Plant_Option;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/*  [Plant Tutone]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Plant Tutone]';
SET @TempCriteria=@Plant_Tutone;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/*  [Plant Repair]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Plant Repair]';
SET @TempCriteria=@Plant_Repair;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/*  [Robot Repair]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Robot Repair]';
SET @TempCriteria=@Robot_Repair;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/*  [Purge Status]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Purge Status]';
SET @TempCriteria=@Purge_Status;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/*  [Completion Status]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Completion Status]';
SET @TempCriteria=@Completion_Status;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);
	


SET @TempField= N'[Completion Status]';
SET @TempCriteria=@Completion_Status_Numeric;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenMaskSQLPart(@TempField,@TempCriteria,@QueryString);


/*  Degrade   Field ***********************************************************
		No wildcards
		IF NOT(@QueryString IS NULL)
			SET @QueryString = @QueryString + N' AND ';
		if @Degrade_Status=0
			SET @QueryString = @QueryString + N'([Degrade Status]=0' +')' 
		else
			SET @QueryString = @QueryString + N'([Degrade Status]=1' +')' 
															
*/
SET @TempField= N'[Degrade Status]';
SET @TempCriteria=@Degrade_Status;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

SET @TempField= N'[Ghost]';
SET @TempCriteria=@Ghost_Status;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/* [Cycle Time]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Cycle Time]';
SET @TempCriteria=@Cycle_Time;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/* [Index Time]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Index Time]';
SET @TempCriteria=@Index_Time;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/* [Paint Total 1]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Paint Total 1]';
SET @TempCriteria=@Paint_Total_1;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);
	
/* [Paint Total 2]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Paint Total 2]';
SET @TempCriteria=@Paint_Total_2;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/* [Material Learned 1]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Material Learned 1]';
SET @TempCriteria=@Material_Learned_1;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/* [Material Learned 2]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Material Learned 2]';
SET @TempCriteria=@Material_Learned_2;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/* [[Material Temperature 1]]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Material Temperature 1]';
SET @TempCriteria=@Material_Temperature_1;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/* [[Material Temperature 2]]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Material Temperature 2]';
SET @TempCriteria=@Material_Temperature_2;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/* [Init String]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Init String]';
SET @TempCriteria=@Init_String;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/* [Color Change Time]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Color Change Time]';
SET @TempCriteria=@Color_Change_Time;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);

/* [Cannister TPSU]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Cannister TPSU]';
SET @TempCriteria=@Cannister_TPSU;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);



/*  [Zone]   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Zone]';
SET @TempCriteria=@Zone;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);


/*  Ratio   Field ***********************************************************
		No wildcards															
*/
SET @TempField= N'[Ratio]';
SET @TempCriteria=@Ratio;

IF NOT(@TempCriteria IS NULL) AND (upper(@TempCriteria) <> @@sALL)
	SET @QueryString = @QueryString + 
		dbo.GenEqualSQLPart(@TempField,@TempCriteria,@QueryString);





-- Time part of where clause
SET @TimeString = N'([Complete Time]>=''' +  convert(nvarchar,@StartQuery_Time,109) + N''' AND '
SET @TimeString = @TimeString + N'[Complete Time]<=''' +  convert(nvarchar,@EndQuery_Time,109) + N''') '

-- add where clause
-- add time clause
IF NOT(@QueryString IS NULL)
	SET @QueryString = N' WHERE (' + @QueryString + N') AND ' + @TimeString;	
ELSE
	SET @QueryString = N' WHERE '  + @TimeString;	
--*****************

--think we always want latest first
SET @QueryString =  @QueryString + N' ORDER BY [Complete Time] DESC'

SET @SQL= @SQL + @QueryString;

-- this is for debug
print @SQL;

--DECLARE @RetVal int

EXEC (@SQL)

--RETURN (@retval)
   
END



















