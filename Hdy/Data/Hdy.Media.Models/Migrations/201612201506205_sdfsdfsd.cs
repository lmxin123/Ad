namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sdfsdfsd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisement", "UserID", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Advertisement", "RefusalReasons", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Advertisement", "RefusalReasons", c => c.String(maxLength: 500));
            DropColumn("dbo.Advertisement", "UserID");
        }
    }
}
