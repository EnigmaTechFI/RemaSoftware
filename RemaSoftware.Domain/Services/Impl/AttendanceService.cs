using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;
using System.Text.Json;

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

        public void ModifyAttendance(int attendanceId, DateTime newInDateTime, DateTime newOutDateTime, string type)
        {
            var attendance = _dbContext.Attendances.Find(attendanceId);
            if (attendance != null)
            {
                if (DateTime.Compare(newInDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) == 0 &&
                    DateTime.Compare(newOutDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) == 0)
                {
                    attendance.Type = type;
                    _dbContext.Update(attendance);
                }
                else
                {
                    Attendance newAttendance = new Attendance();
                    if (DateTime.Compare(newInDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) != 0)
                        newAttendance.DateIn = attendance.DateIn.Date + newInDateTime.TimeOfDay;
                    if (DateTime.Compare(newOutDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) != 0)
                        newAttendance.DateOut = attendance.DateIn.Date + newOutDateTime.TimeOfDay;
                    newAttendance.EmployeeID = attendance.EmployeeID;
                    newAttendance.Type = type;
                    if (attendance.DateOut == null)
                    {
                        attendance.DateOut = attendance.DateIn;
                    }
                    attendance.Type = "Eliminato";
                    _dbContext.Update(attendance);
                    _dbContext.Attendances.Add(newAttendance);
                }
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
                if (userIdList[i] == "ae3cdeac-75c0-4734-9d53-5cebde5eb08a")
                {
                    var o = 0;
                }
                DateTime userClockDate1 = DateTime.ParseExact(userClockList[i], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                // Controlla se esiste un elemento in Attendance con timestart o timeend uguale a userClock.Time

                DateTime userClockDateTime = DateTime.ParseExact(userClockList[i], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                bool exists = _dbContext.Attendances
                    .Any(a => 
                        ((a.DateIn.Date == userClockDateTime.Date && a.DateIn.TimeOfDay == userClockDateTime.TimeOfDay) || 
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

                        //rigistrazione normale timbro 
                        existingAttendance.DateOut = dateIn;
                        
                        //se il timbro va dopo le 5 30 un ora di straordinario
                        if (dateIn.TimeOfDay >= new TimeSpan(17, 30, 0) && dateIn.TimeOfDay < new TimeSpan(18, 30, 0))
                        {
                            existingAttendance.DateOut = dateIn.Date.AddHours(16).AddMinutes(30);
                            Attendance oldAttendance = new Attendance
                            {
                                DateIn = dateIn.Date.AddHours(16).AddMinutes(30),
                                DateOut = dateIn,
                                EmployeeID = existingAttendance.EmployeeID,
                                Type = "Straordinario"
                            };
                            _dbContext.Attendances.Add(oldAttendance);
                        }else if(dateIn.TimeOfDay >= new TimeSpan(18, 30, 0) && dateIn.TimeOfDay < new TimeSpan(19, 00, 0))
                        {
                            //se il timbro va dopo le 6 30 sono due ore di straordinario
                            existingAttendance.DateOut = dateIn.Date.AddHours(17).AddMinutes(30);
                            Attendance oldAttendance = new Attendance
                            {
                                DateIn = dateIn.Date.AddHours(16).AddMinutes(30),
                                DateOut = dateIn.Date.AddHours(17).AddMinutes(30),
                                EmployeeID = existingAttendance.EmployeeID,
                                Type = "Straordinario"
                            };
                            _dbContext.Attendances.Add(oldAttendance);
                            Attendance oldAttendance1 = new Attendance
                            {
                                DateIn = dateIn.Date.AddHours(17).AddMinutes(30),
                                DateOut = dateIn,
                                EmployeeID = existingAttendance.EmployeeID,
                                Type = "Straordinario"
                            };
                            _dbContext.Attendances.Add(oldAttendance1);
                        }
                    }
                    else
                    {
                        //Sezione straordinario notturno
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
                                TimeSpan time = TimeSpan.FromHours(6) + TimeSpan.FromMinutes(35);
                                if (dateIn.TimeOfDay > time && dateIn.TimeOfDay <= TimeSpan.FromHours(7.5))
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
                                else if (dateIn.TimeOfDay >= TimeSpan.FromHours(5.5) && dateIn.TimeOfDay <= time)
                                {
                                    Attendance oldAttendance = new Attendance
                                    {
                                        DateIn = dateIn,
                                        DateOut = dateIn.Date.AddHours(7).AddMinutes(30),
                                        EmployeeID = employee.EmployeeID,
                                        Type = "Straordinario"
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

                                if (newAttendance.DateIn.DayOfWeek == DayOfWeek.Saturday || newAttendance.DateIn.TimeOfDay > TimeSpan.Parse("20:00"))
                                {
                                    newAttendance.Type = "Straordinario";
                                }

                                _dbContext.Attendances.Add(newAttendance);
                            }
                        }
                    }
                    _dbContext.SaveChanges();
                }
            }

        }

        public async Task UpdateAttendancePermit()
        {
            DateTime toDate = DateTime.Today;
            DateTime fromDate = toDate.AddDays(-7);
            
            var attendances = _dbContext.Attendances
                .Where(a => a.DateIn >= fromDate && a.DateIn <= toDate && a.Type != "Eliminato")
                .OrderBy(a => a.DateIn)
                .ToList() // Esegui la query e ottieni l'elenco di tutte le Attendance corrispondenti ai criteri di filtro
                .GroupBy(a => a.EmployeeID)
                .ToList();

            var employees = _dbContext.Employees.ToList();

            foreach (var employee in employees)
            {
                for (DateTime date = fromDate; date < toDate; date = date.AddDays(1))
                {
                    var attendanceGroup = attendances.FirstOrDefault(g => g.Key == employee.EmployeeID);

                    if (attendanceGroup == null || !attendanceGroup.Any(a => a.DateIn.Date == date.Date))
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            DateTime dateIn = date.Date.AddHours(7).AddMinutes(30); // Imposta DateIn a 7:30 del mattino
                            DateTime dateOut = date.Date.AddHours(16).AddMinutes(30); // Imposta DateOut a 16:30

                            Attendance newAttendance = new Attendance
                            {
                                DateIn = dateIn,
                                DateOut = dateOut,
                                EmployeeID = employee.EmployeeID,
                            };

                            if (await IsItalianHoliday(date))
                            {
                                newAttendance.Type = "Festivo";
                            }
                            else
                            {
                                newAttendance.Type = "Permesso";
                            }

                            _dbContext.Attendances.Add(newAttendance);
                            _dbContext.SaveChanges();
                        }
                    }
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
        
        private static async Task<bool> IsItalianHoliday(DateTime date)
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = $"https://date.nager.at/Api/v2/PublicHolidays/{date.Year}/IT";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var holidays = JsonSerializer.Deserialize<PublicHoliday[]>(json);

                    return holidays?.Any(h => h.Date.Date == date.Date) ?? false;
                }
            }

            return false;
        }

        public class PublicHoliday
        {
            public DateTime Date { get; set; }
            public string Name { get; set; }
        }
    }


