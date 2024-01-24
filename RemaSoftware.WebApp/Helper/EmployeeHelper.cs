using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.Models.EmployeeViewModel;


namespace RemaSoftware.WebApp.Helper;

public class EmployeeHelper
{
    private readonly IEmployeeService _employeeService;
    private readonly IAttendanceService _attendanceService;
    private readonly AccountHelper _accountHelper;
    private readonly ApplicationDbContext _dbContext;

    public EmployeeHelper(IEmployeeService employeeService, IAttendanceService attendanceService,
        AccountHelper accountHelper, ApplicationDbContext dbContext)
    {
        _employeeService = employeeService;
        _attendanceService = attendanceService;
        _accountHelper = accountHelper;
        _dbContext = dbContext;
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
        Employee employee = _employeeService.GetEmployeeById(employeeId);
        _employeeService.DeleteEmployeeById(employee);
    }

    public EmployeeViewModel GetEmployeeById(int id, int month, int year)
    {
        var m = month;
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
            Month = m,
            Year = y
        };
    }
    
    public AttendanceViewModel GetAttendanceViewModel(int month, int year)
    {
        var m = month;
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
            Month = m,
            Year = y
        };
    }
    
    public int GetEmployeeId(string userId)
    {
        List<Employee> employees = _employeeService.GetAllEmployees();
        foreach (var employee in employees)
        {
            if (employee.AccountId == userId)
                return employee.EmployeeID;
        }
        return 0;
    }
    
    
    public AttendanceEmployeeViewModel GetAttendanceEmployeeViewModel(int month, int year, int employeeId)
    {
        var m = month;
        var y = year;
        if (year == 0)
        {
            m = DateTime.Today.Month;
            y = DateTime.Today.Year;
        }

        return new AttendanceEmployeeViewModel
        {
            Attendances = GetAllAttendanceEmployee(m, y, employeeId),
            Employee = _employeeService.GetEmployeeById(employeeId),
            Month = m,
            Year = y
        };
    }
    
    public List<Attendance> GetAllAttendance(int month, int year)
    {
        return _employeeService.GetAllAttendance(month, year);
    }
    
    public List<Attendance> GetAllAttendanceEmployee(int month, int year, int employeeId)
    {
        return _employeeService.GetAllAttendance(month, year).Where(a => a.EmployeeID == employeeId).ToList();
    }
    
    public async Task<string> EditEmployee(EmployeeViewModel model)
    {
        Employee employee = _employeeService.GetEmployeeById(model.Employee.EmployeeID);
        
        if(employee.Mail != model.Employee.Mail && employee.Mail.Length != 0)
        {
            await _accountHelper.DeleteAccountByID(employee.AccountId);
            MyUser myUser = await _accountHelper.AddEmployeeAccount(model);
            employee.AccountId = myUser.Id;
            employee.Mail = model.Employee.Mail;
        }
        else if (model.Employee.Mail != null && employee.Mail.Length == 0)
        {
            MyUser myUser = await _accountHelper.AddEmployeeAccount(model);
            employee.AccountId = myUser.Id;
            employee.Mail = model.Employee.Mail;
        }

        await _employeeService.UpdateEmployee(employee);
        
        return "Success";
    }

}
