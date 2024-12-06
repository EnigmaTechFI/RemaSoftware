using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class SubBatch
    {
        public int SubBatchID { get; set; }
        public int BatchID { get; set; }
        public Batch Batch { get; set; }
        public virtual List<Ddt_In> Ddts_In { get; set; }
        
        [MaxLength(1)]
        public string Status { get; set; }
        public virtual List<OperationTimeline> OperationTimelines { get; set; }
    }
}
