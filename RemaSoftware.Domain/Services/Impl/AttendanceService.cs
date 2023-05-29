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

        public bool DeleteAttendanceById(int attendanceId)
        {
            _dbContext.Attendances.Remove(new Attendance { AttendanceID = attendanceId });
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
                    attendance.DateOut = attendance.DateOut?.Date + newOutDateTime.TimeOfDay;

                _dbContext.SaveChanges();
            }
        }

        public void NewAttendance(int EmployeeId, DateTime newInDateTime, DateTime newOutDateTime)
        {
            Attendance attendance = new Attendance();
            attendance.DateIn = newInDateTime;
            attendance.DateOut = newOutDateTime;
            attendance.EmployeeID = EmployeeId;

            bool isDateInAlreadyPresent = _dbContext.Attendances.Where(u => u.EmployeeID == EmployeeId).Any(a => a.DateIn.Date == attendance.DateIn.Date);

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

        public async Task UpdateAttendanceList(List<string> userIdList, List<string> userClockList)
        {
            for(int i=0; i<userIdList.Count; i++)
            {
                DateTime userClockDate1 = DateTime.ParseExact(userClockList[i], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                // Controlla se esiste un elemento in Attendance con timestart o timeend uguale a userClock.Time
                bool exists = _dbContext.Attendances
                    .Any(a => (a.DateIn.Date == userClockDate1.Date || (a.DateOut.HasValue && a.DateOut.Value.Date == userClockDate1.Date)) && a.Employee.FluidaId == userIdList[i]);

                if (!exists)
                {
                    // Cerca se esiste un elemento in Attendance con timestart non nullo e timeend nullo
                    var existingAttendance = _dbContext.Attendances
                        .FirstOrDefault(a => a.DateIn != null && a.DateOut == null && a.Employee.FluidaId == userIdList[i]);

                    if (existingAttendance != null)
                    {
                        // Se esiste un elemento con timestart non nullo e timeend nullo, aggiorna il timeend con userClock.Time
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
                            Attendance newAttendance = new Attendance
                            {
                                DateIn = dateIn,
                                DateOut = null,
                                EmployeeID = employee.EmployeeID
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

