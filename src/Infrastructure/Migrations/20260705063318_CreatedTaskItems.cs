using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatedTaskItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Projects_ProjectId",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Users_CreatedById",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Users_DeletedById",
                table: "TaskItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Users_UpdatedById",
                table: "TaskItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItem",
                table: "TaskItem");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_ProjectId",
                table: "TaskItem");

            migrationBuilder.RenameTable(
                name: "TaskItem",
                newName: "TaskItems");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_UpdatedById",
                table: "TaskItems",
                newName: "IX_TaskItems_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_DeletedById",
                table: "TaskItems",
                newName: "IX_TaskItems_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItem_CreatedById",
                table: "TaskItems",
                newName: "IX_TaskItems_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TaskItems",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TaskItems",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "TaskItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DueDate",
                table: "TaskItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedHours",
                table: "TaskItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "TaskItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReporterId",
                table: "TaskItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "TaskItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TaskItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItems",
                table: "TaskItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AssigneeId",
                table: "TaskItems",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ProjectId_Sequence",
                table: "TaskItems",
                columns: new[] { "ProjectId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ReporterId",
                table: "TaskItems",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TenantId_AssigneeId",
                table: "TaskItems",
                columns: new[] { "TenantId", "AssigneeId" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TenantId_Priority",
                table: "TaskItems",
                columns: new[] { "TenantId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TenantId_ProjectId_Title",
                table: "TaskItems",
                columns: new[] { "TenantId", "ProjectId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TenantId_ReporterId",
                table: "TaskItems",
                columns: new[] { "TenantId", "ReporterId" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TenantId_Status",
                table: "TaskItems",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Projects_ProjectId",
                table: "TaskItems",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Tenants_TenantId",
                table: "TaskItems",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_AssigneeId",
                table: "TaskItems",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_CreatedById",
                table: "TaskItems",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_DeletedById",
                table: "TaskItems",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_ReporterId",
                table: "TaskItems",
                column: "ReporterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_Users_UpdatedById",
                table: "TaskItems",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Projects_ProjectId",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Tenants_TenantId",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_CreatedById",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_DeletedById",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_ReporterId",
                table: "TaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_Users_UpdatedById",
                table: "TaskItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItems",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_ProjectId_Sequence",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_ReporterId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_TenantId_AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_TenantId_Priority",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_TenantId_ProjectId_Title",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_TenantId_ReporterId",
                table: "TaskItems");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_TenantId_Status",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "EstimatedHours",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "ReporterId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TaskItems");

            migrationBuilder.RenameTable(
                name: "TaskItems",
                newName: "TaskItem");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItems_UpdatedById",
                table: "TaskItem",
                newName: "IX_TaskItem_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItems_DeletedById",
                table: "TaskItem",
                newName: "IX_TaskItem_DeletedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItems_CreatedById",
                table: "TaskItem",
                newName: "IX_TaskItem_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TaskItem",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TaskItem",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItem",
                table: "TaskItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_ProjectId",
                table: "TaskItem",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Projects_ProjectId",
                table: "TaskItem",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Users_CreatedById",
                table: "TaskItem",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Users_DeletedById",
                table: "TaskItem",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Users_UpdatedById",
                table: "TaskItem",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
