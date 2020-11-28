namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modelupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Projects", "FinishDate", c => c.DateTime());
            AlterStoredProcedure(
                "dbo.SP_INSERT_PROJECT",
                p => new
                    {
                        Code = p.String(),
                        Name = p.String(maxLength: 50),
                        StartDate = p.DateTime(),
                        FinishDate = p.DateTime(),
                        State = p.Int(),
                        IsSubProject = p.Boolean(),
                        ParentProjectID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Projects]([Code], [Name], [StartDate], [FinishDate], [State], [IsSubProject], [ParentProjectID])
                      VALUES (@Code, @Name, @StartDate, @FinishDate, @State, @IsSubProject, @ParentProjectID)
                      
                      DECLARE @ProjectID int
                      SELECT @ProjectID = [ProjectID]
                      FROM [dbo].[Projects]
                      WHERE @@ROWCOUNT > 0 AND [ProjectID] = scope_identity()
                      
                      SELECT t0.[ProjectID]
                      FROM [dbo].[Projects] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[ProjectID] = @ProjectID"
            );
            
            AlterStoredProcedure(
                "dbo.SP_UPDATE_PROJECT",
                p => new
                    {
                        ProjectID = p.Int(),
                        Code = p.String(),
                        Name = p.String(maxLength: 50),
                        StartDate = p.DateTime(),
                        FinishDate = p.DateTime(),
                        State = p.Int(),
                        IsSubProject = p.Boolean(),
                        ParentProjectID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Projects]
                      SET [Code] = @Code, [Name] = @Name, [StartDate] = @StartDate, [FinishDate] = @FinishDate, [State] = @State, [IsSubProject] = @IsSubProject, [ParentProjectID] = @ParentProjectID
                      WHERE ([ProjectID] = @ProjectID)"
            );
            
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "FinishDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Projects", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
