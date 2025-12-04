using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EvdeSaglik.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "DoctorDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorDocuments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("36c2f28e-5e39-4b46-9c28-4ed975cc3dad"), "c0d980fa-38b4-452c-97d8-e1f2504bd3a6", "Patient", "PATIENT" },
                    { new Guid("4c25ec56-9ce7-4b2e-8f4b-38a55559d70c"), "f75bd582-7abd-48c9-86f8-5926962e65d2", "Admin", "ADMIN" },
                    { new Guid("a67588a8-b721-4a7f-8282-54142a0dd503"), "a71bfb05-5a26-4adc-8ec9-9d62c9525e3d", "Doctor", "DOCTOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("0617e1ea-c5a2-4ff4-9b67-78d0d9f73ab3"), 0, "7b302ff8-9fa0-4c4a-8134-cd6fbe37db6e", new DateTime(2025, 11, 27, 9, 51, 33, 906, DateTimeKind.Utc).AddTicks(182), "admin@evdesaglik.com", true, "Admin", true, "User", false, null, "ADMIN@EVDESAGLIK.COM", "ADMIN@EVDESAGLIK.COM", "AQAAAAIAAYagAAAAEEqewGjRYq7JHOE3zZcgMMu7QhWuQzHYz7v1SPkwLkXHvFUJCW1lsclsrkQMh3NbBg==", "5551234567", false, "522aef6b-240f-46fe-bc16-8491143324a5", false, new DateTime(2025, 11, 27, 9, 51, 33, 906, DateTimeKind.Utc).AddTicks(183), "admin@evdesaglik.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("4c25ec56-9ce7-4b2e-8f4b-38a55559d70c"), new Guid("0617e1ea-c5a2-4ff4-9b67-78d0d9f73ab3") });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorDocuments_DoctorId",
                table: "DoctorDocuments",
                column: "DoctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorDocuments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("36c2f28e-5e39-4b46-9c28-4ed975cc3dad"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a67588a8-b721-4a7f-8282-54142a0dd503"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("4c25ec56-9ce7-4b2e-8f4b-38a55559d70c"), new Guid("0617e1ea-c5a2-4ff4-9b67-78d0d9f73ab3") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4c25ec56-9ce7-4b2e-8f4b-38a55559d70c"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("0617e1ea-c5a2-4ff4-9b67-78d0d9f73ab3"));

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
        }
    }
}
