using System.Globalization;
using Microsoft.EntityFrameworkCore;
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
                if (attendance.DateOut == null)
                {
                    attendance.DateOut = attendance.DateIn;
                }
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
                .Where(u => u.EmployeeID == EmployeeId && u.Type != "Eliminato")
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
        
        public List<Attendance> getAttendanceByFluidaId(string id, int month, int year)
        {
            return _dbContext.Attendances
                .Include(t => t.Employee)
                .Where(i => i.DateIn.Month == month && i.DateIn.Year == year && i.Employee.FluidaId == id)
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
                        string dateFormat = "MM/dd/yyyy HH:mm:ss";
                        DateTime dateIn;
                        DateTime.TryParseExact(userClockList[i], dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateIn);

                        // Calculate the total duration of attendances for the specified day
                        TimeSpan totalDuration = TimeSpan.Zero;
                        var controlend = false;
                        foreach (var attendance in getAttendanceByFluidaId(userIdList[i], existingAttendance.DateIn.Month, existingAttendance.DateIn.Year))
                        {
                            if (attendance.DateIn.Date != null && attendance.DateOut?.Date != null && attendance.DateIn.Date == dateIn.Date && attendance.Type == "Presenza")
                            {
                                totalDuration += (attendance.DateOut - attendance.DateIn) ?? TimeSpan.Zero;
                                if (totalDuration == TimeSpan.FromHours(8))
                                    controlend = true;
                            }
                        }
                        totalDuration += dateIn - existingAttendance.DateIn;
                        if (totalDuration <= TimeSpan.FromHours(8) || controlend){
                            existingAttendance.DateOut = dateIn;
                        }
                        else
                        {
                            Attendance newAttendance = new Attendance
                            {
                                DateIn = dateIn,
                                DateOut = dateIn,
                                EmployeeID = existingAttendance.EmployeeID,
                                Type = "Eliminato"
                            };
                            existingAttendance.DateOut = dateIn.AddHours(-totalDuration.TotalHours + 8);
                            _dbContext.Attendances.Add(newAttendance);
                        }
                    }
                    else
                    {
                        //Guardare se c'è un elemento notturno agire di conseguenza
                        var previousDay = userClockDate1.Date.AddDays(-1);

                        var existingAttendanceNight = _dbContext.Attendances
                            .FirstOrDefault(a => a.DateIn != null &&
                                                 a.DateOut == null &&
                                                 a.Employee.FluidaId == userIdList[i] &&
                                                 a.DateIn.Date == previousDay);

                        if (existingAttendanceNight != null && existingAttendanceNight.DateOut == null &&  userClockDate1.TimeOfDay >= TimeSpan.FromHours(5) &&  userClockDate1.TimeOfDay <= TimeSpan.FromHours(7.5) &&  existingAttendanceNight.DateIn.TimeOfDay >= TimeSpan.FromHours(21))
                        {
                            existingAttendanceNight.DateOut = userClockDate1.Date;
                            
                            Attendance oldAttendance = new Attendance
                            {
                                DateIn = userClockDate1.Date,
                                DateOut = userClockDate1.Date,
                                EmployeeID = existingAttendanceNight.EmployeeID,
                                Type = "Eliminato"
                            };
                            _dbContext.Attendances.Add(oldAttendance);
                            
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
                                    Attendance oldAttendance = new Attendance
                                    {
                                        DateIn = dateIn,
                                        DateOut = dateIn,
                                        EmployeeID = employee.EmployeeID,
                                        Type = "Eliminato"
                                    };
                                    _dbContext.Attendances.Add(oldAttendance);
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
                    }

                    _dbContext.SaveChanges();
                }
            }
        }
        
        public List<Attendance> getAllAttendanceForDay()
        {
            return _dbContext.Attendances
                .Include(t => t.Employee)
                .Where(i => i.DateIn.Date == DateTime.Now.Date)
                .OrderBy(i => i.EmployeeID)
                .ThenBy(i => i.DateIn)
                .ToList();
        }
    }

