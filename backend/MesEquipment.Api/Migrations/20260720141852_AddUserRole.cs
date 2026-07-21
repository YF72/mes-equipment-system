using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MesEquipment.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. 先允許 null，避免既有資料立刻違反限制。
            migrationBuilder.AddColumn<string>(
                    name: "Role",
                    table: "Users",
                    type: "varchar(50)",
                    maxLength: 50,
                    nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // 2. 明確替既有的 admin 指定角色。
            migrationBuilder.Sql("UPDATE `Users` SET `Role` = 'Administrator' WHERE `Username` = 'admin';");

            // 3. 資料補完後，再把欄位改成必填。
            migrationBuilder.AlterColumn<string>(
                    name: "Role",
                    table: "Users",
                    type: "varchar(50)",
                    maxLength: 50,
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "varchar(50)",
                    oldMaxLength: 50,
                    oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
