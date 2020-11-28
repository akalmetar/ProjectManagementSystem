namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingNameFieldofProjectTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "Name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "Name", c => c.String(nullable: false));
        }
    }
}
