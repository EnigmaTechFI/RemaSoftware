using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int ClientID {get; set; }
        public Client Client { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string RemaCode { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(50)]
        public string SKU { get; set; }
        public decimal Price_Uni { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(70)]
        public string Image_URL { get; set; }
        public virtual List<Ddt_In> Ddt_In { get; set; }

    }
}
