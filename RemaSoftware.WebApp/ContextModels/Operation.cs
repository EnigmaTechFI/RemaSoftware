using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.WebApp.ContextModels
{
    public class Operation
    {

        public int OperationID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Order_Operation> Order_Operation { get; set; }
    }
}
