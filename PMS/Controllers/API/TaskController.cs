using PMS.Models;
using PMS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PMS.Controllers.API
{
    public class TaskController : ApiController
    {
        private TaskRepository objTaskRes = new TaskRepository();

        public TaskController()
        {
            objTaskRes = new TaskRepository();
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<Task>))]
        [Route("api/GetTask")]
        public IHttpActionResult GetTaskList()
        {
            try
            {
                return Ok(objTaskRes.GetTaskList());
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while fetching task details.", e);
            }
        }

        [HttpGet]
        [ResponseType(typeof(Task))]
        [Route("api/GetTask/{id}")]
        public IHttpActionResult GetTask(int id)
        {
            try
            {
                var ObjTask = objTaskRes.GetTask(id);

                if (ObjTask == null)
                {
                    return NotFound();
                }

                return Ok(ObjTask);
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while fetching a task. ID = " + id, e);
            }
        }

        [HttpPost]
        [Route("api/CreateNewTask")]
        public IHttpActionResult CreateNewTask(Task objTask)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");

                objTaskRes.Insert(objTask);

                return CreatedAtRoute("DefaultApi", new
                {
                    TaskID = objTask.TaskID
                }, objTask);
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while creating a new task.", e);
            }
        }

        [HttpPut]
        [Route("api/UpdateTask")]
        public IHttpActionResult UpdateTask(Task objTask)
        {
            try
            {
                if (objTask != null)
                {
                    if (!ModelState.IsValid)
                        return BadRequest("Invalid data.");
                    else
                    {
                        objTaskRes.Update(objTask);

                        return Content(HttpStatusCode.Accepted, objTask);
                    }
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while updating the task details. ID = " + objTask.TaskID, e);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(Task))]
        [Route("api/DeleteTask/{id}")]
        public IHttpActionResult DeleteTask(int id)
        {
            try
            {
                var ObjTask = objTaskRes.GetTask(id);

                if (ObjTask == null)
                {
                    return NotFound();
                }

                objTaskRes.Delete(ObjTask);

                return Ok(ObjTask);
            }
            catch (Exception e)
            {
                //Log error
                throw new Exception("Error Occured while deteting a task. ID = " + id, e);
            }
        }
    }
}