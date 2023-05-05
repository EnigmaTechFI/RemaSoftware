using System;
using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.OrderViewModel;

public class DDTEmittedViewModel
{
    public List<DDTEmittedDto> DdtOuts { get; set; }
}

public class DDTEmittedDto
{
    public int Id { get; set; }
    public int NumberPieces { get; set; }
    public string Url { get; set; }
    public string Client { get; set; }
    public List<(string, int)> DdtWithPieces { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; }
}
