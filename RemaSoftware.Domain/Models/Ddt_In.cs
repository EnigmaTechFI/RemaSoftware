using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Ddt_In
    {
        public int Ddt_In_ID { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int SubBatchID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataIn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataOut { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataEnd { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(50)]
        public string Code { get; set; }
        public decimal Price_Uni { get; set; }
        [MaxLength(30)]
        public string FC_Ddt_In_ID { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }
        public int Number_Piece_Now { get; set; }
        public int Number_Piece_ToSupplier { get; set; }
        public int NumberMissingPiece { get; set; }
        public int NumberWastePiece { get; set; }
        public int NumberLostPiece { get; set; }
        
        public int NumberZama { get; set; }
        public int Priority { get; set;}
        [MaxLength(1)]
        public string Status { get; set; }
        public bool IsReso { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public SubBatch SubBatch { get; set; }
        public bool IsPrompted { get; set; }
        
        [MaxLength(500)]
        public string Client_Note { get; set; }
        public decimal TotalPriority => (DateTime.Now.DayOfYear - this.DataOut.DayOfYear) * this.Priority;
        public bool PriceIsPending { get; set; }
        public decimal PendingPrice { get; set; }
        public virtual List<Ddt_Association> Ddt_Associations { get; set; }
        public virtual List<DDT_Supplier_Association> DdtSupplierAssociations { get; set; }
    }
}
