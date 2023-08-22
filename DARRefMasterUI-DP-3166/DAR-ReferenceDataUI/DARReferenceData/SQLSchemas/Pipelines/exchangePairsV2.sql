CREATE OR REPLACE PIPELINE `exchangePairsV2`
AS LOAD DATA LINK link_kafka_dev 'nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_exchange_pairs_v2'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `exchangePairsV2`
FORMAT JSON
(
    `exchangePairsV2`.`legacyExchangeID` <- `legacyExchangeID`,
    `exchangePairsV2`.`legacyAssetID` <- `legacyAssetID`,
    `exchangePairsV2`.`darExchangeID` <- `DARExchangeID`,
    `exchangePairsV2`.`exchangeName` <- `ExchangeName`,
    `exchangePairsV2`.`exchangePair` <- `ExchangePair`,
    `exchangePairsV2`.`darAssetID` <- `DARAssetID`,
    `exchangePairsV2`.`assetTicker` <- `AssetTicker`,
    `exchangePairsV2`.`assetName` <- `AssetName`,
    `exchangePairsV2`.`legacyCurrencyID` <- `legacyCurrencyID`,
    `exchangePairsV2`.`darCurrencyID` <- `DARCurrencyID`,
    `exchangePairsV2`.`currencyTicker` <- `CurrencyTicker`,
    `exchangePairsV2`.`blockchain` <- `blockchain`,
    `exchangePairsV2`.`contractAddress` <- `contractAddress`,
    `exchangePairsV2`.`darPairID` <- `darPairID`,
    `exchangePairsV2`.`currencyName` <- `CurrencyName`,
    `exchangePairsV2`.`startDate` <- `StartDate`,
    `exchangePairsV2`.`endDate` <- `EndDate`
)
SET
    `loadTime` = now()
