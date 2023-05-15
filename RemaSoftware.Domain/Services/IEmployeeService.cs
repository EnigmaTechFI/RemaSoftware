using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee NewEmployee(Employee employee);
        bool DeleteEmployeeById(int employeeId);
        Employee GetEmployeeById(int employeeId);
        List<Attendance> GetAllAttendance();
        bool UpdateEmployee(Employee employee);

    }
}