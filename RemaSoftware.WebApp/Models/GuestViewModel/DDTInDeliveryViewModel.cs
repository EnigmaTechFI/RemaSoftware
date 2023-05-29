using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.GuestViewModel;

public class DDTInDeliveryViewModel
{
    public List<Ddt_OutDto> DdtOut { get; set; }
}

public class Ddt_OutDto {
    public string Code { get; set; }
    public string Type { get; set; }
    public string Product { get; set; }
    public int NumberPieces { get; set; }
}