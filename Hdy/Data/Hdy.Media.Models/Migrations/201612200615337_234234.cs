namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _234234 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Merchant", "Phone", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Merchant", "RegisteredAddress", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Merchant", "RegisteredAddress", c => c.String(maxLength: 50));
            AlterColumn("dbo.Merchant", "Phone", c => c.String(nullable: false));
        }
    }
}
