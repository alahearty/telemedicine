using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace telemedicine_webapi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Release1Scheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PhysicianId",
                table: "Appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityUserId",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"Appointments\" SET \"AppointmentDate\" = COALESCE(\"AppointmentDate\", \"Created\"), \"ScheduleEndTime\" = COALESCE(\"ScheduleEndTime\", COALESCE(\"AppointmentDate\", \"Created\") + interval '30 minutes')");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduleEndTime",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PhysicianAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhysicianId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicianAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicianAvailabilities_User_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist");
            migrationBuilder.Sql("ALTER TABLE \"Appointments\" ADD CONSTRAINT \"EX_Appointments_Physician_Time\" EXCLUDE USING gist (\"PhysicianId\" WITH =, tstzrange(\"AppointmentDate\", \"ScheduleEndTime\", '[)') WITH &&) WHERE (\"PhysicianId\" IS NOT NULL AND \"Status\" IN (0, 1, 4))");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdentityUserId",
                table: "User",
                column: "IdentityUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_License",
                table: "User",
                column: "License",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PhysicianId_AppointmentDate_ScheduleEndTime",
                table: "Appointments",
                columns: new[] { "PhysicianId", "AppointmentDate", "ScheduleEndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_PhysicianAvailabilities_PhysicianId_DayOfWeek_StartTime_End~",
                table: "PhysicianAvailabilities",
                columns: new[] { "PhysicianId", "DayOfWeek", "StartTime", "EndTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Appointments\" DROP CONSTRAINT IF EXISTS \"EX_Appointments_Physician_Time\"");
            migrationBuilder.DropTable(
                name: "PhysicianAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IdentityUserId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_License",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PhysicianId_AppointmentDate_ScheduleEndTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "User");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduleEndTime",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AppointmentDate",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PhysicianId",
                table: "Appointments",
                column: "PhysicianId");
        }
    }
}
