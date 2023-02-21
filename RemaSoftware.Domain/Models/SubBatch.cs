using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemaSoftware.Domain.Models
{
    public class SubBatch
    {
        public int SubBatchID { get; set; }
        public int BatchID { get; set; }
        public Batch Batch { get; set; }
        public int NumberPieces { get; set; }
        public virtual List<Ddt_In> Ddts_In { get; set; }
        
        [MaxLength(1)]
        public string Status { get; set; }
        public virtual List<OperationTimeline> OperationTimelines { get; set; }
    }
}
