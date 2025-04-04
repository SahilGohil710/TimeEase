USE [AmeeShree_Test]
GO
/****** Object:  Table [dbo].[M_State]    Script Date: 12/9/2024 12:04:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_State](
	[StateID] [int] IDENTITY(1,1) NOT NULL,
	[StateName] [nvarchar](100) NOT NULL,
	[IsActiveYN] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[StateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[M_State] ON 

INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (1, N'Andhra Pradesh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (2, N'Arunachal Pradesh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (3, N'Assam', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (4, N'Bihar', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (5, N'Chhattisgarh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (6, N'Goa', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (7, N'Gujarat', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (8, N'Haryana', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (9, N'Himachal Pradesh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (10, N'Jharkhand', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (11, N'Karnataka', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (12, N'Kerala', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (13, N'Madhya Pradesh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (14, N'Maharashtra', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (15, N'Manipur', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (16, N'Meghalaya', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (17, N'Mizoram', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (18, N'Nagaland', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (19, N'Odisha', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (20, N'Punjab', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (21, N'Rajasthan', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (22, N'Sikkim', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (23, N'Tamil Nadu', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (24, N'Telangana', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (25, N'Tripura', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (26, N'Uttar Pradesh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (27, N'Uttarakhand', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (28, N'West Bengal', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (29, N'Andaman and Nicobar Islands', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (30, N'Chandigarh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (31, N'Dadra and Nagar Haveli and Daman and Diu', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (32, N'Lakshadweep', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (33, N'Delhi', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (34, N'Puducherry', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (35, N'Ladakh', N'Y')
INSERT [dbo].[M_State] ([StateID], [StateName], [IsActiveYN]) VALUES (36, N'Jammu and Kashmir', N'Y')
SET IDENTITY_INSERT [dbo].[M_State] OFF
GO
ALTER TABLE [dbo].[M_State] ADD  DEFAULT ('Y') FOR [IsActiveYN]
GO
