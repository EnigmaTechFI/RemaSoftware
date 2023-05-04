using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IEmployeeService
    {
        List<Employee> GetAllEmployees();
    }
}