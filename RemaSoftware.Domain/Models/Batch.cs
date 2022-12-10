using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemaSoftware.Domain.Models
{
    public class Batch
    {
        public int BatchId { get; set; }
        [MaxLength(50)]
        public string QrCode { get; set; }
        public decimal Price_Uni { get; set; }
        public virtual List<Ddt_In> Ddts_In { get; set; }
        public virtual List<BatchOperation> BatchOperations { get; set; }
        public virtual List<SubBatch> SubBatches { get; set; }
    }
}
