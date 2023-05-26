using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services;

    public interface IAttendanceService
    {
        public bool DeleteAttendanceById(int attendanceId);
        public void ModifyAttendance(int modelAttendanceId, DateTime modelInId, DateTime modelOutId);
        public void NewAttendance(int employeeId, DateTime modelInId, DateTime modelOutId);
        public List<Attendance> getAttendanceById(int id, int mouth, int year);
    }

