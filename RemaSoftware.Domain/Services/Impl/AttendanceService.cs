using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;
using System;
using System.Text.RegularExpressions;

namespace RemaSoftware.Domain.Services.Impl;

    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _dbContext;

        public AttendanceService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool DeleteAttendanceById(Attendance attendance)
        {
            attendance.Type = "Eliminato";
            _dbContext.Update(attendance);
            _dbContext.SaveChanges();
            return true;
        }

        public void ModifyAttendance(int attendanceId, DateTime newInDateTime, DateTime newOutDateTime)
        {
            var attendance = _dbContext.Attendances.Find(attendanceId);
            if (attendance != null)
            {
                Attendance newAttendance = new Attendance();
                if (DateTime.Compare(newInDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) != 0)
                    newAttendance.DateIn = attendance.DateIn.Date + newInDateTime.TimeOfDay;
                if (DateTime.Compare(newOutDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) != 0)
                    newAttendance.DateOut = attendance.DateIn.Date + newOutDateTime.TimeOfDay;
                newAttendance.EmployeeID = attendance.EmployeeID;
                newAttendance.Type = attendance.Type;
                attendance.Type = "Eliminato";

                _dbContext.Update(attendance);
                _dbContext.Attendances.Add(newAttendance);
                _dbContext.SaveChanges();
            }
        }

        public void NewAttendance(int EmployeeId, DateTime newInDateTime, DateTime? newOutDateTime, string type)
        {
            if (newInDateTime == default || newInDateTime.TimeOfDay == TimeSpan.Zero)
                throw new Exception("Errore, impossibile salvare presenza.");

            Attendance attendance = new Attendance();
            attendance.DateIn = newInDateTime;
            attendance.DateOut = newOutDateTime;
            attendance.EmployeeID = EmployeeId;
            attendance.Type = type;

            bool isDateInAlreadyPresent = _dbContext.Attendances
                .Where(u => u.EmployeeID == EmployeeId)
                .Any(a => (attendance.DateIn >= a.DateIn && attendance.DateIn < a.DateOut) ||
                          (attendance.DateOut > a.DateIn && attendance.DateOut <= a.DateOut) ||
                          (attendance.DateIn <= a.DateIn && attendance.DateOut >= a.DateOut));

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

        public List<Attendance> getAttendanceById(int id, int month, int year)
        {
            return _dbContext.Attendances
                .Include(t => t.Employee)
                .Where(i => i.DateIn.Month == month && i.DateIn.Year == year && i.EmployeeID == id)
                .OrderBy(i => i.EmployeeID)
                .ThenBy(i => i.DateIn)
                .ToList();
        }
        
        public Attendance getOneAttendanceById(int attendanceid)
        {
            return _dbContext.Attendances.SingleOrDefault(a => a.AttendanceID == attendanceid);
        }

        public async Task UpdateAttendanceListWithPresence(List<string> userIdList, List<string> userClockList)
        {
            for(int i=0; i<userIdList.Count; i++)
            {
                DateTime userClockDate1 = DateTime.ParseExact(userClockList[i], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                // Controlla se esiste un elemento in Attendance con timestart o timeend uguale a userClock.Time

                DateTime userClockDateTime = DateTime.ParseExact(userClockList[i], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                bool exists = _dbContext.Attendances
                    .Any(a => 
                        (a.DateIn.Date == userClockDateTime.Date && a.DateIn.TimeOfDay == userClockDateTime.TimeOfDay || 
                         (a.DateOut.HasValue && a.DateOut.Value.Date == userClockDateTime.Date && a.DateOut.Value.TimeOfDay >= userClockDateTime.TimeOfDay)) && 
                        a.Employee.FluidaId == userIdList[i]);

                if (!exists)
                {
                    // Cerca se esiste un elemento in Attendance con timestart non nullo e timeend nullo
                    var existingAttendance = _dbContext.Attendances
                        .FirstOrDefault(a => a.DateIn != null && a.DateOut == null && a.Employee.FluidaId == userIdList[i] && a.DateIn.Date == userClockDate1.Date);

                    if (existingAttendance != null)
                    {
                        // Se esiste un elemento con timestart non nullo e timeend nullo e se quell'elemento ha data uguale, aggiorna il timeend con userClock.Time
                        string dateFormat = "MM/dd/yyyy HH:mm:ss";
                        DateTime dateIn;
                        DateTime.TryParseExact(userClockList[i], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateIn);
                        existingAttendance.DateOut = dateIn;
                    }
                    else
                    {
                        // Se non esiste un elemento, crea una nuova riga in Attendance
                        var employee = _dbContext.Employees.FirstOrDefault(e => e.FluidaId == userIdList[i]);
                        if (employee != null)
                        {
                            string dateFormat = "MM/dd/yyyy HH:mm:ss";
                            DateTime dateIn;
                            DateTime.TryParseExact(userClockList[i], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateIn);
                            if (dateIn.TimeOfDay >= TimeSpan.FromHours(5) && dateIn.TimeOfDay <= TimeSpan.FromHours(7.5))
                            {
                                dateIn = dateIn.Date.AddHours(7).AddMinutes(30);
                            }
                            Attendance newAttendance = new Attendance
                            {
                                DateIn = dateIn,
                                DateOut = null,
                                EmployeeID = employee.EmployeeID,
                                Type = "Presenza"
                            };

                            _dbContext.Attendances.Add(newAttendance);
                        }
                    }

                    // Salva le modifiche nel database
                    _dbContext.SaveChanges();
                }
            }
        }
    }

