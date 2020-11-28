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
    class ProjectRepository: IDisposable
    {
        ApplicationDBContext _context = new ApplicationDBContext();

        public Project ProjectObj { get; set; } 

        public IEnumerable<Project> ProjectListObj { get; set; }

        public void GetProjectList()
        {
            ProjectListObj = _context.Projects.Where(m => m.IsSubProject == false).Include(k => k.Children).ToList();
        }

        public void GetProject(int id)
        {
            ProjectObj = _context.Projects.Where(m => m.ProjectID == id).Include(k => k.Children).FirstOrDefault();
        }

        public void Insert(Project objProject)
        {
            if (_context.Database.Exists())
            {
                _context.Projects.Add(objProject);
                _context.SaveChanges();
            }
        }

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

            _context.SaveChanges();
        }

        public void Delete(Project objProject)
        {
            IEnumerable<Project> objProjectDelete = _context.Projects.Where(x => x.ProjectID == objProject.ProjectID || 
                                                                            x.ParentProjectID == objProject.ProjectID);
            foreach(Project p in objProjectDelete)
                _context.Projects.Remove(p);

            _context.SaveChanges();
        }


        protected void Dispose(bool disposing)
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
