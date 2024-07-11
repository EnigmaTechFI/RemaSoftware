using Microsoft.AspNetCore.Authorization;
using RemaSoftware.Domain.Constants;

namespace RemaSoftware.WebApp.DTOs;

[Authorize(Roles = Roles.Admin + "," + Roles.Dipendente + "," + Roles.MagazzinoMaterie + "," + Roles.DipendenteControl)]

public class StockJsonResultDTO : JsonResultBaseDTO
{
    public int? Number_Piece { get; set; }
    public decimal? Price_Tot { get; set; }
    public StockJsonResultDTO() { }
        
    public StockJsonResultDTO(bool result, string toastMessage) : base(result, toastMessage) { }
}