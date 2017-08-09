namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _32423423 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Device", "Email", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Device", "Email", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
