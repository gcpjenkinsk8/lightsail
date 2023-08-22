USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[EventInformation]    Script Date: 9/21/2021 8:40:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EventInformation](
	[DAREventID] [int] IDENTITY(10000,1) NOT NULL,
	[DateofReview] [datetime] NULL,
	[EventTypeID] [int] NULL,
	[AssetID] [int] NULL,
	[SourceID] [int] NULL,
	[ExchangeAssetTicker] [nvarchar](20) NULL,
	[ExchangeAssetName] [nvarchar](100) NULL,
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
	[BlockHeight] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EventInformation_ID] PRIMARY KEY CLUSTERED 
(
	[DAREventID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EventInformation] ADD  CONSTRAINT [DF_EventInformation_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[EventInformation] ADD  CONSTRAINT [DF_EventInformation_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[EventInformation] ADD  CONSTRAINT [DF_EventInformation_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[EventInformation]  WITH CHECK ADD  CONSTRAINT [FK_Asset_EventInformation_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[EventInformation] CHECK CONSTRAINT [FK_Asset_EventInformation_AssetID]
GO

ALTER TABLE [dbo].[EventInformation]  WITH CHECK ADD  CONSTRAINT [FK_Exchange_EventInformation_EventTypeID] FOREIGN KEY([EventTypeID])
REFERENCES [dbo].[Event] ([ID])
GO

alter table dbo.EventInformation add [LastEditUser] nvarchar(100) null
GO
alter table dbo.EventInformation add [LastEditTime] datetime null
GO



