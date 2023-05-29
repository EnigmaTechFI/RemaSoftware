using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class PartialDDTViewModel
{
    public int DdtId { get; set; }
    public List<PartialDdtDto> PartialDdtDtos { get; set; }
    public int ClientId { get; set; }
}

public class PartialDdtDto
{
    public bool ToEmit { get; set; }
    public Ddt_Association DdtAssociation {get; set; }
}