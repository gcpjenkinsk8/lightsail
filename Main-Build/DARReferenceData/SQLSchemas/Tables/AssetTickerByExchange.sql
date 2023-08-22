USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[AssetTickerByExchange]    Script Date: 2/14/2022 10:32:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AssetTickerByExchange](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] [int] NOT NULL,
	[ExchangeID] [int] NOT NULL,
	[AssetName] [nvarchar](255) NULL,
	[AssetDescription] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AssetTickerByExchange_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
,CONSTRAINT [UQ_AssetTickerByExchange_AID_ExchangeId_AN_AD] UNIQUE NONCLUSTERED 
(
	[AssetID] ASC,
	[ExchangeID] ASC,
	[AssetName] ASC,
	[AssetDescription] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssetTickerByExchange] ADD  CONSTRAINT [DF_AssetTickerByExchange_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[AssetTickerByExchange] ADD  CONSTRAINT [DF_AssetTickerByExchange_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[AssetTickerByExchange] ADD  CONSTRAINT [DF_AssetTickerByExchange_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[AssetTickerByExchange] ADD  CONSTRAINT [DF_AssetTickerByExchange_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[AssetTickerByExchange] ADD  CONSTRAINT [DF_AssetTickerByExchange_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[AssetTickerByExchange]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetTickerByExchange_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[AssetTickerByExchange] CHECK CONSTRAINT [FK_Asset_AssetTickerByExchange_AssetID]
GO

ALTER TABLE [dbo].[AssetTickerByExchange]  WITH CHECK ADD  CONSTRAINT [FK_Exchange_AssetTickerByExchange_ExchangeID] FOREIGN KEY([ExchangeID])
REFERENCES [dbo].[Exchange] ([ID])
GO

ALTER TABLE [dbo].[AssetTickerByExchange] CHECK CONSTRAINT [FK_Exchange_AssetTickerByExchange_ExchangeID]
GO



