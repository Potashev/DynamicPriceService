using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPrice.Core.Migrations.DynamicPrice
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Companies",
				columns: table => new
				{
					CompanyId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Companies", x => x.CompanyId);
				});

			migrationBuilder.CreateTable(
				name: "Carts",
				columns: table => new
				{
					CartId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CompanyId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Carts", x => x.CartId);
					table.ForeignKey(
						name: "FK_Carts_Companies_CompanyId",
						column: x => x.CompanyId,
						principalTable: "Companies",
						principalColumn: "CompanyId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Orders",
				columns: table => new
				{
					OrderId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CompanyId = table.Column<int>(type: "int", nullable: false),
					Status = table.Column<int>(type: "int", nullable: false),
					OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
					ReceiveKey = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Orders", x => x.OrderId);
					table.ForeignKey(
						name: "FK_Orders_Companies_CompanyId",
						column: x => x.CompanyId,
						principalTable: "Companies",
						principalColumn: "CompanyId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "PriceRules",
				columns: table => new
				{
					PriceRuleId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CompanyId = table.Column<int>(type: "int", nullable: true),
					Increase = table.Column<double>(type: "float", nullable: false),
					Reduction = table.Column<double>(type: "float", nullable: false),
					NoSellTime = table.Column<TimeSpan>(type: "time", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PriceRules", x => x.PriceRuleId);
					table.ForeignKey(
						name: "FK_PriceRules_Companies_CompanyId",
						column: x => x.CompanyId,
						principalTable: "Companies",
						principalColumn: "CompanyId");
				});

			migrationBuilder.CreateTable(
				name: "Products",
				columns: table => new
				{
					ProductId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CompanyId = table.Column<int>(type: "int", nullable: true),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
					MinimumPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: true),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
					LastSellTime = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Products", x => x.ProductId);
					table.ForeignKey(
						name: "FK_Products_Companies_CompanyId",
						column: x => x.CompanyId,
						principalTable: "Companies",
						principalColumn: "CompanyId");
				});

			migrationBuilder.CreateTable(
				name: "CartItems",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CartId = table.Column<int>(type: "int", nullable: false),
					ProductId = table.Column<int>(type: "int", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CartItems", x => x.Id);
					table.ForeignKey(
						name: "FK_CartItems_Carts_CartId",
						column: x => x.CartId,
						principalTable: "Carts",
						principalColumn: "CartId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_CartItems_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "ProductId");
				});

			migrationBuilder.CreateTable(
				name: "OrderItems",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					OrderId = table.Column<int>(type: "int", nullable: false),
					ProductId = table.Column<int>(type: "int", nullable: false),
					ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Quantity = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_OrderItems", x => x.Id);
					table.ForeignKey(
						name: "FK_OrderItems_Orders_OrderId",
						column: x => x.OrderId,
						principalTable: "Orders",
						principalColumn: "OrderId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_OrderItems_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "ProductId",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "PriceDynamics",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ProductId = table.Column<int>(type: "int", nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Date = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PriceDynamics", x => x.Id);
					table.ForeignKey(
						name: "FK_PriceDynamics_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "ProductId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_CartItems_CartId",
				table: "CartItems",
				column: "CartId");

			migrationBuilder.CreateIndex(
				name: "IX_CartItems_ProductId",
				table: "CartItems",
				column: "ProductId");

			migrationBuilder.CreateIndex(
				name: "IX_Carts_CompanyId",
				table: "Carts",
				column: "CompanyId");

			migrationBuilder.CreateIndex(
				name: "IX_OrderItems_OrderId",
				table: "OrderItems",
				column: "OrderId");

			migrationBuilder.CreateIndex(
				name: "IX_OrderItems_ProductId",
				table: "OrderItems",
				column: "ProductId");

			migrationBuilder.CreateIndex(
				name: "IX_Orders_CompanyId",
				table: "Orders",
				column: "CompanyId");

			migrationBuilder.CreateIndex(
				name: "IX_PriceDynamics_ProductId",
				table: "PriceDynamics",
				column: "ProductId");

			migrationBuilder.CreateIndex(
				name: "IX_PriceRules_CompanyId",
				table: "PriceRules",
				column: "CompanyId");

			migrationBuilder.CreateIndex(
				name: "IX_Products_CompanyId",
				table: "Products",
				column: "CompanyId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "CartItems");

			migrationBuilder.DropTable(
				name: "OrderItems");

			migrationBuilder.DropTable(
				name: "PriceDynamics");

			migrationBuilder.DropTable(
				name: "PriceRules");

			migrationBuilder.DropTable(
				name: "Carts");

			migrationBuilder.DropTable(
				name: "Orders");

			migrationBuilder.DropTable(
				name: "Products");

			migrationBuilder.DropTable(
				name: "Companies");
		}
	}
}
