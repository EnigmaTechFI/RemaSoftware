using RemaSoftware.Domain.Services.Impl;
using RemaSoftware.WebApp.Models.EmployeeViewModel;


namespace RemaSoftware.WebApp.Helper;

public class EmployeeHelper
{
    public EmployeeViewModel NewEmployee()
    {
        return new EmployeeViewModel();
    }
    
}
