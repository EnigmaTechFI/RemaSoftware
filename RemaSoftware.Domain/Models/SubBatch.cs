using System;
using System.Collections.Generic;
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
        public virtual List<OperationTimeline> OperationTimelines { get; set; }
    }
}
