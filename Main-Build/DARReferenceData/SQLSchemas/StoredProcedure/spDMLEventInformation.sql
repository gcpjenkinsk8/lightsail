DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLEventInformation`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _DAREventID text NULL DEFAULT 1, _DateofReview datetime NULL DEFAULT NULL, _EventTypeID int(11) NULL DEFAULT NULL, _AssetID int(11) NULL DEFAULT NULL, _SourceID int(11) NULL DEFAULT NULL, _ExchangeAssetTicker varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _ExchangeAssetName varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _DARAssetID varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _EventType varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _EventDate datetime NULL DEFAULT NULL, _AnnouncementDate datetime NULL DEFAULT NULL, _EventDescription varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _SourceURL varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _EventStatus varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Notes varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Deleted varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Exchange varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _BlockHeight int(11) NULL DEFAULT NULL, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _IsActive tinyint(4) NULL DEFAULT 1) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
        v_date DATETIME = CURRENT_TIMESTAMP();
        v_id text =  _DAREventID;
        BEGIN
            SELECT Count(*) FROM EventInformation WHERE DAREventID=_DAREventID into v_count;
            SELECT NOW() into v_date;
            If (UPPER(_OPERATION) = "UPSERT") Then
				IF(v_count = 0) Then
					INSERT INTO EventInformation( DAREventID,DateofReview,EventTypeID,AssetID,SourceID,ExchangeAssetTicker,ExchangeAssetName,
                    DARAssetID,EventType,EventDate,AnnouncementDate,EventDescription,SourceURL,EventStatus,Notes,Deleted,Exchange,BlockHeight,
                    CreateUser, LastEditUser, IsActive, CreateTime, LastEditTime)
                    values(_DAREventID, _DateofReview,_EventTypeID,_AssetID,_SourceID,_ExchangeAssetTicker,_ExchangeAssetName,_DARAssetID,
                    _EventType,_EventDate,_AnnouncementDate,_EventDescription,_SourceURL,_EventStatus,_Notes, _Deleted, _Exchange,_BlockHeight,
                    _CreateUser,_LastEditUser,_IsActive,v_date, v_date);
					COMMIT;
                    SELECT DAREventID FROM EventInformation WHERE LastEditTime=v_date  into v_id;
                    ECHO SELECT v_id as "ID" , 'Data Inserted';
   
				ELSEIF(v_count = 1) Then
						UPDATE EventInformation SET DateofReview=_DateofReview,EventTypeID=_EventTypeID,AssetID=_AssetID,SourceID=_SourceID,
                        ExchangeAssetTicker=_ExchangeAssetTicker,ExchangeAssetName=_ExchangeAssetName,DARAssetID=_DARAssetID,EventType=_EventType,EventDate=_EventDate,
                        AnnouncementDate=_AnnouncementDate,EventDescription=_EventDescription,SourceURL=_SourceURL,EventStatus=_EventStatus,Notes=_Notes,Deleted=_Deleted, 
                        Exchange=_Exchange,BlockHeight=_BlockHeight, LastEditUser=_LastEditUser, IsActive=_IsActive, LastEditTime=v_date WHERE DAREventID=_DAREventID;
						ECHO SELECT  v_id as "ID", 'Data  Updated';
				ELSEIF(v_count > 1) Then
					ECHO SELECT  'Duplicate Date found!!!';
                END IF;
			ELSEIF(UPPER(_OPERATION) = "DELETE") Then
				DELETE FROM EventInformation WHERE DAREventID=_DAREventID;
				COMMIT;
				ECHO SELECT  v_id as "ID", 'Data Deleted';
			END IF;
            Return v_id;
END; //