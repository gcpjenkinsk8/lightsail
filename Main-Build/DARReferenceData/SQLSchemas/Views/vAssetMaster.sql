drop view vAssetMaster

CREATE VIEW `vAssetMaster` AS 
SELECT 
  `a`.`ID` AS `ID`, 
  `a`.`DARAssetID` AS `DARAssetID`, 
  `a`.`DARTicker` AS `DARTicker`, 
  `a`.`Name` AS `Name`, 
  `a`.`AssetType` AS `AssetType`, 
  `a`.`Description` AS `Description`, 
  `a`.`Sponsor` AS `Sponsor`, 
  `a`.`IsBenchmarkAsset` AS `IsBenchmarkAsset`, 
  `a`.`SEDOL` AS `SEDOL`, 
  `a`.`ISIN` AS `ISIN`, 
  `a`.`CUSIP` AS `CUSIP`, 
  `a`.`DTI` AS `DTI`, 
  `a`.`DevelopmentStage` AS `DevelopmentStage`, 
  `a`.`DARSuperSector` AS `DARSuperSector`, 
  `a`.`DARSuperSectorCode` AS `DARSuperSectorCode`, 
  `a`.`DARSector` AS `DARSector`, 
  `a`.`DARSectorCode` AS `DARSectorCode`, 
  `a`.`DARSubSector` AS `DARSubSector`, 
  `a`.`DARSubSectorCode` AS `DARSubSectorCode`, 
  `a`.`DarTaxonomyVersion` AS `DarTaxonomyVersion`, 
  `a`.`DATSSuperSector` AS `DATSSuperSector`, 
  `a`.`DATSSuperSectorCode` AS `DATSSuperSectorCode`, 
  `a`.`DATSSector` AS `DATSSector`, 
  `a`.`DATSSectorCode` AS `DATSSectorCode`, 
  `a`.`DATSSubSector` AS `DATSSubSector`, 
  `a`.`DATSSubSectorCode` AS `DATSSubSectorCode`, 
  `a`.`DATSTaxonomyVersion` AS `DATSTaxonomyVersion`, 
  `a`.`IssuanceFramework` AS `IssuanceFramework`, 
  `a`.`IsRestricted` AS `IsRestricted`, 
  `a`.`EstimatedCirculatingSupply` AS `EstimatedCirculatingSupply`, 
  `a`.`MaxSupply` AS `MaxSupply`, 
  `a`.`MessariTaxonomySector` AS `MessariTaxonomySector`, 
  `a`.`MessariTaxonomyCategory` AS `MessariTaxonomyCategory`, 
  `a`.`IsActive` AS `IsActive`, 
  `a`.`CreateUser` AS `CreateUser`, 
  `a`.`LastEditUser` AS `LastEditUser`, 
  `a`.`CreateTime` AS `CreateTime`, 
  `a`.`LastEditTime` AS `LastEditTime`, 
  `a`.`InstitutionalCustodyAvailable` AS `InstitutionalCustodyAvailable`, 
  `th`.`DarTheme` AS `DarTheme`, 
  `thDATs`.`DatsTheme` AS `DatsTheme`, 
  `au`.`primaryURL` AS `PrimaryURL`, 
  `au`.`twitter` AS `Twitter`, 
  `au`.`reddit` AS `Reddit`, 
  `au`.`blog` AS `Blog`, 
  `au`.`whitePaper` AS `WhitePaper`, 
  `au`.`codeRepositoryURL` AS `CodeRepositoryURL`, 
  `a`.`HasERC20Version` AS `HasERC20Version`, 
  `a`.`HasNYDFSCustoday` AS `HasNYDFSCustoday`, 
  `a`.`LegacyDARAssetId` AS `LegacyDARAssetId`, 
  `a`.`CirculatingSupply` AS `CirculatingSupply`, 
  `a`.`LegacyId` AS `LegacyId`, 
  `a`.`GovernanceToken` AS `GovernanceToken`, 
  `a`.`LayerOne` AS `LayerOne`, 
  `a`.`CMC_ID` AS `CoinMarketCapId`, 
  `a`.`CG_ID` AS `CoinGeckoId`, 
  `a`.`DATSGovernance` AS `DATSGovernance`, 
  `a`.`DATSLayer1` AS `DATSLayer1`, 
  `a`.`isoCurrencyCode` AS `ISOCurrencyCode` 
FROM 
  (
    (
      (
        `Asset` as `a` 
        LEFT JOIN (
          SELECT 
            `d`.`AssetID` AS `AssetID`, 
            MAX(
              CASE WHEN (`d`.`Name` = 'Primary URL') THEN `d`.`URLPath` END
            ) AS `primaryURL`, 
            MAX(
              CASE WHEN (`d`.`Name` = 'Twitter') THEN `d`.`URLPath` END
            ) AS `twitter`, 
            MAX(
              CASE WHEN (`d`.`Name` = 'Reddit') THEN `d`.`URLPath` END
            ) AS `reddit`, 
            MAX(
              CASE WHEN (`d`.`Name` = 'Blog') THEN `d`.`URLPath` END
            ) AS `blog`, 
            MAX(
              CASE WHEN (`d`.`Name` = 'White Paper') THEN `d`.`URLPath` END
            ) AS `whitePaper`, 
            MAX(
              CASE WHEN (
                `d`.`Name` = 'Code Repository URL'
              ) THEN `d`.`URLPath` END
            ) AS `codeRepositoryURL` 
          FROM 
            (
              SELECT 
                `au`.`AssetID` AS `AssetID`, 
                `ut`.`Name` AS `Name`, 
                `au`.`URLPath` AS `URLPath` 
              FROM 
                (
                  `AssetURL` as `au` 
                  JOIN `URLType` as `ut` ON (`au`.`URLTypeID` = `ut`.`ID`)
                )
            ) AS `d` 
          GROUP BY 
            1
        ) AS `au` ON (`a`.`ID` = `au`.`AssetID`)
      ) 
      LEFT JOIN (
        SELECT 
          `t1`.`AssetId` AS `AssetID`, 
          GROUP_CONCAT(
            `t1`.`Theme` 
            ORDER BY 
              `t1`.`Theme` SEPARATOR ','
          ) AS `DarTheme` 
        FROM 
          `vAssetTheme` as `t1` 
        WHERE 
          (`t1`.`ThemeType` = 'DAR') 
        GROUP BY 
          1
      ) AS `th` ON (`th`.`AssetID` = `a`.`ID`)
    ) 
    LEFT JOIN (
      SELECT 
        `t1`.`AssetId` AS `AssetID`, 
        GROUP_CONCAT(
          `t1`.`Theme` 
          ORDER BY 
            `t1`.`Theme` SEPARATOR ','
        ) AS `DatsTheme` 
      FROM 
        `vAssetTheme` as `t1` 
      WHERE 
        (`t1`.`ThemeType` = 'DATS') 
      GROUP BY 
        1
    ) AS `thDATs` ON (`thDATs`.`AssetID` = `a`.`ID`)
  )
WHERE coalesce(a.Deleted,0) = 0