

alter table ExchangePair add ExchangePairNumberId int 
alter table ExchangePair add ExchangePairStringId nvarchar(200)
alter table ExchangePair add ExchangePairShortName varchar(200)
alter table ExchangePair add ExchangePairLongName varchar(255)
alter table ExchangePair add ExchangeAssetStringId nvarchar(200)
alter table ExchangePair add ExchangeAssetNumberId int
alter table ExchangePair add ExchangeAssetShortName nvarchar(200)
alter table ExchangePair add ExchangeAssetLongName nvarchar(255)
alter table ExchangePair add ExchangeCurrencyStringId nvarchar(200)
alter table ExchangePair add ExchangeCurrencyNumberId int
alter table ExchangePair add ExchangeCurrencyShortName nvarchar(200)
alter table ExchangePair add ExchangeCurrencyLongName nvarchar(255)
alter table ExchangePair add IsAvailable  bit


ALTER TABLE [dbo].[ExchangePair] DROP CONSTRAINT [UQ_ExchangePair_ExchangeIDExchangePairName]
GO
ALTER TABLE dbo.ExchangePair ALTER COLUMN [ExchangePairName] VARCHAR(20) COLLATE Latin1_General_CS_AS
GO