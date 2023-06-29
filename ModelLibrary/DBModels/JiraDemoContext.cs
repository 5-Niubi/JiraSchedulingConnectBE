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
        public virtual DbSet<Function> Functions { get; set; } = null!;
        public virtual DbSet<Label> Labels { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Skill> Skills { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<TaskLabel> TaskLabels { get; set; } = null!;
        public virtual DbSet<TaskPrecedence> TaskPrecedences { get; set; } = null!;
        public virtual DbSet<TaskResource> TaskResources { get; set; } = null!;
        public virtual DbSet<TasksSkillsRequired> TasksSkillsRequireds { get; set; } = null!;
        public virtual DbSet<Workforce> Workforces { get; set; } = null!;
        public virtual DbSet<WorkforceSkill> WorkforceSkills { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=(local); database=JiraDemo; uid=sa; pwd=sa; TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.IsDelete1).HasColumnName("is_delete");

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
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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
                    .HasMaxLength(10)
                    .HasColumnName("cloud_id")
                    .IsFixedLength();

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("unit");

                entity.Property(e => e.UnitPrice).HasColumnName("unit_price");

                entity.HasMany(d => d.Functions)
                    .WithMany(p => p.Equipment)
                    .UsingEntity<Dictionary<string, object>>(
                        "EquipmentsFunction",
                        l => l.HasOne<Function>().WithMany().HasForeignKey("FunctionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_equipments_function_function"),
                        r => r.HasOne<Equipment>().WithMany().HasForeignKey("EquipmentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_equipments_function_equipments"),
                        j =>
                        {
                            j.HasKey("EquipmentId", "FunctionId").HasName("PK_equipments_function_1");

                            j.ToTable("equipments_function");

                            j.IndexerProperty<int>("EquipmentId").HasColumnName("equipment_id");

                            j.IndexerProperty<int>("FunctionId").HasColumnName("function_id");
                        });
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("function");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.ToTable("labels");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId).HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
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
                    .HasColumnName("create_datetime");

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

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_tasks_projects");
            });

            modelBuilder.Entity<TaskLabel>(entity =>
            {
                entity.ToTable("task_labels");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloudId).HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.LabelId).HasColumnName("label_id");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.HasOne(d => d.Label)
                    .WithMany(p => p.TaskLabels)
                    .HasForeignKey(d => d.LabelId)
                    .HasConstraintName("FK_task_labels_labels");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskLabels)
                    .HasForeignKey(d => d.TaskId)
                    .HasConstraintName("FK_task_labels_tasks");
            });

            modelBuilder.Entity<TaskPrecedence>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.PrecedenceId });

                entity.ToTable("task_precedences");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.PrecedenceId).HasColumnName("precedence_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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
                entity.ToTable("task_resource");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.ResourceId).HasColumnName("resource_id");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.TaskResources)
                    .HasForeignKey(d => d.ResourceId)
                    .HasConstraintName("FK_task_resource_equipments");

                entity.HasOne(d => d.ResourceNavigation)
                    .WithMany(p => p.TaskResources)
                    .HasForeignKey(d => d.ResourceId)
                    .HasConstraintName("FK_task_resource_workforce");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskResources)
                    .HasForeignKey(d => d.TaskId)
                    .HasConstraintName("FK_task_resource_tasks");

                entity.HasMany(d => d.Functions)
                    .WithMany(p => p.Tasks)
                    .UsingEntity<Dictionary<string, object>>(
                        "TaskFunction",
                        l => l.HasOne<Function>().WithMany().HasForeignKey("FunctionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_task_function_function"),
                        r => r.HasOne<TaskResource>().WithMany().HasForeignKey("TaskId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_task_function_task_resource"),
                        j =>
                        {
                            j.HasKey("TaskId", "FunctionId");

                            j.ToTable("task_function");

                            j.IndexerProperty<int>("TaskId").HasColumnName("task_id");

                            j.IndexerProperty<int>("FunctionId").HasColumnName("function_id");
                        });
            });

            modelBuilder.Entity<TasksSkillsRequired>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.SkillId, e.CloudId })
                    .HasName("PK_tasks_skills_1");

                entity.ToTable("tasks_skills_required");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(10)
                    .HasColumnName("cloud_id")
                    .IsFixedLength();

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete)
                    .HasColumnType("datetime")
                    .HasColumnName("is_delete");

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
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(500)
                    .HasColumnName("display_name");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.UnitSalary).HasColumnName("unit_salary");

                entity.Property(e => e.WorkingEffort).HasColumnName("working_effort");
            });

            modelBuilder.Entity<WorkforceSkill>(entity =>
            {
                entity.HasKey(e => new { e.WorkforceId, e.SkillId });

                entity.ToTable("workforce_skills");

                entity.Property(e => e.WorkforceId).HasColumnName("workforce_id");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.CloudId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cloud_id");

                entity.Property(e => e.CreateDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("create_datetime");

                entity.Property(e => e.DeleteDatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("delete_datetime");

                entity.Property(e => e.IsDelete).HasColumnName("is_delete");

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
