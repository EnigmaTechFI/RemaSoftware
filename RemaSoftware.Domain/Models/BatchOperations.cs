using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemaSoftware.Domain.Models
{
    public class BatchOperations
    {
        public int BatchID { get; set; }

        public Batch Batch { get; set; }

        public int OperationID { get; set; }

        public Operation Operations { get; set; }
        public int Ordering { get; set; }
        [MaxLength(1)]
        public string Status { get; set; }
        public int NumberMissingPiece { get; set; }
        public int NumberWastePiece { get; set; }
        public int NumberLostPiece { get; set; }
        public decimal? ExtraPrice { get; set; }
        public bool? ExtraPriceIsPending { get; set; }
        public virtual List<OperationTimeline> OperationTimelines { get; set; }
    }
}
