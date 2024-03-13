using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Price
    {
        public int PriceID { get; set; }
        
        public int ProductID {get; set; }
        
        public Product Product { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Description { get; set; }
        
        public string CodeBC { get; set; }
        
        public string Note { get; set; }
        
        public DateTime CreationDate { get; set; }
        
        public decimal PriceVal { get; set; }
        
        public int OperationID {get; set; }
        
        public Operation Operation{ get; set; }

    }
}
