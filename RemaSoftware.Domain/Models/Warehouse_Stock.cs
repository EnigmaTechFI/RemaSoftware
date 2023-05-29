using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Domain.Models
{
    public class Warehouse_Stock
    {
        public int Warehouse_StockID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Product_Code { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int SupplierID { get; set; }
        
        public Supplier Supplier { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public decimal Price_Uni { get; set; }
        
        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Reorder_Limit { get; set; }

        public string Measure_Unit { get; set; }
        public decimal Price_Tot => this.Price_Uni * this.Number_Piece;

        public virtual List<Stock_History> Stock_Histories { get; set; }

    }
}
