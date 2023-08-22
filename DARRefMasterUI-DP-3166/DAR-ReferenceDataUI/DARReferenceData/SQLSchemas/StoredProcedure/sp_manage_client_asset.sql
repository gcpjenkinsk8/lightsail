DELIMITER //
CREATE OR REPLACE PROCEDURE sp_manage_client_asset (operation TEXT, client_name TEXT DEFAULT '', asset TEXT DEFAULT '', input_user TEXT DEFAULT '', new_client TEXT DEFAULT '') RETURNS text AS
DECLARE 
  return_text text = "";
  client_id int = 0;
  new_client_id int = 0;
  asset_id bigint(11) = 0;
  asset_mapped int = 0;
  client_id_count int = 0;
  new_client_id_count int = 0;

  
BEGIN
      select 'operation not supported' into return_text;
      if operation = 'ADD' then

        if (client_name = '' or asset = '' or input_user = '') then 
            select 'Missing required parameters for ADD operation' into return_text;
        else
            select ID from refmaster_internal.Clients where ClientName = client_name into client_id; 
            select ID from refmaster_internal.Asset where (DARAssetID = asset or DARTicker = asset) into asset_id;

            select count(*) from refmaster_internal.ClientAssets where AssetID = asset_id and ClientID = client_id into asset_mapped;

            if asset_mapped > 0 then
              select CONCAT('Mapping exists already for ClientID ',client_id,' AssetID ',asset_id) into return_text;
            else
              insert into refmaster_internal.ClientAssets(AssetID,ClientID,CreateUser,LastEditUser,CreateTime,LastEditTime,ReferenceData,Price)
                values (asset_id,client_id,input_user,input_user,current_timestamp,current_timestamp,1,1);
              select 'Mapping added' into return_text;
            end if;
        end if;

      end if;
      
     if operation = 'CLONE' then
        if new_client = '' then
          select 'new_client is a required parameter for CLONE operation' into return_text;
        else

          select count(*) from refmaster_internal.Clients where ClientName = client_name into client_id_count;
          select count(*) from refmaster_internal.Clients where ClientName = new_client into new_client_id_count;
          if (client_id_count = 0  or new_client_id_count = 0) then 
              select 'Invalid client id' into return_text;
          else 

              select ID from refmaster_internal.Clients where ClientName = new_client into new_client_id;
              select ID from refmaster_internal.Clients where ClientName = client_name into client_id; 
              
              insert into refmaster_internal.ClientAssets(AssetID,ClientID,CreateUser,LastEditUser,CreateTime,LastEditTime,ReferenceData,Price)
              select AssetID,new_client_id,input_user,input_user,current_timestamp,current_timestamp,1,1  from refmaster_internal.ClientAssets where ClientID = client_id;

              select CONCAT('Client ID: ',client_id,' New Client ID: ',new_client_id) into return_text; 
          end if;


          
        end if;
        
     end if;
 
       ECHO SELECT return_text;
      return return_text;
END; //

