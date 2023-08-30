using System.ComponentModel.DataAnnotations;
using RemaSoftware.Domain.Constants;

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
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Mail { get; set; }
        
        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Number { get; set; }
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public DateTime BirthDate { get; set; }
        
        [MaxLength(50)]
        public string BirthPlace { get; set; }
        
        [MaxLength(25)]
        public string Task { get; set; }
        
        public int Level { get; set; }

        [MaxLength(25)]
        public string TypeRelationship { get; set; }
        
        [MaxLength(25)]
        public string TypePosition { get; set; }
        
        public DateTime StartRelationship { get; set; }
        
        public DateTime EndRelationship { get; set; }
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]

        public int NumberHour { get; set; }
        
        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Gender { get; set; }
        
        [MaxLength(20)]
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string TaxID { get; set; }
        
        [MaxLength(50)]
        public string FluidaId { get; set; }
        
        public string AccountId { get; set; }
        public bool Extraordinary { get; set; }

        public virtual List<Attendance> Attendances { get; set; }

    }
}
