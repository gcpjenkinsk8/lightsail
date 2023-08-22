USE [ReferenceCore-Dev]
GO

/****** Object:  Table [dbo].[AssetToken]    Script Date: 10/22/2021 2:21:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AssetToken](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] [int] NOT NULL,
	[TokenId] [int] NOT NULL,
	[BlockChainId] [int] NOT NULL,
	[TokenContractAddress] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AssetToken_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssetToken] ADD  CONSTRAINT [DF_AssetToken_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[AssetToken] ADD  CONSTRAINT [DF_AssetToken_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[AssetToken] ADD  CONSTRAINT [DF_AssetToken_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[AssetToken] ADD  CONSTRAINT [DF_AssetToken_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[AssetToken] ADD  CONSTRAINT [DF_AssetToken_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[AssetToken]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AssetToken_AssetID] FOREIGN KEY([AssetID])
REFERENCES [dbo].[Asset] ([ID])
GO

ALTER TABLE [dbo].[AssetToken]  WITH CHECK ADD  CONSTRAINT [FK_Token_AssetToken_TokenId] FOREIGN KEY([TokenId])
REFERENCES [dbo].[Token] ([ID])
GO

ALTER TABLE [dbo].[AssetToken]  WITH CHECK ADD  CONSTRAINT [FK_BlockChain_AssetToken_BlockChainId] FOREIGN KEY([BlockChainId])
REFERENCES [dbo].[BlockChain] ([ID])
GO


ALTER TABLE [dbo].[AssetToken] ADD  CONSTRAINT [UQ_AssetToken_AID_TID_BID]  UNIQUE NONCLUSTERED ([AssetID],[TokenId],[BlockChainId])
GO
