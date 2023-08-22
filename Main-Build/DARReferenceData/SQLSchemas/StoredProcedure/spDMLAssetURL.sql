CREATE OR REPLACE PROCEDURE `spDMLAssetURL`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _AssetID bigint(16) NULL, _URLTypeID int(11) NULL, _URLPath varchar(1500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _ID bigint(16) NULL DEFAULT 0, _IsActive tinyint(4) NULL DEFAULT 1) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
        v_date DATETIME = CURRENT_TIMESTAMP();
        v_user VARCHAR(100) = CURRENT_USER();
        v_del_count1 INT =0;
        v_id BIGINT(16) =  _ID;
        BEGIN
            SELECT Count(*) FROM  AssetURL WHERE AssetID=_AssetID AND URLTypeID=_URLTypeID into v_count;
            SELECT USER() into v_user;
            SELECT NOW() into v_date;
            If (UPPER(_OPERATION) = "UPSERT") Then
				IF(v_count = 0) Then
					INSERT INTO  AssetURL(AssetID, URLTypeID, URLPath, IsActive, CreateUser, LastEditUser, CreateTime, LastEditTime)
                    values(_AssetID, _URLTypeID, _URLPath, _IsActive, _CreateUser, _LastEditUser, v_date, v_date);
					COMMIT;
                    SELECT ID FROM AssetURL WHERE AssetID=_AssetID AND URLTypeID=_URLTypeID into v_id;
                    ECHO SELECT v_id as 'ID', 'Inserted';
   
				ELSEIF(v_count = 1) Then
						UPDATE  AssetURL SET AssetID=_AssetID , URLTypeID=_URLTypeID, URLPath=_URLPath, IsActive= _IsActive, LastEditUser=_LastEditUser, LastEditTime=v_date WHERE Id=_ID;
						SELECT ID FROM AssetURL WHERE AssetID=_AssetID AND URLTypeID=_URLTypeID into v_id;
                        ECHO SELECT v_id as 'ID', 'Updated';
				ELSEIF(v_count > 1) Then
					ECHO SELECT  'Duplicate';
                END IF;
			ELSEIF(UPPER(_OPERATION) = "DELETE") Then
				DELETE FROM  AssetURL WHERE Id=_ID;
				COMMIT;
				ECHO SELECT v_id as 'ID', 'Deleted';
			END IF;
            Return v_id;
END;