using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class SubBatchMonitoringViewModel
{
    public SubBatch SubBatch { get; set; }
    public string DdtSupplierUrl { get; set; }
    public string BasePathImages { get; set; }

}