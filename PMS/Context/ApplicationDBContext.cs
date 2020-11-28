﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PMS.Models;

namespace PMS.Context
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext() : base("name=DefaultConnection")
        {

        }

        public DbSet<Project> Projects { get; set; } // My domain models
        public DbSet<Task> Tasks { get; set; }// My domain models

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().MapToStoredProcedures
            (
               s => s.Insert(i => i.HasName("[dbo].[SP_INSERT_PROJECT]"))
                     .Update(u => u.HasName("[dbo].[SP_UPDATE_PROJECT]"))
                     .Delete(d => d.HasName("[dbo].[SP_DELETE_PROJECT]"))
            );
        }

    }
}