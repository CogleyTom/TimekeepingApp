using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimekeepingApp.Models
{
    public class TimesheetModel
    {
        [Key]
        public int TimesheetId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan TimeIn { get; set; }

        [Required]
        public TimeSpan TimeOut { get; set; }

        public string Status { get; set; } // e.g., "Pending", "Approved", "Rejected"

        // Foreign Key linking to the UserModel
        public int UserId { get; set; }

        // Navigation property to the user
        [ForeignKey("UserId")]
        public UserModel User { get; set; }
    }
}
