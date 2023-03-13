using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class BatchInDeliveryViewModel
{
    public List<SubBatch> SubBatches { get; set; }
    public List<Ddt_Out> DdtOuts { get; set; }
    public string pdfUrl { get; set; }
}