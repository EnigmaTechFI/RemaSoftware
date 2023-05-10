using System.Collections.Generic;
using System.Threading.Tasks;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.EmployeeViewModel;


namespace RemaSoftware.WebApp.Helper;

public class EmployeeHelper
{
    private readonly IEmployeeService _employeeService;

    public EmployeeHelper(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
       
    }
    public EmployeeViewModel NewEmployee()
    {
        return new EmployeeViewModel();
    }
    
    public async Task<Employee> NewEmployee(Employee model)
    {
        return _employeeService.NewEmployee(model);
    }
    
    public EmployeeListViewModel GetEmployeeListViewModel()
    {
        return new EmployeeListViewModel
        {
            Employees = GetAllEmployees()
        };
    }
    
    public List<Employee> GetAllEmployees()
    {
        return _employeeService.GetAllEmployees();
    }
    
    public void DeleteEmployee(int employeeId)
    {
        _employeeService.DeleteEmployeeById(employeeId);
    }
    
    public EmployeeViewModel GetEmployeeById(int id)
    {
        return new EmployeeViewModel()
        {
            Employee = _employeeService.GetEmployeeById(id)
        };
    }
    
    public AttendanceViewModel GetAttendanceViewModel()
    {
        return new AttendanceViewModel
        {
            Attendances = GetAllAttendance()
        };
    }
    
    public List<Attendance> GetAllAttendance()
    {
        return _employeeService.GetAllAttendance();
    }

}
