using RemaSoftware.Domain.Services.Impl;

namespace RemaSoftware.Domain.Services;

public interface IAttendanceService
{
    public bool DeleteAttendanceById(int attendanceId);
    public void ModifyAttendance(int modelAttendanceId, DateTime modelInId, DateTime modelOutId);
    public void NewAttendance(int employeeId, DateTime modelInId, DateTime modelOutId);

}
