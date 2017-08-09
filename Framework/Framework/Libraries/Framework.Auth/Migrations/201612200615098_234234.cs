namespace Framework.Auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _234234 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String(maxLength: 20));
            DropColumn("dbo.AspNetUsers", "Province");
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "Region");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Region", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.AspNetUsers", "City", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.AspNetUsers", "Province", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String(maxLength: 50));
        }
    }
}
