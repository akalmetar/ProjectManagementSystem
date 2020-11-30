using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PMS.Controllers.API;
using System.Net;
using System.Web.Http;
using PMS.Models;
using System.Web.Http.Results;

namespace PMS.Tests
{
    [TestClass]
    public class ProjectTest
    {
        #region TestCasesForInsertOperation

        [TestMethod]
        public void AddInitialProjectTest()
        {
            var controller = new ProjectController();
            Project objProject = new Project
            {
                Code = "C:\\\\MyProjects\\ProjParent1",
                Name = "ProjParent1",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.Planned,
                IsSubProject = false,
                ParentProjectID = null
            };

            IHttpActionResult actionResult = controller.CreateNewProject(objProject);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Project>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["ProjectID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["ProjectID"]);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void AddSubProjectWithouParentProjectIDTest()
        {
            var controller = new ProjectController();
            Project objProject = new Project
            {
                Code = "C:\\\\MyProjects\\ProjParent1\\SubProject1",
                Name = "SubProject1",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.Planned,
                IsSubProject = true,
                ParentProjectID = null
            };

            IHttpActionResult actionResult = controller.CreateNewProject(objProject);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Project>;

        }

        [TestMethod]
        public void AddSubProjectWithParentProjectIDTest()
        {
            var controller = new ProjectController();
            Project objProject = new Project
            {
                Code = "C:\\\\MyProjects\\ProjParent1\\SubProject1",
                Name = "SubProject1",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.Planned,
                IsSubProject = true,
                ParentProjectID = 1
            };

            IHttpActionResult actionResult = controller.CreateNewProject(objProject);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Project>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["ProjectID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["ProjectID"]);
        }

        [TestMethod]
        public void AddInitialTaskTest()
        {
            var controller = new TaskController();
            Task objTask = new Task
            {
                Name = "Task1",
                Description = "This is task 1",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.inProgress,
                IsSubTask = false,
                ParentTaskID = null,
                ProjectID = 1
            };

            IHttpActionResult actionResult = controller.CreateNewTask(objTask);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Task>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["TaskID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["TaskID"]);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void AddTaskWithoutProjectIDTest()
        {
            var controller = new TaskController();
            Task objTask = new Task
            {
                Name = "Task2",
                Description = "This is task 2",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.inProgress,
                IsSubTask = false,
                ParentTaskID = null,
                ProjectID = 0
                //Cannot accept null as we have defined this parameter as mandatory
            };

            IHttpActionResult actionResult = controller.CreateNewTask(objTask);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Task>;
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void AddSubTaskWithoutParentTaskIDTest()
        {
            var controller = new TaskController();
            Task objTask = new Task
            {
                Name = "Task2",
                Description = "This is task 2",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.inProgress,
                IsSubTask = true,
                ParentTaskID = null,
                ProjectID = 1
            };

            IHttpActionResult actionResult = controller.CreateNewTask(objTask);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Task>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["TaskID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["TaskID"]);
        }

        [TestMethod]
        public void AddSubTaskWithParentTaskIDTest()
        {
            var controller = new TaskController();
            Task objTask = new Task
            {
                Name = "SubTask1",
                Description = "This is sub task 1",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.inProgress,
                IsSubTask = true,
                ParentTaskID = 1,
                ProjectID = 1
            };

            IHttpActionResult actionResult = controller.CreateNewTask(objTask);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Task>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["TaskID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["TaskID"]);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception))]
        public void AddSubTaskWithDifferentProjectIDforParentTaskIDwithDifferentProjectIDTest()
        {
            var controller = new TaskController();
            Task objTask = new Task
            {
                Name = "SubTask1",
                Description = "This is sub task 1",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.inProgress,
                IsSubTask = true,
                ParentTaskID = 1,
                ProjectID = 2

                //system should throw an error
                //because Parent Task Project is different and Sub Task project is different
                //cannot insert sub task 
            };

            IHttpActionResult actionResult = controller.CreateNewTask(objTask);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Task>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["TaskID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["TaskID"]);
        }


        #endregion

        #region TestCasesForFetchingRecords

        [TestMethod]
        public void ProjectGetById()
        {
            var controller = new ProjectController();
            var projectList = controller.GetProject(4);

            var contentResult = projectList as OkNegotiatedContentResult<Project>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(4, contentResult.Content.ProjectID);
        }

        [TestMethod]
        public void TaskGetById()
        {
            var controller = new TaskController();
            var projectList = controller.GetTask(2);

            var contentResult = projectList as OkNegotiatedContentResult<Task>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.TaskID);
        }

        #endregion

        #region TestCasesForUpdateOperation

        [TestMethod]
        public void UpdateProjectWithoutSubprojectASStateEqualtoCompletedTest()
        {
            var controller = new ProjectController();
            Project objProject = new Project
            {
                ProjectID = 2,
                Code = "C:\\\\MyProjects\\ProjParent1\\SubProject1",
                Name = "SubProject1",
                StartDate = DateTime.Now.Date,
                FinishDate = DateTime.Now.Date,
                State = Common.State.Completed,
                IsSubProject = true,
                ParentProjectID = 1
            };

            IHttpActionResult actionResult = controller.UpdateProject(objProject);
            var contentResult = actionResult as NegotiatedContentResult<Project>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.State, Common.State.Completed);
        }


        [TestMethod]
        public void UpdateTaskTest()
        {
            var controller = new TaskController();
            Task objTask = new Task
            {
                TaskID = 2,
                Name = "SubTask1",
                Description = "This is sub task 1",
                StartDate = DateTime.Now.Date,
                FinishDate = DateTime.Now.Date,
                State = Common.State.Completed,
                IsSubTask = true,
                ParentTaskID = 1,
                ProjectID = 1
            };

            IHttpActionResult actionResult = controller.UpdateTask(objTask);
            var contentResult = actionResult as NegotiatedContentResult<Task>;

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.State, Common.State.Completed);
        }

        #endregion

        #region TestCasesForDeleteOperation

        [TestMethod]
        public void DeleteProjectTest()
        {
            var controller = new ProjectController();
            IHttpActionResult actionResult = controller.DeleteProject(1);
            var contentResult = actionResult as OkNegotiatedContentResult<Project>;
            Assert.IsNotNull(contentResult);
        }

        [TestMethod]
        public void DeleteTaskTest()
        {
            var controller = new TaskController();
            IHttpActionResult actionResult = controller.DeleteTask(2);
            var contentResult = actionResult as OkNegotiatedContentResult<Task>;
            Assert.IsNotNull(contentResult);
        }


        #endregion

        
    }
}
