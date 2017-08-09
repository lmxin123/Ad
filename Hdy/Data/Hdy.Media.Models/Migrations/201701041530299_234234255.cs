namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _234234255 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdDowloadRecord", "LastDowloadDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdDowloadRecord", "LastDowloadDate");
        }
    }
}
