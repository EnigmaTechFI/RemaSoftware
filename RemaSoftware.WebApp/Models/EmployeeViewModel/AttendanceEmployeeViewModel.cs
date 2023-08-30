using System;
using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.EmployeeViewModel
{
    public class AttendanceEmployeeViewModel
    {
        public List<Attendance> Attendances { get; set; }
        
        public Employee Employee { get; set; }
        
        public int Month { get; set; }
        
        public int Year { get; set; }
    }
}
