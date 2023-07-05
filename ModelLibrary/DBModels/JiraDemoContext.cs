using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ModelLibrary.DBModels
{
    public partial class JiraDemoContext : DbContext
    {
        public JiraDemoContext()
        {
        }

        public JiraDemoContext(DbContextOptions<JiraDemoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountRole> AccountRoles { get; set; } = null!;
        public virtual DbSet<AtlassianToken> AtlassianTokens { get; set; } = null!;
        public virtual DbSet<Equipment> Equipments { get; set; } = null!;
        public virtual DbSet<EquipmentsFunction> EquipmentsFunctions { get; set; } = null!;
        public virtual DbSet<Function> Functions { get; set; } = null!;
        public virtual DbSet<Milestone> Milestones { get; set; } = null!;
        public virtual DbSet<Parameter> Parameters { get; set; } = null!;
        public virtual DbSet<ParameterResource> ParameterResources { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Skill> Skills { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<TaskFunction> TaskFunctions { get; set; } = null!;
        public virtual DbSet<TaskPrecedence> TaskPrecedences { get; set; } = null!;
        public virtual DbSet<TaskResource> TaskResources { get; set; } = null!;
        public virtual DbSet<TasksSkillsRequired> TasksSkillsRequireds { get; set; } = null!;
        public virtual DbSet<Workforce> Workforces { get; set; } = null!;
        public virtual DbSet<WorkforceSkill> WorkforceSkills { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=34.123.177.151,1433; database=JiraDemo; uid=sa; pwd=5Niubipass; TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkforceSkill>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Workforce>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<TasksSkillsRequired>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<TaskResource>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<TaskPrecedence>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<TaskFunction>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Task>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Skill>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Schedule>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Role>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Project>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Function>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<Equipment>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<EquipmentsFunction>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<AtlassianToken>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<AccountRole>().HasQueryFilter(e => e.IsDelete == false);
            modelBuilder.Entity<ParameterResource>().HasQueryFilter(e => e.IsDelete == false);

            modelBuilder.Entity<AccountRole>(entity =>
            {
                entity.ToTable("account_roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TokenId).HasColumnName("token_id");

                entity.HasOne(d => d.Token)
                    .WithMany(p => p.AccountRoles)
                    .HasForeignKey(d => d.TokenId)
                    .HasConstraintName("FK_account_roles_atlassian_token");
            });

            modelBuilder.Entity<AtlassianToken>(entity =>
            {
                entity.ToTable("atlassian_token");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccessToken)
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasColumnName("access_token");

                entity.Property(e => e.AccountInstalledId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_installed_id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RefressToken)
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasColumnName("refress_token");

                entity.Property(e => e.Site)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("site");
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("equipments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("unit");

                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
            });

            modelBuilder.Entity<EquipmentsFunction>(entity =>
            {
                entity.HasKey(e => new { e.EquipmentId, e.FunctionId })
                    .HasName("PK_equipments_function_1");

                entity.ToTable("equipments_function");

                entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");

                entity.Property(e => e.FunctionId).HasColumnName("function_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.EquipmentsFunctions)
                    .HasForeignKey(d => d.EquipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_equipments_function_equipments");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.EquipmentsFunctions)
                    .HasForeignKey(d => d.FunctionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_equipments_function_function");
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("function");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.ToTable("milestones");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Milestones)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_milestones_projects");
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasKey(e => e.Int);

                entity.ToTable("parameter");

                entity.Property(e => e.Int).HasColumnName("int");

                entity.Property(e => e.Budget).HasColumnName("budget");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ObjectiveCost).HasColumnName("objective_cost");

                entity.Property(e => e.ObjectiveQuality).HasColumnName("objective_quality");

                entity.Property(e => e.ObjectiveTime).HasColumnName("objective_time");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Parameters)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_parameter_projects");
            });

            modelBuilder.Entity<ParameterResource>(entity =>
            {
                entity.ToTable("parameter_resource");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ParameterId).HasColumnName("parameter_id");

                entity.Property(e => e.ResourceId).HasColumnName("resource_id");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.HasOne(d => d.Parameter)
                    .WithMany(p => p.ParameterResources)
                    .HasForeignKey(d => d.ParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_parameter_resource_parameter");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.ParameterResources)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_project_resource_equipments");

                entity.HasOne(d => d.ResourceNavigation)
                    .WithMany(p => p.ParameterResources)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_project_resource_workforce");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("projects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_id");

                entity.Property(e => e.Budget).HasColumnName("budget");

                entity.Property(e => e.BudgetUnit)
                    .HasMaxLength(50)
                    .HasColumnName("budget_unit");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Deadline)
                    .HasColumnType("datetime")
                    .HasColumnName("deadline");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.ImageAvatar)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("image_avatar");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.ObjectiveCost).HasColumnName("objective_cost");

                entity.Property(e => e.ObjectiveQuality).HasColumnName("objective_quality");

                entity.Property(e => e.ObjectiveTime).HasColumnName("objective_time");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedules");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_id");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.Quality).HasColumnName("quality");

                entity.Property(e => e.Selected).HasColumnName("selected");

                entity.Property(e => e.Since)
                    .HasColumnType("datetime")
                    .HasColumnName("since");

                entity.Property(e => e.Tasks)
                    .HasColumnType("text")
                    .HasColumnName("tasks");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_schedules_projects");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("skills");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("tasks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.HasOne(d => d.Milestone)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.MilestoneId)
                    .HasConstraintName("FK_tasks_milestones");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_tasks_projects");
            });

            modelBuilder.Entity<TaskFunction>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.FunctionId });

                entity.ToTable("task_function");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.FunctionId).HasColumnName("function_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RequireTime).HasColumnName("require_time");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.TaskFunctions)
                    .HasForeignKey(d => d.FunctionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_function_function");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskFunctions)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_function_tasks");
            });

            modelBuilder.Entity<TaskPrecedence>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.PrecedenceId });

                entity.ToTable("task_precedences");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.PrecedenceId).HasColumnName("precedence_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Precedence)
                    .WithMany(p => p.TaskPrecedencePrecedences)
                    .HasForeignKey(d => d.PrecedenceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_precedences_tasks3");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskPrecedenceTasks)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_precedences_tasks2");
            });

            modelBuilder.Entity<TaskResource>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.ParameterResourceId });

                entity.ToTable("task_resource");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.ParameterResourceId).HasColumnName("parameter_resource_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.HasOne(d => d.ParameterResource)
                    .WithMany(p => p.TaskResources)
                    .HasForeignKey(d => d.ParameterResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_resource_project_resource");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskResources)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_resource_tasks");
            });

            modelBuilder.Entity<TasksSkillsRequired>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.SkillId });

                entity.ToTable("tasks_skills_required");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.TasksSkillsRequireds)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tasks_skills_skills");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TasksSkillsRequireds)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tasks_skills_tasks");
            });

            modelBuilder.Entity<Workforce>(entity =>
            {
                entity.ToTable("workforce");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("account_id");

                entity.Property(e => e.AccountType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_type");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("avatar");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(500)
                    .HasColumnName("display_name");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.UnitSalary).HasColumnName("unit_salary");

                entity.Property(e => e.WorkingEffort)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("working_effort");

                entity.Property(e => e.WorkingType).HasColumnName("working_type");
            });

            modelBuilder.Entity<WorkforceSkill>(entity =>
            {
                entity.HasKey(e => new { e.WorkforceId, e.SkillId });

                entity.ToTable("workforce_skills");

                entity.Property(e => e.WorkforceId).HasColumnName("workforce_id");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.WorkforceSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_workforce_skills_skills");

                entity.HasOne(d => d.Workforce)
                    .WithMany(p => p.WorkforceSkills)
                    .HasForeignKey(d => d.WorkforceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_workforce_skills_workforce");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
