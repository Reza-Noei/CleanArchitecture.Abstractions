using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArch.Messaging.Api.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversionAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSource = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ConvertedRecordsCount = table.Column<int>(type: "integer", nullable: false),
                    TotalRecordCount = table.Column<int>(type: "integer", nullable: false),
                    SuccessRate = table.Column<double>(type: "double precision", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    AuditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversionRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSource = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ConvertedRecordsCount = table.Column<int>(type: "integer", nullable: false),
                    TotalRecordCount = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboxRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Content = table.Column<string>(type: "jsonb", maxLength: 8000, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Content = table.Column<string>(type: "jsonb", maxLength: 8000, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionAuditLogs_AuditedAt",
                table: "ConversionAuditLogs",
                column: "AuditedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionAuditLogs_ConversionId",
                table: "ConversionAuditLogs",
                column: "ConversionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRecords_DataSource",
                table: "ConversionRecords",
                column: "DataSource");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionRecords_StartedAt",
                table: "ConversionRecords",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InboxRecords_Id",
                table: "InboxRecords",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboxRecords_ProcessedAt",
                table: "InboxRecords",
                column: "ProcessedAt",
                filter: "\"ProcessedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxRecords_Id",
                table: "OutboxRecords",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxRecords_ProcessedAt",
                table: "OutboxRecords",
                column: "ProcessedAt",
                filter: "\"ProcessedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversionAuditLogs");

            migrationBuilder.DropTable(
                name: "ConversionRecords");

            migrationBuilder.DropTable(
                name: "InboxRecords");

            migrationBuilder.DropTable(
                name: "OutboxRecords");
        }
    }
}
