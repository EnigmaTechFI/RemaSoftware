using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.ContextModels
{
    public class Warehouse_Stock
    {
        public int Warehouse_StockID { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public string Name { get; set; }

        public string Brand { get; set; }

        [Required(ErrorMessage = "Questo campo è obbligatorio!")]
        public int Number_Piece { get; set; }

        public double Price_Tot { get; set; }

        public double Price_Uni { get; set; }

        public string Size { get; set; }
    }
}
