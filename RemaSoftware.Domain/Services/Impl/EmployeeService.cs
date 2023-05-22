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
        
        public bool DeleteEmployeeById(int employeeId)
        {
            _dbContext.Employees.Remove(new Employee {EmployeeID = employeeId});
            _dbContext.SaveChanges();
            return true;
        }
        
        public Employee GetEmployeeById(int employeeId)
        {
            return _dbContext.Employees.SingleOrDefault(sd => sd.EmployeeID == employeeId);
        }

        public List<Attendance> GetAllAttendance(int mouth, int year)
        {
            return _dbContext.Attendances.Include(t => t.Employee).Where(i => i.DateIn.Month == mouth && i.DateIn.Year == year).ToList();
        }
        
        public List<Employee> GetEmployeesWithoutAttendances(int mouth, int year)
        {
            return _dbContext.Employees
                .Where(e => !_dbContext.Attendances.Any(a => a.EmployeeID == e.EmployeeID && a.DateIn.Month == mouth && a.DateIn.Year == year))
                .ToList();

        }
        
        public bool UpdateEmployee(Employee employee)
        {
            _dbContext.Employees.Update(employee);
            _dbContext.SaveChanges();
            return true;
        }
        
    }
}