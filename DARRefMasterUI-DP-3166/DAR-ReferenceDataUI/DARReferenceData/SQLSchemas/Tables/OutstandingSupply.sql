USE [ReferenceCore]
GO

/****** Object:  Table [dbo].[OutstandingSupply]    Script Date: 9/22/2021 4:42:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OutstandingSupply](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AssetID] int NOT NULL,
	[ProcessID] int NOT NULL,
	[OutstandingSupply] decimal NOT NULL,
	[CollectedTimeStamp] datetime  NULL
 CONSTRAINT [PK_OutstandingSupply_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],

) ON [PRIMARY]
GO
