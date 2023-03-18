using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuildingBlocks.Messaging.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.CreateTable(
                name: "StoreMessages",
                schema: "messaging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    datatype = table.Column<string>(name: "data_type", type: "nvarchar(max)", nullable: false),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    retrycount = table.Column<int>(name: "retry_count", type: "int", nullable: false),
                    messagestatus = table.Column<string>(name: "message_status", type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    deliverytype = table.Column<string>(name: "delivery_type", type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_store_messages", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreMessages",
                schema: "messaging");
        }
    }
}
