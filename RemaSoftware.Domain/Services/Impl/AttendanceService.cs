using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Data;
using System.Text.Json;
using Microsoft.Extensions.Logging.Console;
using RemaSoftware.Domain.Constants;

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
            
            newInDateTime = new DateTime(attendance.DateIn.Date.Year, attendance.DateIn.Date.Month, attendance.DateIn.Date.Day, newInDateTime.Hour == 0 ? attendance.DateIn.Hour : newInDateTime.Hour, newInDateTime.Minute == 0 ? attendance.DateIn.Minute : newInDateTime.Minute, newInDateTime.Second);
            
            newOutDateTime = new DateTime(attendance.DateIn.Year, attendance.DateIn.Month, attendance.DateIn.Day, (int)(newOutDateTime.Hour == 0 ? attendance.DateOut?.Hour : newOutDateTime.Hour), (int)(newOutDateTime.Minute == 0 ? attendance.DateOut?.Minute : newOutDateTime.Minute), newOutDateTime.Second);
                
            if (attendance != null)
            {
                
                if (newInDateTime.Date == newOutDateTime.Date && newOutDateTime.TimeOfDay < newInDateTime.TimeOfDay)
                {
                    newOutDateTime = newOutDateTime.AddDays(1);
                }
                
                if (DateTime.Compare(newInDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) == 0 &&
                    DateTime.Compare(newOutDateTime, new DateTime(0001, 01, 01, 00, 00, 00)) == 0)
                {
                    attendance.Type = type;
                    _dbContext.Update(attendance);
                }
                else
                {
                    Attendance newAttendance = new Attendance();
                    newAttendance.DateIn = newInDateTime;
                    newAttendance.DateOut = newOutDateTime;
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
            
            if (newInDateTime.Date == newOutDateTime?.Date && newOutDateTime?.TimeOfDay < newInDateTime.TimeOfDay)
            {
                newOutDateTime = newOutDateTime?.AddDays(1);
            }
            
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

                        DateTime? dateOut = existingAttendance.DateOut?.Date;

                        var dayAttendances = _dbContext.Attendances
                            .Where(a => dateOut.HasValue && a.DateIn.Date == dateOut.Value && a.EmployeeID == existingAttendance.EmployeeID && a.Type == existingAttendance.Type)
                            .ToList();
                        
                        var employee = _dbContext.Employees
                            .SingleOrDefault(e => e.EmployeeID == existingAttendance.EmployeeID);
                        
                        TimeSpan totalDuration = TimeSpan.Zero;

                        foreach (var attendance in dayAttendances)
                        {
                            if (attendance.DateOut.HasValue)
                            {
                                TimeSpan duration = attendance.DateOut.Value - attendance.DateIn;
                                totalDuration += duration;
                            }
                        }
                        
                        TimeSpan threshold;

                        if (employee.NumberHour == 25)
                            threshold = new TimeSpan(8, 55, 0);
                        else if (employee.NumberHour == 35)
                            threshold = new TimeSpan(7, 55, 0);
                        else
                            threshold = new TimeSpan(5, 55, 0);
                        
                        
                        if (totalDuration > threshold && employee.Extraordinary)
                        {
                            TimeSpan exceedingDuration = totalDuration - threshold;
                            int extraHours = (int)exceedingDuration.TotalHours;
                            
                            if (exceedingDuration.TotalMinutes > 0)
                                extraHours += 1;
                            
                            if ((int)(exceedingDuration.TotalMinutes % 60) > 55)
                                extraHours += 1;
                            
                            Attendance strAttendance = new Attendance
                            {
                                EmployeeID = existingAttendance.EmployeeID,
                            };
                            
                            if(extraHours >= 1){
                                
                                if (existingAttendance.DateOut?.AddHours(-extraHours) > existingAttendance.DateIn)
                                    existingAttendance.DateOut = existingAttendance.DateOut?.AddHours(-extraHours);
                                else
                                    existingAttendance.DateOut = existingAttendance.DateIn;
                                
                                strAttendance.DateIn = existingAttendance.DateOut.Value;
                                strAttendance.DateOut = dateIn;
                                
                                if (employee.NumberHour == 40)
                                    strAttendance.Type = "StraordinarioOrdinario";
                                else
                                    strAttendance.Type = "Supplementare";
                                
                                _dbContext.Attendances.Add(strAttendance);
                            }
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

                        if (existingAttendanceNight != null && existingAttendanceNight.DateOut == null &&  userClockDate1.TimeOfDay >= TimeSpan.FromHours(4) &&  userClockDate1.TimeOfDay <= TimeSpan.FromHours(7.5) &&  existingAttendanceNight.DateIn.TimeOfDay >= TimeSpan.FromHours(20))
                        {
                            existingAttendanceNight.DateOut = userClockDate1;
                            
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

                                Attendance newAttendance = new Attendance
                                {
                                    DateIn = dateIn,
                                    DateOut = null,
                                    EmployeeID = employee.EmployeeID,
                                    Type = "Presenza"
                                };

                                if (newAttendance.DateIn.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    newAttendance.Type = "StraordinarioSabato";
                                }
                                else if (newAttendance.DateIn.TimeOfDay > TimeSpan.Parse("20:00"))
                                {
                                    newAttendance.Type = "StraordinarioNotturno";
                                }

                                _dbContext.Attendances.Add(newAttendance);
                            }
                        }
                    }
                    _dbContext.SaveChanges();
                }
            }

        }
        
        public async Task UpdateAttendanceListWithPresenceEmployee(List<string> userIdList, List<string> userClockList, string FluidaId)
        {
            for(int i=0; i<userIdList.Count; i++)
            {
                if(userIdList[i] == FluidaId){
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

                            DateTime? dateOut = existingAttendance.DateOut?.Date;

                            var dayAttendances = _dbContext.Attendances
                                .Where(a => dateOut.HasValue && a.DateIn.Date == dateOut.Value && a.EmployeeID == existingAttendance.EmployeeID && a.Type == existingAttendance.Type)
                                .ToList();
                            
                            var employee = _dbContext.Employees
                                .SingleOrDefault(e => e.EmployeeID == existingAttendance.EmployeeID);
                            
                            TimeSpan totalDuration = TimeSpan.Zero;

                            foreach (var attendance in dayAttendances)
                            {
                                if (attendance.DateOut.HasValue)
                                {
                                    TimeSpan duration = attendance.DateOut.Value - attendance.DateIn;
                                    totalDuration += duration;
                                }
                            }

                            TimeSpan threshold;

                            if (employee.NumberHour == 25)
                                threshold = new TimeSpan(8, 55, 0);
                            else if (employee.NumberHour == 35)
                                threshold = new TimeSpan(7, 55, 0);
                            else
                                threshold = new TimeSpan(5, 55, 0);
                            
                            
                            if (totalDuration > threshold && employee.Extraordinary)
                            {
                                TimeSpan exceedingDuration = totalDuration - threshold;
                                int extraHours = (int)exceedingDuration.TotalHours;
                                
                                if (exceedingDuration.TotalMinutes > 0)
                                    extraHours += 1;
                                
                                if ((int)(exceedingDuration.TotalMinutes % 60) > 55)
                                    extraHours += 1;
                                
                                Attendance strAttendance = new Attendance
                                {
                                    EmployeeID = existingAttendance.EmployeeID,
                                };
                                
                                if(extraHours >= 1){
                                    
                                    if (existingAttendance.DateOut?.AddHours(-extraHours) > existingAttendance.DateIn)
                                        existingAttendance.DateOut = existingAttendance.DateOut?.AddHours(-extraHours);
                                    else
                                        existingAttendance.DateOut = existingAttendance.DateIn;
                                    
                                    strAttendance.DateIn = existingAttendance.DateOut.Value;
                                    strAttendance.DateOut = dateIn;
                                    
                                    if (employee.NumberHour == 40)
                                        strAttendance.Type = "StraordinarioOrdinario";
                                    else
                                        strAttendance.Type = "Supplementare";
                                    
                                    _dbContext.Attendances.Add(strAttendance);
                                }
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

                            if (existingAttendanceNight != null && existingAttendanceNight.DateOut == null &&  userClockDate1.TimeOfDay >= TimeSpan.FromHours(4) &&  userClockDate1.TimeOfDay <= TimeSpan.FromHours(7.5) &&  existingAttendanceNight.DateIn.TimeOfDay >= TimeSpan.FromHours(20))
                            {
                                existingAttendanceNight.DateOut = userClockDate1;
                                
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

                                    Attendance newAttendance = new Attendance
                                    {
                                        DateIn = dateIn,
                                        DateOut = null,
                                        EmployeeID = employee.EmployeeID,
                                        Type = "Presenza"
                                    };

                                    if (newAttendance.DateIn.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        newAttendance.Type = "StraordinarioSabato";
                                    }
                                    else if (newAttendance.DateIn.TimeOfDay > TimeSpan.Parse("20:00"))
                                    {
                                        newAttendance.Type = "StraordinarioNotturno";
                                    }

                                    _dbContext.Attendances.Add(newAttendance);
                                }
                            }
                        }
                        _dbContext.SaveChanges();
                    }
                }
            }

        }

        public async Task UpdateAttendancePermit()
        {
            DateTime toDate = DateTime.Today;
            DateTime fromDate = toDate.AddDays(-7);
            
            var attendances = _dbContext.Attendances
                .Where(a => a.DateIn >= fromDate && a.DateIn <= toDate)
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
                            DateTime dateOutAlternative = date.Date.AddHours(12).AddMinutes(30);
                            DateTime dateOutAlternative1 = date.Date.AddHours(14).AddMinutes(30);

                            if (employee.NumberHour == 25)
                                dateOut = dateOutAlternative;
                            else if (employee.NumberHour == 35)
                                dateOut = dateOutAlternative1;
                            
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
                            else if(employee.TypeRelationship == TypeRelationship.LavoroAutonomo && employee.TypePosition != TypePosition.Maternità)
                            {
                                newAttendance.Type = "Presenza";
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

        public Attendance GetLastAttendance()
        {
            var oggi = DateTime.Today;
            var domani = oggi.AddDays(1);
            return _dbContext.Attendances
                .Where(a => a.DateIn <= domani && a.Type != "Eliminato")
                .OrderByDescending(a => a.DateIn)
                .FirstOrDefault();} 
    }


