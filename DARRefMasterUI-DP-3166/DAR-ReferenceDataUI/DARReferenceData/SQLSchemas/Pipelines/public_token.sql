CREATE OR REPLACE PIPELINE `refPublicToken`
AS LOAD DATA LINK link_kafka_dev 'nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_public_token'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `token`
FORMAT JSON
(
   `token`.`legacyID` <- `legacyID`,
    `token`.`darTicker` <- `darTicker`,
    `token`.`name` <- `Name`,
    `token`.`ftseStatus` <- `ftseStatus`,
    `token`.`indexStatus` <- `indexStatus`,
    `token`.`exchangeSourceStatus` <- `exchangeSourceStatus`,
    `token`.`otherPricing` <- `otherPricing`,
    `token`.`darAssetID` <- `darAssetID`
)
SET
    `createTime` = now()
