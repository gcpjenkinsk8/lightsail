CREATE OR REPLACE PIPELINE `servListV2`
AS LOAD DATA LINK link_kafka_dev 'nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_servlist_v2'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `servListV2`
FORMAT JSON
(
    `servListV2`.`darMnemonic` <- `darMnemonic`,
    `servListV2`.`darAssetID` <- `darAssetID`,
    `servListV2`.`priceTier` <- `priceTier`,
    `servListV2`.`startTime` <- `startTime`,
    `servListV2`.`endTime` <- `endTime`
)
SET
    `loadTime` = now()


-- refmaster_exchange_pairs_v2
-- refmaster_exclude_from_pricing
-- refmaster_servlist_v2