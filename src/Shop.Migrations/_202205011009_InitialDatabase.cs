using FluentMigrator;

namespace Shop.Migrations
{
    [Migration(202205011009)]
    public class _202205011009_InitialDatabase : Migration
    {
        public override void Up()
        {
            CreateCategories();
            CreateProducts();
            CreateSaleBills();
            CreatePurchaseBills();
        }

        public override void Down()
        {
            Delete.Table("SaleBills");
            Delete.Table("PurchaseBills");
            Delete.Table("Products");
            Delete.Table("Categories");
        }

        private void CreateProducts()
        {
            Create.Table("Products")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("Name").AsString(100).NotNullable()
                    .WithColumn("Code").AsInt32().NotNullable()
                    .WithColumn("MinimumInStock").AsInt32().NotNullable()
                    .WithColumn("InStockCount").AsInt32().WithDefaultValue(0)
                    .WithColumn("Price").AsInt32()
                    .WithColumn("CategoryId").AsInt32().NotNullable()
                    .ForeignKey("FK_Products_Categories", "Categories", "Id")
                    .OnDelete(System.Data.Rule.None);
        }

        private void CreateCategories()
        {
            Create.Table("Categories")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                    .WithColumn("Title").AsString(50).NotNullable();
        }
        private void CreatePurchaseBills()
        {
            Create.Table("PurchaseBills")
                            .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                            .WithColumn("SellerName").AsString(100).NotNullable()
                            .WithColumn("Date").AsDateTime().NotNullable()
                            .WithColumn("Count").AsInt32().NotNullable()
                            .WithColumn("WholePrice").AsInt32().NotNullable()
                            .WithColumn("ProductId").AsInt32().NotNullable()
                            .ForeignKey("FK_PurchaseBills_Products", "Products", "Id")
                            .OnDelete(System.Data.Rule.None);
        }

        private void CreateSaleBills()
        {
            Create.Table("SaleBills")
                            .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
                            .WithColumn("CustomerName").AsString(100).NotNullable()
                            .WithColumn("Date").AsDateTime().NotNullable()
                            .WithColumn("Count").AsInt32().NotNullable()
                            .WithColumn("WholePrice").AsInt32().NotNullable()
                            .WithColumn("ProductId").AsInt32().NotNullable()
                            .ForeignKey("FK_SaleBills_Products", "Products", "Id")
                            .OnDelete(System.Data.Rule.None);
        }
    }
}
