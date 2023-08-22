DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLStaging_CryptoNodeEvents`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _DateofReview datetime NULL, _ExchangeAssetTicker varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _ID bigint(16) NULL DEFAULT 0, _ExchangeAssetName varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _DARAssetID varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _EventType varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _EventDate datetime NULL DEFAULT NULL, _AnnouncementDate datetime NULL DEFAULT NULL, _EventDescription varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _SourceURL varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _EventStatus varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Notes varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Deleted varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Exchange varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _ValidationTime datetime NULL DEFAULT NULL, _AssetID int(11) NULL DEFAULT NULL, _SourceID int(11) NULL DEFAULT NULL, _EventTypeID int(11) NULL DEFAULT NULL, _BlockHeight int(11) NULL DEFAULT NULL, _Error varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) RETURNS void AS
DECLARE 
	v_count INT =0;
	v_date DATETIME = CURRENT_TIMESTAMP();
	v_id BIGINT(16) =  _ID;
	qry QUERY(id VARCHAR(50)) = SELECT DARAssetID FROM Asset WHERE DARTicker=_ExchangeAssetTicker AND Name=_ExchangeAssetName;
	assetid VARCHAR(50);
BEGIN
	assetid = SCALAR(qry); 
	SELECT NOW() into v_date;
	If (UPPER(_OPERATION) = "UPSERT") Then
		IF(_DARAssetID IS NULL OR assetid=_DARAssetID) THEN
			IF(_ID = 0) THEN
				INSERT INTO Staging_CryptoNodeEvents(DARAssetID, DateofReview,ExchangeAssetTicker,ExchangeAssetName,EventType,EventDate,AnnouncementDate,EventDescription,SourceURL,EventStatus,Notes,Deleted,Exchange,ValidationTime,AssetID,SourceID,EventTypeID,BlockHeight,Error,CreateTime)
				values(_DARAssetID,_DateofReview,_ExchangeAssetTicker,_ExchangeAssetName,_EventType,_EventDate,_AnnouncementDate,_EventDescription,_SourceURL,_EventStatus,_Notes,_Deleted,_Exchange,_ValidationTime,_AssetID,_SourceID,_EventTypeID,_BlockHeight,_Error,v_date);
				COMMIT;
				SELECT MAX(ID) FROM Staging_CryptoNodeEvents WHERE DateofReview=_DateofReview and ExchangeAssetTicker=_ExchangeAssetTicker INTO v_id;
				ECHO SELECT v_id AS event_id, 'Insert' AS RowOutput;

			ELSE 
				UPDATE Staging_CryptoNodeEvents 
				SET DARAssetID=_DARAssetID, DateofReview=_DateofReview, ExchangeAssetTicker=_ExchangeAssetTicker, ExchangeAssetName=_ExchangeAssetName,EventType=_EventType,EventDate=_EventDate,AnnouncementDate=_AnnouncementDate,
				EventDescription=_EventDescription, SourceURL=_SourceURL, EventStatus=_EventStatus, Notes=_Notes, Deleted=_Deleted, Exchange=_Exchange, ValidationTime=_ValidationTime, AssetID=_AssetID, SourceID=_SourceID, EventTypeID=_EventTypeID, 
				BlockHeight=_BlockHeight,Error=_Error WHERE ID=_ID;
				ECHO SELECT v_id as event_id, 'Update' AS RowOutput;
			END IF;
		ELSE
			ECHO SELECT "Cannot add or update a child row: a foreign key constraint fails( CONSTRAINT Asset FOREIGN KEY ('ExchangeAssetTicker', 'ExchangeAssetName ') REFERENCES 'Asset' ('DARTicker', 'Name'))" AS RowOutput; 			
        END IF; 
	ELSEIF(UPPER(_OPERATION) = "DELETE") Then
		DELETE FROM Staging_CryptoNodeEvents WHERE ID=_ID;
		COMMIT;
		ECHO SELECT v_id AS event_id, 'Success' as RowOutput;
	END IF;
    
EXCEPTION
	WHEN ER_SCALAR_BUILTIN_NO_ROWS THEN
		ECHO SELECT "Cannot add or update a child row: a foreign key constraint fails( CONSTRAINT Asset FOREIGN KEY ('DARAssetID') REFERENCES 'Asset' ('DARAssetID'))" AS RowOutput; 
        RAISE; 
	WHEN OTHERS THEN
		RAISE user_exception(exception_message()); 
END; //