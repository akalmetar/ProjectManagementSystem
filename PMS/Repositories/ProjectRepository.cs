using PMS.Context;
using PMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Repositories
{
    public class ProjectRepository
    {
        private ApplicationDBContext _context;

        public ProjectRepository()
        {
            _context = new ApplicationDBContext();

        }

        /// <summary>
        /// Function to fetch a list of project (all the projects)
        /// </summary>
        public IEnumerable<Project> GetProjectList()
        {
            return _context.Projects.Where(m => m.IsSubProject == false)
                           .Include(k => k.Children).ToList();
        }

        /// <summary>
        /// Function to Fetch a single project
        /// </summary>
        /// <param name="id">ID of the project that has to be fetched</param>
        public Project GetProject(int id)
        {
            return _context.Projects.Where(m => m.ProjectID == id)
                           .Include(k => k.Children).FirstOrDefault();
        }

        /// <summary>
        /// Function to Insert project
        /// </summary>
        /// <param name="objProject">Object of project which is to be inserted</param>
        public void Insert(Project objProject)
        {
            if (_context.Database.Exists())
            {
                _context.Projects.Add(objProject);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Function to update project
        /// </summary>
        /// <param name="objProject">Object of project which is to be updated</param>
        public void Update(Project objProject)
        {
            Project objProjectUpdate = _context.Projects.FirstOrDefault(x => x.ProjectID == objProject.ProjectID);

            objProjectUpdate.Code = objProject.Code;
            objProjectUpdate.Name = objProject.Name;
            objProjectUpdate.StartDate = objProject.StartDate;
            objProjectUpdate.FinishDate = objProject.FinishDate;
            objProjectUpdate.State = objProject.State;
            objProjectUpdate.IsSubProject = objProject.IsSubProject;
            objProjectUpdate.ParentProjectID = objProject.ParentProjectID;

            _context.ChangeTracker.DetectChanges();
            _context.SaveChanges();
        }

        /// <summary>
        /// Function to delete project and its child elements (sub project, task and subtask)
        /// </summary>
        /// <param name="objProject">Object of project which is to be deleted</param>
        public void Delete(Project objProject)
        {
            IEnumerable<Models.Task> objTaskDelete = _context.Tasks.Where(x => x.ProjectID == objProject.ProjectID);
            foreach (Models.Task p in objTaskDelete)
                _context.Tasks.Remove(p);
            _context.SaveChanges();


            IEnumerable<Project> objProjectDelete = _context.Projects.Where(x => x.ProjectID == objProject.ProjectID ||
                                                                            x.ParentProjectID == objProject.ProjectID);
            foreach (Project p in objProjectDelete)
                _context.Projects.Remove(p);
            _context.SaveChanges();
        }

    }
}
