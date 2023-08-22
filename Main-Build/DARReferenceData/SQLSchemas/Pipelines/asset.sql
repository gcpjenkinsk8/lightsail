STOP PIPELINE refInternalAsset
TEST PIPELINE refInternalAsset
START PIPELINE refInternalAsset

CREATE PIPELINE `refInternalAsset`
AS LOAD DATA LINK link_kafka_prod 'prod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_asset'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `Asset`
FORMAT JSON
(
    `Asset`.`ID` <- `AssetID`,
    `Asset`.`DARAssetID` <- `DARAssetID`,
    `Asset`.`DARTicker` <- `DARTicker`,
    `Asset`.`Name` <- `Name`,
    `Asset`.`AssetType` <- `AssetType` DEFAULT   NULL,
    `Asset`.`IsActive` <- `IsActive`,
    `Asset`.`CreateUser` <- `CreateUser` DEFAULT   NULL,
    `Asset`.`LastEditUser` <- `LastEditUser` DEFAULT   NULL,
    `Asset`.`CreateTime` <- `CreateTime`,
    `Asset`.`LastEditTime` <- `LastEditTime`,
    `Asset`.`Description` <- `Description` DEFAULT   NULL,
    `Asset`.`Sponsor` <- `Sponsor` DEFAULT   NULL,
    `Asset`.`IsBenchmarkAsset` <- `IsBenchmarkAsset` DEFAULT   NULL,
    `Asset`.`GovernanceToken` <- `GovernanceToken` DEFAULT   NULL,
    `Asset`.`LayerOne` <- `LayerOne` DEFAULT   NULL,
    `Asset`.`SEDOL` <- `SEDOL` DEFAULT   NULL,
    `Asset`.`ISIN` <- `ISIN` DEFAULT   NULL,
    `Asset`.`CUSIP` <- `CUSIP` DEFAULT   NULL,
    `Asset`.`DTI` <- `DTI` DEFAULT   NULL,
    `Asset`.`DevelopmentStage` <- `DevelopmentStage` DEFAULT   NULL,
    `Asset`.`MessariTaxonomySector` <- `MessariTaxonomySector` DEFAULT   NULL,
    `Asset`.`MessariTaxonomyCategory` <- `MessariTaxonomyCategory` DEFAULT   NULL,
    `Asset`.`DARSuperSector` <- `DARSuperSector` DEFAULT   NULL,
    `Asset`.`DARSuperSectorCode` <- `DARSuperSectorCode` DEFAULT   NULL,
    `Asset`.`DARSector` <- `DARSector` DEFAULT   NULL,
    `Asset`.`DARSectorCode` <- `DARSectorCode` DEFAULT   NULL,
    `Asset`.`DARSubSector` <- `DARSubSector` DEFAULT   NULL,
    `Asset`.`DARSubSectorCode` <- `DARSubSectorCode` DEFAULT   NULL,
    `Asset`.`DarTaxonomyVersion` <- `DarTaxonomyVersion` DEFAULT   NULL,
    `Asset`.`IssuanceFramework` <- `IssuanceFramework` DEFAULT   NULL,
    `Asset`.`IsRestricted` <- `IsRestricted` DEFAULT   NULL,
    `Asset`.`EstimatedCirculatingSupply` <- `EstimatedCirculatingSupply` DEFAULT   NULL,
    `Asset`.`MaxSupply` <- `MaxSupply` DEFAULT   NULL,
    `Asset`.`LegacyId` <- `LegacyId` DEFAULT   NULL,
    `Asset`.`LegacyDARAssetId` <- `LegacyDARAssetId` DEFAULT   NULL,
    `Asset`.`InstitutionalCustodyAvailable` <- `InstitutionalCustodyAvailable` DEFAULT   NULL,
    `Asset`.`DATSSuperSector` <- `DATSSuperSector` DEFAULT   NULL,
    `Asset`.`DATSSuperSectorCode` <- `DATSSuperSectorCode` DEFAULT   NULL,
    `Asset`.`DATSSector` <- `DATSSector` DEFAULT   NULL,
    `Asset`.`DATSSectorCode` <- `DATSSectorCode` DEFAULT   NULL,
    `Asset`.`DATSSubSector` <- `DATSSubSector` DEFAULT   NULL,
    `Asset`.`DATSSubSectorCode` <- `DATSSubSectorCode` DEFAULT   NULL,
    `Asset`.`DATSTaxonomyVersion` <- `DATSTaxonomyVersion` DEFAULT   NULL,
    `Asset`.`HasERC20Version` <- `HasERC20Version` DEFAULT   NULL,
    `Asset`.`HasNYDFSCustoday` <- `HasNYDFSCustoday` DEFAULT   NULL,
    `Asset`.`CMC_ID` <- `CMC_ID` DEFAULT   NULL,
    `Asset`.`CG_ID` <- `CG_ID` DEFAULT   NULL,
    `Asset`.`CirculatingSupply` <- `CirculatingSupply` DEFAULT   NULL,
    `Asset`.`DATSGovernance` <- `DATSGovernance` DEFAULT   NULL,
    `Asset`.`DATSLayer1` <- `DATSLayer1` DEFAULT   NULL,
    `Asset`.`isoCurrencyCode` <- `isoCurrencyCode` DEFAULT   NULL,
    `Asset`.`Deleted` <- `Deleted` DEFAULT   NULL

)
