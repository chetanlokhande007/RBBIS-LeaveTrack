using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveAttendance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$MGPt6CHo/X4Q35pRNhv5qeKbYHE8SG0wvjOmr2ydsx4lMO2/o5Azi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$uXVLux.5h9X5mZVdR5RiLO5CwOgYKhhfPciV4ud2ateiVBLxUyJFa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$BqVBQPJ388LkrFFoEYPueuQ0zhW5Ik/pRVh.FuTuwne9t6O4J5J9q");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$mAqB27puFpdHkLS0YWt.xeNnT1aGV0y/rkDmhaihgLQy0AwqRjqYm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$2FaVXWOZLiQwW540ek9nL.NYap9cENsi0L0Edo95NpruNPqCZBGr6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$fj7OLE.12TjapsPRK8uZhupJQpleDR54MjVBoc/xkQbWXulDE.2uy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$ppeopDz/yEc0FhadBQSB8.sggSwkB3Fkp4fOrtjohM3jElugDXqmC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Y44zQDf4L2NdTQdrqFpX7eF3ayc9g1ZL83Yry3YvLgQ2KkqgC3YN2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$9OV00WahHnLBByoMY09oOuPjsIQoO890tnLg2Wz0udzrAYx.h/i8a");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$IPfcRV2qgK5RmI.eKmHXpuvbcMUITUEWsArnnD3mFqOSNYTgnfIj2");
        }
    }
}
