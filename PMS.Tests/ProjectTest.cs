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
        [TestMethod]
        public void ProjectGetById()
        {
            // Set up Prerequisites   
            var controller = new ProjectController();
            var projectList = controller.GetProject(4);

            var contentResult = projectList as OkNegotiatedContentResult<Project>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(4, contentResult.Content.ProjectID);
        }

        [TestMethod]
        public void AddProjectTest()
        {
            // Arrange  
            var controller = new ProjectController();
            Project objProject = new Project
            {
                Code = "C:\\\\MyProjects\\ProjParent5",
                Name = "ProjParent5",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.Completed,
                IsSubProject = false,
                ParentProjectID = null
            };
            // Act  
            IHttpActionResult actionResult = controller.CreateNewProject(objProject);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Project>;
            // Assert  
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["ProjectID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["ProjectID"]);
        }

        [TestMethod]
        public void UpdateProjectTest()
        {
            // Arrange  
            var controller = new ProjectController();
            Project objProject = new Project
            {
                ProjectID = 36,
                Code = "C:\\\\MyProjects\\ProjParent3",
                Name = "ProjParent3",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.Completed,
                IsSubProject = true,
                ParentProjectID = 35
            };
            // Act  
            IHttpActionResult actionResult = controller.UpdateProject(objProject);

            var contentResult = actionResult as NegotiatedContentResult<Project>;
            // Assert  
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.Name, "ProjParent3");
        }

        [TestMethod]
        public void DeleteProjectTest()
        {
            var controller = new ProjectController();
            IHttpActionResult actionResult = controller.DeleteProject(35);
            var contentResult = actionResult as OkNegotiatedContentResult<Project>;        
            Assert.IsNotNull(contentResult);
        }

        [TestMethod]
        public void TaskGetById()
        {
            // Set up Prerequisites   
            var controller = new TaskController();
            var projectList = controller.GetTask(2);

            var contentResult = projectList as OkNegotiatedContentResult<Task>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.TaskID);
        }

        [TestMethod]
        public void AddTaskTest()
        {  
            var controller = new TaskController();
            Task objTask = new Task
            {
                Name = "Task3.2",
                Description = "This is task 3.2",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.inProgress,
                IsSubTask = true,
                ParentTaskID = 3,
                ProjectID = 36
            };
 
            IHttpActionResult actionResult = controller.CreateNewTask(objTask);

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Task>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.IsNotNull(createdResult.RouteValues["TaskID"]);
            Assert.AreNotEqual(0, createdResult.RouteValues["TaskID"]);
        }

        [TestMethod]
        public void UpdateTaskTest()
        { 
            var controller = new TaskController();
            Task objTask = new Task
            {
                TaskID = 9,
                Name = "Task3.1",
                Description = "This is task 3.1",
                StartDate = DateTime.Now.Date,
                FinishDate = DateTime.Now.Date,
                State = Common.State.Completed,
                IsSubTask = true,
                ParentTaskID = 3,
                ProjectID = 36
            };
 
            IHttpActionResult actionResult = controller.UpdateTask(objTask);
            var contentResult = actionResult as NegotiatedContentResult<Task>;
 
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.State, Common.State.Completed);
        }

        [TestMethod]
        public void DeleteTaskTest()
        {
            var controller = new TaskController();
            IHttpActionResult actionResult = controller.DeleteTask(10);
            var contentResult = actionResult as OkNegotiatedContentResult<Task>;
            Assert.IsNotNull(contentResult);
        }
    }
}
