using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.EmployeeViewModel;


namespace RemaSoftware.WebApp.Helper;

public class EmployeeHelper
{
    private readonly IEmployeeService _employeeService;
    private readonly IAttendanceService _attendanceService;

    public EmployeeHelper(IEmployeeService employeeService, IAttendanceService attendanceService)
    {
        _employeeService = employeeService;
        _attendanceService = attendanceService;

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
    
    public EmployeeViewModel GetEmployeeById(int id, int mouth, int year)
    {
        var m = mouth;
        var y = year;
        if (year == 0)
        {
            m = DateTime.Today.Month;
            y = DateTime.Today.Year;
        }
        return new EmployeeViewModel()
        {
            Employee = _employeeService.GetEmployeeById(id),
            Attendances = _attendanceService.getAttendanceById(id, m, y),
            Mouth = m,
            Year = y
        };
    }
    
    public AttendanceViewModel GetAttendanceViewModel(int mouth, int year)
    {
        var m = mouth;
        var y = year;
        if (year == 0)
        {
            m = DateTime.Today.Month;
            y = DateTime.Today.Year;
        }

        return new AttendanceViewModel
        {
            Attendances = GetAllAttendance(m, y),
            Employees = _employeeService.GetEmployeesWithoutAttendances(m, y),
            Mouth = m,
            Year = y
        };
    }
    
    public List<Attendance> GetAllAttendance(int mouth, int year)
    {
        return _employeeService.GetAllAttendance(mouth, year);
    }

    public async Task<string> EditEmployee(EmployeeViewModel model)
    {
        _employeeService.UpdateEmployee(model.Employee);
        return "Success";
    }
}
