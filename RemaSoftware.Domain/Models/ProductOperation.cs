using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class ProductOperation
    {
        public int Ddt_In_ID { get; set; }

        public Ddt_In Ddt_In { get; set; }

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
