namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MappingDeleteSPForProject : DbMigration
    {
        public override void Up()
        {
            RenameStoredProcedure(name: "dbo.Project_Delete", newName: "SP_DELETE_PROJECT");
        }
        
        public override void Down()
        {
            RenameStoredProcedure(name: "dbo.SP_DELETE_PROJECT", newName: "Project_Delete");
        }
    }
}
