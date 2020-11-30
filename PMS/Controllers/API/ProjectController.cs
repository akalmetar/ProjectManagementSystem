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
        private ProjectRepository objProjRes;

        public ProjectController()
        {
            objProjRes = new ProjectRepository();
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<Project>))]
        [Route("api/GetProject")]
        public IHttpActionResult GetProjectList()
        {
            try
            {
                return Ok(objProjRes.GetProjectList());
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while fetching project details.", e);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Project))]
        [Route("api/GetProject/{id}")]
        public IHttpActionResult GetProject(int id)
        {
            try
            {
                var ObjProj = objProjRes.GetProject(id);

                if (ObjProj == null)
                {
                    return NotFound();
                }

                return Ok(ObjProj);
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while fetching a project. ID = " + id, e);
            }
        }

        [HttpPost]
        [Route("api/CreateNewProject")]
        public IHttpActionResult CreateNewProject(Project objProj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");

                objProjRes.Insert(objProj);

                return CreatedAtRoute("DefaultApi", new
                {
                    ProjectID = objProj.ProjectID
                }, objProj);
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while creating a new project.", e);
            }
        }

        [HttpPut]
        [Route("api/UpdateProject")]
        public IHttpActionResult UpdateProject(Project objProj)
        {
            try
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
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while updating the project details. ID = " + objProj.ProjectID, e);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(Project))]
        [Route("api/DeleteProject/{id}")]
        public IHttpActionResult DeleteProject(int id)
        {
            try
            {
                var ObjProj = objProjRes.GetProject(id);

                if (ObjProj == null)
                {
                    return NotFound();
                }

                objProjRes.Delete(ObjProj);

                return Ok(ObjProj);
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while deteting a project. ID = " + id, e);
            }
        }

    }
}