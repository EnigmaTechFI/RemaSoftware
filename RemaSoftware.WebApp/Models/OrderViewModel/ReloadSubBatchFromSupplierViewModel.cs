namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class ReloadSubBatchFromSupplierViewModel
{
    public int OkPieces { get; set; }
    public int LostPieces { get; set; }
    public int WastePieces { get; set; }
    public int ZamaPieces { get; set; }
    public int ResoScarto { get; set; }
    
    public int RipaGratuita { get; set; }
    public int DDTSupplierId { get; set; }
    public int DDTSupplierPieces { get; set; }
}