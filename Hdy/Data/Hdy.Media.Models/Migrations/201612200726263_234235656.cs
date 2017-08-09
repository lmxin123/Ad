namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _234235656 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Merchant", "BusinessLicensePath", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Merchant", "BusinessLicensePath", c => c.String(maxLength: 50));
        }
    }
}
