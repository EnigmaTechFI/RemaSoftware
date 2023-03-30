namespace RemaSoftware.Domain.Models;

public class DDT_Supplier_Association
{
    public int Id { get; set; }
    public Ddt_Supplier Ddt_Supplier { get; set; }
    public Ddt_In Ddt_In { get; set; }
    public int Ddt_Supplier_ID { get; set; }
    public int Ddt_In_ID { get; set; }
    public int NumberPieces { get; set; }
}