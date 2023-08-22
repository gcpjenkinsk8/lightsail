USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Pair]    Script Date: 10/5/2021 2:26:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Pair](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UniqueID] [uniqueidentifier] NOT NULL,
	[AssetID] [int] NOT NULL,
	[QuoteAssetID] [int] NOT NULL,
	[ExchangeID] [int] NULL,
	[DARName] [nvarchar](200) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Pair_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Pair_DARName] UNIQUE NONCLUSTERED 
(
	[DARName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Pair_UniqueID] UNIQUE NONCLUSTERED 
(
	[UniqueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Pair] ADD  CONSTRAINT [DF_Pair_UniqueID]  DEFAULT (newid()) FOR [UniqueID]
GO

ALTER TABLE [dbo].[Pair] ADD  CONSTRAINT [DF_Pair_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Pair] ADD  CONSTRAINT [DF_Pair_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Pair] ADD  CONSTRAINT [DF_Pair_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Pair] ADD  CONSTRAINT [DF_Pair_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Pair] ADD  CONSTRAINT [DF_Pair_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[Pair]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Pair_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[Pair] CHECK CONSTRAINT [FK_Asset_Pair_AssetID]
GO

ALTER TABLE [dbo].[Pair]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Pair_QuoteAssetID] FOREIGN KEY([QuoteAssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[Pair] CHECK CONSTRAINT [FK_Asset_Pair_QuoteAssetID]
GO


