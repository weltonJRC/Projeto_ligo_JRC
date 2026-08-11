using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jrc.LigoCampaignGateway.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialGatewaySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispatchesSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tenant = table.Column<string>(type: "text", nullable: false),
                    Campaign = table.Column<string>(type: "text", nullable: false),
                    CampaignRunId = table.Column<string>(type: "text", nullable: false),
                    RecordId = table.Column<string>(type: "text", nullable: false),
                    ProviderCorrelationId = table.Column<string>(type: "text", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaLeaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    NumberChip = table.Column<string>(type: "text", nullable: false),
                    DestinationHash = table.Column<string>(type: "text", nullable: false),
                    DestinationLast4 = table.Column<string>(type: "text", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProviderMessageId = table.Column<string>(type: "text", nullable: true),
                    LastErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchesSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaAssetsSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    StoredFileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Sha256 = table.Column<string>(type: "text", nullable: false),
                    StorageProvider = table.Column<string>(type: "text", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    PublicUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssetsSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaLeasesSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    ProviderMediaId = table.Column<string>(type: "text", nullable: false),
                    ValidUntilRaw = table.Column<string>(type: "text", nullable: false),
                    ValidUntilParsed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParseSucceeded = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaLeasesSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusEventsSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderMessageId = table.Column<string>(type: "text", nullable: false),
                    EventStatus = table.Column<string>(type: "text", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusEventsSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplatesSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tenant = table.Column<string>(type: "text", nullable: false),
                    NumberChip = table.Column<string>(type: "text", nullable: false),
                    ProviderTemplateId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    HeaderType = table.Column<string>(type: "text", nullable: false),
                    ParameterCount = table.Column<int>(type: "integer", nullable: false),
                    CallbackStatusUrl = table.Column<string>(type: "text", nullable: false),
                    CallbackResponsesUrl = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplatesSet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchesSet_ProviderCorrelationId",
                table: "DispatchesSet",
                column: "ProviderCorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchesSet_Tenant_Campaign_CampaignRunId_RecordId_Templa~",
                table: "DispatchesSet",
                columns: new[] { "Tenant", "Campaign", "CampaignRunId", "RecordId", "TemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssetsSet_Sha256",
                table: "MediaAssetsSet",
                column: "Sha256");

            migrationBuilder.CreateIndex(
                name: "IX_TemplatesSet_ProviderTemplateId",
                table: "TemplatesSet",
                column: "ProviderTemplateId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchesSet");

            migrationBuilder.DropTable(
                name: "MediaAssetsSet");

            migrationBuilder.DropTable(
                name: "MediaLeasesSet");

            migrationBuilder.DropTable(
                name: "StatusEventsSet");

            migrationBuilder.DropTable(
                name: "TemplatesSet");
        }
    }
}
