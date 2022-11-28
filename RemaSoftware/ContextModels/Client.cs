using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Client
    {
        public int ClientID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Name { get; set; }
        [MaxLength(20)]
        public string P_Iva { get; set; }
        [MaxLength(200)]
        public string Street { get; set; }
        [MaxLength(10)]
        public string StreetNumber { get; set; }
        [MaxLength(6)]
        public string Cap { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(20)]
        public string Province { get; set; }
        [MaxLength(50)]
        public string Nation { get; set; }
        [MaxLength(5)]
        public string Nation_ISO { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [MaxLength(15)]
        public string Fax { get; set; }
        public int FC_ClientID { get; set; }
        public virtual ICollection<Order> Orders { get; set; } 
        public virtual ICollection<MyUser> MyUsers { get; set; } 
    }
}
