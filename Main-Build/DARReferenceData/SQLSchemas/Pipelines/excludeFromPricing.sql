CREATE OR REPLACE PIPELINE `excludeFromPricing`
AS LOAD DATA LINK link_kafka_dev 'nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_exclude_from_pricing'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `excludeFromPricing`
FORMAT JSON
(
    `excludeFromPricing`.`darExchangeID` <- `DARExchangeID`,
    `excludeFromPricing`.`exchangePair` <- `ExchangePair`,
    `excludeFromPricing`.`startDate` <- `StartDate`,
    `excludeFromPricing`.`endDate` <- `EndDate`
)

