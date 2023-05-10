using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(20)]
        public string Name { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Surname { get; set; }
        
        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string number { get; set; }
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public DateTime BirthDate { get; set; }
        
        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Gender { get; set; }
        
        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string TaxID { get; set; }
        
        public virtual List<Attendance> Attendances { get; set; }

    }
}
