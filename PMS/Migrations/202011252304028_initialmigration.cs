namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialmigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(),
                        State = c.Int(nullable: false),
                        IsSubProject = c.Boolean(nullable: false),
                        ParentProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectID);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(),
                        State = c.Int(nullable: false),
                        IsSubTask = c.Boolean(nullable: false),
                        ParentTaskID = c.Int(nullable: false),
                        ProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.Projects", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ProjectID", "dbo.Projects");
            DropIndex("dbo.Tasks", new[] { "ProjectID" });
            DropTable("dbo.Tasks");
            DropTable("dbo.Projects");
        }
    }
}
