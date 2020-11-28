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
                Code = "C:\\\\MyProjects\\ProjParent3\\SubProj1",
                Name = "SubProj2",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = (int)Common.State.Planned,
                IsSubProject = true,
                ParentProjectID = 10
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
                ProjectID = 10,
                Code = "C:\\\\MyProjects\\ProjParent3",
                Name = "ProjParent3",
                StartDate = DateTime.Now.Date,
                FinishDate = null,
                State = Common.State.Planned,
                IsSubProject = false,
                ParentProjectID = null
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
            IHttpActionResult actionResult = controller.DeleteProject(10);
            var contentResult = actionResult as OkNegotiatedContentResult<Project>;        
            Assert.IsNotNull(contentResult);
        }
    }
}
