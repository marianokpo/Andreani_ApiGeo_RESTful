CREATE DATABASE API_GEO_DB;
GO
USE API_GEO_DB;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocalizadorData](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[latitud] [float] NULL,
	[longitud] [float] NULL,
	[estado] [varchar](50) NULL,
	[calle] [varchar](100) NOT NULL,
	[numero] [int] NOT NULL,
	[ciudad] [varchar](100) NOT NULL,
	[codigo_postal] [varchar](20) NOT NULL,
	[provincia] [varchar](100) NOT NULL,
	[pais] [varchar](100) NOT NULL,
 CONSTRAINT [PK_LocalizadorData] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO