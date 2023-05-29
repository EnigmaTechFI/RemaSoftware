using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.GuestViewModel;

public class BatchInViewModel
{
    public List<SubBatchInProductionGuestDto> SubBatches { get; set; }
}

public class SubBatchInProductionGuestDto
{
    public int Id { get; set; }
    public string Product { get; set; }
    public int NumberPieces { get; set; }
    public int NumberPiecesNow { get; set; }
    public List<Ddt_In_Dto> Ddt_In { get; set; }
}

public class Ddt_In_Dto
{
    public string Code { get; set; }
    public string Date { get; set; }
    public int NowPieces { get; set; }
}