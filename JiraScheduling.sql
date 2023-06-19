USE [master]
GO
/****** Object:  Database [JiraDemo]    Script Date: 6/19/2023 22:28:56 ******/
CREATE DATABASE [JiraDemo]
GO
ALTER DATABASE [JiraDemo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [JiraDemo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [JiraDemo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [JiraDemo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [JiraDemo] SET ARITHABORT OFF 
GO
ALTER DATABASE [JiraDemo] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [JiraDemo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [JiraDemo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [JiraDemo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [JiraDemo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [JiraDemo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [JiraDemo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [JiraDemo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [JiraDemo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [JiraDemo] SET  DISABLE_BROKER 
GO
ALTER DATABASE [JiraDemo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [JiraDemo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [JiraDemo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [JiraDemo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [JiraDemo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [JiraDemo] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [JiraDemo] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [JiraDemo] SET RECOVERY FULL 
GO
ALTER DATABASE [JiraDemo] SET  MULTI_USER 
GO
ALTER DATABASE [JiraDemo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [JiraDemo] SET DB_CHAINING OFF 
GO
ALTER DATABASE [JiraDemo] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [JiraDemo] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [JiraDemo] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [JiraDemo] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'JiraDemo', N'ON'
GO
ALTER DATABASE [JiraDemo] SET QUERY_STORE = ON
GO
ALTER DATABASE [JiraDemo] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [JiraDemo]
GO
/****** Object:  Table [dbo].[account_roles]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[account_roles](
	[role_id] [int] NULL,
	[account_id] [varchar](50) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_account_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[atlassian_token]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[atlassian_token](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [varchar](50) NULL,
	[cloud_id] [varchar](50) NULL,
	[access_token] [varchar](5000) NULL,
	[refress_token] [varchar](5000) NULL,
 CONSTRAINT [PK_access_token] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[equipments]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[equipments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[quantity] [int] NULL,
	[unit] [varchar](50) NULL,
	[unit_price] [float] NULL,
 CONSTRAINT [PK_equipments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[labels]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[labels](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NULL,
	[cloud_id] [int] NULL,
 CONSTRAINT [PK_labels] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[projects]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[projects](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[image_avatar] [varchar](100) NULL,
	[name] [varchar](100) NULL,
	[account_id] [varchar](50) NULL,
	[start_date] [int] NULL,
	[budget] [float] NULL,
	[budget_unit] [varchar](5) NULL,
	[deadline] [int] NULL,
	[objective_time] [float] NULL,
	[objective_cost] [float] NULL,
	[objective_quality] [float] NULL,
	[cloud_id] [varchar](50) NULL,
 CONSTRAINT [PK_projects] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cloud_id] [varchar](50) NULL,
	[name] [varchar](50) NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[schedules]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[schedules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[project_id] [int] NULL,
	[duration] [float] NULL,
	[cost] [float] NULL,
	[quality] [float] NULL,
	[tasks] [text] NULL,
	[cloud_id] [varchar](50) NULL,
	[selected] [int] NULL,
	[since] [int] NULL,
	[account_id] [varchar](50) NULL,
 CONSTRAINT [PK_schedules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[skills]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[skills](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL,
	[cloud_id] [varchar](50) NULL,
 CONSTRAINT [PK_skills] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_labels]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_labels](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[label_id] [int] NULL,
	[cloud_id] [int] NULL,
 CONSTRAINT [PK_task_labels] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_precedence]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_precedence](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[precedence_task_id] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_precedences]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_precedences](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[precedence_id] [int] NULL,
 CONSTRAINT [PK_task_precedences] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_resource]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_resource](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[resource_id] [int] NULL,
	[type] [varchar](50) NULL,
 CONSTRAINT [PK_task_resource] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tasks]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tasks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NULL,
	[duration] [float] NULL,
	[cloud_id] [varchar](50) NULL,
	[project_id] [int] NULL,
 CONSTRAINT [PK_tasks] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[workforce]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[workforce](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [varchar](100) NULL,
	[email] [varchar](100) NULL,
	[account_type] [varchar](50) NULL,
	[name] [varchar](100) NULL,
	[avatar] [varchar](1000) NULL,
	[display_name] [varchar](500) NULL,
	[active] [int] NULL,
	[cloud_id] [varchar](50) NULL,
	[unit_salary] [float] NULL,
	[working_type] [int] NULL,
 CONSTRAINT [PK_workforce] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[workforce_skills]    Script Date: 6/19/2023 22:28:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[workforce_skills](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[workforce_id] [int] NULL,
	[skill_id] [int] NULL,
	[level] [int] NULL,
	[cloud_id] [varchar](50) NULL,
 CONSTRAINT [PK_workforce_skills] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[atlassian_token] ON 

INSERT [dbo].[atlassian_token] ([id], [account_id], [cloud_id], [access_token], [refress_token]) VALUES (4, N'61e1b72f0586a20069de28fe', N'ea48ddc7-ed56-4d60-9b55-02667724849d', N'eyJraWQiOiJmZTM2ZThkMzZjMTA2N2RjYTgyNTg5MmEiLCJhbGciOiJSUzI1NiJ9.eyJqdGkiOiJkZDU1Mjc4Yi0xOGRiLTRmNjQtYTZkZC04OTNmNjBhZGFiZmIiLCJzdWIiOiI2MWUxYjcyZjA1ODZhMjAwNjlkZTI4ZmUiLCJuYmYiOjE2ODY4NTE4OTAsImlzcyI6Imh0dHBzOi8vYXV0aC5hdGxhc3NpYW4uY29tIiwiaWF0IjoxNjg2ODUxODkwLCJleHAiOjE2ODY4NTU0OTAsImF1ZCI6IndEenp4QVpTcnJNOUR0UHdaMjk1Qk1UM1lvRlI2S2VEIiwiaHR0cHM6Ly9pZC5hdGxhc3NpYW4uY29tL3Nlc3Npb25faWQiOiJhNmZhMDYxNy1mMjY1LTQyNTYtODQ0MS1kNzYwYzAxOWFmNGEiLCJodHRwczovL2lkLmF0bGFzc2lhbi5jb20vcmVmcmVzaF9jaGFpbl9pZCI6IndEenp4QVpTcnJNOUR0UHdaMjk1Qk1UM1lvRlI2S2VELTYxZTFiNzJmMDU4NmEyMDA2OWRlMjhmZS1iNmRiMDk3MC01ZWU4LTQyNmItYWM3Mi1hODA0MWViMGFmMWQiLCJodHRwczovL2lkLmF0bGFzc2lhbi5jb20vdWp0IjoiNTdkYWQxNzctZDIyNS00ZDE1LWJiYjMtMGFhN2IxZGNhNDU1IiwiaHR0cHM6Ly9pZC5hdGxhc3NpYW4uY29tL2F0bF90b2tlbl90eXBlIjoiQUNDRVNTIiwiaHR0cHM6Ly9hdGxhc3NpYW4uY29tL2ZpcnN0UGFydHkiOmZhbHNlLCJodHRwczovL2F0bGFzc2lhbi5jb20vdmVyaWZpZWQiOnRydWUsImh0dHBzOi8vYXRsYXNzaWFuLmNvbS9vYXV0aENsaWVudElkIjoid0R6enhBWlNyck05RHRQd1oyOTVCTVQzWW9GUjZLZUQiLCJ2ZXJpZmllZCI6InRydWUiLCJodHRwczovL2lkLmF0bGFzc2lhbi5jb20vcHJvY2Vzc1JlZ2lvbiI6InVzLWVhc3QtMSIsImh0dHBzOi8vYXRsYXNzaWFuLmNvbS9zeXN0ZW1BY2NvdW50RW1haWwiOiIyNWUyOTQyYS1hM2MyLTQ4ZTgtYTkzNC01ZjI5MTgyYWIxMmVAY29ubmVjdC5hdGxhc3NpYW4uY29tIiwiaHR0cHM6Ly9hdGxhc3NpYW4uY29tL3N5c3RlbUFjY291bnRJZCI6IjcxMjAyMDozZjgwN2YyYS0yY2NkLTQ1NzAtOTRjMS0zOWVhZDAyZDhlNzEiLCJodHRwczovL2F0bGFzc2lhbi5jb20vZW1haWxEb21haW4iOiJnbWFpbC5jb20iLCJodHRwczovL2F0bGFzc2lhbi5jb20vM2xvIjp0cnVlLCJzY29wZSI6Im1hbmFnZTpqaXJhLXByb2plY3Qgb2ZmbGluZV9hY2Nlc3MgcmVhZDpqaXJhLXdvcmsgd3JpdGU6amlyYS13b3JrIiwiY2xpZW50X2lkIjoid0R6enhBWlNyck05RHRQd1oyOTVCTVQzWW9GUjZLZUQiLCJodHRwczovL2lkLmF0bGFzc2lhbi5jb20vdmVyaWZpZWQiOnRydWUsImh0dHBzOi8vYXRsYXNzaWFuLmNvbS9zeXN0ZW1BY2NvdW50RW1haWxEb21haW4iOiJjb25uZWN0LmF0bGFzc2lhbi5jb20ifQ.RMkK9IENc9ZmxcuRJZMNxY8NVZoj-kf8uCUioGqZLIaDLCIYuQ_Ye7V59taDOgKxC23VFk3H93wN05TrWaB4VixB41Wmr5BYHHPy89uBflLveuHJc_x1ZAwrmJBuDg6qBtCQ1hioSITE0hYI_rd8G_4z4oHV0KVqZeK2yogf5CJzLvT3_tMkfylab5ijB8myFUSh_3RjDgca469pK29-UR3deT_gKch4znyN6jhQv2ngFZkQhPRvxsGiwhe_b-SxwPiI-sSkVledT1nXP7h35Ejat_O0T8t3nWFgdVTkRmcPz-AIKE4F7ncs-qSBY1HVfzXN7sH3TYYmrle-tW3paQ', N'eyJraWQiOiI1MWE2YjE2MjRlMTQ5ZDFiYTdhM2VmZjciLCJhbGciOiJSUzI1NiJ9.eyJqdGkiOiIzMTFkNjViNS1lZjJkLTQ0YWEtYWUwNy1jN2E0NTc4YjcxN2IiLCJzdWIiOiI2MWUxYjcyZjA1ODZhMjAwNjlkZTI4ZmUiLCJuYmYiOjE2ODY4NTE4OTAsImlzcyI6Imh0dHBzOi8vYXV0aC5hdGxhc3NpYW4uY29tIiwiaWF0IjoxNjg2ODUxODkwLCJleHAiOjE2OTQ2Mjc4OTAsImF1ZCI6IndEenp4QVpTcnJNOUR0UHdaMjk1Qk1UM1lvRlI2S2VEIiwiaHR0cHM6Ly9pZC5hdGxhc3NpYW4uY29tL2F0bF90b2tlbl90eXBlIjoiUk9UQVRJTkdfUkVGUkVTSCIsInZlcmlmaWVkIjoidHJ1ZSIsImh0dHBzOi8vaWQuYXRsYXNzaWFuLmNvbS9zZXNzaW9uX2lkIjoiYTZmYTA2MTctZjI2NS00MjU2LTg0NDEtZDc2MGMwMTlhZjRhIiwiaHR0cHM6Ly9pZC5hdGxhc3NpYW4uY29tL3Byb2Nlc3NSZWdpb24iOiJ1cy1lYXN0LTEiLCJodHRwczovL2lkLmF0bGFzc2lhbi5jb20vcmVmcmVzaF9jaGFpbl9pZCI6IndEenp4QVpTcnJNOUR0UHdaMjk1Qk1UM1lvRlI2S2VELTYxZTFiNzJmMDU4NmEyMDA2OWRlMjhmZS1iNmRiMDk3MC01ZWU4LTQyNmItYWM3Mi1hODA0MWViMGFmMWQiLCJodHRwczovL2lkLmF0bGFzc2lhbi5jb20vcGFyZW50X2FjY2Vzc190b2tlbl9pZCI6ImRkNTUyNzhiLTE4ZGItNGY2NC1hNmRkLTg5M2Y2MGFkYWJmYiIsImh0dHBzOi8vaWQuYXRsYXNzaWFuLmNvbS91anQiOiI1N2RhZDE3Ny1kMjI1LTRkMTUtYmJiMy0wYWE3YjFkY2E0NTUiLCJzY29wZSI6Im1hbmFnZTpqaXJhLXByb2plY3Qgb2ZmbGluZV9hY2Nlc3MgcmVhZDpqaXJhLXdvcmsgd3JpdGU6amlyYS13b3JrIiwiaHR0cHM6Ly9pZC5hdGxhc3NpYW4uY29tL3ZlcmlmaWVkIjp0cnVlfQ.oyPCWqczQazFRYOTBbBvDkFVZODACHCrIvA8M1X4ZXQX_5Hyp7QI5cnNqjRLFe47JlWBMUyG5mWPxMAt4M1k9HrVuVVuLVNbODgeAu2c2G3jSRKP0f-Uqpz3_R5ULNflmzhEhodKO5NNWrAYu-o71ekJkxmmtvOCpRWyFC3AIHzzsFBwq4RXFgslDGn1CJ8JzmKKXIHZrKnHV1kv93qoC4N-v3680i1ZYG7r6bJ1wZ3DSd30v9CZ6s-HYBdbw-XcTNX_xwQwQw5XXTnZxDAWhcFrOWcOgxZluYurkmPatSRQTDFE_KM5fMvhQJ2u5rvjD6jK3mStecNekgaTLcWNUA')
SET IDENTITY_INSERT [dbo].[atlassian_token] OFF
GO
SET IDENTITY_INSERT [dbo].[projects] ON 

INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (1, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (2, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (3, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (4, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (5, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (6, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (7, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (8, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (9, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (10, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (11, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (12, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (13, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (14, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (15, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (16, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (17, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
INSERT [dbo].[projects] ([id], [image_avatar], [name], [account_id], [start_date], [budget], [budget_unit], [deadline], [objective_time], [objective_cost], [objective_quality], [cloud_id]) VALUES (18, NULL, N'Project 1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'ea48ddc7-ed56-4d60-9b55-02667724849d')
SET IDENTITY_INSERT [dbo].[projects] OFF
GO
ALTER TABLE [dbo].[account_roles]  WITH CHECK ADD  CONSTRAINT [FK_account_roles_roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[roles] ([id])
GO
ALTER TABLE [dbo].[account_roles] CHECK CONSTRAINT [FK_account_roles_roles]
GO
ALTER TABLE [dbo].[schedules]  WITH CHECK ADD  CONSTRAINT [FK_schedules_projects] FOREIGN KEY([project_id])
REFERENCES [dbo].[projects] ([id])
GO
ALTER TABLE [dbo].[schedules] CHECK CONSTRAINT [FK_schedules_projects]
GO
ALTER TABLE [dbo].[task_labels]  WITH CHECK ADD  CONSTRAINT [FK_task_labels_labels] FOREIGN KEY([label_id])
REFERENCES [dbo].[labels] ([id])
GO
ALTER TABLE [dbo].[task_labels] CHECK CONSTRAINT [FK_task_labels_labels]
GO
ALTER TABLE [dbo].[task_labels]  WITH CHECK ADD  CONSTRAINT [FK_task_labels_tasks] FOREIGN KEY([task_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_labels] CHECK CONSTRAINT [FK_task_labels_tasks]
GO
ALTER TABLE [dbo].[task_precedences]  WITH CHECK ADD  CONSTRAINT [FK_task_precedences_tasks] FOREIGN KEY([task_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_precedences] CHECK CONSTRAINT [FK_task_precedences_tasks]
GO
ALTER TABLE [dbo].[task_precedences]  WITH CHECK ADD  CONSTRAINT [FK_task_precedences_tasks1] FOREIGN KEY([precedence_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_precedences] CHECK CONSTRAINT [FK_task_precedences_tasks1]
GO
ALTER TABLE [dbo].[task_resource]  WITH CHECK ADD  CONSTRAINT [FK_task_resource_equipments] FOREIGN KEY([resource_id])
REFERENCES [dbo].[equipments] ([id])
GO
ALTER TABLE [dbo].[task_resource] CHECK CONSTRAINT [FK_task_resource_equipments]
GO
ALTER TABLE [dbo].[task_resource]  WITH CHECK ADD  CONSTRAINT [FK_task_resource_tasks] FOREIGN KEY([task_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_resource] CHECK CONSTRAINT [FK_task_resource_tasks]
GO
ALTER TABLE [dbo].[task_resource]  WITH CHECK ADD  CONSTRAINT [FK_task_resource_workforce] FOREIGN KEY([resource_id])
REFERENCES [dbo].[workforce] ([id])
GO
ALTER TABLE [dbo].[task_resource] CHECK CONSTRAINT [FK_task_resource_workforce]
GO
ALTER TABLE [dbo].[tasks]  WITH CHECK ADD  CONSTRAINT [FK_tasks_projects] FOREIGN KEY([project_id])
REFERENCES [dbo].[projects] ([id])
GO
ALTER TABLE [dbo].[tasks] CHECK CONSTRAINT [FK_tasks_projects]
GO
ALTER TABLE [dbo].[workforce_skills]  WITH CHECK ADD  CONSTRAINT [FK_workforce_skills_skills] FOREIGN KEY([skill_id])
REFERENCES [dbo].[skills] ([id])
GO
ALTER TABLE [dbo].[workforce_skills] CHECK CONSTRAINT [FK_workforce_skills_skills]
GO
ALTER TABLE [dbo].[workforce_skills]  WITH CHECK ADD  CONSTRAINT [FK_workforce_skills_workforce] FOREIGN KEY([workforce_id])
REFERENCES [dbo].[workforce] ([id])
GO
ALTER TABLE [dbo].[workforce_skills] CHECK CONSTRAINT [FK_workforce_skills_workforce]
GO
USE [master]
GO
ALTER DATABASE [JiraDemo] SET  READ_WRITE 
GO
