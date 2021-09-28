using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Operation
    {

        public int OperationID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Order_Operation> Order_Operation { get; set; }
    }
}
