USE [ReferenceCore]
GO

/****** Object:  StoredProcedure [dbo].[spGetDARReferenceID]    Script Date: 11/2/2021 2:39:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- exec [dbo].[spGetDARReferenceID] 'role'

ALTER PROCEDURE [dbo].[spGetDARReferenceID]
@TypeofProcess 	VARCHAR(50)
AS
BEGIN
DECLARE @Length int;
DECLARE @CharPool varchar(100);
DECLARE @LoopCount int=0;
DECLARE @PoolLength int;
DECLARE @RandomString varchar(10) ;

set @Length = 5
set @CharPool = 'ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789'
set @PoolLength = Len(@CharPool)

set @RandomString = 'DA'
IF LOWER(@TypeofProcess) = 'asset'
	BEGIN
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(DARAssetID) FROM [dbo].[Asset])
					BEGIN
						SELECT @RandomString ='DA'
						print N'Random string matches the DARAssetID of Asset table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
	SELECT @RandomString as [DARAssetID]
	END

IF LOWER(@TypeofProcess) = 'exchange'
	BEGIN
	
		set @RandomString = 'DE'
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(DARExchangeID) FROM [dbo].[Exchange])
					BEGIN
						SELECT @RandomString ='DE'
						print N'Random string matches the DARExchangeID of Exchange table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
		SELECT @RandomString as [DARExchangeID]
	END

IF LOWER(@TypeofProcess) = 'source'
	BEGIN
	
		set @RandomString = 'DS'
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(DARSourceID) FROM [dbo].[Source])
					BEGIN
						SELECT @RandomString ='DS'
						print N'Random string matches the DARSourceID of Source table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
		SELECT @RandomString as [DARSourceID]
	END

IF LOWER(@TypeofProcess) = 'token'
	BEGIN
	
		set @RandomString = 'DT'
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(DARSourceID) FROM [dbo].[Source])
					BEGIN
						SELECT @RandomString ='DT'
						print N'Random string matches the DARTokenID of Token table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
		SELECT @RandomString as [DARTokenID]
	END


IF LOWER(@TypeofProcess) = 'role'
	BEGIN
	
		set @RandomString = 'DR'
		WHILE (@LoopCount < @Length) 
			BEGIN
				SELECT @RandomString = @RandomString + SUBSTRING(@Charpool, CONVERT(int, RAND() * @PoolLength) + 1, 1)
				IF @RandomString in (SELECT DISTINCT(ID) FROM [dbo].[AspNetRoles])
					BEGIN
						SELECT @RandomString ='DR'
						print N'Random string matches the DAR Role ID of AspNetRoles table';
						SELECT @LoopCount = -1
					END
				ELSE
					BEGIN
						print @RandomString
					END
			SELECT @LoopCount = @LoopCount + 1
			END
		SELECT @RandomString as [DARRoleID]
	END

END

--EXEC [dbo].[spGetDARReferenceID] @TypeofProcess='source'
GO


