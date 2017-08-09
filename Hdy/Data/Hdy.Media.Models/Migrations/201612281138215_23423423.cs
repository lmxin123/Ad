namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _23423423 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdPackage", "AdvertisementID", c => c.String(maxLength: 50));
            CreateIndex("dbo.AdPackage", "AdvertisementID");
            AddForeignKey("dbo.AdPackage", "AdvertisementID", "dbo.Advertisement", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdPackage", "AdvertisementID", "dbo.Advertisement");
            DropIndex("dbo.AdPackage", new[] { "AdvertisementID" });
            DropColumn("dbo.AdPackage", "AdvertisementID");
        }
    }
}
