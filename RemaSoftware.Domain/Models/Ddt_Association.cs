using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemaSoftware.Domain.Models
{
    public class Ddt_Association
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int Ddt_In_ID { get; set; }
        public Ddt_In Ddt_In { get; set; }
        public int Ddt_Out_ID { get; set; }
        public Ddt_Out Ddt_Out { get; set; }
        public int NumberPieces { get; set; }
    }
}
