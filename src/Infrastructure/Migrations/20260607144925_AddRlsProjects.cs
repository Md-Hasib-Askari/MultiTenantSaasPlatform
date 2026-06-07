using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRlsProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE projects ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql(
                @"
            CREATE POLICY tenant_isolation_policy ON projects
            USING (tenant_id = current_setting('app.tenant_id')::uuid);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS tenant_isolation_policy ON projects;");
            migrationBuilder.Sql("ALTER TABLE projects NO FORCE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE projects DISABLE ROW LEVEL SECURITY;");
        }
    }
}
