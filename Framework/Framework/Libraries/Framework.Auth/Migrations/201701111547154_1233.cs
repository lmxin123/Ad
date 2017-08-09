namespace Framework.Auth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1233 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUserRoles", "CreateTime");
            DropColumn("dbo.AspNetUserRoles", "Operator");
            DropColumn("dbo.AspNetUserRoles", "Remark");
            DropColumn("dbo.AspNetUserRoles", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetUserRoles", "Remark", c => c.String(maxLength: 500));
            AddColumn("dbo.AspNetUserRoles", "Operator", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUserRoles", "CreateTime", c => c.DateTime());
        }
    }
}
