Use CuplexApi
CREATE TABLE [dbo].[GeoIPCity](
	[LocationId] [int] NOT NULL,
	[CountryCode] [char](2) NOT NULL,
	[RegionCode] [char](2) NOT NULL,
	[CityName] [varchar](255) NOT NULL,
	[PostalCode] [varchar](8) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[MetroCode] [bigint] NULL,
	[AreaCode] [char](3) NULL,
 CONSTRAINT [PK_GeoIPCity] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GeoIPCityBlock]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeoIPCityBlock](
	[LocationId] [int] NOT NULL,
	[IPFrom] [bigint] NOT NULL,
	[IPTo] [bigint] NOT NULL,
 CONSTRAINT [PK_GeoIPCityBlock] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC,
	[IPFrom] ASC,
	[IPTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GeoIPCountry]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GeoIPCountry](
	[IPFrom] [bigint] NOT NULL,
	[IPTo] [bigint] NOT NULL,
	[IPAddressFrom] [varchar](15) NOT NULL,
	[IPAddressTo] [varchar](15) NOT NULL,
	[CountryCode] [varchar](2) NOT NULL,
	[CountryName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_IpLookup] PRIMARY KEY CLUSTERED 
(
	[IPFrom] ASC,
	[IPTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Language]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Language](
	[id] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ISOCode] [char](2) NOT NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SecureChatMessage]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SecureChatMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Message] [varchar](8000) NOT NULL,
	[ReceiverUserId] [int] NOT NULL,
	[SenderUserId] [int] NOT NULL,
 CONSTRAINT [PK_SecureChatMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SecureChatSettings]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SecureChatSettings](
	[KeyType] [varchar](50) NOT NULL,
	[Value] [varchar](8000) NOT NULL,
	[Description] [varchar](150) NULL,
	[DataType] [int] NOT NULL,
 CONSTRAINT [PK_SecureChatSettings] PRIMARY KEY CLUSTERED 
(
	[KeyType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SecureChatUser]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SecureChatUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nickname] [varchar](255) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[GUID] [char](36) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_SecureChatUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WordDictionary]    Script Date: 2015-06-11 19:14:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WordDictionary](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Word] [varchar](100) NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[WordLength] [int] NOT NULL,
 CONSTRAINT [PK_WordDictionary] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[SecureChatSettings] ADD  CONSTRAINT [DF_SecureChatSettings_DataType]  DEFAULT ((0)) FOR [DataType]
GO
ALTER TABLE [dbo].[WordDictionary] ADD  CONSTRAINT [DF_WordDictionary_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[GeoIPCity]  WITH NOCHECK ADD  CONSTRAINT [FK_GeoIPCity_GeoIPBlock] FOREIGN KEY([LocationId])
REFERENCES [dbo].[GeoIPCity] ([LocationId])
GO
ALTER TABLE [dbo].[GeoIPCity] CHECK CONSTRAINT [FK_GeoIPCity_GeoIPBlock]
GO
ALTER TABLE [dbo].[GeoIPCityBlock]  WITH NOCHECK ADD  CONSTRAINT [FK_GeoIPCityBlock_GeoIPCity] FOREIGN KEY([LocationId])
REFERENCES [dbo].[GeoIPCity] ([LocationId])
GO
ALTER TABLE [dbo].[GeoIPCityBlock] CHECK CONSTRAINT [FK_GeoIPCityBlock_GeoIPCity]
GO
ALTER TABLE [dbo].[SecureChatMessage]  WITH CHECK ADD  CONSTRAINT [FK_SecureChatMessage_SecureChatUser_Receiver] FOREIGN KEY([ReceiverUserId])
REFERENCES [dbo].[SecureChatUser] ([Id])
GO
ALTER TABLE [dbo].[SecureChatMessage] CHECK CONSTRAINT [FK_SecureChatMessage_SecureChatUser_Receiver]
GO
ALTER TABLE [dbo].[SecureChatMessage]  WITH CHECK ADD  CONSTRAINT [FK_SecureChatMessage_SecureChatUser_Sender] FOREIGN KEY([SenderUserId])
REFERENCES [dbo].[SecureChatUser] ([Id])
GO
ALTER TABLE [dbo].[SecureChatMessage] CHECK CONSTRAINT [FK_SecureChatMessage_SecureChatUser_Sender]
GO
ALTER TABLE [dbo].[WordDictionary]  WITH CHECK ADD  CONSTRAINT [FK_WordDictionary_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([id])
GO
ALTER TABLE [dbo].[WordDictionary] CHECK CONSTRAINT [FK_WordDictionary_Language]
GO
USE [master]
GO
ALTER DATABASE [CuplexApi] SET  READ_WRITE 
GO
