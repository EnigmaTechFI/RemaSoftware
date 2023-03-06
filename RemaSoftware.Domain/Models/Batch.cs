using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Batch
    {
        public int BatchId { get; set; }
        [MaxLength(50)]
        public string QrCode { get; set; }
        public decimal Price_Uni { get; set; }
        public virtual List<BatchOperation> BatchOperations { get; set; }
        public virtual List<SubBatch> SubBatches { get; set; }
    }
}
