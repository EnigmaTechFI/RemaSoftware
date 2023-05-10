using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }

        public DateTime DateIn { get; set; }
        
        public DateTime DateOut { get; set; }
        
        public int EmployeeID { get; set; }
        
        public Employee Employee { get; set; }
    }
}
