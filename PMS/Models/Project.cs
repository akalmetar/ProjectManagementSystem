using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PMS.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectID { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        [Required]
        public Common.State State { get; set; }

        [Required]
        public bool IsSubProject { get; set; }

        public int? ParentProjectID { get; set; }

        public virtual Project Parent { get; set; }

        public virtual ICollection<Project> Children { get; set; }

    }
}