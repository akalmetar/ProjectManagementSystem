namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatingHierarchicalProject : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Projects", "ParentProjectID");
            AddForeignKey("dbo.Projects", "ParentProjectID", "dbo.Projects", "ProjectID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "ParentProjectID", "dbo.Projects");
            DropIndex("dbo.Projects", new[] { "ParentProjectID" });
        }
    }
}
