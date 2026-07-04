using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsolationMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsolationMode",
                table: "Tenants");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Tenants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                table: "Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "TaskItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                table: "TaskItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "TaskItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Projects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Projects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                table: "Projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "ApiKeys",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedById",
                table: "ApiKeys",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "ApiKeys",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectMembers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CreatedById",
                table: "Tenants",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_DeletedById",
                table: "Tenants",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_UpdatedById",
                table: "Tenants",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_CreatedById",
                table: "TaskItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_DeletedById",
                table: "TaskItem",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_UpdatedById",
                table: "TaskItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DeletedById",
                table: "Projects",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UpdatedById",
                table: "Projects",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedById",
                table: "ApiKeys",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_DeletedById",
                table: "ApiKeys",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedById",
                table: "ApiKeys",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_CreatedById",
                table: "ProjectMembers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_DeletedById",
                table: "ProjectMembers",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId_UserId",
                table: "ProjectMembers",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_TenantId",
                table: "ProjectMembers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UpdatedById",
                table: "ProjectMembers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_AspNetUsers_CreatedById",
                table: "ApiKeys",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_AspNetUsers_DeletedById",
                table: "ApiKeys",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_AspNetUsers_UpdatedById",
                table: "ApiKeys",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_DeletedById",
                table: "Projects",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UpdatedById",
                table: "Projects",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_AspNetUsers_CreatedById",
                table: "TaskItem",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_AspNetUsers_DeletedById",
                table: "TaskItem",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_AspNetUsers_UpdatedById",
                table: "TaskItem",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_CreatedById",
                table: "Tenants",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_DeletedById",
                table: "Tenants",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_UpdatedById",
                table: "Tenants",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_AspNetUsers_CreatedById",
                table: "ApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_AspNetUsers_DeletedById",
                table: "ApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_AspNetUsers_UpdatedById",
                table: "ApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_DeletedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UpdatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_AspNetUsers_CreatedById",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_AspNetUsers_DeletedById",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_AspNetUsers_UpdatedById",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_CreatedById",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_DeletedById",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_UpdatedById",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "ProjectMembers");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_CreatedById",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_DeletedById",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_UpdatedById",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_CreatedById",
                table: "TaskItem");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_DeletedById",
                table: "TaskItem");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_UpdatedById",
                table: "TaskItem");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DeletedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UpdatedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_CreatedById",
                table: "ApiKeys");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_DeletedById",
                table: "ApiKeys");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_UpdatedById",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ApiKeys");

            migrationBuilder.AddColumn<string>(
                name: "IsolationMode",
                table: "Tenants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
