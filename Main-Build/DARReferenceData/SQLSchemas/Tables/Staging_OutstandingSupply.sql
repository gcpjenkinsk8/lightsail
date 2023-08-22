USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Staging_OutstandingSupply]    Script Date: 9/22/2021 4:42:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Staging_OutstandingSupply](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceId] int NOT NULL,
	[AssetID] int NOT NULL,
	[ProcessID] int NOT NULL,
	[OutstandingSupply] decimal NOT NULL,
	[Error] nvarchar(500) NULL, 
	[CollectedTimeStamp] datetime  NULL
 CONSTRAINT [PK_Staging_OutstandingSupply_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],

) ON [PRIMARY]
GO


ALTER TABLE [dbo].[Staging_OutstandingSupply]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Staging_OutstandingSupply_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO


ALTER TABLE [dbo].[Staging_OutstandingSupply]  WITH CHECK ADD  CONSTRAINT [FK_Source_Staging_OutstandingSupply_SourceId] FOREIGN KEY([SourceId])
REFERENCES [dbo].[Source] ([ID])
GO

ALTER TABLE [dbo].[Staging_OutstandingSupply] ADD  CONSTRAINT [DF_Staging_OutstandingSupply_CollectedTimeStamp]  DEFAULT (getutcdate()) FOR [CollectedTimeStamp]
GO