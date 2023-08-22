USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Staging_CryptoNodeEvents]    Script Date: 9/21/2021 8:43:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Staging_CryptoNodeEvents](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DateofReview] [datetime] NOT NULL,
	[ExchangeAssetTicker] [nvarchar](100) NOT NULL,
	[ExchangeAssetName] [nvarchar](250) NULL,
	[DARAssetID] [nvarchar](50) NULL,
	[EventType] [nvarchar](100) NULL,
	[EventDate] [datetime] NULL,
	[AnnouncementDate] [datetime] NULL,
	[EventDescription] [nvarchar](500) NULL,
	[SourceURL] [nvarchar](500) NULL,
	[EventStatus] [nvarchar](500) NULL,
	[Notes] [nvarchar](500) NULL,
	[Deleted] [nvarchar](500) NULL,
	[Source] [nvarchar](50) NULL,
	[ValidationTime] [datetime] NULL,
	[AssetID] [int] NULL,
	[SourceID] [int] NULL,
	[EventTypeID] [int] NULL,
	[CreateTime] [datetime] NOT NULL,
	[BlockHeight] [int] NULL,
	[Error]  [nvarchar](500) NULL
 CONSTRAINT [PK_Staging_CryptoNodeEvents_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents] ADD  CONSTRAINT [DF_Staging_CryptoNodeEvents_DateofReview]  DEFAULT (getutcdate()) FOR [DateofReview]
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents] ADD  CONSTRAINT [DF_Staging_CryptoNodeEvents_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Staging_CryptoNodeEvents_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents] CHECK CONSTRAINT [FK_Asset_Staging_CryptoNodeEvents_AssetID]
GO



ALTER TABLE [dbo].[Staging_CryptoNodeEvents] CHECK CONSTRAINT [FK_Asset_Staging_CryptoNodeEvents_SourceID]
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents]  WITH CHECK ADD  CONSTRAINT [FK_EventInformation_Staging_CryptoNodeEvents_EventId] FOREIGN KEY([EventTypeID])
REFERENCES [dbo].[Event] ([ID])
GO

ALTER TABLE [dbo].[Staging_CryptoNodeEvents] CHECK CONSTRAINT [FK_EventInformation_Staging_CryptoNodeEvents_EventId]
GO


