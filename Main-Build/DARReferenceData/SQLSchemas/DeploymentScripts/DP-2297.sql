drop table AssetIdMap
create table AssetIdMap (
  DARAssetID varchar(20) not null,
  InputId text not null,
  CreateTime datetime NOT NULL default now(),
  UNIQUE KEY `PRIMARY` (`DARAssetID`,`InputId`) USING HASH,
  SHARD KEY `__SHARDKEY` (`DARAssetID`,`InputId`)
)




DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLMapAssetID`() AS
DECLARE 
        
BEGIN
      -- 1 Ticker to DAR Asset Id
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select distinct a.DARAssetID, a.DARTicker
      from refmaster_internal.Asset a
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap  ) m on m.DARAssetID = a.DARAssetID and m.InputId = a.DARTicker
      where m.DARAssetID is null;

      -- 2 Ticker to Legacy Dar Asset Id
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select distinct LegacyDARAssetID, DARTicker
      from refmaster_internal.Asset a 
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap  ) m on m.DARAssetID = a.LegacyDARAssetID and m.InputId = a.DARTicker
      where a.LegacyDARAssetID is not null 
        and LTRIM(RTRIM(a.LegacyDARAssetID)) != ''
        and a.LegacyDARAssetID != a.DARAssetID
        and m.DARAssetID is null;

      -- 3 DARAssetID to DARAssetID
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select distinct a.DARAssetID, a.DARAssetID
      from refmaster_internal.Asset a
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap ) m on m.DARAssetID = a.DARAssetID and m.InputId = a.DARAssetID
      where m.DARAssetID is null;



      -- 4 LegacyDARAssetID to LegacyDARAssetID
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select a.LegacyDARAssetID, a.InputId
      from (
            select distinct LegacyDARAssetID, LegacyDARAssetID as InputId
            from refmaster_internal.Asset 
            where LegacyDARAssetID is not null 
              and LTRIM(RTRIM(LegacyDARAssetID)) != ''
              and LegacyDARAssetID != DARAssetID
          ) a
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap ) m on m.DARAssetID = a.LegacyDARAssetID and m.InputId = a.InputId
      where m.DARAssetID is null;


      -- 5 LegacyDARAssetID to DARAssetID
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select a.DARAssetID, a.LegacyDARAssetID
      from (
            select distinct DARAssetID, LegacyDARAssetID
            from refmaster_internal.Asset
            where LegacyDARAssetID is not null 
              and LTRIM(RTRIM(LegacyDARAssetID)) != ''
              and LegacyDARAssetID !=DARAssetID
          ) a 
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap ) m on m.DARAssetID = a.DARAssetID and m.InputId = a.LegacyDARAssetID
      where m.DARAssetID is null;
    

      -- 6 DARAssetID to LegacyDARAssetID
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select a.LegacyDARAssetID,a.DARAssetID
      from (
            select distinct LegacyDARAssetID, DARAssetID
            from refmaster_internal.Asset
            where LegacyDARAssetID is not null 
              and LTRIM(RTRIM(LegacyDARAssetID)) != ''
              and LegacyDARAssetID !=DARAssetID
          ) a
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap ) m on m.DARAssetID = a.LegacyDARAssetID and m.InputId = a.DARAssetID
      where m.DARAssetID is null;

      -- 7 DARAssetID to TokenContract Address
      insert into refmaster_internal.AssetIdMap(DARAssetID,InputId)
      select distinct a.DARAssetID,concat(a.TokenContractAddress,'+',a.BlockChain)
      from refmaster_internal.vAssetToken a
      left join (select DARAssetID,InputId from refmaster_internal.AssetIdMap ) m on m.DARAssetID = a.DARAssetID and m.InputId = concat(TokenContractAddress,'+',BlockChain)
      where m.DARAssetID is null;

    
END; //