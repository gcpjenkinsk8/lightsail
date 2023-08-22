
USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[ServingList]    Script Date: 9/23/2021 11:15:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ServingList](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] int NULL,
	[PairID] int NULL,
	[SourceID] int NULL,
	[ProcessID] int NOT NULL,
	[Start] [datetime] NOT NULL,
	[End] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ServingList_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[ServingList] ADD  CONSTRAINT [DF_ServingList_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[ServingList]  WITH CHECK ADD  CONSTRAINT [FK_Asset_ServingList_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[ServingList]  WITH CHECK ADD  CONSTRAINT [FK_Pair_ServingList_PairID] FOREIGN KEY([PairID])
REFERENCES [dbo].[Pair] ([ID])
GO
