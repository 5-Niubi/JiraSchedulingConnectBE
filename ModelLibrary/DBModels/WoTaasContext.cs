using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UtilsLibrary;

namespace ModelLibrary.DBModels
{
    public partial class WoTaasContext : DbContext
    {
        public WoTaasContext()
        {
        }

        public WoTaasContext(DbContextOptions<WoTaasContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountRole> AccountRoles { get; set; } = null!;
        public virtual DbSet<AdminAccount> AdminAccounts { get; set; } = null!;
        public virtual DbSet<AtlassianToken> AtlassianTokens { get; set; } = null!;
        public virtual DbSet<Equipment> Equipments { get; set; } = null!;
        public virtual DbSet<EquipmentsFunction> EquipmentsFunctions { get; set; } = null!;
        public virtual DbSet<Function> Functions { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<Milestone> Milestones { get; set; } = null!;
        public virtual DbSet<Parameter> Parameters { get; set; } = null!;
        public virtual DbSet<ParameterResource> ParameterResources { get; set; } = null!;
        public virtual DbSet<PlanPermission> PlanPermissions { get; set; } = null!;
        public virtual DbSet<PlanSubscription> PlanSubscriptions { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Skill> Skills { get; set; } = null!;
        public virtual DbSet<Subscription> Subscriptions { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<TaskFunction> TaskFunctions { get; set; } = null!;
        public virtual DbSet<TaskPrecedence> TaskPrecedences { get; set; } = null!;
        public virtual DbSet<TasksSkillsRequired> TasksSkillsRequireds { get; set; } = null!;
        public virtual DbSet<Workforce> Workforces { get; set; } = null!;
        public virtual DbSet<WorkforceSkill> WorkforceSkills { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=35.240.163.125;Database=WoTaas;User Id=sqlserver;Password=5nIUbi@260384K; TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AccountRole>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<AdminAccount>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<AtlassianToken>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Equipment>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<EquipmentsFunction>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Function>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Milestone>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Parameter>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<ParameterResource>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<PlanPermission>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<PlanSubscription>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Project>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Role>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Schedule>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Skill>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Subscription>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Task>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<TaskFunction>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<TaskPrecedence>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<TasksSkillsRequired>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<Workforce>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<WorkforceSkill>().HasQueryFilter(e => e.IsDelete == Const.DELETE_STATE.NOT_DELETE);
            modelBuilder.Entity<AccountRole>(entity =>
            {
                entity.ToTable("account_roles");

                entity.HasIndex(e => e.TokenId, "IX_account_roles_token_id");

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

            modelBuilder.Entity<AdminAccount>(entity =>
            {
                entity.ToTable("admin_account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("avatar");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .HasColumnName("email");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("username");
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

                entity.Property(e => e.UserToken)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_token");
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

                entity.HasIndex(e => e.FunctionId, "IX_equipments_function_function_id");

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

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.ToTable("milestones");

                entity.HasIndex(e => e.ProjectId, "IX_milestones_project_id");

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
                entity.ToTable("parameter");

                entity.HasIndex(e => e.ProjectId, "IX_parameter_project_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Budget).HasColumnName("budget");

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

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ObjectiveCost).HasColumnName("objective_cost");

                entity.Property(e => e.ObjectiveQuality).HasColumnName("objective_quality");

                entity.Property(e => e.ObjectiveTime).HasColumnName("objective_time");

                entity.Property(e => e.Optimizer)
                    .HasColumnName("optimizer")
                    .HasComment("");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Parameters)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_parameter_projects");
            });

            modelBuilder.Entity<ParameterResource>(entity =>
            {
                entity.ToTable("parameter_resource");

                entity.HasIndex(e => e.ParameterId, "IX_parameter_resource_parameter_id");

                entity.HasIndex(e => e.ResourceId, "IX_parameter_resource_resource_id");

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
                    .HasConstraintName("FK_project_resource_workforce");
            });

            modelBuilder.Entity<PlanPermission>(entity =>
            {
                entity.ToTable("plan_permissions");

                entity.HasIndex(e => e.PlanSubscriptionId, "IX_plan_permissions_plan_subscription_id");

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

                entity.Property(e => e.Permission)
                    .HasMaxLength(100)
                    .HasColumnName("permission");

                entity.Property(e => e.PlanSubscriptionId).HasColumnName("plan_subscription_id");

                entity.HasOne(d => d.PlanSubscription)
                    .WithMany(p => p.PlanPermissions)
                    .HasForeignKey(d => d.PlanSubscriptionId)
                    .HasConstraintName("FK__PlanPermi__plan___44952D46");
            });

            modelBuilder.Entity<PlanSubscription>(entity =>
            {
                entity.ToTable("plan_subscription");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

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

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("projects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("account_id");

                entity.Property(e => e.BaseWorkingHour).HasColumnName("base_working_hour");

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

                entity.Property(e => e.WorkingTimes)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("working_times");
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

                entity.HasIndex(e => e.ParameterId, "IX_schedules_parameter_id");

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

                entity.Property(e => e.Desciption)
                    .HasMaxLength(1000)
                    .HasColumnName("desciption");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ParameterId).HasColumnName("parameter_id");

                entity.Property(e => e.Quality).HasColumnName("quality");

                entity.Property(e => e.Selected).HasColumnName("selected");

                entity.Property(e => e.Since)
                    .HasColumnType("datetime")
                    .HasColumnName("since");

                entity.Property(e => e.Tasks)
                    .HasColumnType("text")
                    .HasColumnName("tasks");

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .HasColumnName("title");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Parameter)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.ParameterId)
                    .HasConstraintName("FK__schedules__param__607251E5");
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

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.ToTable("subscription");

                entity.HasIndex(e => e.AtlassianTokenId, "IX_subscription_atlassian_token_id");

                entity.HasIndex(e => e.PlanId, "IX_subscription_plan_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AtlassianTokenId).HasColumnName("atlassian_token_id");

                entity.Property(e => e.CancelAt)
                    .HasColumnType("datetime")
                    .HasColumnName("cancel_at");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrentPeriodEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("current_period_end");

                entity.Property(e => e.CurrentPeriodStart)
                    .HasColumnType("datetime")
                    .HasColumnName("current_period_start");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.Token)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.HasOne(d => d.AtlassianToken)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.AtlassianTokenId)
                    .HasConstraintName("FK_subscription_atlassian_token");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("FK_subscription_plan_subscription");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("tasks");

                entity.HasIndex(e => e.MilestoneId, "IX_tasks_milestone_id");

                entity.HasIndex(e => e.ProjectId, "IX_tasks_project_id");

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

                entity.HasIndex(e => e.FunctionId, "IX_task_function_function_id");

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

                entity.HasIndex(e => e.PrecedenceId, "IX_task_precedences_precedence_id");

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

            modelBuilder.Entity<TasksSkillsRequired>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.SkillId });

                entity.ToTable("tasks_skills_required");

                entity.HasIndex(e => e.SkillId, "IX_tasks_skills_required_skill_id");

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

                entity.HasIndex(e => e.SkillId, "IX_workforce_skills_skill_id");

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
