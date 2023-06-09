USE [master]
GO
/****** Object:  Database [JiraDemo]    Script Date: 7/1/2023 6:41:04 PM ******/
CREATE DATABASE [JiraDemo]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'JiraDemo', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\JiraDemo.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'JiraDemo_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\JiraDemo_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [JiraDemo] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JiraDemo].[dbo].[sp_fulltext_database] @action = 'enable'
end
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
ALTER DATABASE [JiraDemo] SET QUERY_STORE = ON
GO
ALTER DATABASE [JiraDemo] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [JiraDemo]
GO
/****** Object:  Table [dbo].[account_roles]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[account_roles](
	[account_id] [varchar](50) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[token_id] [int] NULL,
	[isDelete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_account_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[atlassian_token]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[atlassian_token](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account_installed_id] [varchar](50) NULL,
	[cloud_id] [varchar](50) NULL,
	[site] [varchar](500) NULL,
	[access_token] [varchar](5000) NULL,
	[refress_token] [varchar](5000) NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_access_token] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[equipments]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[equipments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cloud_id] [varchar](50) NULL,
	[name] [nvarchar](100) NOT NULL,
	[quantity] [int] NULL,
	[unit] [varchar](50) NULL,
	[unit_price] [float] NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_equipments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[equipments_function]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[equipments_function](
	[equipment_id] [int] NOT NULL,
	[function_id] [int] NOT NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
	[cloud_id] [varchar](50) NULL,
 CONSTRAINT [PK_equipments_function_1] PRIMARY KEY CLUSTERED 
(
	[equipment_id] ASC,
	[function_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[function]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[function](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NULL,
	[cloud_id] [varchar](100) NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_function] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[milestones]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[milestones](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[project_id] [int] NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_labels] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[projects]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[projects](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[image_avatar] [varchar](100) NULL,
	[name] [nvarchar](100) NULL,
	[account_id] [varchar](50) NULL,
	[start_date] [datetime] NULL,
	[budget] [float] NULL,
	[budget_unit] [nvarchar](50) NULL,
	[deadline] [datetime] NULL,
	[objective_time] [float] NULL,
	[objective_cost] [float] NULL,
	[objective_quality] [float] NULL,
	[cloud_id] [varchar](50) NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_projects] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cloud_id] [varchar](50) NULL,
	[name] [nvarchar](50) NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_roles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[schedules]    Script Date: 7/1/2023 6:41:05 PM ******/
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
	[since] [datetime] NULL,
	[account_id] [varchar](50) NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_schedules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[skills]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[skills](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[cloud_id] [varchar](50) NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_skills] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_function]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_function](
	[task_id] [int] NOT NULL,
	[function_id] [int] NOT NULL,
	[require_time] [int] NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_task_function] PRIMARY KEY CLUSTERED 
(
	[task_id] ASC,
	[function_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_precedences]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_precedences](
	[task_id] [int] NOT NULL,
	[precedence_id] [int] NOT NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_task_precedences] PRIMARY KEY CLUSTERED 
(
	[task_id] ASC,
	[precedence_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[task_resource]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[task_resource](
	[task_id] [int] NOT NULL,
	[resource_id] [int] NOT NULL,
	[type] [varchar](50) NOT NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_task_resource] PRIMARY KEY CLUSTERED 
(
	[task_id] ASC,
	[resource_id] ASC,
	[type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tasks]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tasks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NULL,
	[duration] [float] NULL,
	[cloud_id] [varchar](50) NULL,
	[project_id] [int] NULL,
	[milestone_id] [int] NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_tasks] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tasks_skills_required]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tasks_skills_required](
	[task_id] [int] NOT NULL,
	[skill_id] [int] NOT NULL,
	[level] [int] NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_tasks_skills_required] PRIMARY KEY CLUSTERED 
(
	[task_id] ASC,
	[skill_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[workforce]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[workforce](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [varchar](100) NULL,
	[email] [nvarchar](100) NULL,
	[account_type] [varchar](50) NULL,
	[name] [nvarchar](100) NULL,
	[avatar] [varchar](1000) NULL,
	[display_name] [nvarchar](500) NULL,
	[active] [int] NULL,
	[cloud_id] [varchar](50) NULL,
	[unit_salary] [float] NULL,
	[working_effort] [int] NULL,
	[is_delete] [bit] NULL,
	[create_datetime] [datetime] NULL,
	[delete_datetime] [datetime] NULL,
 CONSTRAINT [PK_workforce] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[workforce_skills]    Script Date: 7/1/2023 6:41:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[workforce_skills](
	[workforce_id] [int] NOT NULL,
	[skill_id] [int] NOT NULL,
	[level] [int] NULL,
	[create_datetime] [datetime] NULL,
	[is_delete] [bit] NULL,
	[delete_datetime] [datetime] NULL,
	[cloud_id] [varchar](50) NULL,
 CONSTRAINT [PK_workforce_skills] PRIMARY KEY CLUSTERED 
(
	[workforce_id] ASC,
	[skill_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[account_roles]  WITH CHECK ADD  CONSTRAINT [FK_account_roles_atlassian_token] FOREIGN KEY([token_id])
REFERENCES [dbo].[atlassian_token] ([id])
GO
ALTER TABLE [dbo].[account_roles] CHECK CONSTRAINT [FK_account_roles_atlassian_token]
GO
ALTER TABLE [dbo].[equipments_function]  WITH CHECK ADD  CONSTRAINT [FK_equipments_function_equipments] FOREIGN KEY([equipment_id])
REFERENCES [dbo].[equipments] ([id])
GO
ALTER TABLE [dbo].[equipments_function] CHECK CONSTRAINT [FK_equipments_function_equipments]
GO
ALTER TABLE [dbo].[equipments_function]  WITH CHECK ADD  CONSTRAINT [FK_equipments_function_function] FOREIGN KEY([function_id])
REFERENCES [dbo].[function] ([id])
GO
ALTER TABLE [dbo].[equipments_function] CHECK CONSTRAINT [FK_equipments_function_function]
GO
ALTER TABLE [dbo].[milestones]  WITH CHECK ADD  CONSTRAINT [FK_milestones_projects] FOREIGN KEY([project_id])
REFERENCES [dbo].[projects] ([id])
GO
ALTER TABLE [dbo].[milestones] CHECK CONSTRAINT [FK_milestones_projects]
GO
ALTER TABLE [dbo].[schedules]  WITH CHECK ADD  CONSTRAINT [FK_schedules_projects] FOREIGN KEY([project_id])
REFERENCES [dbo].[projects] ([id])
GO
ALTER TABLE [dbo].[schedules] CHECK CONSTRAINT [FK_schedules_projects]
GO
ALTER TABLE [dbo].[task_function]  WITH CHECK ADD  CONSTRAINT [FK_task_function_function] FOREIGN KEY([function_id])
REFERENCES [dbo].[function] ([id])
GO
ALTER TABLE [dbo].[task_function] CHECK CONSTRAINT [FK_task_function_function]
GO
ALTER TABLE [dbo].[task_function]  WITH CHECK ADD  CONSTRAINT [FK_task_function_tasks] FOREIGN KEY([task_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_function] CHECK CONSTRAINT [FK_task_function_tasks]
GO
ALTER TABLE [dbo].[task_precedences]  WITH CHECK ADD  CONSTRAINT [FK_task_precedences_tasks2] FOREIGN KEY([task_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_precedences] CHECK CONSTRAINT [FK_task_precedences_tasks2]
GO
ALTER TABLE [dbo].[task_precedences]  WITH CHECK ADD  CONSTRAINT [FK_task_precedences_tasks3] FOREIGN KEY([precedence_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[task_precedences] CHECK CONSTRAINT [FK_task_precedences_tasks3]
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
ALTER TABLE [dbo].[tasks]  WITH CHECK ADD  CONSTRAINT [FK_tasks_milestones] FOREIGN KEY([milestone_id])
REFERENCES [dbo].[milestones] ([id])
GO
ALTER TABLE [dbo].[tasks] CHECK CONSTRAINT [FK_tasks_milestones]
GO
ALTER TABLE [dbo].[tasks]  WITH CHECK ADD  CONSTRAINT [FK_tasks_projects] FOREIGN KEY([project_id])
REFERENCES [dbo].[projects] ([id])
GO
ALTER TABLE [dbo].[tasks] CHECK CONSTRAINT [FK_tasks_projects]
GO
ALTER TABLE [dbo].[tasks_skills_required]  WITH CHECK ADD  CONSTRAINT [FK_tasks_skills_skills] FOREIGN KEY([skill_id])
REFERENCES [dbo].[skills] ([id])
GO
ALTER TABLE [dbo].[tasks_skills_required] CHECK CONSTRAINT [FK_tasks_skills_skills]
GO
ALTER TABLE [dbo].[tasks_skills_required]  WITH CHECK ADD  CONSTRAINT [FK_tasks_skills_tasks] FOREIGN KEY([task_id])
REFERENCES [dbo].[tasks] ([id])
GO
ALTER TABLE [dbo].[tasks_skills_required] CHECK CONSTRAINT [FK_tasks_skills_tasks]
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
