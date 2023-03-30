using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class ExitToSupplierViewModel
{
    public SubBatch SubBatch { get; set; }
    public List<Supplier> Suppliers { get; set; }
    public Ddt_Supplier DdtSupplier { get; set; }
    public List<BatchOperation> BatchOperations { get; set; }
    public string CostUni { get; set; }
    public int BatchOperationID { get; set; }    
}