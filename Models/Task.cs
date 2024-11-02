using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HouseworkManager.Models
{
    public class Task
    {
        [Key]
        public int TaskID { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Boolean Done { get; set; }

        // reffered to this web page https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many
        [Required]
        public int GroupID { get; set; } // Required foreign key property
        public Group Group { get; set; } = null!; // Required reference navigation to principal

        // reffered to this video https://www.youtube.com/watch?v=_l8pOjGeBZ8
        [Required]
        public string UserID { get; set; } // Required foreign key property
        public IdentityUser User { get; set; } = null!; // Required reference navigation to principal
    }
}
