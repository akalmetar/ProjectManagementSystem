namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldsupdatedtonull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "ParentProjectID", c => c.Int());
            AlterColumn("dbo.Tasks", "ParentTaskID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "ParentTaskID", c => c.Int(nullable: false));
            AlterColumn("dbo.Projects", "ParentProjectID", c => c.Int(nullable: false));
        }
    }
}
