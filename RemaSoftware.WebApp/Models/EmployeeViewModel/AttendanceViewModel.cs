using System;
using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.EmployeeViewModel
{
    public class AttendanceViewModel
    {
        public List<Attendance> Attendances { get; set; }
        
        public List<Employee> Employees { get; set; }
        
        public int Month { get; set; }
        
        public int Year { get; set; }
    }
}
