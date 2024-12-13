using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services.Impl;

namespace RemaSoftware.Domain.Services;

    public interface IAttendanceService
    {
        public bool DeleteAttendanceById(Attendance attendance);
        public void ModifyAttendance(int modelAttendanceId, DateTime modelInId, DateTime modelOutId, string type);
        public void NewAttendance(int employeeId, DateTime modelInId, DateTime? modelOutId, string type);
        public List<Attendance> getAttendanceById(int id, int mouth, int year);
        public Task UpdateAttendanceListWithPresence(List<string> userIdList, List<string> userClockList);
        public Task UpdateAttendanceListWithPresenceEmployee(List<string> userIdList, List<string> userClockList, string FluidaId);

        public Attendance getOneAttendanceById(int attendanceid);
        public List<Attendance> getAllAttendanceForDay();
        public Task UpdateAttendancePermit();
        public Attendance GetLastAttendance();

    }

