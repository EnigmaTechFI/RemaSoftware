using System;
using Microsoft.AspNetCore.Authorization;
using RemaSoftware.Domain.Constants;

namespace RemaSoftware.WebApp.DTOs;

[Authorize(Roles = Roles.Admin)]

public class ModifyAttendanceDTO
{
    public int AttendanceId { get; set; }
    public DateTime InId { get; set; }
    public DateTime OutId { get; set; }
}