using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Stock_History
    {
        public int Stock_HistoryID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        
        public bool Entry { get; set; }
        
        public int Number_Piece { get; set; }
        
        public int Warehouse_StockID { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}
