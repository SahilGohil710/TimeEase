USE [AmeeShree_Test]
GO
/****** Object:  Table [dbo].[M_BusinessType]    Script Date: 12/9/2024 12:07:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_BusinessType](
	[BusinessTypeID] [int] IDENTITY(1,1) NOT NULL,
	[BusinessTypeName] [nvarchar](100) NOT NULL,
	[IsActiveYN] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[BusinessTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[M_BusinessType] ON 

INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (1, N'E-Tailer(B2B)', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (2, N'E-Tailer(B2C)', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (3, N'Investor', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (4, N'Jewellery Manufacturer', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (5, N'Jewellery Retailer', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (6, N'Other', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (7, N'Personal', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (8, N'Retailer', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (9, N'Retailer(100+)', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (10, N'Retailer(10-100)', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (11, N'Retailer(1-10)', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (12, N'Watch', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (13, N'Wholesaler', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (14, N'Diamond Manufacturer', N'Y')
INSERT [dbo].[M_BusinessType] ([BusinessTypeID], [BusinessTypeName], [IsActiveYN]) VALUES (15, N'Diamond Trader', N'Y')
SET IDENTITY_INSERT [dbo].[M_BusinessType] OFF
GO
ALTER TABLE [dbo].[M_BusinessType] ADD  DEFAULT ('Y') FOR [IsActiveYN]
GO
