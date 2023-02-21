using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class BatchInProductionViewModel
{
    public List<SubBatch> SubBatches { get; set; }
}