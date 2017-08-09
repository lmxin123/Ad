namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addredirecturl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdvertisementDetail", "RedirectUrl", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdvertisementDetail", "RedirectUrl");
        }
    }
}
