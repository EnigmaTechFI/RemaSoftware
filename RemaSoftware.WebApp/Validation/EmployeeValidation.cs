using System;
using RemaSoftware.WebApp.Models.EmployeeViewModel;
using RemaSoftware.WebApp.Models.StockViewModel;

namespace RemaSoftware.WebApp.Validation
{
    public class EmployeeValidation
    {
        public string ValidateEmployee(EmployeeViewModel employee)
        {

            if (string.IsNullOrEmpty(employee.Employee.Name))
                return "Inserire nome.";
            if (string.IsNullOrEmpty(employee.Employee.Surname))
                return "Inserire cognome.";
            if (string.IsNullOrEmpty(employee.Employee.number))
                return "Inserire matricola.";
            if (string.IsNullOrEmpty(employee.Employee.Gender))
                return "Inserire sesso.";
            if (string.IsNullOrEmpty(employee.Employee.TaxID))
                return "Inserire codice fiscale.";
            if (employee.Employee.BirthDate > DateTime.Now.Date)
                return "Inserire data di nascita.";
            return "";
        }
    }
}
