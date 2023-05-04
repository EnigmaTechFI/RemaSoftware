using Microsoft.AspNetCore.Authorization;
using RemaSoftware.Domain.Constants;

namespace RemaSoftware.WebApp.DTOs;

[Authorize(Roles = Roles.Admin + "," + Roles.Dipendente + "," + Roles.MagazzinoMaterie)]

public class QtyAddRemoveJsDTO
{
    public int ArticleId { get; set; }
    public int QtyToAddRemove { get; set; }
    public int QtyToAddRemoveRadio { get; set; }
}