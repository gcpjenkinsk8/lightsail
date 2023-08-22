DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLAssetTheme`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _AssetID bigint(16) NULL, _ThemeID bigint(16) NOT NULL, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _Id bigint(16) NULL DEFAULT 1) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
        v_date DATETIME = CURRENT_TIMESTAMP();
        v_del_count1 INT =0;
        v_id BIGINT(16) =  _Id;
        BEGIN
            SELECT Count(*) FROM  AssetTheme WHERE AssetID=_AssetID and ThemeID=_ThemeID  into v_count;
            SELECT NOW() into v_date;
            If (UPPER(_OPERATION) = "UPSERT") Then
                IF(v_count = 0) Then
                  INSERT INTO  AssetTheme(AssetID, ThemeID, CreateUser, LastEditUser, CreateTime, LastEditTime)
                            values(_AssetID, _ThemeID, _CreateUser, _LastEditUser, v_date, v_date);
                  COMMIT;
                            SELECT ID FROM AssetTheme WHERE AssetID=_AssetID and ThemeID=_ThemeID into v_id;
                            ECHO SELECT v_id as "ID", 'Data Inserted';
          
                ELSEIF(v_count = 1) Then
                    UPDATE  AssetTheme SET AssetID=_AssetID , ThemeID=_ThemeID, LastEditUser=_LastEditUser, LastEditTime=v_date WHERE Id=_Id;
                    ECHO SELECT v_id as "ID", 'Data  Updated';
                ELSEIF(v_count > 1) Then
                  ECHO SELECT  'Duplicate Date found!!!';
                END IF;
			      END IF;
            If (UPPER(_OPERATION) = "DELETE") Then
              DELETE FROM  AssetTheme WHERE Id=_Id;
              ECHO SELECT v_id as "ID", 'Data  Deleted';
            END IF;

            Return v_id;
END; //
