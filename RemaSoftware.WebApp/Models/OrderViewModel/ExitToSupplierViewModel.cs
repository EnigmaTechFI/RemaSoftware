using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class ExitToSupplierViewModel
{
    public SubBatch SubBatch { get; set; }
    public List<Supplier> Suppliers { get; set; }
}