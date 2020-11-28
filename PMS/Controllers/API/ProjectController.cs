using PMS.Context;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using PMS.Repositories;
using System.Web.Http.Description;

namespace PMS.Controllers.API
{
    public class ProjectController : ApiController
    {
        private ProjectRepository objProjRes = new ProjectRepository();

        public ProjectController()
        {
            objProjRes = new ProjectRepository();
        }

        ~ProjectController()
        {
            objProjRes.Dispose();
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<Project>))]
        [Route("api/GetProject")]
        public IHttpActionResult GetProjectList()
        {
            objProjRes.GetProjectList();
            return Ok(objProjRes.ProjectListObj);
        }

        [HttpGet]
        [ResponseType(typeof(Project))]
        [Route("api/GetProject/{id}")]
        public IHttpActionResult GetProject(int id)
        {
            objProjRes.GetProject(id);

            if (objProjRes.ProjectObj == null)
            {
                return NotFound();
            }

            return Ok(objProjRes.ProjectObj);
        }

        [HttpPost]
        [Route("api/CreateNewProject")]
        public IHttpActionResult CreateNewProject(Project objProj)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            objProjRes.Insert(objProj);

            return CreatedAtRoute("DefaultApi", new
            {
                ProjectID = objProj.ProjectID
            }, objProj);
        }

        [HttpPut]
        [Route("api/UpdateProject")]
        public IHttpActionResult UpdateProject(Project objProj)
        {
            if (objProj != null)
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                else
                {
                    objProjRes.Update(objProj);

                    return Content(HttpStatusCode.Accepted, objProj);
                }
            }

            return BadRequest();
        }

        // DELETE: api/Students/5  
        [HttpDelete]
        [ResponseType(typeof(Project))]
        [Route("api/DeleteProject/{id}")]
        public IHttpActionResult DeleteProject(int id)
        {
            objProjRes.GetProject(id);
            
            if (objProjRes.ProjectObj == null)
            {
                return NotFound();
            }

            objProjRes.Delete(objProjRes.ProjectObj);
 
            return Ok(objProjRes.ProjectObj);
        }

    }
}