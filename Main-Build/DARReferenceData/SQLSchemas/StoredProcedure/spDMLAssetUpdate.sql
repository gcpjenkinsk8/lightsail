CREATE 
OR REPLACE PROCEDURE `spDMLAssetUpdate`(
  _ID bigint(16) NULL, 
  _CreateTime datetime NULL, 
  _DARAssetID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, 
  _DARTicker varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, 
  _Name varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, 
  _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, 
  _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, 
  _AssetType varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _Description varchar(1500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _Sponsor varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _IsBenchmarkAsset tinyint(4) NULL DEFAULT NULL, 
  _SEDOL varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _ISIN varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _CUSIP varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DTI varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DevelopmentStage varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _MessariTaxonomySector varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _MessariTaxonomyCategory varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DARSuperSector varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DARSuperSectorCode varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DARSector varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DARSectorCode varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DARSubSector varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DARSubSectorCode varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DarTaxonomyVersion decimal(11, 10) NULL DEFAULT NULL, 
  _IssuanceFramework varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _IsRestricted tinyint(4) NULL DEFAULT NULL, 
  _EstimatedCirculatingSupply decimal(16, 15) NULL DEFAULT NULL, 
  _MaxSupply decimal(18, 0) NULL DEFAULT NULL, 
  _LegacyId int(11) NULL DEFAULT NULL, 
  _LegacyDARAssetId varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _InstitutionalCustodyAvailable tinyint(4) NULL DEFAULT NULL, 
  _DATSSuperSector varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DATSSuperSectorCode varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DATSSector varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DATSSectorCode varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DATSSubSector varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DATSSubSectorCode varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _DATSTaxonomyVersion decimal(11, 10) NULL DEFAULT NULL, 
  _HasERC20Version tinyint(4) NULL DEFAULT NULL, 
  _HasNYDFSCustoday tinyint(4) NULL DEFAULT NULL, 
  _CMC_ID varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _CG_ID varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, 
  _CirculatingSupply decimal(18, 0) NULL DEFAULT NULL, 
  _IsActive tinyint(4) NULL DEFAULT 1, 
  _GovernanceToken tinyint(4) NULL DEFAULT null, 
  _LayerOne tinyint(4) NULL DEFAULT null, 
  _DATSGovernance tinyint(4) NULL DEFAULT null, 
  _DATSLayer1 tinyint(4) NULL DEFAULT null
) RETURNS void AS DECLARE v_date DATETIME;
BEGIN 
SELECT 
  NOW() INTO v_date;
START TRANSACTION;
DELETE FROM 
  Asset 
WHERE 
  DARAssetID = _DARAssetID;
INSERT INTO Asset(
  ID, DARAssetID, DARTicker, Name, AssetType, 
  IsActive, CreateUser, LastEditUser, 
  CreateTime, LastEditTime, Description, 
  Sponsor, IsBenchmarkAsset, GovernanceToken, 
  LayerOne, SEDOL, ISIN, CUSIP, DTI, 
  DevelopmentStage, MessariTaxonomySector, 
  MessariTaxonomyCategory, DARSuperSector, 
  DARSuperSectorCode, DARSector, DARSectorCode, 
  DARSubSector, DARSubSectorCode, 
  DarTaxonomyVersion, IssuanceFramework, 
  IsRestricted, EstimatedCirculatingSupply, 
  MaxSupply, LegacyId, LegacyDARAssetId, 
  InstitutionalCustodyAvailable, 
  DATSSuperSector, DATSSuperSectorCode, 
  DATSSector, DATSSectorCode, DATSSubSector, 
  DATSSubSectorCode, DATSTaxonomyVersion, 
  HasERC20Version, HasNYDFSCustoday, 
  CMC_ID, CG_ID, CirculatingSupply, 
  DATSGovernance, DATSLayer1
) 
Values 
  (
    _ID, _DARAssetID, _DARTicker, _Name, 
    _AssetType, _IsActive, _CreateUser, 
    _LastEditUser, _CreateTime, v_date, 
    _Description, _Sponsor, _IsBenchmarkAsset, 
    _GovernanceToken, _LayerOne, _SEDOL, 
    _ISIN, _CUSIP, _DTI, _DevelopmentStage, 
    _MessariTaxonomySector, _MessariTaxonomyCategory, 
    _DARSuperSector, _DARSuperSectorCode, 
    _DARSector, _DARSectorCode, _DARSubSector, 
    _DARSubSectorCode, _DarTaxonomyVersion, 
    _IssuanceFramework, _IsRestricted, 
    _EstimatedCirculatingSupply, _MaxSupply, 
    _LegacyId, _LegacyDARAssetId, _InstitutionalCustodyAvailable, 
    _DATSSuperSector, _DATSSuperSectorCode, 
    _DATSSector, _DATSSectorCode, _DATSSubSector, 
    _DATSSubSectorCode, _DATSTaxonomyVersion, 
    _HasERC20Version, _HasNYDFSCustoday, 
    _CMC_ID, _CG_ID, _CirculatingSupply, 
    _DATSGovernance, _DATSLayer1
  );
COMMIT;
CALL spDMLAssetAudit(
  "UPDATE", _DARAssetID, _LastEditUser
);
END;
