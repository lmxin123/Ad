namespace Hdy.Media.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2342342 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdDowloadRecord", "Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdDowloadRecord", "Name");
        }
    }
}
