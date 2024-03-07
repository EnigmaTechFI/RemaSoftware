using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Ddt_Supplier
    {
        public int Ddt_Supplier_ID { get; set; }
        public string Code { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataReIn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataOut { get; set; }

        public decimal Cost_Uni { get; set; }
        [MaxLength(30)]
        public string FC_Ddt_Supplier_ID { get; set; }
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }
        public int NumberReInPiece { get; set; }
        public int NumberWastePiece { get; set; }
        public int NumberLostPiece { get; set; }
        public int NumberZamaPiece { get; set; }
        
        public int NumberResoScarto { get; set; }

        [MaxLength(1)]
        public string Status { get; set; }
        public bool IsReso { get; set; }
        public OperationTimeline OperationTimeline { get; set; }
        public int OperationTimelineID { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string Url { get; set; }
        public int SupplierID { get; set; }
        public virtual List<DDT_Supplier_Association> DdtSupplierAssociations { get; set; }

    }
}
