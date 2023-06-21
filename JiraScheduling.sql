USE [master]
GO
/****** Object:  Database [JiraDemo]    Script Date: 6/21/2023 11:28:12 ******/
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
/****** Object:  Table [dbo].[account_roles]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[atlassian_token]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[equipments]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[labels]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[projects]    Script Date: 6/21/2023 11:28:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[projects](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[image_avatar] [varchar](100) NULL,
	[name] [varchar](100) NULL,
	[account_id] [varchar](50) NULL,
	[start_date] [datetime] NULL,
	[budget] [float] NULL,
	[budget_unit] [varchar](5) NULL,
	[deadline] [datetime] NULL,
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
/****** Object:  Table [dbo].[roles]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[schedules]    Script Date: 6/21/2023 11:28:13 ******/
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
 CONSTRAINT [PK_schedules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[skills]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[task_labels]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[task_precedence]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[task_precedences]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[task_resource]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[tasks]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[workforce]    Script Date: 6/21/2023 11:28:13 ******/
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
/****** Object:  Table [dbo].[workforce_skills]    Script Date: 6/21/2023 11:28:13 ******/
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
