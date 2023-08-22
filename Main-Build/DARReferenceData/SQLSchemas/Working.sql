

------------------------------------------------------------------------------------
--- Section: Pricipal Market Price
------------------------------------------------------------------------------------
select from_unixtime(effectiveTime),*
from calcprice.v1hPrincipalMarketPrice
where ticker = 'btc'
order by from_unixtime(effectiveTime) desc


select * from calcprice.v1hPrincipalMarketPrice where priceID = 'btc-12-1673636400'

select distinct principal_market from calcprice.v15sPriceInNonUSD 

select distinct principal_market from calcprice.vAll_full_window_price 

select * 
from refmaster_internal.vSource
where ShortName like '%OVER%'


select 
    ticker Ticker
  ,methodology  Methodology
  ,windowStart  WindowStart
  ,windowEnd WindowEnd
  ,price UsdPrice
  ,volume UsdVolume
  ,effectiveTime EffectiveTime
  ,priceID  PriceId
  ,darID  DarIdentifier
  ,exchangeName  PrincipalMarket 
  ,FORMAT(pricingTier,0) PricingTier
  ,assetName  AssetName
  ,Currency
from calcprice.v1hPrincipalMarketPrice
left join refmaster_internal.vSource
where priceID = 'btc-12-1673652600'

------------------------------------------------------------------------------------
--- Section: Portal
------------------------------------------------------------------------------------
select * from refmaster_public.vPortalDerivRef where Exchange = 'DERIBIT' and contractType = 'PERPETUAL'
select count(*) from refmaster_internal.Derivatives where ContractExchange = 'BINANCE'



------------------------------------------------------------------------------------
--- Section: Crypto Events
------------------------------------------------------------------------------------

select DAREventID
          ,e.DARAssetID
          ,ExchangeAssetTicker
          ,ExchangeAssetName
          ,SourceId
          ,Exchange
          ,EventType
            ,EventDescription
          ,DATE_FORMAT(EventDate, '%Y-%m-%d') as EventDate
          ,AnnouncementDate
          ,BlockHeight
          ,SourceURL
          ,Notes
          ,Deleted
          ,e.CreateTime
    ,a.DARTicker
        from refmaster_internal.EventInformation e
        left join refmaster_internal.Asset a on e.AssetId = a.ID
where DATE(e.EventDate) >= '2022-12-01' 
  and DATE(e.EventDate) < '2022-12-21' 


------------------------------------------------------------------------------------
--- Section: Client Configuration
------------------------------------------------------------------------------------

select * from refmaster_public.exchange where name like 'As%'

select * from ClientIPs where ClientID = 5008

-- delete from ClientIPs where ClientID = 5008

-- update refmaster_internal.Clients set ExpiryDate = '2024-12-13' Where ClientName like 'Tanweer%'

select * from refmaster_internal.Clients Where ClientName like 'Tanweer%'

------------------------------------------------------------------------------------
--- Section: Jumpy price
------------------------------------------------------------------------------------
select Ticker,name, Pair as exchangePair, UCASE( CONCAT(Ticker,'/',Currency)) as darPair, ExchangeId, 
FORMAT(AVG(USDPrice), 6) as avgUSDPrice,
COUNT(*) as tradeCount
from daxanddex.Pricing_engine_input_trades peit 
join refmaster_public.exchange e on peit.ExchangeId =e.legacyID 
where ticker in ('aaa')
and TSTradeDate > '2022-11-28'
group by Pair , Ticker
order by darPair, avgUSDPrice desc


select date(from_unixtime(effective_time)) as tradeDate,ticker, methodology, abs(
(min(usd_price)- max(usd_price))/ min(usd_price)
) as 1dayChange
from metadata.full_window_price_v2
where from_unixtime(effective_time) > date_add(now(),interval -1 day)
group by date(from_unixtime(effective_time)),ticker, methodology
having 1dayChange > 10
order by 1 desc, 4 desc


select date(from_unixtime(effective_time)) as tradeDate,ticker, methodology, abs(
(min(usd_price)- max(usd_price))/ min(usd_price)
) as 1dayChange
from metadata.full_window_price_v2
where from_unixtime(effective_time) > date_add(now(),interval -1 day)
group by date(from_unixtime(effective_time)),ticker, methodology
having 1dayChange > 50
order by 1 desc, 4 desc

------------------------------------------------------------------------------------


------------------------------------------------------------------------------------
--- Section: Liquidity Pool Pricing 
------------------------------------------------------------------------------------
ALTER TABLE refmaster_internal.Clients add LiquidityPoolPrice tinyint(4) null


select *
from refmaster_internal.Clients 
where ClientName = 'GoldenTree'


update refmaster_internal.Clients  set LiquidityPoolPrice= 1 where ClientName = 'GoldenTree'


SELECT LiquidityPoolPrice
FROM Clients c
INNER JOIN ClientIPs i on c.ID = i.ClientID
WHERE i.CallerID = '1.2.3.4'
AND  LiquidityPoolPrice = 1

select *
from refmaster_public.vLiquidityPoolContracts
where DARLiqPoolID = 'DLPYLIKPTTDH'
  and UseForSupply = 1
select * 
from daxanddex.lpPoolBalance
where darPoolID = 'DLPWCOKRVUQU'
order by timestampUTC desc

select *
from daxanddex.lpTokenBalance
where darPoolID = 'DLPWCOKRVUQU'
order by timeStampUTC desc

select *
from daxanddex.lpTokenBalance
where darPoolID = 'DLPWCOKRVUQU'

SELECT REPLACE(DATE_FORMAT('2022-11-04 05:38:23.000000', '%Y-%m-%dT%TZ'),'Z','+00:00')

select darAssetID,last(tokenSupply,timestampUTC)
from daxanddex.lpTokenBalance lpb 
where darPoolID = 'DLPWCOKRVUQU'
  and timestampUTC > '2022-11-04 05:38:23.000000'
  and timestampUTC <= '2022-11-05 05:38:23.000000'
group by darAssetID


select * 
from refmaster_public.vLiquidityPool 

-- Query

                           select pb.darPoolID
                                    ,vlp.DARTicker as darPoolTicker
                                    ,vlpc.Description as darPoolName
                                    ,vlp.Description  as darPoolDescription
                                    ,pb.poolSupply as poolBalance
                                    ,'USD'  as poolQuoteCurrency
                                    ,pa.tokenDARTicker
                                    ,pa.tokenDARAssetID
                                    ,pa.tokenBalance
                                    ,pa.tokenPrice
                                    ,pa.tokenValueInPool
                                    ,pa.tokenQuoteCurrency
                                    ,pa.WindowStart as windowStart
                                    ,pa.WindowEnd as windowEnd
                                    ,pa.EffectiveTime as effectiveTime
                                    ,'DAR-LP-Latest' as methodologyCode
                            from (
                                select pb.darPoolID
                                ,last(pb.poolSupply,timestampUTC) as poolSupply
                                from daxanddex.lpPoolBalance pb
                                where pb.darPoolID in ('DLPYLIKPTTDH','DLPWCOKRVUQU')
                                  and pb.timestampUTC > '2022-11-04 05:38:23.000000'
                                  and pb.timestampUTC <= '2022-11-05 05:38:23.000000'
                                group by pb.darPoolID
                            ) pb 
                            inner join refmaster_public.vLiquidityPool vlp  on pb.darPoolID = vlp.DARLiqPoolID
                            inner join refmaster_public.vLiquidityPoolContracts vlpc on pb.darPoolID = vlpc.DARLiqPoolID and vlpc.UseForSupply = 1
                            inner join (
                                    select lpb.darPoolID,upper(ticker) as tokenDARTicker,darAssetID as tokenDARAssetID,last(tokenSupply,timestampUTC)  as tokenBalance, last(usd_price,from_unixtime(p.effective_time)) as tokenPrice,tokenBalance * tokenPrice as tokenValueInPool, 'USD' as tokenQuoteCurrency
                                    ,DATE_FORMAT(last(from_unixtime(p.effective_time),from_unixtime(p.effective_time)), '%Y-%m-%dT%TZ') as EffectiveTime
                                    ,DATE_FORMAT(last(from_unixtime(p.window_start),from_unixtime(p.window_start)), '%Y-%m-%dT%TZ')  as WindowStart
                                    ,DATE_FORMAT(last(from_unixtime(p.window_end),from_unixtime(p.window_end)), '%Y-%m-%dT%TZ') as WindowEnd
                                    from daxanddex.lpTokenBalance lpb 
                                    inner join metadata.vAll_full_window_price p on lpb.darAssetID = p.dar_identifier
                                    where darPoolID in ('DLPYLIKPTTDH','DLPWCOKRVUQU')
                                    and timestampUTC > '2022-11-04 05:38:23.000000'
                                    and timestampUTC <= '2022-11-05 05:38:23.000000'
                                    and from_unixtime(p.effective_time) > '2022-11-04 05:38:23.000000'
                                    and from_unixtime(p.effective_time) <= '2022-11-05 05:38:23.000000'
                                    group by lpb.darPoolID,darAssetID,ticker
                            ) pa on pb.darPoolID = pa.darPoolID 
                            
                            
    





select *
from refmaster_public.vLiquidityPool vlp 
where  DARLiqPoolID = 'DLPWCOKRVUQU'

select * 
from refmaster_public.vLiquidityPoolContracts
where  DARLiqPoolID = 'DLPWCOKRVUQU'


select * from token 


select * 
from vAssetToken
where TokenContractAddress= '0x4a86c01d67965f8cb3d0aaa2c655705e64097c31'


select 
  lp.DARLiqPoolID as darPoolID
  ,DARTicker as darPoolTicker
  ,pc.Description as darPoolName
  ,lp.Description as darPoolDescription
  ,last(lpb.poolSupply,timestampUTC) as poolBalance
from refmaster_public.vLiquidityPool lp 
inner join refmaster_public.vLiquidityPoolContracts pc on lp.DARLiqPoolID = pc.DARLiqPoolID
inner join daxanddex.lpPoolBalance lpb on lpb.darPoolID = lp.DARLiqPoolID
where lp.DARLiqPoolID = 'DLPWCOKRVUQU'
  and timestampUTC > '2022-11-04 05:38:23.000000'
  and timestampUTC <= '2022-11-05 05:38:23.000000'





select *
from MarketVector_Events
where '2022-09-16' >= Event_Start_Date
  and '2022-09-16' <= Event_End_Date
  and IndexTicker = 'MVSTAKE'
------------------------------------------------------------------------------------


------------------------------------------------------------------------------------
--- Section: MV
------------------------------------------------------------------------------------
select dar_identifier as  darAssetID,ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
                    from calcprice.vHourly_price t
                    where methodology = 8
                    and dar_identifier in ( select DARAssetId from refmaster_internal.Asset where DARTicker in ('BTC'))
                    -- and from_unixtime(effective_time)  > DATE_ADD(now(), INTERVAL -120 MINUTE)
                    and from_unixtime(effective_time) <= '2023-01-18'
                    group by  dar_identifier,ticker



-- To Create History file
select DATE_FORMAT(RunDate,'%Y%m%d') as 'RunDate', LAST(IndexValue,DATE_FORMAT(RunDate,'%Y%m%d'))  as IndexValue 
from clientReporting.MarketVector_Index_Divisor 
where Type = 'FINAL' and IndexTicker = 'MVSTAK' 
group by DATE_FORMAT(RunDate,'%Y%m%d') 
order by RunDate desc

select * 
from clientReporting.MarketVector_Index_Divisor 
where Type = 'FINAL' 
and IndexTicker = 'MVSTAK'  
order by DATE_FORMAT(RunDate,'%Y%m%d') desc

3179931.5


select * from clientReporting.MarketVector_Events
select * from clientReporting.MarketVector_Handover order by Start desc

select Ticker,count(*)
from clientReporting.MarketVector_Handover  
where DATE_FORMAT(Start, '%Y-%m-%d') = '2023-01-16' 
group by Ticker



select * from clientReporting.MarketVector_Index_Divisor where Type = 'PROFORMA' order by RunDate desc
select * from clientReporting.MarketVector_Index_Divisor where Type = 'FINAL' order by RunDate desc

select * from clientReporting.MarketVector_Index_Divisor order by RunDate desc
delete from clientReporting.MarketVector_Index_Divisor where RunDate = '2023-01-12 22:15:05'


select last(IndexDivisor,RunDate) as IndexDivisor, last(UnitMarketCap,RunDate) as UnitMarketCap 
from clientReporting.MarketVector_Index_Divisor 
where Type = 'FINAL' 
and IndexTicker = 'MVSTAK' 
and RunDate = '2023-01-03 22:00:07'

delete from clientReporting.MarketVector_Index_Divisor where Type = 'PROFORMA' and RunDate > '2022-12-18 22:10:03'
insert into clientReporting.MarketVector_Handover(IndexTicker,Name,Ticker,Conversion,AmountOut,CapFactor,CreateTime,Start)
values('MVSTAK','Bitcoin','BTC','USD',19255406,.4,'2022-12-13 14:10:35','2022-12-14 00:00:00')
update clientReporting.MarketVector_Handover set CreateTime = '2022-12-13 14:10:35', Start = '2022-12-14 00:00:00' 
select * from clientReporting.MarketVector_Index_Divisor where Type = 'FINAL' order by RunDate desc
select * from clientReporting.MarketVector_Index_Divisor where Type = 'PROFORMA' order by RunDate desc
select * from clientReporting.MarketVector_Index_Divisor where Type = 'FINAL' order by RunDate desc


 select IndexTicker,Name,Ticker,Conversion,AmountOut,CapFactor,Start
                    from clientReporting.MarketVector_Handover
                    where Start in ( select max(Start) from clientReporting.MarketVector_Handover where Start < '2023-01-03' )
                      and IndexTicker='MVSTAK'
                    order by Name


select *
from clientReporting.MarketVector_Handover 
where IndexTicker='MVSTAK'
order by Start desc
  -- and Start in ( select max(Start) from clientReporting.MarketVector_Handover where Start < '2022-12-01' )
  and CreateTime = '2022-12-07 11:40:04'
order by CreateTime desc

select * from clientReporting.MarketVector_Events order by CreateTime desc


select Last(IndexDivisor,RunDate)
from clientReporting.MarketVector_Index_Divisor
where Type = 'Final'
select * 
from clientReporting.MarketVector_Index_Divisor
where Type = 'Final'
order by RunDate desc



select * from clientReporting.MarketVector_Index_Divisor
select * from clientReporting.MarketVector_Index_Divisor_20230112
50823464.757747

select last(IndexDivisor,RunDate) as IndexDivisor, last(UnitMarketCap,RunDate) as UnitMarketCap from clientReporting.MarketVector_Index_Divisor where Type = 'FINAL' order by RunDate desc
select IndexTicker,Name,Ticker,Conversion,AmountOut,CapFactor from clientReporting.MarketVector_Handover where Start in ( select max(CreateTime) from clientReporting.MarketVector_Handover where CreateTime < '2022-11-01' ) and IndexTicker='{index}' order by Name

select * from clientReporting.MarketVector_Handover order by CreateTime desc

select * from clientReporting.MarketVector_Index_Divisor where Type = 'FINAL' order by RunDate desc

-- delete from clientReporting.MarketVector_Index_Divisor where RunDate > '2022-11-30 22:00:05'


-- insert into MarketVector_Index_Divisor(RunDate,IndexTicker,Type,IndexDivisor,UnitMarketCap) values (now(),'MVSTAK','PROFORMA',22199760.29, 226667292936.21  )
-- insert into MarketVector_Index_Divisor(RunDate,IndexTicker,Type,IndexDivisor,UnitMarketCap) values (now(),'MVSTAK','FINAL',22199760.29, 226667292936.21)
-- update clientReporting.MarketVector_Handover set CreateTime = '2022-04-30 11:43:09' where CreateTime ='2022-10-27 14:17:08'

------------------------------------------------------------------------------------

------------------------------------------------------------------------------------
--- Section: DAR Task Manager
------------------------------------------------------------------------------------

select t.Name,ts.*
from refmaster_internal.DARTaskStatus ts 
inner join refmaster_internal.DARTask t on ts.DARTaskID = t.DARTaskID 
where Name like 'map_asset_ids' order by ts.TaskRunID desc

select * from refmaster_internal.DARTask where Name = 'MarketVectorOpenFile'

/*
INSERT INTO refmaster_internal.DARTask(DARTaskID,Name,DESCRIPTION,TaskPath,TaskLocation,CreateUser,LastEditUser) 
VALUES(CONCAT('DT',UPPER(LEFT(UUID(), 8))),'find_jumpy_price','Identify mapping errors that are causing incorrect price','TBD','TBD','tzaman','tzaman')
*/
-- delete from  refmaster_internal.DARTask where Name = 'MarketVectorOpen'


------------------------------------------------------------------------------------
--- Section: CIRCULATING SUPPLY
------------------------------------------------------------------------------------

select max(collectedTimeStamp) from refmaster_internal.OutstandingSupply


select * from refmaster_internal.OutstandingSupply where darAssetId in (select DARAssetID from refmaster_internal.Asset where DARTicker = 'OSMO')  and collectedTimeStamp  > DATE_ADD(now(), INTERVAL -120 MINUTE) 
select * from refmaster_internal.circulatingsupplysource where darAssetId  in (select DARAssetID from refmaster_internal.Asset where DARTicker = 'GUSD')
select ExternalSource,CirculatingSupply,LoadTS,* from refmaster_internal.rawCirculatingSupply where DARAssetID in (select DARAssetID from refmaster_internal.Asset where DARTicker = 'LDO') order by LoadTS desc

-- To see how we got the circulating supply in last one hour run following query
select * from refmaster_internal.rawCirculatingSupply where LoadTS >  DATE_ADD(now(), INTERVAL -1 HOUR)   and DARAssetID = 'DAXJV2E'
-- To lookup a source by source id (ExternalSource )
select * from refmaster_internal.vSource where DARSourceID = 'DSFS8BQ' 

select *
from refmaster_internal.circulatingsupplysource  
where ShortName = 'KLAY'

select distinct ShortName from refmaster_internal.circulatingsupplysource  



SELECT  CMC_ID,CG_ID,* from refmaster_internal.Asset where DARTicker = 'BTC'
SELECT  CMC_ID,CG_ID,* from refmaster_internal.Asset where darAssetID = 'DAIK2K1'
select * from refmaster_internal.vSource where ShortName = 'CIRCULATING_SUPPLY_OVERRIDE'
select * from refmaster_internal.vSource where DARSourceID = 'DE26W31'
select * from refmaster_internal.vSource where DARSourceID like 'DS%'

select dar_identifier as  DARAssetID,ticker as Ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
from metadata.vHourly_price t
where methodology = 8
  and dar_identifier in ( select DARAssetId from refmaster_internal.Asset where DARTicker in ('OSMO'))
  and from_unixtime(effective_time)  > DATE_ADD(now(), INTERVAL -120 MINUTE) 
  and from_unixtime(effective_time) <= now() 
group by  dar_identifier,ticker   

select distinct darAssetID
from refmaster_internal.OutstandingSupply 
where collectedTimeStamp  > DATE_ADD(now(), INTERVAL -60 MINUTE) 
limit 10000


select *
from refmaster_internal.rawCirculatingSupply rcs
order by loadTS DESC

select *
from refmaster_internal.OutstandingSupply 
where collectedTimeStamp  > '2022-11-14 00:00:00'
-- and collectedTimeStamp  < '2022-11-14 00:00:00'
order by collectedTimeStamp desc
limit 10000

select name, Pair, Ticker,AVG(USDPrice) as AvgUSDPrice,COUNT(*) as TradeCount, sum(USDPrice* USDSize) as USDVolume
from daxanddex.Pricing_engine_input_trades peit 
join refmaster_public.exchange e on peit.ExchangeId =e.legacyID 
where ticker in ('dfyn')
and TSTradeDate > DATE_ADD(now(), interval -1 day ) 
group by name, Pair , Ticker
order by tradeCount



-- insert into refmaster_internal.circulatingsupplysource(DARAssetID,darsourceid,ShortName,LoadTimestamp) values ('DAAFG27','DSIENWR','COINGECKO',now())
-- insert into refmaster_internal.circulatingsupplysource(DARAssetID,darsourceid,ShortName,LoadTimestamp) values ('DAIK2K1','DSASNFU','COINMARKETCAP',now())
-- update refmaster_internal_DEV.Asset set CG_ID = 'terra-luna' where darAssetID = 'DAFAYJB' 

------------------------------------------------------------------------------------








SELECT CMC_ID,CG_ID,s.* 
from refmaster_internal_DEV.Asset a 
inner join refmaster_internal_DEV.OutstandingSupply s on s.DARAssetID = a.DARAssetID
where a.CMC_ID is not null 
  and a.CG_ID is not null
  and a.CMC_ID != 0
  and a.CG_ID != 0
  and s.CollectedTimeStamp > '2022-10-24 00:00:00'
  order by CollectedTimeStamp desc

SELECT CMC_ID,CG_ID,s.* 
from refmaster_internal.Asset a 
inner join refmaster_internal.OutstandingSupply s on s.DARAssetID = a.DARAssetID
where a.CMC_ID is not null 
  and a.CG_ID is not null
  and a.CMC_ID != 0
  and a.CG_ID != 0
  and s.CollectedTimeStamp > '2022-10-24 00:00:00'
  order by CollectedTimeStamp desc






SELECT CMC_ID,CG_ID,*
from refmaster_internal.Asset a
where CreateUser = 'lalmanda'
  and CreateTime > '2022-10-24 00:00:00'










select * from refmaster_internal_DEV.vSource where DARSourceID in ('DEC5D97', 'DEH8D0U', 'DEMWGI1', 'DE26W31')

select * from refmaster_internal_DEV.vSource where ShortName like '%CoinGec%'

/*
update refmaster_internal.OutstandingSupply s 
inner join refmaster_internal.Asset a on s.darAssetID = a.darAssetId 
set OutstandingSupply = 66725694030
where a.DARTicker = 'USDT'
  and s.CollectedTimeStamp >= '2022-06-27 00:30:00'
  and s.CollectedTimeStamp < '2022-06-28 00:30:00'
*/



SELECT * from refmaster_internal_DEV.circulatingsupplysource where darsourceid=9000001 order by LoadTimestamp desc
SELECT * from refmaster_internal_DEV.rawCirculatingSupply


select *
from vSource
where ID = '9000001'




select * from refmaster_public.vDerivativesReferenceMaster where ContractTicker like 'BTC%15NOV%'
select * from refmaster_public.vDerivativesReferenceMaster where contract_type like 'future' and ContractTicker like 'BTC%' order by ContractTicker

CREATE DEFINER=`afc136ab-b36f-4d4c-b1fd-9fd14194ae75`@`%` SCHEMA_BINDING=OFF VIEW `vDerivativesReferenceMaster` AS SELECT DISTINCT `SUB`.`ContractTicker` AS `ContractTicker`, `SUB`.`DARContractID` AS `DARContractID`, `SUB`.`settlement_period` AS `settlement_period`, `SUB`.`settlement_currency` AS `settlement_currency`, `SUB`.`quote_currency` AS `quote_currency`, `SUB`.`price_index` AS `price_index`, `SUB`.`option_type` AS `option_type`, `SUB`.`contract_type` AS `contract_type`, `SUB`.`instrument_id` AS `instrument_id`, `SUB`.`counter_currency` AS `counter_currency`, `SUB`.`base_currency` AS `base_currency`, `SUB`.`future_type` AS `future_type`, `SUB`.`ContractExchangeTicker` AS `ContractExchangeTicker`, `SUB`.`Exchange` AS `Exchange`, `SUB`.`DARAssetID` AS `DARAssetID` FROM  ( SELECT `dr`.`ContractTicker` AS `ContractTicker`, `dr`.`DARContractID` AS `DARContractID`, MAX(`dr`.`settlement_period`) AS `settlement_period`, MAX(`dr`.`settlement_currency`) AS `settlement_currency`, MAX(`dr`.`quote_currency`) AS `quote_currency`, MAX(`dr`.`price_index`) AS `price_index`, MAX(`dr`.`option_type`) AS `option_type`, MAX(`dr`.`kind`) AS `contract_type`, MAX(`dr`.`instrument_id`) AS `instrument_id`, MAX(`dr`.`counter_currency`) AS `counter_currency`, MAX(`dr`.`base_currency`) AS `base_currency`, MAX(`dr`.`future_type`) AS `future_type`, MAX(`dr`.`ContractExchangeTicker`) AS `ContractExchangeTicker`, MAX(`dr`.`Exchange`) AS `Exchange`, MAX(`dr`.`DARAssetID`) AS `DARAssetID` FROM  `refmaster_internal`.`DerivativesRisk` as `dr`  GROUP BY 1, 2 UNION ALL (SELECT `Derivatives`.`ContractTicker` AS `ContractTicker`, `Derivatives`.`DARContractID` AS `DARContractID`, '' AS `settlement_period`, `Derivatives`.`SettlementCurrency` AS `settlement_currency`, `Derivatives`.`QuoteCurrency` AS `quote_currency`, '' AS `price_index`, `Derivatives`.`OptionType` AS `option_type`, `Derivatives`.`ContractType` AS `contract_type`, (NULL!:>int(20) NULL) AS `instrument_id`, `Derivatives`.`QuoteCurrency` AS `counter_currency`, `Derivatives`.`BaseCurrency` AS `base_currency`, '' AS `future_type`, `Derivatives`.`ContractExchangeTicker` AS `ContractExchangeTicker`, `Derivatives`.`ContractExchange` AS `Exchange`, `Derivatives`.`UnderlierDARAssetID` AS `DARAssetID` FROM  `refmaster_internal`.`Derivatives` as `Derivatives`  WHERE (`Derivatives`.`ContractExchange` IN ('BINANCE','FTX_SPOT'))) ) AS `SUB` /*!90623 OPTION(CLIENT_FOUND_ROWS=1)*/
select distinct contract_type from refmaster_public.vDerivativesReferenceMaster where ContractTicker like 'BTC%15NOV%'

-- delete from Derivatives_temp
ALTER TABLE Derivatives_temp  
    ADD CONSTRAINT DF_Constraint_ID  
    DEFAULT 1 FOR ID;  
    
ALTER TABLE Derivatives_temp ADD CONSTRAINT DF_Constraint_ID DEFAULT 1 FOR ID;

select * from refmaster_internal.Derivatives_temp
select * from refmaster_internal.Exchange where ShortName like 'FTX'

select * 
from refmaster_public.vDerivativesReferenceMaster
where DARContractID = 'DC40EXNG'

SELECT d.ExpirationDate,d.*
FROM refmaster_internal.Derivatives d
where DARContractID = 'DC40EXNG'
                          




select * from refmaster_public.vDerivativesReferenceMaster

drop view refmaster_public.vDerivativesReferenceMaster

drop view refmaster_public.vDerivativesReferenceMaster1

drop view refmaster_public.vDerivativesReferenceMaster;
CREATE  VIEW `vDerivativesReferenceMaster` AS 
SELECT DISTINCT `SUB`.`ContractTicker` AS `ContractTicker`, `SUB`.`DARContractID` AS `DARContractID`, `SUB`.`settlement_period` AS `settlement_period`, `SUB`.`settlement_currency` AS `settlement_currency`
, `SUB`.`quote_currency` AS `quote_currency`, `SUB`.`price_index` AS `price_index`, `SUB`.`option_type` AS `option_type`, `SUB`.`contract_type` AS `contract_type`, `SUB`.`instrument_id` AS `instrument_id`
, `SUB`.`counter_currency` AS `counter_currency`, `SUB`.`base_currency` AS `base_currency`, `SUB`.`future_type` AS `future_type`, `SUB`.`ContractExchangeTicker` AS `ContractExchangeTicker`
, `SUB`.`Exchange` AS `Exchange`, `SUB`.`DARAssetID` AS `DARAssetID` 
FROM  (
        SELECT `dr`.`ContractTicker` AS `ContractTicker`, `dr`.`DARContractID` AS `DARContractID`, MAX(`dr`.`settlement_period`) AS `settlement_period`, MAX(`dr`.`settlement_currency`) AS `settlement_currency`
        , MAX(`dr`.`quote_currency`) AS `quote_currency`, MAX(`dr`.`price_index`) AS `price_index`, MAX(`dr`.`option_type`) AS `option_type`, MAX(`dr`.`kind`) AS `contract_type`
        , MAX(`dr`.`instrument_id`) AS `instrument_id`, MAX(`dr`.`counter_currency`) AS `counter_currency`, MAX(`dr`.`base_currency`) AS `base_currency`, MAX(`dr`.`future_type`) AS `future_type`
        , MAX(`dr`.`ContractExchangeTicker`) AS `ContractExchangeTicker`, MAX(`dr`.`Exchange`) AS `Exchange`, MAX(`dr`.`DARAssetID`) AS `DARAssetID` 
        FROM  `refmaster_internal`.`DerivativesRisk` as `dr`  GROUP BY 1, 2 
        UNION ALL 
        (SELECT `Derivatives`.`ContractTicker` AS `ContractTicker`, `Derivatives`.`DARContractID` AS `DARContractID`, '' AS `settlement_period`, `Derivatives`.`SettlementCurrency` AS `settlement_currency`
        , `Derivatives`.`QuoteCurrency` AS `quote_currency`, '' AS `price_index`, `Derivatives`.`OptionType` AS `option_type`, `Derivatives`.`ContractType` AS `contract_type`
        , (NULL!:>int(20) NULL) AS `instrument_id`, `Derivatives`.`QuoteCurrency` AS `counter_currency`, `Derivatives`.`BaseCurrency` AS `base_currency`, '' AS `future_type`
        , `Derivatives`.`ContractExchangeTicker` AS `ContractExchangeTicker`, `Derivatives`.`ContractExchange` AS `Exchange`, `Derivatives`.`UnderlierDARAssetID` AS `DARAssetID` 
        FROM  `refmaster_internal`.`Derivatives` as `Derivatives`  WHERE (`Derivatives`.`ContractExchange` in ( 'BINANCE','FTX_SPOT'))
        ) 
      ) AS `SUB`; 


select * from refmaster_internal.Derivatives where ContractExchange = 'FTX_SPOT'
update refmaster_internal.Derivatives_temp set ContractExchange = 'FTX_SPOT' where ContractExchange = 'FTX'

insert into refmaster_internal.Derivatives
select * from refmaster_internal.Derivatives_temp

select * from refmaster_internal.Derivatives_temp


select * from refmaster_public.vDerivativesReferenceMaster  where Exchange = 'BINANCE' and ContractExchangeTicker like  'SOL%'

select * from refmaster_public.vDerivativesReferenceMaster where Exchange  in ( 'FTX_SPOT')

select distinct ContractSize from refmaster_internal.Derivatives

insert into Derivatives_temp 
select * from refmaster_internal.Derivatives where ContractExchange = 'BINANCE'

SHOW COLUMNS FROM refmaster_public.vDerivativesReferenceMaster
SHOW COLUMNS FROM  refmaster_public.vDerivativesReferenceMaster1

-- delete from refmaster_internal.Derivatives_temp
select * from refmaster_internal.Derivatives_temp

drop table Derivatives_temp;
CREATE TABLE `Derivatives_temp` (
  `ID` bigint(11) NULL,
  `UnderlierDARAssetID` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ContractType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OptionType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `ContractTicker` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DARContractID` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ContractExchange` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ContractExchangeDARID` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Status` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TradingHours` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MinimumTickSize` decimal(18,0) NOT NULL,
  `SettlementTime` varchar(700) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SettlementType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SettlementCurrency` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ExpirationDate` datetime NOT NULL,
  `ContractSize` int(11) NOT NULL,
  `InitialMargin` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MaintenanceMargin` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MarkPrice` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DeliveryPrice` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DeliveryMethod` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FeesURL` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PositionLimit` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PositionLimitURL` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `BlockTradeMinimum` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `LinktoTAndC` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FundingRates` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `IsActive` tinyint(4) NOT NULL,
  `CreateUser` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `LastEditUser` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `CreateTime` datetime NOT NULL,
  `LastEditTime` datetime NOT NULL,
  `ContractExchangeTicker` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `QuoteDARAssetID` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `BaseCurrency` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `QuoteCurrency` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL
) 


select * from refmaster_internal.DerivativesContractID where DARContractID in (select DARContractID from refmaster_public.vDerivativesReferenceMaster1  where Exchange = 'BINANCE')


alter table refmaster_internal.Derivatives add  QuoteCurrency varchar(20) 
alter table refmaster_internal.Derivatives add  BaseCurrency varchar(20) 

select count(*) from refmaster_public.vDerivativesReferenceMaster1;
select count(*) from refmaster_public.vDerivativesReferenceMaster;

select * from  refmaster_public.vDerivativesReferenceMaster



select * from refmaster_public.vDerivativesReferenceMaster

select * from vSource where ShortName = 'Binance'

CREATE PIPELINE `derivative_referene_0`
AS LOAD DATA KAFKA 'nonprod-0-digitalassetresearch-b9b3.aivencloud.com:23893/derivative_referene_0'
CONFIG '{\"sasl.username\": \"avnadmin\",\r\n         \"sasl.mechanism\": \"PLAIN\",\r\n         \"security.protocol\": \"SASL_SSL\",\r\n         \"ssl.ca.location\": \"/etc/memsql/extra/ca-aa435ecbce2021a5a16ce8071dc1aef88b3b4a77.crt\"}'
CREDENTIALS '{"sasl.password": "AVNS_nj_TjUFCxdE_eNL"}'
BATCH_INTERVAL 0
MAX_PARTITIONS_PER_BATCH 1
DISABLE OUT_OF_ORDER OPTIMIZATION
SKIP DUPLICATE KEY ERRORS
INTO TABLE `Derivatives`
FORMAT JSON
(
    `Derivatives`.`UnderlierDARAssetID` <- `UnderlierDARAssetID`,
    `Derivatives`.`ContractType` <- `ContractType`,
    `Derivatives`.`OptionType` <- `OptionType`,
    `Derivatives`.`ContractTicker` <- `ContractTicker`,
    `Derivatives`.`DARContractID` <- `DARContractID`,
    `Derivatives`.`ContractExchange` <- `ContractExchange`,
    `Derivatives`.`ContractExchangeDARID` <- `ContractExchangeDARID`,
    `Derivatives`.`Status` <- `Status`,
    `Derivatives`.`TradingHours` <- `TradingHours`,
    `Derivatives`.`MinimumTickSize` <- `MinimumTickSize`,
    `Derivatives`.`SettlementTime` <- `SettlementTime`,
    `Derivatives`.`SettlementType` <- `SettlementType`,
    `Derivatives`.`SettlementCurrency` <- `SettlementCurrency`,
    `Derivatives`.`ExpirationDate` <- `ExpirationDate`,
    `Derivatives`.`ContractSize` <- `ContractSize`,
    `Derivatives`.`InitialMargin` <- `InitialMargin`,
    `Derivatives`.`MaintenanceMargin` <- `MaintenanceMargin`,
    `Derivatives`.`MarkPrice` <- `MarkPrice`,
    `Derivatives`.`DeliveryPrice` <- `DeliveryPrice`,
    `Derivatives`.`DeliveryMethod` <- `DeliveryMethod`,
    `Derivatives`.`FeesURL` <- `FeesURL`,
    `Derivatives`.`PositionLimit` <- `PositionLimit`,
    `Derivatives`.`PositionLimitURL` <- `PositionLimitURL`,
    `Derivatives`.`BlockTradeMinimum` <- `BlockTradeMinimum`,
    `Derivatives`.`LinktoTAndC` <- `LinktoTAndC`,
    `Derivatives`.`FundingRates` <- `FundingRates`,
    `Derivatives`.`IsActive` <- `IsActive`,
    `Derivatives`.`CreateUser` <- `CreateUser`,
    `Derivatives`.`LastEditUser` <- `LastEditUser`,
    `Derivatives`.`CreateTime` <- `CreateTime`,
    `Derivatives`.`LastEditTime` <- `LastEditTime`,
    `Derivatives`.`ContractExchangeTicker` <- `ContractExchangeTicker`
)





select distinct DARContractID,ContractTicker, ContractExchangeTicker from vDerivativesReferenceMaster limit 20000

select * from refmaster_internal.Derivatives


------------------------------------------------------------------------------------
--- Section: TOP Assets
------------------------------------------------------------------------------------
select
	ROW_NUMBER () over (order by last(a.outstandingSupply,	a.LoadTimestamp) * last(usd_price,	FROM_UNIXTIME(effective_time) )desc ) as 'Rank',
	a.darAssetID as 'DAR Asset ID',
	a2.Name as 'Name',
	upper(b.ticker) as 'DAR Ticker',
	last(a.outstandingSupply,	a.LoadTimestamp) * last(usd_price,	FROM_UNIXTIME(effective_time)) as 'DAR Market Cap'
from	refmaster_internal.OutstandingSupply a
join metadata.hourly_price b on	a.darAssetID = b.dar_identifier
	and effective_time > unix_timestamp(date(now()))
join refmaster_internal.Asset a2 on
	a.darAssetID = a2.DARAssetID
left join refmaster_internal.OutstandingSupply ar on ar.darAssetID = a.darAssetID and ar.Reviewed =1
where
	usd_volume > 0
	and a.darAssetID not in (	select		darAssetID	from refmaster_public.non_pricing_serv_list	where darMnemonic = 'asset-vetting-report-exclude')
GROUP BY a.darAssetID
having	max(a.LoadTimestamp) > date(now())
order by 'DAR Market Cap' desc
limit 75
------------------------------------------------------------------------------------

------------------------------------------------------------------------------------
--- Section: Clients 
------------------------------------------------------------------------------------

select SessionID, c.CallerID
from refmaster_internal.ClientSession cs 
inner join refmaster_internal.ClientIPs c on cs.ClientID = c.ClientID 
where cs.API = 'DAR1Sec'
group by SessionID
103.46.203.12
24.46.135.72
108.14.255.123

select SessionID, max(c.CallerID)
from refmaster_internal.ClientSession cs 
inner join refmaster_internal.ClientIPs c on cs.ClientID = c.ClientID 
where cs.API = 'DAR400Ms'
group by SessionID







select c.ID,c.ClientName, cs.SessionID
from refmaster_internal.ClientSession cs
inner join refmaster_internal.Clients c on cs.ClientID = c.ID 
where cs.API = 'DAR400Ms'
 


-- 'DAR15Sec'
-- 'DAR400Ms'
select c.ClientName,cs.* 
from refmaster_internal.ClientSession cs 
inner join refmaster_internal.Clients c on cs.ClientID = c.ID
where API = 'DAR15Sec' order by ClientID





-- delete from refmaster_internal_DEV.ClientSession 
select * from refmaster_internal.ClientSession 


select c.ClientName,s.* from refmaster_internal.ClientSession s inner join refmaster_internal.Clients c on s.ClientId = c.Id where API = 'DAR1Sec'


select * from refmaster_internal.ClientSession
insert into refmaster_internal.ClientSession values (3,'ZmMaccCUoAMCEIA=','DAR15Sec')


select ID from refmaster_internal.Clients where ClientName in ( 'Tanweer', 'Chainlink-nop1')

select CMC_ID,CG_ID,* from refmaster_internal.Asset where darAssetID = 'DAXKJKR'
select * from refmaster_internal.Asset where DarTicker like 'poly%'

insert into refmaster_internal.ClientAssets(AssetID,ClientID,CreateUser,LastEditUser,CreateTime,LastEditTime,ReferenceData,Price)
values (1125899908000006,11,'tzaman','tzaman',current_timestamp,current_timestamp,1,1)

select * from refmaster_internal.ClientAssets 

select  AssetID from refmaster_internal.ClientAssets where ClientID = 6000
 Chainlink-nop1

select count(*) from refmaster_internal.ClientAssets where ClientID in (select ID from refmaster_internal.Clients where ClientName = 'Chainlink-nop2')

select * from refmaster_internal.Clients where ClientName = 'Chainlink'
select * from refmaster_internal.Asset where DARTicker = 'X2Y2'

call sp_manage_client_asset('ADD','Chainlink','X2Y2','tzaman');

-- TO Add an asset to a client 
call sp_manage_client_asset('ADD','Chainlink-nop1','BTC','<YOUR ID HERE>');
-- TO CLONE a client permission call sp_manage_client_asset('CLONE','FROM CLIENT NAME','BTC','tzaman','TO CLINET NAME')
call sp_manage_client_asset('CLONE','Chainlink-nop1','BTC','tzaman','Chainlink-nop2')


------------------------------------------------------------------------------------


select * from refmaster_public.exchangePairs where darAssetID = 'DAIFBTZ'

select * from refmaster_internal.Asset where DARTicker = 'Sol'

insert into refmaster_public.dar_serv_list values ('asset-vetting-report-include','DAXVC4R','')
insert into refmaster_public.dar_serv_list values ('asset-vetting-report-exclude','TBD','')



ALTER TABLE `dar_serv_list` RENAME `non_pricing_serv_list`

-- DO Following insert to add an entry for pricing engine to start pricing an asset 
--insert ignore into refmaster_public.serv_list 
select 'dar-std-1s-vw',legacyExchangeID, exchangePairName, quoteCurrency, darExchangeVettingStatus,quoteCurrencyPriceTier, now(), now(), '9999-12-31' as endTime, darAssetID, darCurrencyID, darExchangeID, NULL
from refmaster_public.serv_list sl 
where darMnemonic ='dar-std-15s-vw' and darAssetID in (
'DAIFBTZ')




select count(*) from refmaster_public.serv_list sl where darMnemonic = 'asset-vetting-report' and exchangePairName is null






      select p.darAssetID,a.DARTicker, p.lastPrice * supply as marketCap, p.lastPrice,supply, a.CMC_ID,a.CG_ID
                from (
                    select dar_identifier as  darAssetID,ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
                    from metadata.vAll_full_window_price t
                    where methodology = 'DAR'
                    and from_unixtime(effective_time)  > DATE_ADD(now(), INTERVAL -5 MINUTE) 
                    and from_unixtime(effective_time) <= now() 
                    group by  dar_identifier,ticker   
                ) p
                inner join (
                    select darAssetID,last(OutstandingSupply,LoadTimestamp) as supply
                    from refmaster_internal.OutstandingSupply
                    where LoadTimestamp > DATE_ADD(now(), INTERVAL -24 HOUR)
                    group by darAssetID
                ) o on p.darAssetID = o.darAssetID
                inner join refmaster_internal.Asset a on a.DARAssetId = p.darAssetID
                inner join refmaster_public.serv_list sl on a.DARAssetId = sl.darAssetID and sl.darMnemonic = 'asset-vetting-report' and exchangePairName is null
                order by marketCap desc
limit 10000          




select a.DARAssetID,a.DARTicker,a.Name,CMC_ID as CoinmarketcapID, CG_ID as CoingeckoID 
from refmaster_public.serv_list sl
inner join refmaster_internal.Asset a on sl.darAssetId = a.DARAssetId
where sl.darMnemonic = 'asset-vetting-report'
  and exchangePairName not in ('PASSED')




select a.ID,a.DARAssetID
from refmaster_internal.Asset a
left join (select AssetID from refmaster_internal.ClientAssets where ClientID = 11) ca on ca.AssetId = a.ID
where ca.AssetID is null

select a.DARTicker,a.DARAssetId
from refmaster_internal.Asset a
where  a.DARAssetId in (
'DA7GRTR')
limit 1000

select * from refmaster_internal.Asset where DARTicker like 'True%'
select * from refmaster_internal.ClientAssets where ClientId = 11 and AssetID = 1103

select * from logging.logUsage
select c.ClientName,count(*)
from logging.logUsage l 
inner join refmaster_internal.ClientIPs ip on l.callerID = ip.CallerID
inner join refmaster_internal.Clients c on ip.ClientID = c.ID
where calledTS  > DATE_ADD(now(), INTERVAL -24 HOUR) 
  and endPoint = '/prices/latest'
group by c.ClientName


select c.ClientName,l.*
from logging.logUsage l 
inner join refmaster_internal.ClientIPs ip on l.callerID = ip.CallerID
inner join refmaster_internal.Clients c on ip.ClientID = c.ID
where calledTS  > DATE_ADD(now(), INTERVAL -24 HOUR) 
  and endPoint = '/prices/latest'
  --and c.ClientName like 'Stover'
  -- and identifiers = 'BTC'
order by windowStart
limit 1000



select * from refmaster_internal.ClientIPs
select * from refmaster_internal.Clients
select distinct AssetType
from refmaster_internal.Asset


select * from refmaster_internal.OutstandingSupply where darAssetID = 'DARFFL9' order by LoadTimestamp desc




select s.ShortName,last(OutstandingSupply,CollectedTimeStamp) as Supply
from refmaster_internal.Staging_OutstandingSupply os
inner join refmaster_internal.vSource s on os.SourceId = s.id
where darAssetID = 'DAGPHF9' 
and CollectedTimeStamp > '2022-09-26'

select name, Pair, Ticker,AVG(USDPrice) as avgUSDPrice,COUNT(*) as tradeCount
from dax.Pricing_engine_input_trades peit
join refmaster_public.exchange e on peit.ExchangeId =e.legacyID
where ticker in ('spore')
and TSTradeDate > '2022-09-28'
group by name, Pair , Ticker
order by tradeCount



select * from refmaster_internal.Override_Outstanding_supply


select * from refmaster_public.exchangePairs where darAssetId = 'DA72XVM'


select p.darAssetID,a.DARTicker, p.lastPrice * supply as marketCap, p.lastPrice,supply, a.CMC_ID,a.CG_ID
from (
    select dar_identifier as  darAssetID,ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
    from metadata.vAll_full_window_price t
    where methodology = 'DAR'
      and from_unixtime(effective_time)  > DATE_ADD(now(), INTERVAL -5 MINUTE) 
      and from_unixtime(effective_time) <= now() 
    group by  dar_identifier,ticker   
  ) p
inner join (
    select darAssetID,last(OutstandingSupply,LoadTimestamp) as supply
    from refmaster_internal.OutstandingSupply
    where LoadTimestamp > DATE_ADD(now(), INTERVAL -24 HOUR)
    group by darAssetID
   ) o on p.darAssetID = o.darAssetID
inner join refmaster_internal.Asset a on a.DARAssetId = p.darAssetID
order by marketCap desc
limit 1000

13,284,466,354,409,798


select count(*)
 from refmaster_internal.OutstandingSupply a
 where LoadTimestamp > DATE_ADD(now(), INTERVAL -1 HOUR)
 

select 
  ClientName
  ,case when HasFullAccess = 1 then 'True' else 'False' end as HasAccessToAllPricedAssets
  ,case when HourlyPrice = 1 then 'True' else 'False' end as HasAccessToHourlyPrice
  ,case when LatestPrice = 1 then 'True' else 'False' end as hasAccessToLatestPrice
  ,case when Derivatives = 1 then 'True' else 'False' end as hasAccessToDerivativePrices
  ,'False' as HasAccessToLPTokenPrices
  ,case when NFT = 1 then 'True' else 'False' end as HasAccessToNFTPrices
  ,DARAssetID,AssetName,DARTicker
from refmaster_internal.vClientAssets
where ClientName = 'Chainlink'

ALTER VIEW `vClientAssets` 
AS 
SELECT 
`ca`.`ID` AS `ID`, `c`.`ClientName` AS `ClientName`, `c`.`Description` AS `Description`, `ip`.`CallerID` AS `CallerID`, `a`.`DARAssetID` AS `DARAssetID`
, `a`.`Name` AS `AssetName`, `a`.`DARTicker` AS `DARTicker`,  COALESCE(`ca`.`ReferenceData`,0) AS `ReferenceData`,  COALESCE(`ca`.`Price`,0) AS `Price`
, `a`.`ID` AS `AssetID`, `c`.`ID` AS `ClientID`, `c`.`HourlyPrice` AS `HourlyPrice`, `c`.`LatestPrice` AS `LatestPrice`, `c`.`Derivatives` AS `Derivatives`
, `c`.`NFT` AS `NFT` 
, `c`.`HasFullAccess` AS `HasFullAccess` 
, `c`.`Websocket` AS `Websocket` 
, `c`.`Events` AS `Events` 
FROM (((`Clients` as `c`  JOIN `ClientIPs` as `ip`  ON (`c`.`ID` = `ip`.`ClientID`)) JOIN `ClientAssets` as `ca`  ON (`c`.`ID` = `ca`.`ClientID`)) JOIN `Asset` as `a`  ON (`a`.`ID` = `ca`.`AssetID`)) 

select * from token where name like 'Unicrypt%'

select * from token where darAssetID = 'DAZ3BDF'
select * from token where name like 'GreenTrustT'

update token set name = 'Rarible' where darAssetID =  'DA8JD85'

select * from refmaster_internal.Asset where darAssetId  in ('DATCCQZ','DA7NZF3')



insert into refmaster_internal.ClientAssets(AssetID,ClientID,CreateTime,CreateUser,LastEditTime,LastEditUser,ReferenceData,Price)
select ID as AssetId,11 as ClientID, now() as CreateTime, 'tzamman' as CreateUser,now() as LastEditTime, 'tzamman' as LastEditUser,1 as ReferenceData,1 as Price
from refmaster_internal.Asset
where DARAssetID in (
select distinct(darAssetID)
from refmaster_public.serv_list sl
where darMnemonic = 'dar-std-1s-vw'
)

select *
from  refmaster_internal.ClientAssets 
where ClientID = 11

select * from refmaster_internal.ClientIPs where ClientId = 11
select * from refmaster_internal.Clients where ClientName = 'ChainLink'

select distinct ClientName 
from refmaster_internal.vClientAssets 

select * from refmaster_internal.Clients 

select * 
from refmaster_internal.vClientAssets 


where ClientName = 'ChainLink' and CallerID = '81.24.248.16' and DARTicker = 'MSOL'


select * from refmaster_internal.Asset where DARTicker = 'btc'

select CMC_ID,CG_ID,* from refmaster_internal.Asset where CMC_ID is not null


select count(*) FROM full_window_price_v2 where asset_name = 'Apy finance'
select count(*) FROM full_window_price_v2 where asset_name = 'CBC.network'

select * FROM full_window_price_v2 where price_id ='apy-2-1644348210' asset_name = 'Apy finance'
update metadata.full_window_price_v2 set asset_name= 'APY.Finance' where price_id ='apy-2-1644348210';
select * from metadata.full_window_price_v2_pre2022
select * from metadata.full_window_price_v2_derived

select * from metadata.Token where Literal = 'Apy finance'
select * from dardb.Token where Literal = 'Apy finance'
select * from dax.Token where Literal = 'Apy finance'

update refmaster_public.token set name = 'APY.Finance' where darAssetID = 'DAMXN30';

select * from refmaster_public.token 


  

select darExchangeID,exchangePair,darAssetID,darCurrencyID,date(startTime),date(endTime),count(*)
from refmaster_internal_DEV.exchangePairs
group by darExchangeID,exchangePair,darAssetID,darCurrencyID,date(startTime),date(endTime)
having count(*) > 1

select date(startTime),date(endTime)
from refmaster_internal_DEV.exchangePairs

select * from Asset where darAssetId in ('DACCDFC', 'DAVPMBQ')
select * from refmaster_public.token where darTicker like 'FIL3%'
select * from refmaster_public.token where name like '3X Long FIL on Bitrue'

CALL refmaster_public.sp_upsert_asset
                                (lower('FIL3L.BITRUE')
                                ,'3X Long FIL on Bitrue'
                                ,0
                                ,0
                                ,3
                                ,0
                                ,'DAVPMBQ'
                                )

select * from refmaster_public.token  where darAssetId = 'DAVPMBQ'
select * from dax.Token where shortName like 'FIL3L.BITRUE%'
select * from dardb.Token where shortName like 'FIL3L.BITRUE%'
select * from metadata.Token where shortName like 'FIL3L.BITRUE%'



Update  refmaster_public_DEV.serv_list sl 
join refmaster_internal_DEV.exchangePairs ep on sl.exchangePairName = ep.exchangePair and sl.darExchangeID = ep.darExchangeID 
set sl.darAssetID = ep.darAssetID, quoteCurrency = 
where sl.darAssetID <> ep.darAssetID 
 and sl.endTime > now()

Update  refmaster_public_DEV.serv_list sl 
join refmaster_internal_DEV.exchangePairs ep on sl.exchangePairName = ep.exchangePair and sl.darExchangeID = ep.darExchangeID 
set sl.darCurrencyID = ep.darCurrencyID  
where sl.darAssetID <> ep.darAssetID 
 and sl.endTime > now()



select * from exchangePairs where exchangePair = 'XRPGBP'
select * from serv_list where exchangePairName = 'XRPGBP' and darExchangeID = 'DEMWGI1'
select * from refmaster_internal.Asset where DARAssetId = 'DA7PVIQ'



select * from refmaster_internal_DEV.exchangePairs 

select InstitutionalCustodyAvailable,*
from vAssetMaster
where DARTicker  = 'GLP'

select * from refmaster_public.token where darAssetID = 'DA15DPN'
select * from refmaster_public.token where name like 'glp%'

select InstitutionalCustodyAvailable,c.Custodian  
from vAssetCustodian c
inner join Asset a on c.AssetID = a.ID
where a.DARAssetID = 'DA28AAS'

SELECT a.DARAssetID
,case when a.InstitutionalCustodyAvailable = 1 then 'TRUE' else 'FALSE' end as InstitutionalCustodyAvailable
, c.Custodians
FROM vAssetMaster a
LEFT JOIN (SELECT  AssetID, GROUP_CONCAT(Custodian ORDER BY Custodian ASC SEPARATOR ',') as Custodians
            FROM vAssetCustodian
            GROUP BY AssetID
      ) c on a.ID = c.AssetId
where DARAssetID in (
 'DA0119W'
)

select *
from Users
where UserName in ('tim','tsumner')

-- YdvvAcq-oAMCI3w=
select *
from ClientSession
where API = 'DAR1Sec'
-- delete from ClientSession


select * from Clients where id =6

select Concat(DARAssetID,'.', CallerID) as ClientAsset from vClientAssets where CallerID='108.14.255.123'

select SessionID, c.CallerID from ClientSession cs inner join ClientIPs c on cs.ClientID = c.ClientID where cs.API = 'DAR15Sec'

alter table ClientSession add API text 




select o.darExchangeID
          ,o.darContractID
          ,o.underlierDARAssetID
          ,o.currencyTicker
          ,o.bestBidPrice
          ,o.bestBidPrice * p.lastPrice as bestBidPriceUSD
          ,o.bestAskPrice
          ,o.bestAskPrice * p.lastPrice as bestAskPriceUSD
          ,o.bestBidSize
          ,o.bestBidSize * p.lastPrice as bestBidSizeUSD
          ,o.bestAskSize
          ,o.bestAskSize * p.lastPrice as bestAskSizeUSD
          ,t.price
          ,t.price * p.lastPrice as priceUSD
          ,t.size
          ,t.size * p.lastPrice as sizeUSD
          ,t.Side
          ,t.markPrice
          ,t.markPrice * p.lastPrice as markPriceUSD
          ,t.indexPrice
          ,t.tSTradeDate as tradeDate
          ,p.lastPrice
        from
            (
            select  darExchangeID,t.darContractID,underlyingDARAssetID as underlierDARAssetID,currencyTicker,bidPrice as bestBidPrice,askPrice as bestAskPrice,bidSize as bestBidSize, askSize as  bestAskSize,tStampCollected
            from dax.derivativesOrder_TS t
            inner join DerivativesContractID c on t.darContractID = c.DARContractID
            where ( c.ContractTicker = 'ETH-23SEP22-1600-C-DERIBIT' or c.DARContractID = '{darContractID}')
              and darExchangeID = 'DE0VEZY'
              and tStampCollected <= now()
              order by tStampCollected desc
              limit 1
            ) o
          full outer join (
                  select underlyingDARAssetID,t.darContractID,darExchangeID,
                  price,size,Side,currencyTicker,markPrice,indexPrice,tSTradeDate
                  from dax.derivativesTrade_TS t
                  inner join DerivativesContractID c on t.darContractID = c.DARContractID
                  where ( c.ContractTicker = 'ETH-23SEP22-1600-C-DERIBIT' or c.DARContractID = '{darContractID}')
                    and darExchangeID = 'DE0VEZY'
                    and tSTradeDate <= now()
                    order by tSTradeDate desc
                    limit 1
        ) t on o.darExchangeID = t.darExchangeID and o.darContractID = t.darContractID and o.underlierDARAssetID=t.underlyingDARAssetID
        left join (
              select ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
            from metadata.vAll_full_window_price t
            where methodology = 'DAR'
              and ticker in ( 'btc','sol','eth')
              and from_unixtime(effective_time)  > DATE_ADD(now(), INTERVAL -5 MINUTE) 
              and from_unixtime(effective_time) <= now() 
              group by  ticker   
        )  p on p.ticker = o.currencyTicker 

select ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice, last(from_unixtime(effective_time),from_unixtime(effective_time)) LastPriceTime
from metadata.vAll_full_window_price t
where from_unixtime(effective_time) > DATE_ADD('2022-09-12 23:00:00', INTERVAL -5 MINUTE) 
and from_unixtime(effective_time) <= '2022-09-12 23:00:00'
 group by  ticker   

select DATE_ADD('2022-09-12 23:00:00', INTERVAL -5 MINUTE) 
 select *
 from Asset 
 where darticker in ( 'eth','btc','sol')       
        
select ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
from metadata.vAll_full_window_price t
where methodology = 'DAR'
  and ticker in ( 'eth','btc','sol')
  and effective_time > UNIX_TIMESTAMP() - ( 2 *  60 )
  group by  ticker     


select from_unixtime(effective_time),*
from calcprice.vAll_full_window_price 
where ticker = 'BTC' 
and from_unixtime(effective_time) > '2022-12-19'
and usd_price  > 19000

select max(usd_price) as MaxPrice, min(usd_price) as MinPrice
from calcprice.vAll_full_window_price 
where ticker = 'BTC' 
and from_unixtime(effective_time) > '2022-12-15'


ETH-23SEP22-1600-C-DERIBIT

select underlyingDARAssetID,t.darContractID,darExchangeID,
price,size,Side,currencyTicker,markPrice,indexPrice,tSTradeDate,tStampCollected
from dax.derivativesTrade_TS t
inner join DerivativesContractID c on t.darContractID = c.DARContractID
where ( c.ContractTicker = 'ETH-23SEP22-1600-C-DERIBIT' or c.DARContractID = '{darContractID}')
  and darExchangeID = 'DE0VEZY'
  and tSTradeDate <= now()
  order by tStampCollected desc


select  darExchangeID,t.darContractID,underlyingDARAssetID as underlierDARAssetID,currencyTicker,bidPrice as bestBidPrice,askPrice as bestAskPrice,bidSize as bestBidSize, askSize as  bestAskSize,tStampCollected
from dax.derivativesOrder_TS t
inner join DerivativesContractID c on t.darContractID = c.DARContractID
where ( c.ContractTicker = 'ETH-23SEP22-1600-C-DERIBIT' or c.DARContractID = '{darContractID}')
  and darExchangeID = 'DE0VEZY'
  and tStampCollected <= now()
  order by tStampCollected desc


select * from Asset where DARTicker = 'RACCOON'
select * from OutstandingSupply_temp
select * from OutstandingSupply where darAssetID = 'DAYUCX2'
select * from  refmaster_internal.OutstandingSupply a where darAssetID = 'DAQRPSH'

select * from OutstandingSupply_Audit


select * from refmaster_public.exchangePairs

select * from dardb.Exchange
select count(*) from refmaster_internal_DEV.exchangePairs

select *
from refmaster_public.token
order by legacyID desc

select * from refmaster_public.exchange

select count(*)

select * from vExchangePairs 
create view vExchangePairs 
as
select e.LegacyID as legacyExchangeId,e.name as exchangeName,e.darExchangeID,a.legacyID as legacyAssetID,a.darTicker as assetTicker,a.name as assetName,a.darAssetID,c.legacyID as legacyCurrencyID,c.darTicker as currencyTicker,c.name as currencyName,c.darAssetID as darCurrencyID,ep.CreateTime as loadTime
from refmaster_internal_DEV.exchangePairs ep
inner join refmaster_public.token a on ep.darAssetID = a.darAssetID 
inner join refmaster_public.token c on ep.darCurrencyID = c.darAssetID 
inner join refmaster_public.exchange e  on ep.darExchangeID = e.darExchangeID 


select darExchangeID,count(*)
from refmaster_public.exchange
group by darExchangeID
having count(*) > 1

select * 
from refmaster_public.exchange
where darExchangeID in ('DEYU5E6','DENPQ4D')
order by darExchangeID



select * from refmaster_internal.Exchange

select * from dax.Exchange
select * from dax.vExchange
/* delete from refmaster_internal_DEV.exchangePairs */

select count(*) from exchangePairs

select darExchangeID as DARExchangeID
    ,exchangePair as ExchangePair 
    ,darAssetId as DARAssetID
    ,darCurrencyID as DARCurrencyID
from exchangePairs

select *
from Asset where ID in (
select ID
from Asset
group by ID
having count(*) > 1
)
order by CreateTime desc



select *
from refmaster_internal_DEV.exchangePairs ep







SELECT ep.*,a.NAME as Asset,c.Name as Currency,e.ShortName as Exchange
FROM refmaster_internal_DEV.exchangePairs ep
inner join refmaster_internal_DEV.Asset a on ep.darAssetID = a.darAssetId
inner join refmaster_internal_DEV.Asset c on ep.darCurrencyID = c.darAssetId
inner join refmaster_internal_DEV.Exchange e on ep.darExchangeID = e.darExchangeID

select LENGTH(DARPairID) from refmaster_internal_DEV.exchangePairs
select * from refmaster_public.exchangePairs

select * from Asset where ID =    21392098230009900                       

select ID,count(*)
from Asset
group by ID 
having count(*) > 1


DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLClientAssets`(_AssetID bigint(16) NULL, _ClientID bigint(16) NULL, _ID bigint(16) NULL DEFAULT 0, _Price tinyint(4) NULL DEFAULT NULL, _ReferenceData tinyint(4) NULL DEFAULT NULL, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) RETURNS void AS 
DECLARE 
	v_date DATETIME;
    q QUERY(a BIGINT) = SELECT ID FROM Asset WHERE ID=_AssetID;
    asset_id BIGINT; 
BEGIN
    asset_id = SCALAR(q); 
    SELECT NOW() into v_date;
	INSERT INTO ClientAssets(AssetID, ClientID, ReferenceData, Price, CreateUser,LastEditUser, CreateTime, LastEditTime)
	values(_AssetID, _ClientID, _ReferenceData, _Price, _CreateUser,_LastEditUser,v_date, v_date);
	COMMIT;
	ECHO SELECT ID, "Successfully Inserted" AS RowOutput FROM ClientAssets WHERE AssetID=_AssetID AND ClientID=_ClientID; 
    EXCEPTION 
		WHEN ER_SCALAR_BUILTIN_NO_ROWS THEN
			RAISE user_exception("Cannot add or update a child row: a foreign key constraint fails( CONSTRAINT ClientAssets FOREIGN KEY ('AssetID') REFERENCES 'Asset' ('Id'))");
        WHEN ER_DUP_ENTRY THEN
			UPDATE ClientAssets SET  
			ReferenceData=_ReferenceData, 
            Price=_Price,
            LastEditUser=_LastEditUser,
            LastEditTime=v_date 
            WHERE ID=_ID;
			ECHO SELECT ID, "Successfully Updated" AS RowOutput FROM ClientAssets WHERE AssetID=_AssetID AND ClientID=_ClientID; 		
END; //






 SELECT * 
from refmaster_internal_DEV.exchangePairs

select date_format(TSTradeDate, '%M'), count(*)
from dax.SpotTrade_TS
group by date_format(TSTradeDate, '%M') 
union
select date_format(TSTradeDate, '%M'), count(*)
from dax.SpotTrade_TS_2022_Q1
group by date_format(TSTradeDate, '%M') 




select date_format(CreateTme, '%m/%d'), count(*)
from refmaster_internal.DerivativesContractID 
where CreateTme  >  DATE_ADD(now(), INTERVAL -7 DAY)
group by date_format(CreateTme, '%M/%d')
order by date_format(CreateTme, '%M/%d')




CALL refmaster_internal_DEV.spDMLRoleAppModule(
                        'UPSERT'
                        , 1125899906842625
                        , 'DRK7DJJ'
                        , 0
                        , 'darrefadmin'
                        , 'darrefadmin'
                        )

SELECT Count(*) FROM RoleAppModule WHERE AppModuleId=1125899906842625 and RoleId=_RoleId into v_count;

select *
from RoleAppModule 
WHERE AppModuleId=1125899906842625 and RoleId='DRK7DJJ' 

where ID = '1125899906842625'

select *
from AppModule
where Id = 1125899906842625

select *
from Roles
where ID  = 'DRK7DJJ'

DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLRoleAppModule`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _AppModuleId bigint(16) NULL, _RoleId varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL, _ID bigint(16) NULL DEFAULT 0, _CreateUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL, _LastEditUser varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
        v_id BIGINT(16) =  _ID;
        v_date DATETIME = CURRENT_TIMESTAMP();
        v_del_count1 INT = 0;
        v_del_count2 INT = 0;
       
        BEGIN
            SELECT Count(*) FROM RoleAppModule WHERE AppModuleId=_AppModuleId and RoleId=_RoleId into v_count;
            SELECT NOW() into v_date;
            If (UPPER(_OPERATION) = "UPSERT") Then
				IF(v_count = 0) Then
					INSERT INTO RoleAppModule( AppModuleId, RoleId, CreateUser, LastEditUser, CreateTime, LastEditTime)
                    values( _AppModuleId,_RoleId, _CreateUser, _LastEditUser, v_date, v_date);
                    SELECT ID FROM RoleAppModule WHERE AppModuleId=_AppModuleId and RoleId=_RoleId into v_id;
                    ECHO SELECT v_id as "ID" , 'Data Inserted';
   
				ELSEIF(v_count = 1) Then
						UPDATE RoleAppModule SET LastEditUser=_LastEditUser, LastEditTime=v_date WHERE ID=_ID;
						ECHO SELECT  v_id as "ID", 'Data  Updated', v_count as "v_count", _AppModuleId as "_AppModuleId", _RoleId as "_RoleId";
				ELSEIF(v_count > 1) Then
					ECHO SELECT  'Duplicate Date found!!!';
                END IF;
			
            SELECT Count(*) FROM AppModule WHERE ID=_AppModuleId into v_del_count1;
			SELECT Count(*) FROM RoleAppModule WHERE ID=_RoleId into v_del_count2;
			ELSEIF(UPPER(_OPERATION) = "DELETE") Then
				IF(v_del_count1 = 0 and v_del_count2 =0) Then
					DELETE FROM RoleAppModule WHERE ID=_ID;
          ECHO SELECT  v_id as "ID", 'Data Deleted';
				ELSEIF v_del_count1 !=0 Then
					ECHO SELECT "Foreign Key constraint violet here for Table AppModule field (AppModuleId,Id)";
				ELSEIF v_del_count2 !=0 Then
					ECHO SELECT "Foreign Key constraint violet here for Table RoleAppModule field (RoleId,Id)";
				END IF;
				
			END IF;
            Return v_id;
END; //


select *
from dax.derivativesOrder_TS dot
where darContractID = 'DC8M8J29'
order by tStampCollected desc limit 1


DROP VIEW vDerivativesRiskApi;
CREATE VIEW `vDerivativesRiskApi` AS 
SELECT  FROM_UNIXTIME(`dr`.`effective_time`) AS `AsOfDate`
, `dr`.`effective_time` AS `AsOfDateUnixTime`
, `dr`.`ContractExchangeTicker` AS `ContractTicker`
, `dr`.`DARContractID` AS `DARContractID`
, `a`.`DARTicker` AS `UnderlierDARTicker`
, `a`.`DARAssetID` AS `UnderlierDARAssetID`
, `dr`.`kind` AS `ContractType`
, `dr`.`option_type` AS `OptionType`
, `dr`.`Exchange` AS `ContractExchange`
, 'DE0VEZY' AS `ContractExchangeDARID`
, `dr`.`timestamp` AS `Timestamp`
, `dr`.`greeks_vega` AS `Vega`
, `dr`.`greeks_theta` AS `Theta`
, `dr`.`greeks_rho` AS `Rho`
, `dr`.`greeks_gamma` AS `Gamma`
, `dr`.`greeks_delta` AS `Delta` 
, `dr`.`open_interest` AS `OpenInterest` 
FROM ( `refmaster_internal`.`DerivativesRisk` as `dr`  
JOIN  `refmaster_internal`.`Asset` as `a`  ON (`dr`.`DARAssetID` = `a`.`DARAssetID`)); /*!90623 OPTION(CLIENT_FOUND_ROWS=1)*/

select *
from refmaster_public.vDerivativesRiskApi
where DARContractID = 'DC8M8J29'
order by ContractTicker




        select  darExchangeID,darContractID,underlyingDARAssetID as underlierDARAssetID,currencyTicker,bidPrice as bestBidPrice,askPrice as bestAskPrice,bidSize as bestBidSize, askSize as  bestAskSize
                    from dax.derivativesOrder_TS
                    where darContractID = 'DC8M8J29'
                      and darExchangeID = 'DE0VEZY'
                      and tStampCollected <= '2022-08-31 14:35:00.000000'
                      order by tStampCollected desc limit 1

select * from refmaster_internal.Clients where ClientName = 'MikeZ'
/* 
update refmaster_internal.Clients set Events = 1 where ClientName = 'Tanweer' 
update refmaster_internal.Clients set Derivatives = 1 where ClientName = 'MikeZ' 
*/


   SELECT Derivatives
                            FROM refmaster_internal_DEV.Clients c
                            INNER JOIN refmaster_internal_DEV.ClientIPs i on c.ID = i.ClientID
                            WHERE i.CallerID = '24.46.135.72'
                            AND  c.Derivatives = 1

select count(*)
from metadata.vAll_full_window_price l
where effective_time > UNIX_TIMESTAMP() - ( 1 *  (60 * 60 * 24))
  and methodology = 'DAR'
  and dar_identifier = 'DA00GRT'
order by effective_time 

select usd_price,effective_time,from_unixtime(effective_time)
from metadata.vAll_full_window_price
where methodology = 'DAR'
  and ticker = 'eth'
  and effective_time > UNIX_TIMESTAMP() - ( 2 *  60 )
order by effective_time desc
limit 1;

select last(t.usd_price,from_unixtime(effective_time)) as lastPrice
from metadata.vAll_full_window_price t
where methodology = 'DAR'
  and ticker = 'eth'
  and effective_time > UNIX_TIMESTAMP() - ( 2 *  60 )
limit 1;


select last(t.usd_price,t.effective_time) as lastPrice
from metadata.vAll_full_window_price t
where methodology = 'DAR'
  and ticker = 'sol'
  and effective_time > UNIX_TIMESTAMP() - ( 2 *  60 )
limit 1

select last(t.usd_price,t.effective_time) as lastPrice from metadata.vAll_full_window_price t




select o.darExchangeID
      ,o.darContractID
      ,o.underlierDARAssetID
      ,o.currencyTicker
      ,o.bestBidPrice
      ,o.bestBidPrice * t.indexPrice as bestBidPriceUSD
      ,o.bestAskPrice
      ,o.bestAskPrice * t.indexPrice as bestAskPriceUSD
      ,o.bestBidSize
      ,o.bestBidSize * t.indexPrice as bestBidSizeUSD
      ,o.bestAskSize
      ,o.bestAskSize * t.indexPrice as bestAskSizeUSD
      ,t.price
      ,t.price * t.indexPrice as priceUSD
      ,t.size
      ,t.size * t.indexPrice as sizeUSD
      ,t.Side
      ,t.markPrice
      ,t.markPrice * t.indexPrice as markPriceUSD
      ,t.indexPrice
      ,t.tSTradeDate as tradeDate
    from
        (
        select  darExchangeID,t.darContractID,underlyingDARAssetID as underlierDARAssetID,currencyTicker,bidPrice as bestBidPrice,askPrice as bestAskPrice,bidSize as bestBidSize, askSize as  bestAskSize
        from dax.derivativesOrder_TS t
        inner join {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesContractID c on t.darContractID = c.DARContractID
        where ( c.ContractTicker = '{darContractID}' or c.DARContractID = '{darContractID}')
          and darExchangeID = '{darExchangeID}'
          and tStampCollected <= '{windowEnd}'
          order by tStampCollected desc
          limit 1
        ) o
      full outer join (
              select underlyingDARAssetID,t.darContractID,darExchangeID,
              price,size,Side,currencyTicker,markPrice,indexPrice,tSTradeDate
              from dax.derivativesTrade_TS t
              inner join {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesContractID c on t.darContractID = c.DARContractID
              where ( c.ContractTicker = '{darContractID}' or c.DARContractID = '{darContractID}')
                and darExchangeID = '{darExchangeID}'
                and tSTradeDate <= '{windowEnd}'
                order by tSTradeDate desc
                limit 1
    ) t on o.darExchangeID = t.darExchangeID and o.darContractID = t.darContractID and o.underlierDARAssetID=t.underlyingDARAssetID




select *
from Asset
where DARTicker = '1INCH'

key = DA00GRT_0 value = Yes
key = DA00GRT_6 value = Yes

select 60 * 60 * 24



select c.ClientName, l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end) as LookbackDays ,count(*) as CallCount
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
group by c.ClientName,l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end)
order by c.ClientName,l.CallerID,l.endPoint

select *
from exchangePairs
-- delete from exchangePairs
CALL refmaster_internal_DEV. spDMLExchangePairs(
                        'INSERT'
                        , 'DEPJ0LSK9BQ'
                        , 'test'
                        , 'test'
                        , 'stes'
                        , 'test'
                        , '2022-08-25 20:36:00.0'
                        , '2022-08-25 20:36:00.0'
                        , 'tazma'
                        )



DELIMITER //
CREATE OR REPLACE PROCEDURE `spDMLExchangePairs`(_OPERATION varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL
, _DARPairID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL
, _DARExchangeID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL
, _ExchangePair varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL
, _DARAssetID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL
, _DARCurrencyID varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL
, _StartTime datetime(6) NULL
, _EndTime datetime(6) NULL
, _User varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL) 
RETURNS text CHARACTER SET utf8 COLLATE utf8_general_ci NULL AS
	DECLARE 
		v_count INT =0;
    v_date DATETIME = CURRENT_TIMESTAMP();
    v_result text = 'result';

		BEGIN
          SELECT Count(*) FROM exchangePairs WHERE DARPairID=_DARPairID into v_count;
          SELECT NOW() into v_date;


          IF(v_count > 0 AND UPPER(_OPERATION) = "INSERT") Then
            select _DARPairID + ' Exists' into v_result;
          END IF;

         If (UPPER(_OPERATION) = "INSERT") Then
                INSERT INTO exchangePairs( DARPairID, darExchangeID, exchangePair,darAssetID,darCurrencyID,startTime, endTime, CreateUser, LastEditUser, CreateTime, LastEditTime)
                  values( _DARPairID,_DARExchangeID, _ExchangePair,_DARAssetID,_DARCurrencyID,_StartTime,_EndTime,_User,_User,v_date,v_date);
                COMMIT;
                select 'Inserted' into v_result;
            ELSEIF(UPPER(_OPERATION) = "UPDATE") Then
                UPDATE exchangePairs 
                    SET darExchangeID=_DARExchangeID
                    ,exchangePair=_ExchangePair
                    ,darAssetID = _DARAssetID
                    ,darCurrencyID = _DARCurrencyID 
                    ,startTime = _StartTime
                    ,endTime = _EndTime
                    ,LastEditUser=_User
                    ,LastEditTime=v_date
                WHERE DARPairID=_DARPairID;
                select 'Updated' into v_result;
            ELSEIF(UPPER(_OPERATION) = "DELETE") Then
                DELETE FROM exchangePairs WHERE DARPairID=_DARPairID;
                select 'Deleted' into v_result;
            END IF;
            ECHO SELECT _DARPairID as "Id", v_result as  "Result",_StartTime as StartTime ;
            Return v_result;
END; //
-------------------------------------------
-- Section: PRICE
-------------------------------------------



select from_unixtime(effective_time),*
from metadata.vAll_full_window_price
where effective_time >= 1660597683
    and effective_time < 1660597803
    and methodology = 'DAR'
    and dar_identifier  = 'DAMFI9C'
order by effective_time desc

select *
from refmaster_internal.Asset
where DARTicker = 'BTC'


select max(effective_time) from vAll_full_window_price
from vAll_full_window_price
where methodology = 'DAR'
  -- and effective_time > 1661455279
  and dar_identifier  = 'DA0WV5E'
order by effective_time desc
select * from refmaster_internal.Asset where DARTicker = 'AAVE'



----------------------------------------------------------------

----------------------------
  select `DARAssetID`
  ,`MessariTaxonomySector`
  ,`MessariTaxonomyCategory` 
  ,`DARSuperSector` 
  ,`DARSuperSectorCode` 
  ,`DARSector` 
  ,`DARSectorCode` 
  ,`DARSubSector` 
  ,`DARSubSectorCode`
  ,`DarTaxonomyVersion`
  ,`IssuanceFramework`
  ,`InstitutionalCustodyAvailable` 
  ,`DATSSuperSector` 
  ,`DATSSuperSectorCode`
  ,`DATSSector` 
  ,`DATSSectorCode`
  ,`DATSSubSector` 
  ,`DATSSubSectorCode`
  ,`DATSTaxonomyVersion`
  ,`HasERC20Version` 
  ,`HasNYDFSCustoday`
  from Asset
  limit 10000


-----------------------------------





select i.* 
from Clients c
inner join ClientIPs i on c.Id = i.ClientID
where ClientName = 'Tanweer'

                   SELECT Events
                            FROM refmaster_internal_DEV.Clients c
                            INNER JOIN refmaster_internal_DEV.ClientIPs i on c.ID = i.ClientID
                            WHERE i.CallerID = '108.14.255.123'
                            AND  c.Events = 1


    SELECT Events
  FROM Clients c
  INNER JOIN ClientIPs i on c.ID = i.ClientID
  WHERE i.CallerID = '108.14.255.123'
  AND  c.Events = 1

update Clients set Events = 0 where ClientName = 'Tanweer'

alter table Clients add Events tinyint(4) DEFAULT 0





-------------------------------------------
-- Section: Run this query to see which exchange price is off 
-------------------------------------------
select date(from_unixtime(effective_time)) as tradeDate,ticker, methodology, abs(
(min(usd_price)- max(usd_price))/ min(usd_price)
) as 1dayChange
from metadata.full_window_price_v2
where pricing_tier in (1,2) AND
from_unixtime(effective_time) > DATE_ADD(now(), INTERVAL -1 DAY)
group by date(from_unixtime(effective_time)),ticker, methodology
having 1dayChange > 100
order by tradeDate desc, 1dayChange desc

select * from refmaster_public.token where darTicker = 'hec'
select * from refmaster_public.token where upper(name) like  '%HEC%'

select  e.Name,max(Price),min(Price)
from dax.Pricing_engine_input_trades t
inner join  refmaster_public.exchange e on t.ExchangeId = e.legacyID
where upper(pair) like '%hec%'
  and TSTradeDate >= '2022-08-11'
  and Ticker = 'hec'
group by e.Name
order by e.Name
-------------------------------------------

-- Offending exchanges 
select * from refmaster_public.exchange where legacyID in (22)
select * from refmaster_public.exchangePairs where darExchangeID = 'DERJ24W'  and darAssetID = 'DA3TU4C' 

select * from refmaster_public.token where darTicker like '%hec%'

----------------------------------------------------------
-- Section:  Create the new asset.
-- Run following query to update mapping from old asset to new asset
--
----------------------------------------------------------

-- Add new asset id 
SELECT 'DACY9MA' into @new_dar_id; 
-- Add old Asset Id
SELECT 'DA3TU4C' into @old_dar_id;
-- Add exchange name
SELECT 'Bitrue' into @exchange;
select darTicker,name,legacyID
into @new_ticker,@new_name,@new_legacy_id from refmaster_public.token where darAssetID = @new_dar_id;
select darTicker,name,legacyID
into @old_ticker,@old_name,@old_legacy_id from refmaster_public.token where darAssetID = @old_dar_id;
select legacyID into @exchange_id from refmaster_public.exchange where name = @exchange;
select @old_ticker,@old_name,@old_legacy_id,@new_ticker,@new_name,@new_legacy_id,@exchange,@exchange_id;

update refmaster_public.serv_list 
set DARAssetID = @new_dar_id
where legacyExchangeID = @exchange_id
  and DARAssetID = @old_dar_id
  and endTime > current_timestamp();

select * from refmaster_public.serv_list 
where legacyExchangeID = @exchange_id
  and DARAssetID = @new_dar_id
  and endTime > current_timestamp();

update refmaster_public.exchangePairs
set legacyAssetID = @new_legacy_id
, assetTicker = @new_ticker
  , assetName = @new_name
  , darAssetId= @new_dar_id
where legacyAssetID = @old_legacy_id
and legacyExchangeID = @exchange_id;

select * from refmaster_public.exchangePairs
where legacyAssetID = @new_legacy_id
  and legacyExchangeID = @exchange_id;

----------------------------------------------------------


select a.darTicker as PricingEngineTicker, b.DarTicker as AssetMasterTicker, a.Name, b.Name
from refmaster_public.token a
join refmaster_internal.Asset b on a.darAssetID = b.DARAssetID
where upper(a.Name) <> upper(b.Name)



 update refmaster_public.exchangePairs
 set legacyAssetID = 5504
  , assetTicker = 'bancoin'
    , assetName = 'Bananacoin'
    , darAssetId= 'DAMD8UV'
    where legacyAssetID = 1492
  and legacyExchangeID = 57

update refmaster_public.serv_list set DARAssetID = 'DAMD8UV'
where legacyExchangeID = 57
  and DARAssetID = 'DALLQYG'
  and endTime = '9999-12-31 00:00:00.000000'

select * 
from refmaster_internal_DEV.exchangePairs

select * from refmaster_public.exchangePairs limit 10000000

 update refmaster_public.exchangePairs
 set legacyAssetID = 5504
  , assetTicker = 'bancoin'
    , assetName = 'Bananacoin'
    , darAssetId= 'DAMD8UV'
    where legacyAssetID = 1492
  and legacyExchangeID = 57

select * from dardb.ExchangePairs where upper(pair) like '%BOX%' and exchange = 'gateio'
select * from dax.Pricing_engine_input_trades where upper(pair) like '%BOX%'
select * from dardb.ExchangePairs  where upper(pair) like '%BOX%' and  exchange like '%gate%'
select * from refmaster_public.exchangePairs where legacyExchangeID = 71 and exchangePair like '%BOX%' and legacyAssetID = 34
select * from refmaster_public.exchangePairs where legacyExchangeID = 223 and exchangePair like '%BOX%' and legacyAssetID = 34
select * from dardb.Exchange where Id = 71
select * from refmaster_internal.Exchange where LegacyId = 71
select ExchangeId,max(Price),min(Price) from dax.SpotTrade_TS where upper(pair) like '%BOX%'  and TSTradeDate >= '2022-08-08'   and TokenId = 34 group by ExchangeId
select * from refmaster_public.serv_list where legacyExchangeID = 71 and upper(exchangePairName) like 'BOX_USDT' and endTime = '9999-12-31 00:00:00.000000'
select * from refmaster_public.serv_list where legacyExchangeID = 223 and upper(exchangePairName) like '%BOX%' and endTime = '9999-12-31 00:00:00.000000'
select * from refmaster_public.token where legacyId = 34
select * from refmaster_public.token where name like '%defibox%'


select date(from_unixtime(effective_time)) as tradeDate,ticker, methodology, abs(
(min(usd_price)- max(usd_price))/ min(usd_price)
) as 1dayChange
from metadata.full_window_price_v2
where pricing_tier in (1,2) AND
from_unixtime(effective_time) > DATE_ADD(now(), INTERVAL -1 DAY)
group by date(from_unixtime(effective_time)),ticker, methodology
having 1dayChange > 200
order by tradeDate desc, 1dayChange desc

------------------------------------
-- Section: Asset Replication
------------------------------------
CALL refmaster_public.sp_upsert_asset('gamer','Game Station',0,0,3,0,'DAEGJ1H')

select * from refmaster_internal.Asset where DARAssetId = 'DAUKRTJ'  -- Delete this
select * from refmaster_internal.Asset where DARAssetId = 'DA6FKCD'

select * from refmaster_internal.AssetIdMap where DARAssetID = 'DAUKRTJ'
select * from refmaster_internal.AssetIdMap where DARAssetID = 'DA6FKCD'


select * from refmaster_internal.Asset where DARTicker like 'Benqi%' 

select * from metadata.vAll_full_window_price where ticker = 'btmx'
select * from refmaster_public.token where darAssetID = 'DAUKRTJ'
select * from refmaster_public.token where darAssetID = 'DA6FKCD'
select * from refmaster_public.token where darticker = 'GAMER'
select * from refmaster_public.token where name = 'bmax'
/* update refmaster_public.token set name = 'Cheems Inu' where darticker = 'CINU' */
select * from refmaster_public.token_methodology where legacyID = 488

select * from dax.Token where ShortName = 'CINU'
/* update dax.Token set Literal = 'Cheems Inu' where ShortName = 'CINU' */
select * from dardb.Token where ShortName = 'CINU'
/* update dardb.Token set Literal = 'Cheems Inu'  where ShortName = 'CINU'*/
select * from metadata.Token where ShortName = 'CINU'
/* 
delete from metadata.Token where ShortName = 'CINU';
insert into metadata.Token(Id,ShortName,Literal,OtherPricing,FtseStatus,IndexStatus,ExchangeSourceStatus,dar_identifier)
 values (3459,'cinu','Cheems Inu',0,0,0,3,'DAZYOJV');
 */


select * from refmaster_public.token where darTicker = 'arn'
select * from refmaster_public.token where legacyID = 1467
select * from refmaster_internal.Asset where darTicker = 'arn'

select t.legacyID, a.LegacyId, a.DARTicker 
 from refmaster_internal.Asset a 
join refmaster_public_DEV.token t on a.DARAssetID = t.darAssetID 
where a.LegacyId <> t.legacyID  
and a.LegacyId <> 0


select *
from refmaster_internal.Exchange t
where DARExchangeId = 'DEPEFPS'

-- insert into refmaster_internal.Exchange(DARExchangeID,ShortName,CreateUser,LastEditUser,CreateTime,LastEditTime,IsActive)
-- values ('DEPEFPS','KyberSwap Elastic on Polygon','lalmanda','lalmanda',now(),now(),1)


delete from  refmaster_internal.Exchange where DARExchangeId = 'DEPEFPS'

select * from refmaster_public.exchange where darExchangeID = 'DEPEFPS'
-- update refmaster_public.exchange set name = 'KyberSwap Elastic on Polygon', literal = 'KyberSwap Elastic on Polygon' where darExchangeID = 'DEPEFPS'




select CONCAT('DP',char(round(rand()*25)+65),char(round(rand()*25)+65),char(round(rand()*25)+65)
,char(round(rand()*25)+65),char(round(rand()*25)+65),char(round(rand()*25)+65),char(round(rand()*25)+65),char(round(rand()*25)+65),char(round(rand()*25)+65)) as newDarID
from refmaster_internal.exchangePairs ep
LIMIT 1

create view refmaster_public.vExchangePairs as 
select e.LegacyId as legacyExchangeID, e.name as ExchangeName,e.darExchangeId as darExchangeID, ep.exchangePair as exchangePair
,a.legacyId as legacyAssetID, a.darTicker as assetTicker, a.name as assetName,a.darAssetID
,c.legacyId as legacyCurrencyID, c.darTicker as currencyTicker, c.name as currencyName,a.darAssetId as darCurrencyID
,ep.startTime as loadTime
from refmaster_internal.exchangePairs ep 
inner join refmaster_public.exchange e on ep.darExchangeID = e.darExchangeId
inner join refmaster_public.token a on a.darAssetId = ep.darAssetId
inner join refmaster_public.token c on c.darAssetId = ep.darCurrencyId


select *
from refmaster_public.vExchangePairs


select * from refmaster_public.exchangePairs where legacyExchangeId = 71 limit 3000


select legacyExchangeId,count(*)
from refmaster_public.exchangePairs 
group by legacyExchangeId
order by 2 desc


select t.legacyID,a.legacyID
from refmaster_public.token t 
left join refmaster_internal.Asset a on t.legacyID = coalesce(a.legacyID,a.id)
where t.darAssetID = 'DACR4T8' 

select max(legacyId) from refmaster_public.token
select * from refmaster_internal.Asset where darAssetId = 'DACR4T8'
select * from refmaster_public.exchangePairs

select * from refmaster_public.vExchangePairs









select * from daxanddex.vExchangeRates 
where currency ='EUR' and DATE(from_unixtime(Timestamp)) = CURDATE() 
order by timestamp desc LIMIT 1

select * from refmaster_public.token


select c.ClientName, s.*
from Clients c
inner join ClientSession s on c.ID = s.ClientID
-- delete from ClientSession where ClientId = 3

select c.ID,c.ClientName,i.CallerID,cs.SessionID 
from ClientSession cs
inner join Clients c on cs.ClientID = c.ID
inner join ClientIPs i on c.Id = i.ClientID
where cs.SessionID = 'WqfEucjFoAMCEQQ='
  and i.CallerID = '98.245.146.132'


select * from dax.SpotTrade_TS where TokenId = 1492 and TSTradeDate > 


select ExchangeId,max(Price),min(Price)
from dax.SpotTrade_TS
where TSTradeDate >= '2022-08-11'
group by ExchangeId


-- DAMD8UV BANCOIN
update refmaster_public.exchangePairs
set legacyAssetID = 5498
where legacyExchangeID = 71
  and exchangePair = 'BOX_USDT'
  and legacyAssetID = 34

select * from dax.SpotTrade_TS where TokenId = 5504
select *
/*
 update refmaster_public.exchangePairs
 set legacyAssetID = 5504
  , assetTicker = 'bancoin'
    , assetName = 'Bananacoin'
    , darAssetId= 'DAMD8UV'
    where legacyAssetID = 1492
  and legacyExchangeID = 57
  */
from refmaster_public.exchangePairs
where legacyAssetID = 1492
  and legacyExchangeID = 57


/*
update refmaster_public.serv_list set DARAssetID = 'DAMD8UV'
where legacyExchangeID = 57
  and DARAssetID = 'DALLQYG'
  and endTime = '9999-12-31 00:00:00.000000'
*/

select *
from refmaster_public.serv_list
where legacyExchangeID = 57
  and DARAssetID = 'DAMD8UV'
  and endTime = '9999-12-31 00:00:00.000000'





select * from Asset where DARAssetId = 'DALLQYG'
select * from Asset where DARTicker = 'BANCOIN'
select * from refmaster_public.token where darTicker = 'bancoin'

update refmaster_public.serv_list set endTime = current_timestamp
where legacyExchangeID = 71
and upper(exchangePairName) like 'BOX_USDT'
and endTime = '9999-12-31 00:00:00.000000'


insert into refmaster_public.serv_list
  (darMnemonic,legacyExchangeID,exchangePairName,quoteCurrency,darExchangeVettingStatus,loadTime,startTime,endTime,darAssetID,darCurrencyID,darExchangeID)
values
  ('dar-std-15s-vw',71,'BOX_USDT','USDT',2,current_timestamp,current_timestamp,'9999-12-31 00:00:00.000000','DAUHXXY','DAHDALR','DEGL5CS')


select * from Asset where DARTicker = 'BANANA'
select * from Asset where DARTicker = 'BANCOIN'

select *
from dax.Exchange
where id = 57

select date(TSTradeDate) as tradeDay, Pair, name, avg(USDPrice)
from dax.Pricing_engine_input_trades a 
join refmaster_public.exchange b on a.ExchangeId = b.legacyID
where ticker = 'banana'
and TSTradeDate > '2022-08-01'
group by tradeDay, Pair, ExchangeId
order by tradeDay desc


select * from  ClientSession
select *
from Clients
where APIKey is not null 
  and APIKey != 'NULL'



-- Create a new asset called DefiBox in Asset table
-- update refmaster_public.exchangePairs set legacyAssetID = <ID for new asset DefiBox> where legacyExchangeID = 71 and exchangePair like '%BOX%' and legacyAssetID = 34
-- update refmaster_public.serv_list set endTime = now() where legacyExchangeID = 71 and upper(exchangePairName) like 'BOX_USDT' and endTime = '9999-12-31 00:00:00.000000'
-- insert into refmaster_public.serv_list copy above row and set start time to now and end time to '9999-12-31 00:00:00.000000'. Use then new darAssetId for DefiBox


Cloudwall - DASK8KY

select distinct ClientName,CallerID
from Clients_ARCHIVE
where HasFullAccess = 0
order by ClientName

select *
from Clients_ARCHIVE
where HasFullAccess = 0


Select *
from vClientAssets
where ClientName = 'Cloudwall'
  and DARAssetId = 'DAMSTS2'








select *
from (
    select a.DARTicker,a.Name
    from ClientAssets_ARCHIVE ca
    inner join Asset a on ca.AssetId = a.ID
    where ca.ClientID = 68
    ) o 
full outer join vClientAssets v on o.DARTicker = v.DARTicker
where v.CallerID = '64.238.144.194'
  and v.DARTicker is null

select *
from vClientAssets
where ClientName = 'GoldenTree'

select *
from refmaster_public.exchangePairs 
where legacyExchangeID = 71 and exchangePair like '%BOX%' and legacyAssetID = 34

update refmaster_public.exchangePairs 
set legacyAssetID = 5498 
where legacyExchangeID = 71 
  and exchangePair = 'BOX_USDT' 
  and legacyAssetID = 34


update refmaster_public.serv_list set endTime = current_timestamp
where legacyExchangeID = 71 
and upper(exchangePairName) like 'BOX_USDT' 
and endTime = '9999-12-31 00:00:00.000000'


insert into refmaster_public.serv_list
  (darMnemonic,legacyExchangeID,exchangePairName,quoteCurrency,darExchangeVettingStatus,loadTime,startTime,endTime,darAssetID,darCurrencyID,darExchangeID)
values 
  ('dar-std-15s-vw',71,'BOX_USDT','USDT',2,current_timestamp,current_timestamp,'9999-12-31 00:00:00.000000','DAUHXXY','DAHDALR','DEGL5CS')

-------------------------

select * from refmaster_public.exchange where name like 'bis%'
order by name
limit 500

select * from refmaster_internal.Exchange where ShortName like 'bis%'

select * from dax.Exchange where Literal like 'bis%'
select * from refmaster_public.exchange where name like 'bis%'
insert into Exchange(DARExchangeId, ShortName,ExchangeType,CreateUser,CreateTime,IsActive,LastEditUser,LastEditTime) values ('DEKWC6T','Biswap','30','tzaman',current_timestamp,1,'tzaman',current_timestamp)


select *
from Exchange
where DARExchangeId = 'DEKWC6T'

CALL refmaster_public.sp_upsert_exchange('Biswap',3,'DEKWC6T')

select *
from refmaster_public.exchange e2
where darExchangeID in ('DEYU5E6','DENPQ4D')
order by darExchangeID

--------------------------------------------------------------------------------------------------------------------------------
--- Section: Log Usage
--------------------------------------------------------------------------------------------------------------------------------
select * from logging.logUsage where endPoint = '/prices/latest' order by calledTS desc
select * from logging.logUsage where endPoint = '/prices/hourly' order by calledTS desc
select * from logging.logUsage where endPoint = '/prices/LPTokensLatest' order by calledTS desc

select distinct date_format(calledTS, '%Y-%m-%d-%H')
from logging.logUsage
where endPoint = '/prices/derviativesLatest'
  and calledTS > '2023-01-06 00:00:00.000000'
  and calledTS < '2023-01-08 00:00:00.000000'
  and callerID in (select CallerID
from refmaster_internal.ClientIPs
where ClientID in (select ID from refmaster_internal.Clients where ClientName = 'GoldenTree'))
order by date_format(calledTS, '%Y-%m-%d-%H')

select max(calledTS) from logging.logUsage

selec
select ClientID from refmaster_internal.ClientIPs where CallerID in (
select distinct callerID
from logging.logUsage
where endPoint = '/prices/derviativesLatest'
  and calledTS > '2022-01-05 00:00:00.000000'
  and calledTS < '2023-01-06 00:00:00.000000'
)

select * from refmaster_internal.Clients where ClientName = 'GoldenTree'


select date_format(CreateTme, '%m/%d') as date, count(*) as value
from logging.logUsage
where CreateTme  >  DATE_ADD(now(), INTERVAL -7 DAY)
group by date_format(CreateTme, '%M/%d')
order by date_format(CreateTme, '%M/%d')

select c.ClientName as date,count(*) as value
from logging.logUsage l 
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where l.calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
  and c.ClientName not in (select user from dds.api_reporting_filterout)
group by  c.ClientName
order by  c.ClientName

select * from dds.api_reporting_filterout
insert into dds.api_reporting_filterout values('MikeZ',now())



select CURRENT_DATE 

CallerId = '20.108.254.136'
  and 
  and identifiers = 'DADGF0L'
  and DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end) > 0


select c.ClientName, l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end) as LookbackDays ,count(*) as CallCount
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -1 DAY)
group by c.ClientName,l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end)
order by c.ClientName,l.CallerID,l.endPoint


select l.*
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -1 HOUR)
  and c.ClientName = 'Tanweer'

select c.ClientName, l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end) as LookbackDays ,count(*) as CallCount
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
group by c.ClientName,l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end)
order by c.ClientName,l.CallerID,l.endPoint
limit 1000000

select DATE(calledTS),c.ClientName,l.endPoint,count(*)
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
  and c.ClientName like 'dPar%'
group by DATE(calledTS),c.ClientName,l.endPoint
order by DATE(calledTS),c.ClientName,l.endPoint


-- Calls in last 24 Hours
select c.ClientName,l.endPoint,count(*)
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -1 DAY)
group by c.ClientName,l.endPoint
order by c.ClientName,l.endPoint

-- Calls in last 7 days
select DATE(calledTS),c.ClientName,l.endPoint,count(*)
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
group by DATE(calledTS),c.ClientName,l.endPoint
order by DATE(calledTS),c.ClientName,l.endPoint

-- Total number of calls
select DATE(calledTS),c.ClientName,l.endPoint,count(*)
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
group by DATE(calledTS),c.ClientName,l.endPoint
order by DATE(calledTS),c.ClientName,l.endPoint


select c.ClientName, l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end) as LookbackDays ,count(*) as CallCount
from logging.logUsage l
inner join refmaster_internal.ClientIPs i on l.CallerID = i.CallerID
inner join refmaster_internal.Clients c on i.ClientID = c.ID
where calledTS  >  DATE_ADD(now(), INTERVAL -7 DAY)
group by c.ClientName,l.CallerID, endPoint, identifiers,DATEDIFF(calledTS,case when windowStart = '1970-01-01 00:00:00.000000' then calledTS else windowStart end)
order by c.ClientName,l.CallerID,l.endPoint

select *
from ClientIPs

select * from refmaster_internal.Asset where DARTicker = 'DOT'
ClientName   CallerID       EndPoint        Identifier    LookbackDays  COUNT
Anchorage    34.148.107.44  /prices/latest  DASK8KY       0             872
Anchorage    34.148.107.44  /prices/latest  DASK8KY       1             6
Anchorage    34.148.107.44  /prices/latest  DAK708M       0             872
Anchorage    34.148.107.44  /prices/latest  DAK708M       1             6
Anchorage    34.148.107.44  /prices/latest  DAYKXMG       0             872
Anchorage    34.148.107.44  /prices/latest  DAYKXMG       1             6





SELECT *
FROM vClientAssets
where ClientName = 'Tanweer'
  and CallerId = '1.2.3.0'

and ClientID = ( SELECT max(ClientID) from vClientAssets where ClientName = 'Tanweer')

select *
from vAssetMaster


select *
from ClientAssets_ARCHIVE
where ClientId 



select c.ClientName,c.ID,ca.AssetId

from (
      select UPPER(ClientName), max(ID) as ID
      from Clients_ARCHIVE
      group by  UPPER(ClientName)
c
INNER JOIN ClientAssets_ARCHIVE ca on c.ID = ca.ClientID
order by c.ClientName

select *
from ClientAssets


select distinct c1.ID as ClientID,AssetId
from Clients_ARCHIVE c
inner join ClientAssets_ARCHIVE ca on c.ID = ca.ClientID
inner join Clients c1 on  lower(c.ClientName) = lower(c1.ClientName)
where upper(c.ClientName)  in ('CLOUDWALL') 


select distinct ReferenceData,Price
from refmaster_internal.vClientAssets c
where upper(c.ClientName)  in ('CLOUDWALL') 


SELECT *
FROM Clients c
INNER JOIN ClientIPs i on c.ID = i.ClientId
WHERE i.CallerID = '72.80.203.177'
AND  HasFullAccess = 1
                          

-- 3301
select count(*) from refmaster_internal_DEV.vClientAssets where upper(ClientName) not in ('CLOUDWALL') 
select count(*) from refmaster_internal.vClientAssets  where upper(ClientName) not in ('CLOUDWALL') 

select * from refmaster_internal_DEV.vClientAssets where upper(ClientName) not in ('CLOUDWALL') 
select * 
from refmaster_internal.vClientAssets  
where upper(ClientName) in ('CLOUDWALL') 



select * 
from refmaster_internal_DEV.vClientAssets 
where upper(ClientName) not in ('CLOUDWALL') 

select CallerID, count(*)
from refmaster_internal.vClientAssets 
where upper(ClientName) in ('CLOUDWALL') 
group by CallerID


select *

delete from ClientAssets
where ClientId in (
              select distinct ca.ClientId
              from ClientAssets  ca 
              inner join Clients c on ca.ClientID = c.ID
              where upper(ClientName) = ('CLOUDWALL') 
              )

select *
from ClientAssets 
where ClientID = 8


select CallerID,AssetID,count(*)
select *
from refmaster_internal.vClientAssets
group by CallerID,AssetID
having count(*) > 1


select count(*) from refmaster_internal.Clients
select count(*) from refmaster_internal_DEV.Clients
select count(*) from refmaster_internal.ClientAssets

select count(*) from refmaster_internal_DEV.Clients
select count(*) from refmaster_internal_DEV.ClientIp
select count(*) from refmaster_internal_DEV.ClientAssets

SELECT ClientID, COUNT(DISTINCT AssetID) 
FROM refmaster_internal.ClientAssets
group by ClientID;

select ClientID,count(*)
FROM refmaster_internal.ClientAssets
group by ClientID

select *
FROM refmaster_internal.Clients
where Id = 2251799813685250




select * from Asset  where DARTicker like '%box%'
select * from dax.SpotTrade_TS_Unmapped where Pair like 'box%'
select * from refmaster_public.token where darTicker like 'box'
select * from dax.SpotTrade_TS where TokenId = 34 and ExchangeId = 223
select * from dax.SpotTrade_TS where TokenId = 34 and ExchangeId = 77


select * from refmaster_public.exchangePairs where assetTicker = 'box'

select distinct ExchangeId 
select *
from dax.Pricing_engine_input_trades 
where ticker = 'box' 
and TSTradeDate between '2022-08-03' and '2022-08-04' and ExchangeId = 223

select * from metadata.full_window_price_v2 where methodology = 'DAR' and ticker = 'box' order by effective_time desc




-- Get DARAssetID
select DARAssetid
from Asset
where DARAssetId in (ASSET_LIST)
union
-- Get LegacyDARAssetID for a given DARAssetID (We will use legacy DAR assetID to get price from vAll_full_window_price)
select LegacyDarAssetId
from Asset
where DARAssetId in (ASSET_LIST)
  and LegacyDarAssetId is not null
union
-- Get LegacyDARAssetID ( If user passses LegacyDARAssetId then we want to query price using that). Note: Maybe we can skip this 
select LegacyDarAssetId
from Asset
where LegacyDarAssetId in (ASSET_LIST)
  and LegacyDarAssetId is not null
union
-- Get DARAssetID from LegacyId
select DARAssetId
from  Asset
where LegacyDarAssetId in (ASSET_LIST)
union
-- Get DARAssetID from ticker
select DARAssetId
from Asset
where DARTicker in (ASSET_LIST)
union
-- Get LegacyDARAssetID from ticker
select LegacyDarAssetId
from Asset
where DARTicker in (ASSET_LIST)
  and LegacyDarAssetId is not null
union
-- Get DARAssetID from token
select DARAssetID
from vAssetToken
where concat(TokenContractAddress,'+',BlockChain) in (ASSET_LIST)
-- We should have one more union where we get LegacyDARAssetId for input TokenContractAddress,'+',BlockChain






















select b.legacyID,  b.name as exchange, darExchangeID, a.Pair, 
d.legacyID as assetID,d.darTicker as assetTicker,d.name as assetName, d.darAssetID as assetID, 
e.legacyID as legacyCurrencyID,e.darTicker as currencyTicker,e.name as currencyName, e.darAssetID,now()
from dax.SpotTrade_TS_Unmapped a
join refmaster_public.exchange b on a.ExchangeId = b.legacyID 
full outer join dardb.ExchangePairs c on a.Pair = c.pair and b.name = c.exchange
join refmaster_public.token d on upper(c.asset) = upper(d.darTicker)
join refmaster_public.token e on upper(c.currency) = upper(e.darTicker)
where c.pair is  not null and a.Pair is not null and assetTicker not in (select ticker 
                                                                          from (
                                                                                select Upper(a.assetTicker) as Ticker 
                                                                                from refmaster_public.exchangePairs a
                                                                                join refmaster_public.exchange b on a.darExchangeID = b.darExchangeID
                                                                                full outer join dardb.ExchangePairs c on a.exchangePair = c.pair and b.name = c.exchange
                                                                                where a.assetTicker not like '%.%' and Upper(a.assetTicker) <>  Upper(c.asset) group by  a.assetTicker
                                                                                UNION ALL 
                                                                                select Upper(c.asset) as Ticker 
                                                                                  from refmaster_public.exchangePairs a
                                                                                  join refmaster_public.exchange b on a.darExchangeID = b.darExchangeID
                                                                                  full outer join dardb.ExchangePairs c on a.exchangePair = c.pair and b.name = c.exchange
                                                                                where a.assetTicker not like '%.%' and Upper(a.assetTicker) <>  Upper(c.asset) group by  c.asset
                                                                              )
                                                                        )
group by a.Pair, b.name


select * from dax.Token where ShortName = 'rdnt'
select * from metadata.Token where ShortName = 'rdnt'
select * from dardb.Token where ShortName = 'rdnt'
select * from refmaster_public.token where darTicker = 'rdnt'
select * from Asset where darTicker like  '%RDN%'


SELECT '2022-07-19 10:10:50.032000' into @windowStart;
SELECT '2022-07-19 10:10:52.032000' into @windowEnd;
select o.darExchangeID
  ,o.darContractID
  ,o.underlyingDARAssetID
  ,o.currencyTicker,max(bidPrice) as bestBidPrice
  ,min(askPrice) as bestAskPrice
  ,max(bidSize) as bestBidSize
  ,max(askSize) as bestAskSize
  ,t.price
  ,t.size
  ,t.Side
  ,t.currencyTicker
  ,t.markPrice
  ,t.indexPrice
  ,t.tSTradeDate as tradeDate
from dax.derivativesOrder_TS o
inner join (
          select underlyingDARAssetID,darContractID,darExchangeID,
          price,size,Side,currencyTicker,markPrice,indexPrice,tSTradeDate
          from dax.derivativesTrade_TS
          where darContractID = 'DC1Y0RQG'
            and darExchangeID = 'DE0VEZY'
            and tStampCollected <= @windowEnd
            order by tStampCollected desc
            limit 1
) t on o.darExchangeID = t.darExchangeID and o.darContractID = t.darContractID and o.underlyingDARAssetID=t.underlyingDARAssetID
where o.darContractID = 'DC1Y0RQG'
  and o.darExchangeID = 'DE0VEZY'
  and o.tStampCollected <= @windowEnd
   and o.tStampCollected < @windowStart
group by o.darExchangeID,o.darContractID,o.underlyingDARAssetID;





select * from token where darTicker = 'wbtc'
select * from dax.Token where ShortName = 'wbtc'
select * from metadata.Token where ShortName = 'wbtc'

select darTicker,count(*)
select ShortName,count(*)
from dardb.Token
group by ShortName
having count(*) > 1


select * from refmaster_internal.Clients where ClientName = 'Chainlink'

select distinct ClientID,CallerID
from vClientAssets
where ClientName = 'Cloudwall'
 and DARTicker = 'XLR'
 order by CallerID


select * from dax.SpotTrade_2

select count(*)
from refmaster_internal.ClientAssets
where ClientId = 11

insert into refmaster_internal.ClientAssets(AssetID,ClientID,CreateTime,CreateUser,LastEditTime,LastEditUser,ReferenceData,Price)
select ID,11,now(),'tzaman',now(),'tzaman',1,1
from refmaster_internal.Asset
where ID not in ( select AssetID from refmaster_internal.ClientAssets  where ClientId = 11)
 and 
DARAssetID in (
'DASNJU1'
,'DA4GGLQ'

)







select *
from vClientAssets
where CallerId = '24.46.135.72'



select * from UserClaims
-- delete from ClientSession where SessionID = 'V4aoHcLaIAMCK123'

select *
from ClientSession

insert into ClientSession(ClientID,SessionID) values (2251799813685249,'V4aoHcLaIAMCK123')


select s.*,c.*
from ClientSession s
inner join Clients c on s.ClientID = c.ID

select * from Asset 
where DARAssetID = 'DA042MM'

select user from dds.api_reporting_filterout


delete from ClientSession where SessionID = 'V4aoHcLaIAMCK123'



select *
from vClientAssets
where CallerId like '54%'

--------------------------------------------------------------------------------------------------------------------------------
--- Section: Grants
--------------------------------------------------------------------------------------------------------------------------------
SHOW GRANTS FOR  refmaster_svc_uat
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON `calcprice_DEV`.* TO 'refmaster_svc_uat'@'%'

--------------------------------------------------------------------------------------------------------------------------------
--- Section: Derivatives
--------------------------------------------------------------------------------------------------------------------------------

SELECT CONVERT(-123, UNSIGNED);

select '2023-01-04 16:00:00'  into @windowEnd;
select 'DCT3KMPF' into @ticker; 
select 'DE26W31' into @exchange;

select * from calcprice_DEV.1mDerivPrice
select Exchange,* from refmaster_public.vDerivativesReferenceMaster where DARContractID = 'DC8BAZJ9'
select * from refmaster_internal.vExchange where ShortName = 'DERIBIT'
select * from refmaster_public.vDerivativesRisk




select darExchangeID,* from calcprice_DEV.1mDerivPrice where darContractID = 'DCF2E3K2'



                        select 
                                darExchangeID
                                ,t.darContractID
                                ,underlyingDARAssetID
                                ,quoteCCY as currencyTicker
                                ,bestBidPrice as bestBidPrice
                                ,bestBidPriceUSD as bestBidPriceUSD
                                ,bestAskPrice  as bestAskPrice
                                ,bestAskPriceUSD
                                ,bestBidSize
                                ,null as  bestBidSizeUSD
                                ,bestAskSize
                                ,null as bestAskSizeUSD
                                ,lastTradePrice as price
                                ,lastTradePrice as priceUSD
                                ,lastTradeSize as size
                                ,lastTradeSizeUSD as sizeUSD
                                ,lastTradeSide as Side
                                ,markPrice
                                ,markPriceUSD as markPriceUSD
                                ,indexPrice
                                ,lastTradeTimestamp as tradeDate
                        from calcprice_DEV.1mDerivPrice t
                        where (darContractID = 'DCF2E3K2' or darTicker = 'DCF2E3K2' )
                            and darExchangeID = 'DE0VEZY'
                            and effectiveTimestamp in (select max(effectiveTimestamp) from calcprice_DEV.1mDerivPrice where (darContractID = 'DCWMY9JQ' or darTicker = 'DCWMY9JQ' ) and  effectiveTimestamp <= '2023-01-05 23:40:00')
                        order by effectiveTimestamp desc
                                







select '2023-01-04 16:00:00'  into @windowEnd;
select 'DCT3KMPF' into @ticker; 
select 'DE26W31' into @exchange;

select o.darExchangeID
    ,o.darContractID
    ,o.underlierDARAssetID
    ,o.currencyTicker
    ,o.bestBidPrice
    ,o.bestBidPrice * p.lastPrice as bestBidPriceUSD
    ,o.bestAskPrice
    ,o.bestAskPrice * p.lastPrice as bestAskPriceUSD
    ,o.bestBidSize
    ,o.bestBidSize * p.lastPrice as bestBidSizeUSD
    ,o.bestAskSize
    ,o.bestAskSize * p.lastPrice as bestAskSizeUSD
    ,t.price
    ,t.price * p.lastPrice as priceUSD
    ,t.size
    ,t.size * p.lastPrice as sizeUSD
    ,t.Side
    ,t.markPrice
    ,t.markPrice * p.lastPrice as markPriceUSD
    ,t.indexPrice
    ,t.tSTradeDate as tradeDate
  from
      (
      select  darExchangeID,t.darContractID,underlyingDARAssetID as underlierDARAssetID,currencyTicker,bidPrice as bestBidPrice,askPrice as bestAskPrice,bidSize as bestBidSize, askSize as  bestAskSize
      from orderbook.derivativesOrder_TS t
      inner join refmaster_internal.DerivativesContractID c on t.darContractID = c.DARContractID
      where ( c.ContractTicker = @ticker or c.DARContractID = @ticker)
        and darExchangeID = @exchange
        and tStampCollected <= @windowEnd
        order by tStampCollected desc
        limit 1
      ) o
    full outer join (
            select underlyingDARAssetID,t.darContractID,darExchangeID,
            price,size,Side,currencyTicker,markPrice,indexPrice,tSTradeDate
            from daxanddex.derivativesTrade_TS t
            inner join refmaster_internal.DerivativesContractID c on t.darContractID = c.DARContractID
            where ( c.ContractTicker = @ticker  or c.DARContractID = @ticker )
              and darExchangeID = @exchange
              and tSTradeDate <= @windowEnd
              order by tSTradeDate desc
              limit 1
  ) t on o.darExchangeID = t.darExchangeID and o.darContractID = t.darContractID and o.underlierDARAssetID=t.underlyingDARAssetID
  left join (
        select ticker,last(t.usd_price,from_unixtime(effective_time)) as lastPrice
      from calcprice.vAll_full_window_price t
      where methodology = 'DAR'
        and ticker in ( 'btc','sol','eth')
        and from_unixtime(effective_time)  > DATE_ADD( @windowEnd, INTERVAL -5 MINUTE) 
        and from_unixtime(effective_time) <= @windowEnd
        group by  ticker   
  )  p on p.ticker = o.currencyTicker 


select * from refmaster_internal.derv