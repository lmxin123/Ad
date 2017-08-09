namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _23423 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Advertisement", name: "MarchantID", newName: "MerchantID");
            RenameIndex(table: "dbo.Advertisement", name: "IX_MarchantID", newName: "IX_MerchantID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Advertisement", name: "IX_MerchantID", newName: "IX_MarchantID");
            RenameColumn(table: "dbo.Advertisement", name: "MerchantID", newName: "MarchantID");
        }
    }
}
