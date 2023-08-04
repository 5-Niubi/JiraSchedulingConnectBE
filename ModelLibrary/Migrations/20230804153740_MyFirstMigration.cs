using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelLibrary.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_account",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    avatar = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    password = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "atlassian_token",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    account_installed_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    site = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    access_token = table.Column<string>(type: "varchar(5000)", unicode: false, maxLength: 5000, nullable: true),
                    refress_token = table.Column<string>(type: "varchar(5000)", unicode: false, maxLength: 5000, nullable: true),
                    user_token = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atlassian_token", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    unit = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    unit_price = table.Column<double>(type: "float", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "function",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    cloud_id = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_function", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThreadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "plan_subscription",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    price = table.Column<double>(type: "float", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_subscription", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    image_avatar = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    account_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    budget = table.Column<double>(type: "float", nullable: true),
                    budget_unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    deadline = table.Column<DateTime>(type: "datetime", nullable: true),
                    objective_time = table.Column<double>(type: "float", nullable: true),
                    objective_cost = table.Column<double>(type: "float", nullable: true),
                    objective_quality = table.Column<double>(type: "float", nullable: true),
                    base_working_hour = table.Column<double>(type: "float", nullable: true),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workforce",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    account_id = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    account_type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    avatar = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    display_name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    active = table.Column<int>(type: "int", nullable: true),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    unit_salary = table.Column<double>(type: "float", nullable: true),
                    working_type = table.Column<int>(type: "int", nullable: true),
                    working_effort = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workforce", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    account_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    token_id = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_roles_atlassian_token",
                        column: x => x.token_id,
                        principalTable: "atlassian_token",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "equipments_function",
                columns: table => new
                {
                    equipment_id = table.Column<int>(type: "int", nullable: false),
                    function_id = table.Column<int>(type: "int", nullable: false),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipments_function_1", x => new { x.equipment_id, x.function_id });
                    table.ForeignKey(
                        name: "FK_equipments_function_equipments",
                        column: x => x.equipment_id,
                        principalTable: "equipments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_equipments_function_function",
                        column: x => x.function_id,
                        principalTable: "function",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "plan_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plan_subscription_id = table.Column<int>(type: "int", nullable: true),
                    permission = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK__PlanPermi__plan___44952D46",
                        column: x => x.plan_subscription_id,
                        principalTable: "plan_subscription",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    atlassian_token_id = table.Column<int>(type: "int", nullable: true),
                    token = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    plan_id = table.Column<int>(type: "int", nullable: true),
                    current_period_start = table.Column<DateTime>(type: "datetime", nullable: true),
                    current_period_end = table.Column<DateTime>(type: "datetime", nullable: true),
                    cancel_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_atlassian_token",
                        column: x => x.atlassian_token_id,
                        principalTable: "atlassian_token",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_subscription_plan_subscription",
                        column: x => x.plan_id,
                        principalTable: "plan_subscription",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "milestones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    project_id = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_milestones", x => x.id);
                    table.ForeignKey(
                        name: "FK_milestones_projects",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "parameter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<int>(type: "int", nullable: true),
                    budget = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    deadline = table.Column<DateTime>(type: "datetime", nullable: true),
                    objective_time = table.Column<int>(type: "int", nullable: true),
                    objective_cost = table.Column<int>(type: "int", nullable: true),
                    objective_quality = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parameter", x => x.id);
                    table.ForeignKey(
                        name: "FK_parameter_projects",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "workforce_skills",
                columns: table => new
                {
                    workforce_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workforce_skills", x => new { x.workforce_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_workforce_skills_skills",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_workforce_skills_workforce",
                        column: x => x.workforce_id,
                        principalTable: "workforce",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    duration = table.Column<double>(type: "float", nullable: true),
                    cloud_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    project_id = table.Column<int>(type: "int", nullable: true),
                    milestone_id = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_milestones",
                        column: x => x.milestone_id,
                        principalTable: "milestones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tasks_projects",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "parameter_resource",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parameter_id = table.Column<int>(type: "int", nullable: false),
                    resource_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parameter_resource", x => x.id);
                    table.ForeignKey(
                        name: "FK_parameter_resource_parameter",
                        column: x => x.parameter_id,
                        principalTable: "parameter",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_project_resource_workforce",
                        column: x => x.resource_id,
                        principalTable: "workforce",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parameter_id = table.Column<int>(type: "int", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    cost = table.Column<int>(type: "int", nullable: true),
                    quality = table.Column<int>(type: "int", nullable: true),
                    tasks = table.Column<string>(type: "text", nullable: true),
                    selected = table.Column<int>(type: "int", nullable: true),
                    since = table.Column<DateTime>(type: "datetime", nullable: true),
                    account_id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK__schedules__param__607251E5",
                        column: x => x.parameter_id,
                        principalTable: "parameter",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "task_function",
                columns: table => new
                {
                    task_id = table.Column<int>(type: "int", nullable: false),
                    function_id = table.Column<int>(type: "int", nullable: false),
                    require_time = table.Column<int>(type: "int", nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_function", x => new { x.task_id, x.function_id });
                    table.ForeignKey(
                        name: "FK_task_function_function",
                        column: x => x.function_id,
                        principalTable: "function",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_task_function_tasks",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "task_precedences",
                columns: table => new
                {
                    task_id = table.Column<int>(type: "int", nullable: false),
                    precedence_id = table.Column<int>(type: "int", nullable: false),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_precedences", x => new { x.task_id, x.precedence_id });
                    table.ForeignKey(
                        name: "FK_task_precedences_tasks2",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_task_precedences_tasks3",
                        column: x => x.precedence_id,
                        principalTable: "tasks",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tasks_skills_required",
                columns: table => new
                {
                    task_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: true),
                    create_datetime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    delete_datetime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks_skills_required", x => new { x.task_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_tasks_skills_skills",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tasks_skills_tasks",
                        column: x => x.task_id,
                        principalTable: "tasks",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_roles_token_id",
                table: "account_roles",
                column: "token_id");

            migrationBuilder.CreateIndex(
                name: "IX_equipments_function_function_id",
                table: "equipments_function",
                column: "function_id");

            migrationBuilder.CreateIndex(
                name: "IX_milestones_project_id",
                table: "milestones",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_parameter_project_id",
                table: "parameter",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_parameter_resource_parameter_id",
                table: "parameter_resource",
                column: "parameter_id");

            migrationBuilder.CreateIndex(
                name: "IX_parameter_resource_resource_id",
                table: "parameter_resource",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_plan_permissions_plan_subscription_id",
                table: "plan_permissions",
                column: "plan_subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_parameter_id",
                table: "schedules",
                column: "parameter_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_atlassian_token_id",
                table: "subscription",
                column: "atlassian_token_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plan_id",
                table: "subscription",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_task_function_function_id",
                table: "task_function",
                column: "function_id");

            migrationBuilder.CreateIndex(
                name: "IX_task_precedences_precedence_id",
                table: "task_precedences",
                column: "precedence_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_milestone_id",
                table: "tasks",
                column: "milestone_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_project_id",
                table: "tasks",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_skills_required_skill_id",
                table: "tasks_skills_required",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "UC_Email",
                table: "workforce",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_workforce_skills_skill_id",
                table: "workforce_skills",
                column: "skill_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_roles");

            migrationBuilder.DropTable(
                name: "admin_account");

            migrationBuilder.DropTable(
                name: "equipments_function");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "parameter_resource");

            migrationBuilder.DropTable(
                name: "plan_permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.DropTable(
                name: "task_function");

            migrationBuilder.DropTable(
                name: "task_precedences");

            migrationBuilder.DropTable(
                name: "tasks_skills_required");

            migrationBuilder.DropTable(
                name: "workforce_skills");

            migrationBuilder.DropTable(
                name: "equipments");

            migrationBuilder.DropTable(
                name: "parameter");

            migrationBuilder.DropTable(
                name: "atlassian_token");

            migrationBuilder.DropTable(
                name: "plan_subscription");

            migrationBuilder.DropTable(
                name: "function");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "workforce");

            migrationBuilder.DropTable(
                name: "milestones");

            migrationBuilder.DropTable(
                name: "projects");
        }
    }
}
