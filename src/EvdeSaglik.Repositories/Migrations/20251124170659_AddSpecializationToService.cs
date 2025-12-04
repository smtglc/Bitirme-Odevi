using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EvdeSaglik.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecializationToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("20a32fa8-ece9-4cd0-829f-c81efff87a8b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3be18aee-eff0-4e36-8b10-6b87f458733d"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("542999f9-63b5-414e-8b52-bae616da28cd"), new Guid("ffe0fbbc-e6d3-4b40-8c1f-ccf03128e680") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("542999f9-63b5-414e-8b52-bae616da28cd"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ffe0fbbc-e6d3-4b40-8c1f-ccf03128e680"));

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Specialization",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1866c1d0-4cf0-4b26-8f6e-023367651907"), "28ce3115-6014-4b99-8850-7fd4d8451689", "Doctor", "DOCTOR" },
                    { new Guid("2ed2e4fa-2d7f-4e38-9ddd-d4e91a82dc3d"), "2b41babb-e820-4971-8db7-bc95aa7dba25", "Patient", "PATIENT" },
                    { new Guid("7af53ea2-3847-439a-b5ba-19d20cc0ab76"), "e19f0f7d-85da-4d11-afc7-88d51985c9f0", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("5d8a2912-0cdb-49f4-921f-fba0e35c9a20"), 0, "b95e0ca5-daa2-4b1a-9a24-7c8c5d0930c2", new DateTime(2025, 11, 24, 17, 6, 56, 693, DateTimeKind.Utc).AddTicks(1551), "admin@evdesaglik.com", true, "Admin", true, "User", false, null, "ADMIN@EVDESAGLIK.COM", "ADMIN@EVDESAGLIK.COM", "AQAAAAIAAYagAAAAEDanfphgm35pt7995P1IuNoQh5zGybS+NqMJ0KUxWZWyxq/XgE4vNGsOMrpPir1YZg==", "5551234567", false, "8e8f5bbc-04a4-482e-a5d4-2d2d59358d93", false, new DateTime(2025, 11, 24, 17, 6, 56, 693, DateTimeKind.Utc).AddTicks(1553), "admin@evdesaglik.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("7af53ea2-3847-439a-b5ba-19d20cc0ab76"), new Guid("5d8a2912-0cdb-49f4-921f-fba0e35c9a20") });

            migrationBuilder.CreateIndex(
                name: "IX_Services_DoctorId",
                table: "Services",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedAt",
                table: "Messages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId_ReceiverId",
                table: "Messages",
                columns: new[] { "SenderId", "ReceiverId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ServiceId",
                table: "Messages",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Doctors_DoctorId",
                table: "Services",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Doctors_DoctorId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Services_DoctorId",
                table: "Services");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("1866c1d0-4cf0-4b26-8f6e-023367651907"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2ed2e4fa-2d7f-4e38-9ddd-d4e91a82dc3d"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("7af53ea2-3847-439a-b5ba-19d20cc0ab76"), new Guid("5d8a2912-0cdb-49f4-921f-fba0e35c9a20") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7af53ea2-3847-439a-b5ba-19d20cc0ab76"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("5d8a2912-0cdb-49f4-921f-fba0e35c9a20"));

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Services");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("20a32fa8-ece9-4cd0-829f-c81efff87a8b"), "bfdcbb52-6d46-4371-9ee4-d55908221207", "Patient", "PATIENT" },
                    { new Guid("3be18aee-eff0-4e36-8b10-6b87f458733d"), "4e46e5c5-5e01-4eb2-a699-5898d0383ea1", "Doctor", "DOCTOR" },
                    { new Guid("542999f9-63b5-414e-8b52-bae616da28cd"), "705bbbcc-61dd-4961-9829-ed8d5601b620", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("ffe0fbbc-e6d3-4b40-8c1f-ccf03128e680"), 0, "5daed1a9-6cf8-4fbd-9856-1319ba017ae0", new DateTime(2025, 11, 23, 20, 27, 23, 1, DateTimeKind.Utc).AddTicks(45), "admin@evdesaglik.com", true, "Admin", true, "User", false, null, "ADMIN@EVDESAGLIK.COM", "ADMIN@EVDESAGLIK.COM", "AQAAAAIAAYagAAAAEM5PToIbCKWQcwyBwb8BZ+4uw+urCG+571vHpXkulZQe6qpkHOwUErhfD1FXNltbwA==", "5551234567", false, "5fcf7ca3-6880-46a6-bbe7-50bf3c61580e", false, new DateTime(2025, 11, 23, 20, 27, 23, 1, DateTimeKind.Utc).AddTicks(47), "admin@evdesaglik.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("542999f9-63b5-414e-8b52-bae616da28cd"), new Guid("ffe0fbbc-e6d3-4b40-8c1f-ccf03128e680") });
        }
    }
}
