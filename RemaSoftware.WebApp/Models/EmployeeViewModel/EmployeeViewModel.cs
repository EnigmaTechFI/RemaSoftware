using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.EmployeeViewModel
{
    public class EmployeeViewModel
    {
        
        public Employee Employee { get; set; }
        public List<Attendance> Attendances { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        
    }
}
