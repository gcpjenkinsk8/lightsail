USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[AssetTheme]    Script Date: 9/23/2021 2:59:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AssetTheme](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] [int] NOT NULL,
	[ThemeID] [int] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AssetTheme_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],

) ON [PRIMARY]
GO




ALTER TABLE [dbo].[AssetTheme] ADD  CONSTRAINT [DF_AssetTheme_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[AssetTheme] ADD  CONSTRAINT [DF_AssetTheme_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[AssetTheme] ADD  CONSTRAINT [DF_AssetTheme_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[AssetTheme] ADD  CONSTRAINT [DF_AssetTheme_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[AssetTheme]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetTheme_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[AssetTheme]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetTheme_ThemeID] FOREIGN KEY([ThemeID])
REFERENCES [dbo].[Theme] ([ID])
GO


ALTER TABLE [dbo].[AssetTheme] CHECK CONSTRAINT [FK_Asset_AssetTheme_AssetID]
GO
ALTER TABLE [dbo].[AssetTheme] CHECK CONSTRAINT [FK_Asset_AssetTheme_ThemeID]
GO

