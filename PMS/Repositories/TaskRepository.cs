using PMS.Context;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PMS.Repositories
{
    public class TaskRepository
    {
        private ApplicationDBContext _context;

        public TaskRepository()
        {
            _context = new ApplicationDBContext();

        }

        /// <summary>
        /// Function to fetch a list of tasks (all the tasks)
        /// </summary>
        public IEnumerable<Task> GetTaskList()
        {
            return _context.Tasks.Where(m => m.IsSubTask == false).Include(k => k.Children).ToList();
        }

        /// <summary>
        /// Function to Fetch a single task
        /// </summary>
        /// <param name="id">ID of the Task that has to be fetched</param>
        public Task GetTask(int id)
        {
            return _context.Tasks.Where(m => m.TaskID == id).Include(k => k.Children).FirstOrDefault();
        }

        /// <summary>
        /// Function to insert task
        /// </summary>
        /// <param name="objTask">Object of Task which is to be inserted</param>
        public void Insert(Task objTask)
        {
            if (_context.Database.Exists())
            {
                _context.Tasks.Add(objTask);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Function to update task
        /// </summary>
        /// <param name="objTask">Object of Task which is to be updated</param>
        public void Update(Task objTask)
        {
            Task objTaskUpdate = _context.Tasks.FirstOrDefault(x => x.TaskID == objTask.TaskID);

            objTaskUpdate.Name = objTask.Name;
            objTaskUpdate.Description = objTask.Description;
            objTaskUpdate.StartDate = objTask.StartDate;
            objTaskUpdate.FinishDate = objTask.FinishDate;
            objTaskUpdate.State = objTask.State;
            objTaskUpdate.IsSubTask = objTask.IsSubTask;
            objTaskUpdate.ParentTaskID = objTask.ParentTaskID;
            objTaskUpdate.ProjectID = objTask.ProjectID;

            _context.SaveChanges();
        }

        /// <summary>
        /// Function to delete task and its sub task
        /// </summary>
        /// <param name="objTask">Object of Task which is to be deleted</param>
        public void Delete(Task objTask)
        {
            IEnumerable<Task> objTaskDelete = _context.Tasks.Where(x => x.TaskID == objTask.TaskID ||
                                                                   x.ParentTaskID == objTask.TaskID);
            foreach (Task p in objTaskDelete)
                _context.Tasks.Remove(p);

            _context.SaveChanges();
        }

    }
}