DELIMITER //
CREATE OR REPLACE PROCEDURE `sp_upsert_asset`(ticker text CHARACTER SET utf8 COLLATE utf8_general_ci NULL, asset_name text CHARACTER SET utf8 COLLATE utf8_general_ci NULL, ftse_status int(11) NULL, index_status int(11) NULL, exchange_source_status int(11) NULL, other_pricing int(11) NULL, dar_asset_identifier text CHARACTER SET utf8 COLLATE utf8_general_ci NULL) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
  DECLARE
    next_Id INT = 1;
    return_message TEXT = '';
    v_total_row INT = 0;
    t_total_row INT = 0;
    n_total_row INT = 0;
    total_row INT = 0;

    BEGIN
      select count(*)  from refmaster_public.token where darAssetId = dar_asset_identifier into v_total_row;
      select count(*)  from refmaster_public.token where darTicker = ticker into t_total_row;
      select count(*)  from refmaster_public.token where name = asset_name into n_total_row;
      total_row = v_total_row + t_total_row + n_total_row;

      -- echo select total_row;

      IF total_row  = 0 then
          SELECT max(legacyID)+1 FROM refmaster_public.token into next_Id;
          INSERT INTO refmaster_public.token(legacyID,darTicker,name,ftseStatus,indexStatus,exchangeSourceStatus,otherPricing,darAssetId,createTime)
            value (next_Id,ticker,asset_name,ftse_status,index_status,exchange_source_status,other_pricing,dar_asset_identifier,CURRENT_TIMESTAMP());

          INSERT INTO refmaster_public.token_methodology(legacyID,methodologyID) value (next_Id,2);
    

      end if;

       if total_row  > 0 then
          return_message = 'Asset exists already';
          next_Id = -1;
       end if;


      echo select next_Id;

      return next_Id;

  
       
END; //