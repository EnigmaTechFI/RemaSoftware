using RemaSoftware.Domain.Models;
using Attendance = RemaSoftware.Domain.Migrations.Attendance;

namespace RemaSoftware.Domain.Services
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
        Employee NewEmployee(Employee employee);
        void DeleteEmployeeById(Employee employee);
        Employee GetEmployeeById(int employeeId);
        List<Models.Attendance> GetAllAttendance(int mouth, int year);
        bool UpdateEmployee(Employee employee);
        public List<Employee> GetEmployeesWithoutAttendances(int mouth, int year);

    }
}