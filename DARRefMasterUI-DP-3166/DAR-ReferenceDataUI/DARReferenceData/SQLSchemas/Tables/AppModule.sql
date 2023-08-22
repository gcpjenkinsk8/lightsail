USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[AppModule]    Script Date: 11/4/2021 10:10:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AppModule](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ModuleName] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[Link] [nvarchar](250) NULL,
	[IsActive] [bit] NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AppModule_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AppModule_Name] UNIQUE NONCLUSTERED 
(
	[ModuleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AppModule] ADD  CONSTRAINT [DF_AppModule_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[AppModule] ADD  CONSTRAINT [DF_AppModule_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[AppModule] ADD  CONSTRAINT [DF_AppModule_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[AppModule] ADD  CONSTRAINT [DF_AppModule_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[AppModule] ADD  CONSTRAINT [DF_AppModule_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[AppModule] ADD CONSTRAINT UC_ModuleName_AppModule UNIQUE (ModuleName)
GO

ALTER TABLE [dbo].[AppModule] ADD CONSTRAINT UC_Link_AppModule UNIQUE (Link)
GO
