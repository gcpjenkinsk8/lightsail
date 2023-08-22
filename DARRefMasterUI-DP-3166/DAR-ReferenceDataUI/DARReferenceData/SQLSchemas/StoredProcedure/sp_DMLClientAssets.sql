CREATE OR REPLACE PROCEDURE `spDMLClientAssets`(_AssetID bigint(16) NULL, _ClientID bigint(16) NULL, _ID bigint(16) NULL DEFAULT 0, _Price tinyint(4) NULL DEFAULT NULL, _ReferenceData tinyint(4) NULL DEFAULT NULL, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) RETURNS void AS 
DECLARE 
	v_date DATETIME;
    q QUERY(a BIGINT) = SELECT ID FROM Asset WHERE ID=_AssetID;
    asset_id BIGINT; 
BEGIN
    asset_id = SCALAR(q); 
    SELECT NOW() into v_date;
	INSERT INTO ClientAssets(AssetID, ClientID, ReferenceData, Price, CreateUser,LastEditUser, CreateTime, LastEditTime)
	values(_AssetID, _ClientID, _ReferenceData, _Price, _CreateUser,_LastEditUser,v_date, v_date);
	COMMIT;
	ECHO SELECT ID, "Successfully Inserted" AS RowOutput FROM ClientAssets WHERE AssetID=_AssetID AND ClientID=_ClientID; 
    EXCEPTION 
		WHEN ER_SCALAR_BUILTIN_NO_ROWS THEN
			RAISE user_exception("Cannot add or update a child row: a foreign key constraint fails( CONSTRAINT ClientAssets FOREIGN KEY ('AssetID') REFERENCES 'Asset' ('Id'))");
        WHEN ER_DUP_ENTRY THEN
			UPDATE ClientAssets SET  
			ReferenceData=_ReferenceData, 
            Price=_Price,
            LastEditUser=_LastEditUser,
            LastEditTime=v_date 
            WHERE ID=_ID;
			ECHO SELECT ID, "Successfully Updated" AS RowOutput FROM ClientAssets WHERE AssetID=_AssetID AND ClientID=_ClientID; 		
END;