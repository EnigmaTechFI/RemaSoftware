using System;

namespace UtilityServices.Dtos
{
    public class OrderDto
    {
        public string Name { get; set; }

        public string Cliente { get; set; }

        public int Number_Piece { get; set; }

        public DateTime DataIn { get; set; }

        public DateTime DataOut { get; set; }

        public string SKU { get; set; }

        public string Image_URL { get; set; }

        public string Description { get; set; }

        public double Price_Tot { get; set; }

        public double Price_Uni { get; set; }
    }
}