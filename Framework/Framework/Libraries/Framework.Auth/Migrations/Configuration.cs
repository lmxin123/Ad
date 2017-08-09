namespace Framework.Auth.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.SqlServer;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Auth.AuthDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Framework.Auth.AuthDbContext";
          //  SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(AuthDbContext context)
        {
            // 初始化配置将放在这儿
        }
    }

    //internal class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    //{
    //    protected override void Generate(AddColumnOperation addColumnOperation)
    //    {
    //        SetCreateDateColumn(addColumnOperation.Column);

    //        base.Generate(addColumnOperation);
    //    }

    //    protected override void Generate(CreateTableOperation createTableOperation)
    //    {
    //        SetCreatedUtcColumn(createTableOperation.Columns);

    //        base.Generate(createTableOperation);
    //    }

    //    private static void SetCreatedUtcColumn(IEnumerable<ColumnModel> columns)
    //    {
    //        foreach (var columnModel in columns)
    //        {
    //            SetCreateDateColumn(columnModel);
    //        }
    //    }

    //    private static void SetCreateDateColumn(PropertyModel column)
    //    {
    //        if (column.Name == "CreateDate")
    //        {
    //            column.DefaultValueSql = "GETDATE()";
    //        }
    //    }
   // }
}
