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

        ~TaskController()
        {
            objTaskRes.Dispose();
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<Task>))]
        [Route("api/GetTask")]
        public IHttpActionResult GetTaskList()
        {
            objTaskRes.GetTaskList();
            return Ok(objTaskRes.TaskListObj);
        }

        [HttpGet]
        [ResponseType(typeof(Task))]
        [Route("api/GetTask/{id}")]
        public IHttpActionResult GetTask(int id)
        {
            objTaskRes.GetTask(id);

            if (objTaskRes.TaskObj == null)
            {
                return NotFound();
            }

            return Ok(objTaskRes.TaskObj);
        }

        [HttpPost]
        [Route("api/CreateNewTask")]
        public IHttpActionResult CreateNewTask(Task objTask)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            objTaskRes.Insert(objTask);

            return CreatedAtRoute("DefaultApi", new
            {
                TaskID = objTask.TaskID
            }, objTask);
        }

        [HttpPut]
        [Route("api/UpdateTask")]
        public IHttpActionResult UpdateTask(Task objTask)
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
 
        [HttpDelete]
        [ResponseType(typeof(Task))]
        [Route("api/DeleteTask/{id}")]
        public IHttpActionResult DeleteTask(int id)
        {
            objTaskRes.GetTask(id);

            if (objTaskRes.TaskObj == null)
            {
                return NotFound();
            }

            objTaskRes.Delete(objTaskRes.TaskObj);

            return Ok(objTaskRes.TaskObj);
        }
    }
}