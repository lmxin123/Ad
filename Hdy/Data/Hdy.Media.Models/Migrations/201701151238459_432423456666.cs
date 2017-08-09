namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _432423456666 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Merchant", newName: "Corp");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Corp", newName: "Merchant");
        }
    }
}
