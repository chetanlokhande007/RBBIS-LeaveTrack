using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveAttendance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "HR" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordResetExpiry", "PasswordResetToken" },
                values: new object[] { "$2a$11$fj7OLE.12TjapsPRK8uZhupJQpleDR54MjVBoc/xkQbWXulDE.2uy", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordResetExpiry", "PasswordResetToken" },
                values: new object[] { "$2a$11$ppeopDz/yEc0FhadBQSB8.sggSwkB3Fkp4fOrtjohM3jElugDXqmC", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "PasswordResetExpiry", "PasswordResetToken" },
                values: new object[] { "$2a$11$Y44zQDf4L2NdTQdrqFpX7eF3ayc9g1ZL83Yry3YvLgQ2KkqgC3YN2", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PasswordHash", "PasswordResetExpiry", "PasswordResetToken" },
                values: new object[] { "$2a$11$9OV00WahHnLBByoMY09oOuPjsIQoO890tnLg2Wz0udzrAYx.h/i8a", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "PasswordHash", "PasswordResetExpiry", "PasswordResetToken" },
                values: new object[] { "$2a$11$IPfcRV2qgK5RmI.eKmHXpuvbcMUITUEWsArnnD3mFqOSNYTgnfIj2", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "PasswordResetExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$XuK2DNxOS2RInAZpsH.To.DIfynt8yvGtJk6eTTqzRTeHT97zWzzy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$WjWfK7WQM752XLAJSl4KP.K29xz7uinsQ9ppscjn67UryQWTvjV/O");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Le9HunJNK2Mjnm1D1rrqhOGKVZFGxHVW8qa28gf9PA.pKAetzckIW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$QrWgUSvsq3k/DpL.djE0HONfUyeSpXrlok.jbC1KLYJCbnBcmoL6a");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$xaykmj6.CiAHnBkMHwcXoOpUSsl2sJdJ2txkbGhP0HkQgvj7R.4Cu");
        }
    }
}
