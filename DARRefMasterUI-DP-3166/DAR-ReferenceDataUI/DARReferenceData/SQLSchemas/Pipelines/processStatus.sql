drop pipeline processStatus
CREATE OR REPLACE PIPELINE `processStatus`
AS LOAD DATA LINK link_kafka_prod 'prod-0-digitalassetresearch-b9b3.aivencloud.com:23893/process_status'
BATCH_INTERVAL 1000
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
DISABLE OFFSETS METADATA GC
REPLACE
INTO TABLE `processStatus`
FORMAT JSON
(
    `processStatus`.`processName` <- `processName`,
    `processStatus`.`lastRunStatus` <- `lastRunStatus`,
    `processStatus`.`businessDate` <- `businessDate` DEFAULT   NULL
)
SET
    `lastUpdateTime` = now()

test pipeline processStatus
start pipeline processStatus
