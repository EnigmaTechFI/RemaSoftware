using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public List<Employee> GetAllEmployees()
        {
            return _dbContext.Employees.ToList();
        }
        
        public Employee NewEmployee(Employee employee)
        {
            _dbContext.Add(employee);
            _dbContext.SaveChanges();
            return employee;
        }

        public void DeleteEmployeeById(Employee employee)
        {
            var existingEmployee = _dbContext.Employees.Find(employee.EmployeeID);
            
            var employeeUser = _dbContext.Users.SingleOrDefault(i => i.Id == existingEmployee.AccountId);
            if (existingEmployee != null)
            {
                if (employeeUser != null)
                { 
                    _dbContext.Users.Remove(employeeUser);
                }
                _dbContext.Employees.Remove(existingEmployee);
                _dbContext.SaveChanges();
            }
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return _dbContext.Employees.SingleOrDefault(sd => sd.EmployeeID == employeeId);
        }

        public List<Attendance> GetAllAttendance(int month, int year)
        {
            return _dbContext.Attendances
                .Include(t => t.Employee)
                .Where(i => i.DateIn.Month == month && i.DateIn.Year == year)
                .OrderBy(i => i.EmployeeID)
                .ThenBy(i => i.DateIn)
                .ToList();
        }
        
        public List<Attendance> GetAllAttendance(int year)
        {
            return _dbContext.Attendances
                .Include(t => t.Employee)
                .Where(i => i.DateIn.Year == year)
                .OrderBy(i => i.EmployeeID)
                .ThenBy(i => i.DateIn)
                .ToList();
        }
        
        public List<Employee> GetEmployeesWithoutAttendances(int mouth, int year)
        {
            return _dbContext.Employees
                .Where(e => !_dbContext.Attendances.Any(a => a.EmployeeID == e.EmployeeID && a.DateIn.Month == mouth && a.DateIn.Year == year))
                .ToList();
        }
        
        public List<Employee> GetEmployeesWithoutAttendances(int year)
        {
            return _dbContext.Employees
                .Where(e => !_dbContext.Attendances.Any(a => a.EmployeeID == e.EmployeeID && a.DateIn.Year == year))
                .ToList();
        }
        
        public async Task UpdateEmployee(Employee employee)
        {
            var existingEmployee = _dbContext.Employees.Find(employee.EmployeeID);
            if (employee.Mail == null)
            {
                employee.Mail = "";
            }
            if (existingEmployee != null)
            {
                _dbContext.Entry(existingEmployee).CurrentValues.SetValues(employee);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}