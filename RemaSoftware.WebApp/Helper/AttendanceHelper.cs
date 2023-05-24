using System.Collections.Generic;
using System.Threading.Tasks;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.DTOs;
using RemaSoftware.WebApp.Models.StockViewModel;


namespace RemaSoftware.WebApp.Helper;

public class AttendanceHelper
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceHelper(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    public void DeleteAttendance(int attendanceId)
    {
        _attendanceService.DeleteAttendanceById(attendanceId);
    }

    public void ModifyAttendance(ModifyAttendanceDTO model)
    {
        _attendanceService.ModifyAttendance(model.AttendanceId, model.InId, model.OutId);
    }
    
    public void NewAttendance(ModifyAttendanceDTO model)
    {
        _attendanceService.NewAttendance(model.AttendanceId, model.InId, model.OutId);
    }
}