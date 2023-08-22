USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[RoleAppModule]    Script Date: 11/4/2021 10:13:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleAppModule](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AppModuleId] [int] NOT NULL,
	[RoleId] [nvarchar] (128) NOT NULL,
	[CreateUser] [nvarchar](100) NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[CreateTime] [datetime] NOT NULL,
	[LastEditTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RoleAppModule_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RoleAppModule] ADD  CONSTRAINT [DF_RoleAppModule_CreateUser]  DEFAULT (suser_sname()) FOR [CreateUser]
GO

ALTER TABLE [dbo].[RoleAppModule] ADD  CONSTRAINT [DF_RoleAppModule_LastEditUser]  DEFAULT (suser_sname()) FOR [LastEditUser]
GO

ALTER TABLE [dbo].[RoleAppModule] ADD  CONSTRAINT [DF_RoleAppModule_CreateTime]  DEFAULT (getutcdate()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[RoleAppModule] ADD  CONSTRAINT [DF_RoleAppModule_LastEditTime]  DEFAULT (getutcdate()) FOR [LastEditTime]
GO

ALTER TABLE [dbo].[RoleAppModule]  WITH CHECK ADD  CONSTRAINT [FK_AppModule_RoleAppModule_AppModuleId] FOREIGN KEY([AppModuleId])
REFERENCES [dbo].[AppModule] ([ID])
GO

ALTER TABLE [dbo].[RoleAppModule]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoles_RoleAppModule_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([ID])
GO




ALTER TABLE [dbo].[RoleAppModule] ADD CONSTRAINT UC_RoleAppModule_AppModuleId_RoleId UNIQUE (AppModuleId,RoleId)
GO