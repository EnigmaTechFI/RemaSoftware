using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class BatchOperation
    {
        public int BatchOperationID {get; set;}
        public int BatchID { get; set; }
        public Batch Batch { get; set; }
        public int OperationID { get; set; }
        public Operation Operations { get; set; }
        public int Ordering { get; set; }
        [MaxLength(1)]
        public string Status { get; set; }
        public decimal? ExtraPrice { get; set; }
        public bool? ExtraPriceIsPending { get; set; }
        public virtual List<OperationTimeline> OperationTimelines { get; set; }
    }
}
