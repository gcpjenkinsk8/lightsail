USE [ReferenceCore-Dev]
GO

/****** Object:  Table [dbo].[BlockChain]    Script Date: 10/22/2021 2:21:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BlockChain](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[ConsensusMechanism] [nvarchar](255) NULL,
	[HashAlgorithm] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_BlockChain_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[BlockChain] ADD  CONSTRAINT [UQ_BlockChain_Name]  UNIQUE NONCLUSTERED (	[Name] ASC)
GO


ALTER TABLE [dbo].[BlockChain] ADD  CONSTRAINT [DF_BlockChain_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[BlockChain] ADD  CONSTRAINT [DF_BlockChain_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[BlockChain] ADD  CONSTRAINT [DF_BlockChain_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[BlockChain] ADD  CONSTRAINT [DF_BlockChain_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[BlockChain] ADD  CONSTRAINT [DF_BlockChain_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO


