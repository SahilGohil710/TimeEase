USE [AmeeShree_Test]
GO
/****** Object:  Table [dbo].[M_BankAcc]    Script Date: 12/9/2024 12:09:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_BankAcc](
	[AutoCode] [int] IDENTITY(1,1) NOT NULL,
	[AccType] [varchar](50) NULL,
	[ActiveYN] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[AutoCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[M_BankAcc] ON 

INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (1, N'Savings Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (2, N'Current Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (3, N'Recurring Deposit Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (4, N'Fixed Deposit Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (5, N'Demat Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (6, N'NRI Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (7, N'Salary Account', N'Y')
INSERT [dbo].[M_BankAcc] ([AutoCode], [AccType], [ActiveYN]) VALUES (8, N'BSBDA', N'Y')
SET IDENTITY_INSERT [dbo].[M_BankAcc] OFF
GO
ALTER TABLE [dbo].[M_BankAcc] ADD  DEFAULT ('Y') FOR [ActiveYN]
GO
