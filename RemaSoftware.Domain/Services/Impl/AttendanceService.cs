using Microsoft.Extensions.Logging.Console;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;

namespace RemaSoftware.Domain.Services.Impl;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _dbContext;

    public AttendanceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool DeleteAttendanceById(int attendanceId)
    {
        _dbContext.Attendances.Remove(new Attendance {AttendanceID = attendanceId});
        _dbContext.SaveChanges();
        return true;
    }

    public void ModifyAttendance(int attendanceId, DateTime newInDateTime, DateTime newOutDateTime)
    {
        var attendance = _dbContext.Attendances.Find(attendanceId);

        if (attendance != null)
        {
            if (DateTime.Compare(newInDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) != 0)
                attendance.DateIn = attendance.DateIn.Date + newInDateTime.TimeOfDay;
            if (DateTime.Compare(newOutDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) != 0)
                attendance.DateOut = attendance.DateOut.Date + newOutDateTime.TimeOfDay;

            _dbContext.SaveChanges();
        }
    }

    public void NewAttendance(int EmployeeId, DateTime newInDateTime, DateTime newOutDateTime)
    {
        Attendance attendance = new Attendance();
        attendance.DateIn = newInDateTime;
        attendance.DateOut = newOutDateTime;
        attendance.EmployeeID = EmployeeId;

        bool isDateInAlreadyPresent = _dbContext.Attendances.Any(a => a.DateIn.Date == attendance.DateIn.Date);

        if (attendance.DateIn <= attendance.DateOut && !isDateInAlreadyPresent)
        {
            _dbContext.Attendances.Add(attendance);
            _dbContext.SaveChanges();
        }
        else
        {
            throw new Exception("Errore, impossibile salvare presenza.");
        }
        
    }

}
