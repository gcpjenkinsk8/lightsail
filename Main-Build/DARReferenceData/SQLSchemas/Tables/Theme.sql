USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[Theme]    Script Date: 9/23/2021 2:51:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Theme](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Theme_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Theme_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],


) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[Theme] ADD  CONSTRAINT [DF_Theme_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO


