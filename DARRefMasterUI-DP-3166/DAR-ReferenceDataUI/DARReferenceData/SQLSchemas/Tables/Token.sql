USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Token]    Script Date: 10/22/2021 2:21:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Token](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DARTokenID] [nvarchar](20) NOT NULL,
	[TokenName] [nvarchar](255) NOT NULL,
	[TokenDescription] [nvarchar](1500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Token_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [UQ_Token_DARTokenID]  UNIQUE NONCLUSTERED ([DARTokenID] ASC)
GO

ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [UQ_Token_TokenName]  UNIQUE NONCLUSTERED ([TokenName])
GO

