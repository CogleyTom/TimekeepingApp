using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimekeepingApp.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        public string Role { get; set; } // "IC" for Individual Contributor, "Manager" for Manager

        // Navigation property for related timesheets
        public ICollection<TimesheetModel> Timesheets { get; set; }
    }
}
