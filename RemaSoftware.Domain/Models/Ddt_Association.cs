namespace RemaSoftware.Domain.Models
{
    public class Ddt_Association
    {
        public Ddt_In Ddt_In { get; set; }
        public int Ddt_In_ID { get; set; }
        public Ddt_Out Ddt_Out { get; set; }
        public int Ddt_Out_ID { get; set; }
        public int NumberPieces { get; set; }
    }
}
