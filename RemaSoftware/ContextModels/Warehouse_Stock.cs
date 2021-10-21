using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Warehouse_Stock
    {
        public int Warehouse_StockID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }

        public decimal Price_Uni { get; set; }

        [MaxLength(20)]
        public string Size { get; set; }        
        public decimal Price_Tot => this.Price_Uni * this.Number_Piece;

    }
}
