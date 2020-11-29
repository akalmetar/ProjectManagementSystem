namespace PMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingModelForTask : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Tasks", "ParentTaskID");
            AddForeignKey("dbo.Tasks", "ParentTaskID", "dbo.Tasks", "TaskID");
            CreateStoredProcedure(
                "dbo.SP_INSERT_TASK",
                p => new
                    {
                        Name = p.String(maxLength: 50),
                        Description = p.String(),
                        StartDate = p.DateTime(),
                        FinishDate = p.DateTime(),
                        State = p.Int(),
                        IsSubTask = p.Boolean(),
                        ParentTaskID = p.Int(),
                        ProjectID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Tasks]([Name], [Description], [StartDate], [FinishDate], [State], [IsSubTask], [ParentTaskID], [ProjectID])
                      VALUES (@Name, @Description, @StartDate, @FinishDate, @State, @IsSubTask, @ParentTaskID, @ProjectID)
                      
                      DECLARE @TaskID int
                      SELECT @TaskID = [TaskID]
                      FROM [dbo].[Tasks]
                      WHERE @@ROWCOUNT > 0 AND [TaskID] = scope_identity()
                      
                      SELECT t0.[TaskID]
                      FROM [dbo].[Tasks] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[TaskID] = @TaskID"
            );
            
            CreateStoredProcedure(
                "dbo.SP_UPDATE_TASK",
                p => new
                    {
                        TaskID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Description = p.String(),
                        StartDate = p.DateTime(),
                        FinishDate = p.DateTime(),
                        State = p.Int(),
                        IsSubTask = p.Boolean(),
                        ParentTaskID = p.Int(),
                        ProjectID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Tasks]
                      SET [Name] = @Name, [Description] = @Description, [StartDate] = @StartDate, [FinishDate] = @FinishDate, [State] = @State, [IsSubTask] = @IsSubTask, [ParentTaskID] = @ParentTaskID, [ProjectID] = @ProjectID
                      WHERE ([TaskID] = @TaskID)"
            );
            
            CreateStoredProcedure(
                "dbo.SP_DELETE_TASK",
                p => new
                    {
                        TaskID = p.Int(),
                    },
                body:
                    @"DELETE [dbo].[Tasks]
                      WHERE ([TaskID] = @TaskID)"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.SP_DELETE_TASK");
            DropStoredProcedure("dbo.SP_UPDATE_TASK");
            DropStoredProcedure("dbo.SP_INSERT_TASK");
            DropForeignKey("dbo.Tasks", "ParentTaskID", "dbo.Tasks");
            DropIndex("dbo.Tasks", new[] { "ParentTaskID" });
        }
    }
}
