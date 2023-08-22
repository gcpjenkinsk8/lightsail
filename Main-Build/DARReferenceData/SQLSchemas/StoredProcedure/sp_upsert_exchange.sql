DELIMITER //
CREATE OR REPLACE PROCEDURE `sp_upsert_exchange`(short_name text CHARACTER SET utf8 COLLATE utf8_general_ci NULL, vetted_status int(11) NULL, dar_exchange_id text CHARACTER SET utf8 COLLATE utf8_general_ci NULL) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
  DECLARE
    next_Id INT = 1;
    dar_id TEXT = '';
    v_total_row INT = 0;
    BEGIN
      select count(*)  from refmaster_public.exchange where darExchangeID = dar_exchange_id into v_total_row;
      
      echo select v_total_row;

      IF v_total_row  = 0 then
          SELECT max(legacyID)+1 FROM refmaster_public.exchange into next_Id;
          INSERT INTO refmaster_public.exchange(legacyID,name,vettedStatus,darExchangeID,createTime,literal)
            value (next_Id,short_name,vetted_status,dar_exchange_id,CURRENT_TIMESTAMP(),short_name);

            


      end if;

      echo select next_Id;

      return next_Id;

  
       
END; //