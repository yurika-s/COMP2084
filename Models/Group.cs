using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HouseworkManager.Models
{
    public class Group
    {
        [Key]
        public int GroupID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DisplayName("Administrator")]
        public string AdministratorID { get; set; }

        // reffered to this web page https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many
        // Collection navigation containing dependents
        public ICollection<GroupMember> GroupMembers { get; } = new List<GroupMember>();
        public ICollection<Task> Tasks { get; } = new List<Task>();
    }
}
