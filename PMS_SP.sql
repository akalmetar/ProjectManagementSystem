USE [pms]
GO

DROP PROCEDURE [dbo].[SP_DELETE_PROJECT]
GO
/****** Object:  StoredProcedure [dbo].[SP_DELETE_PROJECT]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_DELETE_PROJECT]
    @ProjectID [int]
AS
BEGIN
    DELETE [dbo].[Projects]
    WHERE ([ProjectID] = @ProjectID)
END
GO

DROP PROCEDURE [dbo].[SP_DELETE_TASK]
GO
/****** Object:  StoredProcedure [dbo].[SP_DELETE_TASK]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_DELETE_TASK]
    @TaskID [int]
AS
BEGIN
    DELETE [dbo].[Tasks]
    WHERE ([TaskID] = @TaskID)
END
GO

DROP PROCEDURE [dbo].[SP_INSERT_PROJECT]
GO
/****** Object:  StoredProcedure [dbo].[SP_INSERT_PROJECT]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_INSERT_PROJECT]
    @Code [nvarchar](max),
    @Name [nvarchar](50),
    @StartDate [datetime],
    @FinishDate [datetime],
    @State [int],
    @IsSubProject [bit],
    @ParentProjectID [int]
AS
BEGIN TRY
	IF (@isSubProject = 1)
		BEGIN
			IF(@ParentProjectID IS NULL OR @ParentProjectID = 0)
				BEGIN
					RAISERROR('ParentProjectID is missing for sub project', 11, 1)
				END
			ELSE
				BEGIN
					DECLARE @ParentType BIT


					SELECT @ParentType = IsSubProject  from Projects WHERE ProjectID = @ParentProjectID

					IF(@ParentType = 1)
						RAISERROR('Cannot create subprojects within subprojects', 11, 1)
				END
			
		END

    INSERT [dbo].[Projects]([Code], [Name], [StartDate], [FinishDate], [State], [IsSubProject], [ParentProjectID])
    VALUES (@Code, @Name, @StartDate, @FinishDate, @State, @IsSubProject, @ParentProjectID)
    
    DECLARE @ProjectID int
    SELECT @ProjectID = [ProjectID]
    FROM [dbo].[Projects]
    WHERE @@ROWCOUNT > 0 AND [ProjectID] = scope_identity()
    
    SELECT t0.[ProjectID]
    FROM [dbo].[Projects] AS t0
    WHERE @@ROWCOUNT > 0 AND t0.[ProjectID] = @ProjectID

	IF(@isSubProject = 1)
	BEGIN
		EXEC [dbo].[SP_UPDATE_PARENT_PROJECT_STATE] @ParentProjectID = @ParentProjectID
	END
END TRY
BEGIN CATCH
	DECLARE @Message VARCHAR(MAX) = ERROR_MESSAGE(),
			@Severity INT = ERROR_SEVERITY(),
			@ErrorState SMALLINT = ERROR_STATE()

	RAISERROR(@Message, @Severity, @State)
END CATCH
GO

DROP PROCEDURE [dbo].[SP_INSERT_TASK]
GO
/****** Object:  StoredProcedure [dbo].[SP_INSERT_TASK]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_INSERT_TASK]
    @Name [nvarchar](50),
    @Description [nvarchar](max),
    @StartDate [datetime],
    @FinishDate [datetime],
    @State [int],
    @IsSubTask [bit],
    @ParentTaskID [int],
    @ProjectID [int]
AS
BEGIN TRY

	IF (@ProjectID = 0)
		RAISERROR('ProjectID is required for the task', 11, 1)

	IF (@IsSubTask = 1)
		BEGIN
			IF(@ParentTaskID IS NULL OR @ParentTaskID = 0)
				BEGIN
					RAISERROR('ParentTaskID is missing for sub task', 11, 1)
				END
			ELSE
				BEGIN
					DECLARE @ParentTaskType BIT
					DECLARE @ParentProjectID INT

					SELECT @ParentTaskType = IsSubTask, @ParentProjectID = ProjectID  from Tasks WHERE TaskID = @ParentTaskID

					IF(@ParentTaskType = 1)
						RAISERROR('Cannot create sub task within sub task', 11, 1)
					
					IF(@ParentProjectID <> @ProjectID)
						RAISERROR('Project ID of Parent task and sub task does not match', 11, 1)
				END
			
		END

	INSERT [dbo].[Tasks]([Name], [Description], [StartDate], [FinishDate], [State], [IsSubTask], [ParentTaskID], [ProjectID])
	VALUES (@Name, @Description, @StartDate, @FinishDate, @State, @IsSubTask, @ParentTaskID, @ProjectID)
    
	DECLARE @TaskID int
	SELECT @TaskID = [TaskID]
	FROM [dbo].[Tasks]
	WHERE @@ROWCOUNT > 0 AND [TaskID] = scope_identity()
    
	SELECT t0.[TaskID]
	FROM [dbo].[Tasks] AS t0
	WHERE @@ROWCOUNT > 0 AND t0.[TaskID] = @TaskID

	IF(@IsSubTask = 1)
		EXEC [dbo].[SP_UPDATE_PARENT_TASK_STATE] @ParentTaskID = @ParentTaskID, @ProjectID = @ProjectID
	ELSE
		EXEC [dbo].[SP_UPDATE_PARENT_TASK_STATE] @ParentTaskID = 0, @ProjectID = @ProjectID

END TRY
BEGIN CATCH
	DECLARE @Message VARCHAR(MAX) = ERROR_MESSAGE(),
			@Severity INT = ERROR_SEVERITY(),
			@ErrorState SMALLINT = ERROR_STATE()

	RAISERROR(@Message, @Severity, @State)
END CATCH
GO


/****** Object:  StoredProcedure [dbo].[SP_UPDATE_PARENT_PROJECT_STATE]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_UPDATE_PARENT_PROJECT_STATE]
	@ParentProjectID INT
AS
	BEGIN TRY
		/*
			State = 1-Planned
			State = 2-Inprogress
			State = 3-Completed
		*/

		DECLARE @ProjectType as BIT
		DECLARE @TempParentID INT
		DECLARE @State INT 
		DECLARE @TaskPlannedState	as INT
		DECLARE @TaskInprogressState as INT		
		DECLARE @ParentPlannedState	as INT
		DECLARE @ParentInprogressState as INT

		SELECT @ProjectType = IsSubProject FROM Projects WHERE ProjectID = @ParentProjectID

		IF @ProjectType = 1
			BEGIN			
				--Checking for Task State		
				SELECT @TaskPlannedState = COUNT(*) from Projects P
					INNER JOIN Tasks T ON T.ProjectID = p.ProjectID
				WHERE P.ProjectID = @ParentProjectID AND T.State = 1

				SELECT @TaskInprogressState = COUNT(*) from Projects P
					INNER JOIN Tasks T ON T.ProjectID = p.ProjectID
				WHERE P.ProjectID = @ParentProjectID AND T.State = 2

				IF @TaskInprogressState > 0
					SET @State = 2
				ELSE IF @TaskPlannedState > 0
					SET @State = 1
				ELSE 
					SET @State = 3

				UPDATE Projects 
				SET [State] = @State
				WHERE ProjectID = @ParentProjectID	
				
				--Checking for Project Parent Project
				SELECT @TempParentID = ParentProjectID FROM Projects WHERE ProjectID = @ParentProjectID	
			END
		ELSE
			SET @TempParentID = @ParentProjectID

			--Checking for Task State for Parent		
			SELECT @TaskPlannedState = COUNT(*) from Projects P
				INNER JOIN Tasks T ON T.ProjectID = p.ProjectID
			WHERE P.ProjectID = @ParentProjectID AND T.State = 1

			SELECT @TaskInprogressState = COUNT(*) from Projects P
				INNER JOIN Tasks T ON T.ProjectID = p.ProjectID
			WHERE P.ProjectID = @ParentProjectID AND T.State = 2

			IF @TaskInprogressState > 0
				SET @State = 2
			ELSE IF @TaskPlannedState > 0
				SET @State = 1
			ELSE 
				SET @State = 3

			UPDATE Projects 
			SET [State] = @State
			WHERE ProjectID = @TempParentID

			IF(@State = 3)
			BEGIN
				--Checking for Project Parent Project
				SELECT @ParentPlannedState = COUNT(*) FROM Projects WHERE STATE IN (1) AND ParentProjectID = @TempParentID
				SELECT @ParentInprogressState = COUNT(*) FROM Projects WHERE STATE IN (2) AND ParentProjectID = @TempParentID
			
				IF @ParentInprogressState > 0
					SET @State = 2
				ELSE IF @ParentPlannedState > 0
					SET @State = 1
				ELSE 
					SET @State = 3


				UPDATE Projects 
				SET [State] = @State
				WHERE ProjectID = @TempParentID	
			END
	END TRY
	BEGIN CATCH
		DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
				@Severity int = ERROR_SEVERITY(),
				@ErrorState smallint = ERROR_STATE()

		RAISERROR(@Message, @Severity, @State)
	END CATCH
GO


/****** Object:  StoredProcedure [dbo].[SP_UPDATE_PARENT_TASK_STATE]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_UPDATE_PARENT_TASK_STATE]
	@ParentTaskID INT,
	@ProjectID INT
AS
	BEGIN TRY
		/*
			State = 1-Planned
			State = 2-Inprogress
			State = 3-Completed
		*/

		IF @ParentTaskID > 0
			BEGIN
				DECLARE @State INT 
				DECLARE @ParentPlannedState	as INT
				DECLARE @ParentInprogressState as INT
		
				SELECT @ParentPlannedState = COUNT(*) FROM Tasks WHERE STATE IN (1) AND ParentTaskID = @ParentTaskID
				SELECT @ParentInprogressState = COUNT(*) FROM Tasks WHERE STATE IN (2) AND ParentTaskID = @ParentTaskID
			
				IF @ParentInprogressState > 0
					SET @State = 2
				ELSE IF @ParentPlannedState > 0
					SET @State = 1
				ELSE 
					SET @State = 3

				UPDATE Tasks 
				SET [State] = @State
				WHERE TaskID = @ParentTaskID
			END
		
		EXEC [dbo].[SP_UPDATE_PARENT_PROJECT_STATE] @ParentProjectID = @ProjectID
	END TRY
	BEGIN CATCH
		DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
				@Severity int = ERROR_SEVERITY(),
				@ErrorState smallint = ERROR_STATE()

		RAISERROR(@Message, @Severity, @State)
	END CATCH
GO

DROP PROCEDURE [dbo].[SP_UPDATE_PROJECT]
GO
/****** Object:  StoredProcedure [dbo].[SP_UPDATE_PROJECT]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_UPDATE_PROJECT] 
    @ProjectID [int],
    @Code [nvarchar](max),
    @Name [nvarchar](50),
    @StartDate [datetime],
    @FinishDate [datetime],
    @State [int],
    @IsSubProject [bit],
    @ParentProjectID [int]
AS
BEGIN TRY

	IF (@isSubProject = 1)
		BEGIN
			IF(@ParentProjectID IS NULL OR @ParentProjectID = 0)
				BEGIN
					RAISERROR('ParentProjectID is missing for sub project', 11, 1)
				END
			ELSE IF(@ParentProjectID = @ProjectID)
				BEGIN
					RAISERROR('A project cannot become parent of itself', 11, 1)
				END
			ELSE
				BEGIN
					DECLARE @ParentType BIT

					SELECT @ParentType = IsSubProject  from Projects WHERE ProjectID = @ParentProjectID

					IF(@ParentType = 1)
						RAISERROR('Cannot create subprojects within subprojects', 11, 1)
				END
		END
	
	UPDATE Projects
	SET [Code] = @Code,
		[Name] = @Name,
		[StartDate] = @StartDate,
		[FinishDate] = @FinishDate,
		[State] = @State,
		[IsSubProject] = @IsSubProject,
		[ParentProjectID] = @ParentProjectID
	WHERE
		[ProjectID] = @ProjectID
					
	IF(@isSubProject = 1)
		EXEC [dbo].[SP_UPDATE_PARENT_PROJECT_STATE] @ParentProjectID = @ParentProjectID
	ELSE
		EXEC [dbo].[SP_UPDATE_PARENT_PROJECT_STATE] @ParentProjectID = @ProjectID

END TRY
BEGIN CATCH
	DECLARE @Message VARCHAR(MAX) = ERROR_MESSAGE(),
			@Severity INT = ERROR_SEVERITY(),
			@ErrorState SMALLINT = ERROR_STATE()

	RAISERROR(@Message, @Severity, @State)
END CATCH
GO

DROP PROCEDURE [dbo].[SP_UPDATE_TASK]
GO
/****** Object:  StoredProcedure [dbo].[SP_UPDATE_TASK]    Script Date: 30-11-2020 01:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_UPDATE_TASK]
    @TaskID [int],
    @Name [nvarchar](50),
    @Description [nvarchar](max),
    @StartDate [datetime],
    @FinishDate [datetime],
    @State [int],
    @IsSubTask [bit],
    @ParentTaskID [int],
    @ProjectID [int]
AS
BEGIN TRY
	IF (@ProjectID = 0)
		RAISERROR('ProjectID is required for the task', 11, 1)

	IF (@IsSubTask = 1)
		BEGIN
			IF(@ParentTaskID IS NULL OR @ParentTaskID = 0)
				BEGIN
					RAISERROR('ParentTaskID is missing for sub task', 11, 1)
				END
			ELSE IF(@ParentTaskID = @TaskID)
				BEGIN
					RAISERROR('A task cannot become parent of itself', 11, 1)
				END
			ELSE
				BEGIN
					DECLARE @ParentTaskType BIT
					DECLARE @ParentProjectID INT

					SELECT @ParentTaskType = IsSubTask  from Tasks WHERE TaskID = @ParentTaskID

					IF(@ParentTaskType = 1)
						RAISERROR('Cannot create sub task within sub task', 11, 1)
					
					IF(@ParentProjectID <> @ProjectID)
						RAISERROR('Project ID of Parent task and sub task does not match', 11, 1)
				END
		END

    UPDATE [dbo].[Tasks]
    SET [Name] = @Name, 
		[Description] = @Description, 
		[StartDate] = @StartDate, 
		[FinishDate] = @FinishDate, 
		[State] = @State, 
		[IsSubTask] = @IsSubTask, 
		[ParentTaskID] = @ParentTaskID, 
		[ProjectID] = @ProjectID
    WHERE ([TaskID] = @TaskID)

	IF(@IsSubTask = 1)
		EXEC [dbo].[SP_UPDATE_PARENT_TASK_STATE] @ParentTaskID = @ParentTaskID, @ProjectID = @ProjectID
	ELSE
		EXEC [dbo].[SP_UPDATE_PARENT_TASK_STATE] @ParentTaskID = 0, @ProjectID = @ProjectID
END TRY
BEGIN CATCH
	DECLARE @Message VARCHAR(MAX) = ERROR_MESSAGE(),
			@Severity INT = ERROR_SEVERITY(),
			@ErrorState SMALLINT = ERROR_STATE()

	RAISERROR(@Message, @Severity, @State)
END CATCH
GO
