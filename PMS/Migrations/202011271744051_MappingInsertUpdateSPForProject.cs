namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MappingInsertUpdateSPForProject : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
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
            
            CreateStoredProcedure(
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
            
            CreateStoredProcedure(
                "dbo.Project_Delete",
                p => new
                    {
                        ProjectID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Projects]
                      WHERE ([ProjectID] = @ProjectID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Project_Delete");
            DropStoredProcedure("dbo.SP_UPDATE_PROJECT");
            DropStoredProcedure("dbo.SP_INSERT_PROJECT");
        }
    }
}
