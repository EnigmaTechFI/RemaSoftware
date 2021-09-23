using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Client
    {
        public int ClientID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Name { get; set; }

        public char? P_Iva { get; set; }

        public char? Street { get; set; }

        public char? StreetNumber { get; set; }

        public int? Cap { get; set; }

        public string? City { get; set; }
        
        public string? State { get; set; }
    
        public virtual ICollection<Order> Orders { get; set; } 
    }
}
