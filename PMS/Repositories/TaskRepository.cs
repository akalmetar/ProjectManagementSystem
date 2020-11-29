using PMS.Context;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PMS.Repositories
{
    public class TaskRepository : IDisposable
    {
        private ApplicationDBContext _context = new ApplicationDBContext();

        public Task TaskObj { get; set; }

        public IEnumerable<Task> TaskListObj { get; set; }

        public void GetTaskList()
        {
            TaskListObj = _context.Tasks.Where(m => m.IsSubTask == false).Include(k => k.Children).ToList();
        }

        public void GetTask(int id)
        {
            TaskObj = _context.Tasks.Where(m => m.TaskID == id).Include(k => k.Children).FirstOrDefault();
        }

        public void Insert(Task objTask)
        {
            if (_context.Database.Exists())
            {
                _context.Tasks.Add(objTask);
                _context.SaveChanges();
            }
        }

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

        public void Delete(Task objTask)
        {
            IEnumerable<Task> objTaskDelete = _context.Tasks.Where(x => x.TaskID == objTask.TaskID ||
                                                                   x.ParentTaskID == objTask.TaskID);
            foreach (Task p in objTaskDelete)
                _context.Tasks.Remove(p);

            _context.SaveChanges();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}