CREATE OR REPLACE PIPELINE `exchangeStatus`
AS LOAD DATA LINK link_kafka_dev 'nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893/refmaster_exchange_status'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `exchangeStatus`
FORMAT JSON
(
    `exchangeStatus`.`darExchangeID` <- `DARExchangeID`,
    `exchangeStatus`.`darMnemonicFamily` <- `DARMnemonicFamily`,
    `exchangeStatus`.`vettedStatus` <- `vettedStatus`,
    `exchangeStatus`.`startTime` <- `StartTime`,
    `exchangeStatus`.`endTime` <- `EndTime`
)
SET
    `loadTime` = now()
