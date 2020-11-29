using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Models
{
    public class Task
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        [Required]
        public Common.State State { get; set; }

        [Required]
        public bool IsSubTask { get; set; }

        public int? ParentTaskID { get; set; }

        public virtual Task Parent { get; set; }

        public virtual ICollection<Task> Children { get; set; }

        [Required]
        public int ProjectID { get; set; }

        public Project Project { get; set; }
    }
}
