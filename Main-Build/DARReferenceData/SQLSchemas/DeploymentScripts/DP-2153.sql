DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLExchangePairs`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _DARPairID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _DARExchangeID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _ExchangePair varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _DARAssetID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _DARCurrencyID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _StartTime datetime(6) NULL, _EndTime datetime(6) NULL, _User varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
        v_duplicate_count INT =0;
        v_date DATETIME = CURRENT_TIMESTAMP();
        v_result text = 'result';

		BEGIN
          SELECT Count(*) FROM exchangePairs WHERE DARPairID=_DARPairID into v_count;
          SELECT Count(*) FROM exchangePairs WHERE darExchangeID = _DARExchangeID AND exchangePair = _ExchangePair AND darAssetID = _DARAssetID AND darCurrencyID = _DARCurrencyID AND  date(startTime) =  date(_StartTime) AND date(endTime) = date(_EndTime)  into v_duplicate_count;
          SELECT NOW() into v_date;


          IF(v_count > 0 AND UPPER(_OPERATION) = "INSERT") Then
            select _DARPairID + ' Exists' into v_result;
          END IF;

          IF(v_duplicate_count > 0)  Then
            select 'Duplicate Error: Record exists already' into v_result;
            ECHO SELECT 'failed' as "Id", v_result as  "Result",_StartTime as StartTime ;
            Return v_result;
          END IF;


         If (UPPER(_OPERATION) = "INSERT") Then
                INSERT INTO exchangePairs( DARPairID, darExchangeID, exchangePair,darAssetID,darCurrencyID,startTime, endTime, CreateUser, LastEditUser, CreateTime, LastEditTime)
                  values( _DARPairID,_DARExchangeID, _ExchangePair,_DARAssetID,_DARCurrencyID,_StartTime,_EndTime,_User,_User,v_date,v_date);
                COMMIT;
                select 'Inserted' into v_result;
            ELSEIF(UPPER(_OPERATION) = "UPDATE") Then
                UPDATE exchangePairs 
                    SET darExchangeID=_DARExchangeID
                    ,exchangePair=_ExchangePair
                    ,darAssetID = _DARAssetID
                    ,darCurrencyID = _DARCurrencyID 
                    ,startTime = _StartTime
                    ,endTime = _EndTime
                    ,LastEditUser=_User
                    ,LastEditTime=v_date
                WHERE DARPairID=_DARPairID;
                select 'Updated' into v_result;
            ELSEIF(UPPER(_OPERATION) = "DELETE") Then
                DELETE FROM exchangePairs WHERE DARPairID=_DARPairID;
                select 'Deleted' into v_result;
            END IF;
            ECHO SELECT _DARPairID as "Id", v_result as  "Result",_StartTime as StartTime ;
            Return v_result;
END; //

DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLRoleAppModule`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _AppModuleId bigint(16) NULL, _RoleId varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _ID bigint(16) NULL DEFAULT 0, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
        v_id BIGINT(16) =  _ID;
        v_date DATETIME = CURRENT_TIMESTAMP();
        v_del_count1 INT = 0;
        v_del_count2 INT = 0;
       
        BEGIN
            SELECT Count(*) FROM RoleAppModule WHERE AppModuleId=_AppModuleId and RoleId=_RoleId into v_count;
            SELECT NOW() into v_date;
            If (UPPER(_OPERATION) = "UPSERT") Then
				IF(v_count = 0) Then
					INSERT INTO RoleAppModule( AppModuleId, RoleId, CreateUser, LastEditUser, CreateTime, LastEditTime)
                    values( _AppModuleId,_RoleId, _CreateUser, _LastEditUser, v_date, v_date);
                    SELECT ID FROM RoleAppModule WHERE AppModuleId=_AppModuleId and RoleId=_RoleId into v_id;
                    ECHO SELECT v_id as "ID" , 'Data Inserted';
   
				ELSEIF(v_count = 1) Then
						UPDATE RoleAppModule SET LastEditUser=_LastEditUser, LastEditTime=v_date WHERE ID=_ID;
						ECHO SELECT  v_id as "ID", 'Data  Updated', v_count as "v_count", _AppModuleId as "_AppModuleId", _RoleId as "_RoleId";
				ELSEIF(v_count > 1) Then
					ECHO SELECT  'Duplicate Date found!!!';
                END IF;
			
            SELECT Count(*) FROM AppModule WHERE ID=_AppModuleId into v_del_count1;
			SELECT Count(*) FROM RoleAppModule WHERE ID=_RoleId into v_del_count2;
			ELSEIF(UPPER(_OPERATION) = "DELETE") Then
				IF(v_del_count1 = 0 and v_del_count2 =0) Then
					DELETE FROM RoleAppModule WHERE ID=_ID;
          ECHO SELECT  v_id as "ID", 'Data Deleted';
				ELSEIF v_del_count1 !=0 Then
					ECHO SELECT "Foreign Key constraint violet here for Table AppModule field (AppModuleId,Id)";
				ELSEIF v_del_count2 !=0 Then
					ECHO SELECT "Foreign Key constraint violet here for Table RoleAppModule field (RoleId,Id)";
				END IF;
				
			END IF;
            Return v_id;
END; //


create view vExchangePairs 
as
select e.LegacyID as legacyExchangeId,e.name as exchangeName,e.darExchangeID,a.legacyID as legacyAssetID,a.darTicker as assetTicker,a.name as assetName,a.darAssetID,c.legacyID as legacyCurrencyID,c.darTicker as currencyTicker,c.name as currencyName,c.darAssetID as darCurrencyID,ep.CreateTime as loadTime
from refmaster_internal.exchangePairs ep
inner join refmaster_public.token a on ep.darAssetID = a.darAssetID 
inner join refmaster_public.token c on ep.darCurrencyID = c.darAssetID 
inner join refmaster_public.exchange e  on ep.darExchangeID = e.darExchangeID 