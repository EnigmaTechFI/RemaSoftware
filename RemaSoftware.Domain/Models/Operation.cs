﻿using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Operation
    {

        public int OperationID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual List<BatchOperation> BatchOperations { get; set; }
        
        public virtual List<PriceOperation> PriceOperation { get; set; }

    }
}
