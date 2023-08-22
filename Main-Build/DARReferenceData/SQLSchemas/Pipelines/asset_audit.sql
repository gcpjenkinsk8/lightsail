
STOP PIPELINE refInternalAssetAudit
TEST PIPELINE refInternalAssetAudit
START PIPELINE refInternalAssetAudit


CREATE PIPELINE `refInternalAssetAudit`
AS LOAD DATA LINK link_kafka_prod 'prod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_asset_audit'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `Asset_Audit`
FORMAT JSON
(
    `Asset_Audit`.`ChangeID` <- `AssetID`,
    `Asset_Audit`.`ID` <- `UniqueID`,
    `Asset_Audit`.`DARAssetID` <- `DARAssetID`,
    `Asset_Audit`.`DARTicker` <- `DARTicker`,
    `Asset_Audit`.`Name` <- `Name`,
    `Asset_Audit`.`AssetType` <- `AssetType` DEFAULT   NULL,
    `Asset_Audit`.`IsActive` <- `IsActive`,
    `Asset_Audit`.`CreateUser` <- `CreateUser` DEFAULT   NULL,
    `Asset_Audit`.`LastEditUser` <- `LastEditUser` DEFAULT   NULL,
    `Asset_Audit`.`CreateTime` <- `CreateTime`,
    `Asset_Audit`.`LastEditTime` <- `LastEditTime`,
    `Asset_Audit`.`Description` <- `Description` DEFAULT   NULL,
    `Asset_Audit`.`Sponsor` <- `Sponsor` DEFAULT   NULL,
    `Asset_Audit`.`IsBenchmarkAsset` <- `IsBenchmarkAsset` DEFAULT   NULL,
    `Asset_Audit`.`GovernanceToken` <- `GovernanceToken` DEFAULT   NULL,
    `Asset_Audit`.`LayerOne` <- `LayerOne` DEFAULT   NULL,
    `Asset_Audit`.`SEDOL` <- `SEDOL` DEFAULT   NULL,
    `Asset_Audit`.`ISIN` <- `ISIN` DEFAULT   NULL,
    `Asset_Audit`.`CUSIP` <- `CUSIP` DEFAULT   NULL,
    `Asset_Audit`.`DTI` <- `DTI` DEFAULT   NULL,
    `Asset_Audit`.`DevelopmentStage` <- `DevelopmentStage` DEFAULT   NULL,
    `Asset_Audit`.`MessariTaxonomySector` <- `MessariTaxonomySector` DEFAULT   NULL,
    `Asset_Audit`.`MessariTaxonomyCategory` <- `MessariTaxonomyCategory` DEFAULT   NULL,
    `Asset_Audit`.`DARSuperSector` <- `DARSuperSector` DEFAULT   NULL,
    `Asset_Audit`.`DARSuperSectorCode` <- `DARSuperSectorCode` DEFAULT   NULL,
    `Asset_Audit`.`DARSector` <- `DARSector` DEFAULT   NULL,
    `Asset_Audit`.`DARSectorCode` <- `DARSectorCode` DEFAULT   NULL,
    `Asset_Audit`.`DARSubSector` <- `DARSubSector` DEFAULT   NULL,
    `Asset_Audit`.`DARSubSectorCode` <- `DARSubSectorCode` DEFAULT   NULL,
    `Asset_Audit`.`DarTaxonomyVersion` <- `DarTaxonomyVersion` DEFAULT   NULL,
    `Asset_Audit`.`IssuanceFramework` <- `IssuanceFramework` DEFAULT   NULL,
    `Asset_Audit`.`IsRestricted` <- `IsRestricted` DEFAULT   NULL,
    `Asset_Audit`.`EstimatedCirculatingSupply` <- `EstimatedCirculatingSupply` DEFAULT   NULL,
    `Asset_Audit`.`MaxSupply` <- `MaxSupply` DEFAULT   NULL,
    `Asset_Audit`.`LegacyId` <- `LegacyId` DEFAULT   NULL,
    `Asset_Audit`.`LegacyDARAssetId` <- `LegacyDARAssetId` DEFAULT   NULL,
    `Asset_Audit`.`InstitutionalCustodyAvailable` <- `InstitutionalCustodyAvailable` DEFAULT   NULL,
    `Asset_Audit`.`DATSSuperSector` <- `DATSSuperSector` DEFAULT   NULL,
    `Asset_Audit`.`DATSSuperSectorCode` <- `DATSSuperSectorCode` DEFAULT   NULL,
    `Asset_Audit`.`DATSSector` <- `DATSSector` DEFAULT   NULL,
    `Asset_Audit`.`DATSSectorCode` <- `DATSSectorCode` DEFAULT   NULL,
    `Asset_Audit`.`DATSSubSector` <- `DATSSubSector` DEFAULT   NULL,
    `Asset_Audit`.`DATSSubSectorCode` <- `DATSSubSectorCode` DEFAULT   NULL,
    `Asset_Audit`.`DATSTaxonomyVersion` <- `DATSTaxonomyVersion` DEFAULT   NULL,
    `Asset_Audit`.`HasERC20Version` <- `HasERC20Version` DEFAULT   NULL,
    `Asset_Audit`.`HasNYDFSCustoday` <- `HasNYDFSCustoday` DEFAULT   NULL,
    `Asset_Audit`.`CMC_ID` <- `CMC_ID` DEFAULT   NULL,
    `Asset_Audit`.`CG_ID` <- `CG_ID` DEFAULT   NULL,
    `Asset_Audit`.`CirculatingSupply` <- `CirculatingSupply` DEFAULT   NULL,
    `Asset_Audit`.`DATSGovernance` <- `DATSGovernance` DEFAULT   NULL,
    `Asset_Audit`.`DATSLayer1` <- `DATSLayer1` DEFAULT   NULL,
    `Asset_Audit`.`isoCurrencyCode` <- `isoCurrencyCode` DEFAULT   NULL,
    `Asset_Audit`.`operation` <- `Operation` DEFAULT   NULL

)
