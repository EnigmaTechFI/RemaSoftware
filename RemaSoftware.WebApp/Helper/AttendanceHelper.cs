using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.DTOs;

namespace RemaSoftware.WebApp.Helper;

public class AttendanceHelper
{
    private readonly IAttendanceService _attendanceService;
    private readonly IEmployeeService _employeeService;
    private readonly IEmailService _emailService;
    private readonly UserManager<MyUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AttendanceHelper(IAttendanceService attendanceService, UserManager<MyUser> userManager, IEmployeeService employeeService, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
    {
        _attendanceService = attendanceService;
        _employeeService = employeeService;
        _emailService = emailService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task DeleteAttendance(int attendanceId)
    {
        var attendance = _attendanceService.getOneAttendanceById(attendanceId);
        _attendanceService.DeleteAttendanceById(attendance);
    }

    public void ModifyAttendance(ModifyAttendanceDTO model)
    {
        _attendanceService.ModifyAttendance(model.AttendanceId, model.InId, model.OutId, model.Type);
    }
    
    public void NewAttendance(ModifyAttendanceDTO model)
    {
        _attendanceService.NewAttendance(model.AttendanceId, model.InId, model.OutId, model.Type);
    }
    
    public void NewAllAttendance(ModifyAttendanceDTO model)
    {
        var allEmployees = _employeeService.GetAllEmployees();
        foreach(var employee in allEmployees)
            _attendanceService.NewAttendance(employee.EmployeeID, model.InId, model.OutId, model.Type);
    }
    
    public async Task UpdateAttendance(int month, int year)
    {
        
        bool AttendanceOk = true;
        var LastAttendance = _attendanceService.GetLastAttendance();
        if ((LastAttendance != null && LastAttendance.DateIn.Date == DateTime.Today) || DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
        {
            AttendanceOk = false;
        }
        
        const string companyId = "29c78fb8-8e97-4a2b-9df2-302ace7481ce";
        const string apiKey = "0575b429-d35a-4e83-bee6-f02149997cf2";
        DateTime toDate;
        DateTime fromDate;
        string toDateAPI;
        string fromDateAPI;
        if (month == 0 && year == 0)
        {
            toDate = DateTime.Today;
            fromDate = toDate.AddDays(-7);
        }else
        {
            fromDate = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            toDate = lastDayOfMonth;    
        }

        toDateAPI = toDate.ToString("yyyy-MM-dd");
        fromDateAPI = fromDate.ToString("yyyy-MM-dd");
        
        string url = $"https://api.fluida.io/api/v1/stampings/{companyId}/daily_clock_records?from_date={fromDateAPI}&to_date={toDateAPI}";

        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-fluida-app-uuid", apiKey);

        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseContent);

            if (data != null && data.data != null)
            {
                List<dynamic> records = data.data.ToObject<List<dynamic>>();
                List<string> userIdList = new List<string>();
                List<string> userClockList = new List<string>();

                foreach (dynamic record in records)
                {
                    string userId = record.contract_id;
                    List<dynamic> days = record.days.ToObject<List<dynamic>>();

                    foreach (dynamic day in days)
                    {
                        List<dynamic> clockRecords = day.clock_records.ToObject<List<dynamic>>();
                        foreach (dynamic clockRecord in clockRecords)
                        {
                            string clockAt = clockRecord.clock_at;
                            userIdList.Add($"{userId}");
                            userClockList.Add($"{clockAt}");
                        }
                    }
                }
                
                await _attendanceService.UpdateAttendanceListWithPresence(userIdList, userClockList);
                
                if (month == 0)
                {
                    await _attendanceService.UpdateAttendancePermit();
                }

            }
        }

        var request = _httpContextAccessor.HttpContext?.Request;
        
        if (AttendanceOk && DateTime.Now.TimeOfDay > new TimeSpan(7, 30, 0) && DateTime.Now.TimeOfDay < new TimeSpan(10, 0, 0))
        {
            if (request != null && !request.Host.Host.Contains("localhost"))
            {
                await ControlFirstAttendance();
            }
        }
    }

    public void SendAttendance(int month, int year, string mail, byte[] pdfBytes)
    {
        // Calcola le date di inizio e fine in base al mese e all'anno forniti
        DateTime startDate = new DateTime(year, month, 1);
        DateTime endDate = startDate.AddMonths(1).AddDays(-1);

        // Formatta le date nel formato desiderato
        string formattedDates = startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy");

        string filePath = CreateTxtAttendance(month, year);

        if (!string.IsNullOrEmpty(filePath))
        {
            _emailService.SendEmailAttendance(formattedDates, mail, filePath, pdfBytes);
            Task.Delay(1000).ContinueWith(_ =>
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Si è verificato un errore durante l'eliminazione del file: " + ex.Message);
                }
            });
        }
    }
    
    public async Task SendNoAttendance(int employee, string note)
    {
        Employee Employee = _employeeService.GetEmployeeById(employee);
        var users = _userManager.GetUsersInRoleAsync(Roles.Admin).Result;
        var adminEmails = users.Select(user => user.Email).ToList();
        _emailService.SendEmailNoAttendance(Employee, note, adminEmails);
    }

    public string CreateTxtAttendance(int month, int year)
    {
        var mese = month.ToString("D2"); 
        var anno = (year % 100).ToString("D2");
        List<string> stringFile = new List<string>();
        List<Employee> Employees = _employeeService.GetAllEmployees();

        for (int i = 0; i < Employees.Count; i++)
        {
            
            var employee = Employees[i];
            List<Attendance> attendance = _attendanceService.getAttendanceById(employee.EmployeeID, month, year);
            List<Attendance> filteredAttendance = attendance.Where(a => a.Type != "Eliminato" && a.Type != "Permesso").ToList();
            List<string> uniqueTypes = filteredAttendance.Select(a => a.Type).Distinct().ToList();

            var permiss = "U5176" + mese + anno + employee.Number + "03040000000";
            
            for (int j = 0; j < uniqueTypes.Count(); j++)
            {
                var sp = "U5176" + mese + anno + employee.Number;
                int numberOfDaysInMonth = DateTime.DaysInMonth(year, month);

                if (uniqueTypes[j] == "Presenza")
                    sp += "03000000000";
                else if (uniqueTypes[j] == "Malattia")
                    sp += "09090000000";
                else if (uniqueTypes[j] == "Festivo")
                    sp += "03010000000";
                else if (uniqueTypes[j] == "Ferie")
                    sp += "03020000000";
                else if (uniqueTypes[j] == "StraordinarioNotturno")
                    sp += "03549020000";
                else if (uniqueTypes[j] == "StraordinarioOrdinario")
                    sp += "03549050000";
                else if (uniqueTypes[j] == "StraordinarioSabato")
                    sp += "03539010000";
                else if (uniqueTypes[j] == "CassaIntegrazione")
                    sp += "67670000000";
                else if (uniqueTypes[j] == "Maternita")
                    sp += "03110000000";
                else if (uniqueTypes[j] == "MaternitaFacoltativa")
                    sp += "06917000000";
                else if (uniqueTypes[j] == "LavoroFestivo")
                    sp += "03610000000";
                else if (uniqueTypes[j] == "Supplementare")
                    sp += "03620000000";
                else if (uniqueTypes[j] == "MaggiorazioneNotturno")
                    sp += "03490000000";
                else if (uniqueTypes[j] == "Permesso104")
                    sp += "07360000000";
                else if (uniqueTypes[j] == "InfortunioIniziale")
                    sp += "03130000000";
                else if (uniqueTypes[j] == "InfortunioProlungato")
                    sp += "03100000000";
                else if (uniqueTypes[j] == "CongedoMatrimoniale")
                    sp += "03140000000";
                else if (uniqueTypes[j] == "Paternita")
                    sp += "06680000000";
                else if (uniqueTypes[j] == "PermessoStudio")
                    sp += "06306000000";
                else
                    sp += "00000000000";
                
                
                for (int u = 1; u <= numberOfDaysInMonth; u++)
                {

                    List<Attendance> attendanceForDay = attendance.Where(a => a.DateIn.Day == u && a.Type == uniqueTypes[j] && a.EmployeeID == employee.EmployeeID ).ToList();
                    
                    List<Attendance> allAttendanceForDay = attendance.Where(a => a.DateIn.Day == u && a.EmployeeID == employee.EmployeeID).ToList();

                    TimeSpan totalDuration = TimeSpan.Zero;
                    TimeSpan otherDuration = TimeSpan.Zero;
                    foreach (var attendanceTmp in attendanceForDay)
                    {
                        if (attendanceTmp.DateIn != null && attendanceTmp.DateOut != null)
                        {
                            totalDuration += attendanceTmp.DateOut.Value - attendanceTmp.DateIn;
                        }
                    }
                    foreach (var attendanceOth in allAttendanceForDay)
                    {
                        if (attendanceOth.DateIn != null && attendanceOth.DateOut != null  && attendanceOth.Type != "Permesso" && attendanceOth.Type != "Maternita" && attendanceOth.Type != "MaternitaFacoltativa" && attendanceOth.Type != "Paternita" && attendanceOth.Type != "PermessoStudio" && attendanceOth.Type != "CongedoMatrimoniale" && attendanceOth.Type != "Presenza" && attendanceOth.Type != "Eliminato" && attendanceOth.Type != "StraordinarioSabato" && attendanceOth.Type != "StraordinarioOrdinario" && attendanceOth.Type != "MaggiorazioneNotturno")
                        {
                            otherDuration += attendanceOth.DateOut.Value - attendanceOth.DateIn;
                            if (otherDuration.TotalHours > 8 && employee.NumberHour == 40)
                            {
                                otherDuration = new TimeSpan(8, 0, 0);
                            }
                            else if (otherDuration.TotalHours > 5 && employee.NumberHour == 25)
                            {
                                otherDuration = new TimeSpan(5, 0, 0);
                            }else if (otherDuration.TotalHours > 7 && employee.NumberHour == 35)
                            {
                                otherDuration = new TimeSpan(7, 0, 0);
                            }
                        }
                    }

                    if (employee.TypePosition == "In servizio" && uniqueTypes[j] == "Presenza" && employee.NumberHour == 25 && totalDuration > TimeSpan.FromHours(4) + TimeSpan.FromMinutes(50))
                    {
                        sp += "50000";
                        permiss += "00000";
                    }else if (employee.TypePosition == "In servizio" && uniqueTypes[j] == "Presenza" && employee.NumberHour == 35 && totalDuration > TimeSpan.FromHours(6) + TimeSpan.FromMinutes(50))
                    {
                        sp += "70000";
                        permiss += "00000";
                    }else
                    {
                        switch (totalDuration)
                        {
                            case TimeSpan td when td >= TimeSpan.FromHours(7) + TimeSpan.FromMinutes(50):
                                sp += "80000";
                                if (uniqueTypes[j] == "Presenza" && attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday && attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday)
                                    permiss += "00000";
                                break;
                            case var td when td >= TimeSpan.FromHours(6) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(7) + TimeSpan.FromMinutes(30) && employee.NumberHour == 40)
                                    sp += "75000";
                                else 
                                    sp += "70000";
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        if (td >= TimeSpan.FromHours(7) + TimeSpan.FromMinutes(30))
                                            permiss += "05000";
                                        else
                                            permiss += "10000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += "00000";
                                    }
                                }
                                break;
                            case var td when td >= TimeSpan.FromHours(5) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(6) + TimeSpan.FromMinutes(30))
                                    sp += "65000";
                                else 
                                    sp += "60000";
                                
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        permiss += td >= TimeSpan.FromHours(6) + TimeSpan.FromMinutes(30) ? "15000" : "20000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += td >= TimeSpan.FromHours(6) + TimeSpan.FromMinutes(30) ? "05000" : "10000";
                                    }
                                }
                                
                                break;
                            case var td when td >= TimeSpan.FromHours(4) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30))
                                    sp += "55000";
                                else 
                                    sp += "50000";
                                
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        permiss += td >= TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30) ? "25000" : "30000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += td >= TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30) ? "15000" : "20000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 25)
                                    {
                                        permiss += "00000";
                                    }
                                }
                                
                                break;
                            case var td when td >= TimeSpan.FromHours(3) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(4) + TimeSpan.FromMinutes(30))
                                    sp += "45000";
                                else 
                                    sp += "40000";
                                
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        permiss += td >= TimeSpan.FromHours(4) + TimeSpan.FromMinutes(30) ? "35000" : "40000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += td >= TimeSpan.FromHours(4) + TimeSpan.FromMinutes(30) ? "25000" : "30000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 25)
                                    {
                                        permiss += td >= TimeSpan.FromHours(4) + TimeSpan.FromMinutes(30) ? "05000" : "10000";
                                    }
                                }
                                
                                break;
                            case var td when td >= TimeSpan.FromHours(2) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(3) + TimeSpan.FromMinutes(30))
                                    sp += "35000";
                                else 
                                    sp += "30000";
                                
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        permiss += td >= TimeSpan.FromHours(3) + TimeSpan.FromMinutes(30) ? "45000" : "50000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += td >= TimeSpan.FromHours(3) + TimeSpan.FromMinutes(30) ? "35000" : "40000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 25)
                                    {
                                        permiss += td >= TimeSpan.FromHours(3) + TimeSpan.FromMinutes(30) ? "15000" : "20000";
                                    }
                                }
                                
                                break;
                            case var td when td >= TimeSpan.FromHours(1) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30))
                                    sp += "25000";
                                else 
                                    sp += "20000";
                                
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        permiss += td >= TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30) ? "55000" : "60000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += td >= TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30) ? "45000" : "50000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 25)
                                    {
                                        permiss += td >= TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30) ? "25000" : "30000";
                                    }
                                }
                                
                                break;
                            case var td when td >= TimeSpan.FromHours(0) + TimeSpan.FromMinutes(50):
                                if(td >= TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30))
                                    sp += "15000";
                                else 
                                    sp += "10000";
                                
                                if (uniqueTypes[j] == "Presenza")
                                {
                                    if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                        attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                        employee.NumberHour == 40)
                                    {
                                        permiss += td >= TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30) ? "65000" : "70000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                    {
                                        permiss += td >= TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30) ? "55000" : "60000";
                                    }
                                    else if (employee.TypePosition == "In servizio" && employee.NumberHour == 25)
                                    {
                                        permiss += td >= TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30) ? "35000" : "40000";
                                    }
                                }
                                
                                break;
                            case var td when td >= TimeSpan.FromHours(0) + TimeSpan.FromMinutes(30):
                                    sp += "05000";
                                    
                                    if (uniqueTypes[j] == "Presenza")
                                    {
                                        if (attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Sunday &&
                                            attendanceForDay[0].DateIn.DayOfWeek != DayOfWeek.Saturday &&
                                            employee.NumberHour == 40)
                                        {
                                            permiss += "75000";
                                        }
                                        else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35)
                                        {
                                            permiss += "65000";
                                        }
                                        else if (employee.TypePosition == "In servizio" && employee.NumberHour == 25)
                                        {
                                            permiss += "45000";
                                        }
                                    }
                                break;
                            default:
                                sp += "00000";
                                DateTime myDate = new DateTime(year, month, u);
                                
                                if(uniqueTypes[j] == "Presenza"){
                                    var presenceInDay = false;
                                    var permissInDay = false;
                                    foreach (var allattendanceForDay in allAttendanceForDay)
                                    {
                                        if (allattendanceForDay.Type == "Ferie" || allattendanceForDay.Type == "Festivo" || allattendanceForDay.Type == "Malattia" || allattendanceForDay.Type == "CassaIntegrazione" || allattendanceForDay.Type == "Maternita" || allattendanceForDay.Type == "MaternitaFacoltativa"  || allattendanceForDay.Type == "Paternita" || allattendanceForDay.Type == "CongedoMatrimoniale"  || allattendanceForDay.Type == "PermessoStudio") 
                                        {
                                            presenceInDay = true;
                                        }

                                        if (allattendanceForDay.Type == "Permesso")
                                        {
                                            permissInDay = true;
                                        }
                                    }

                                    if (presenceInDay)
                                    {
                                        permiss += "00000";
                                    }
                                    else
                                    {
                                        if (employee.TypePosition == "In servizio" && employee.NumberHour == 25 && myDate.DayOfWeek != DayOfWeek.Sunday && myDate.DayOfWeek != DayOfWeek.Saturday && myDate < DateTime.Today && permissInDay)
                                        {
                                            permiss += "50000";
                                        }else if (employee.TypePosition == "In servizio" && employee.NumberHour == 35 && myDate.DayOfWeek != DayOfWeek.Sunday && myDate.DayOfWeek != DayOfWeek.Saturday && myDate < DateTime.Today && permissInDay)
                                        {
                                            permiss += "70000";
                                        }
                                        else
                                        {
                                            if (employee.TypePosition == "In servizio" && myDate.DayOfWeek != DayOfWeek.Sunday && myDate.DayOfWeek != DayOfWeek.Saturday && myDate < DateTime.Today && permissInDay)
                                            {
                                                permiss += "80000";
                                            }
                                            else
                                            {
                                                permiss += "00000";
                                            }
                                        }
                                    }
                                }
                            break;
                        }
                        
                        
                        if (otherDuration.TotalMinutes != 0 && uniqueTypes[j] == "Presenza" && totalDuration > TimeSpan.FromHours(7) + TimeSpan.FromMinutes(50))
                        {
                            int reduction = 0;
                            if (otherDuration.Minutes > 50)
                            {
                                reduction = (otherDuration.Hours + 1) * 10000;
                            }
                            else
                            {
                                reduction = otherDuration.Hours * 10000;
                            }

                            if (permiss.Substring(permiss.Length - 5) != "00000")
                            {
                                permiss = permiss.Substring(0, permiss.Length - 5) +
                                          (int.Parse(permiss.Substring(permiss.Length - 5)) - reduction).ToString("D5");
                            }
                            else
                            {
                                if (employee.NumberHour == 40)
                                {
                                    permiss = permiss.Substring(0, permiss.Length - 5) +
                                              (80000 - reduction).ToString("D5");
                                }
                                else
                                {
                                    permiss = permiss.Substring(0, permiss.Length - 5) +
                                              (50000 - reduction).ToString("D5");
                                }
                            }
                        }
                    }

                    
                    //Serve per gestire alcuni casi altrimenti mi segnerebbe anche il premesso
                    if (uniqueTypes[j] == "Presenza" && totalDuration.TotalHours > 0)
                    {
                        var OneTime = false;
                        foreach (var allattendanceForDay in allAttendanceForDay)
                        {
                            if (allattendanceForDay.Type == "Malattia" || allattendanceForDay.Type == "Ferie" || allattendanceForDay.Type == "CassaIntegrazione" || allattendanceForDay.Type == "Maternita" || allattendanceForDay.Type == "MaternitaFacoltativa" ||  allattendanceForDay.Type == "Paternita" ||  allattendanceForDay.Type == "CongedoMatrimoniale" ||  allattendanceForDay.Type == "PermessoStudio")
                            {
                                OneTime = true;
                            }
                        }

                        if (OneTime)
                        {
                            permiss = permiss.Remove(permiss.Length - 5);
                            permiss += "00000"; 
                        }
                    }
                }

                if ((sp.Length-31)/5 == 30)
                {
                    sp += "00000";
                    if (uniqueTypes[j] == "Presenza")
                        permiss += "00000";
                }
                else if((sp.Length-31)/5 == 28)
                {
                    sp += "000000000000000";
                    if (uniqueTypes[j] == "Presenza")
                        permiss += "000000000000000";
                }
                else if ((sp.Length-31)/5 == 29)
                {
                    sp += "0000000000";
                    if (uniqueTypes[j] == "Presenza")
                        permiss += "0000000000";
                }
                
                if(uniqueTypes[j] =="Presenza")
                {
                    sp = sp.Remove(sp.Length - 2);
                    stringFile.Add(sp);
                    permiss = permiss.Remove(permiss.Length - 2);
                    stringFile.Add(permiss);
                }
                else
                {
                    sp = sp.Remove(sp.Length - 2);
                    stringFile.Add(sp);
                }
            }
        }

        string filePath = Path.GetTempFileName();
        string newFilePath = Path.ChangeExtension(filePath, ".txt");

        try
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in stringFile)
            {
                sb.AppendLine(str);
            }

            File.WriteAllText(newFilePath, sb.ToString());
            return newFilePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore durante la creazione del file: " + ex.Message);
            return null;
        }
    }

    public async Task ControlFirstAttendance()
    {
        List<Attendance> attendances = _attendanceService.getAllAttendanceForDay().Where(s => s.Type != "Eliminato").ToList();
        List<Employee> employees = _employeeService.GetAllEmployees();

        List<Employee> employeesAttendance = new List<Employee>();

        foreach (var employee in employees)
        {
            bool hasAttendance = false;

            foreach (var attendance in attendances)
            {
                if (attendance.EmployeeID == employee.EmployeeID)
                {
                    hasAttendance = true;
                }
            }

            if (!hasAttendance && employee.TypePosition == TypePosition.InServizio && employee.TypeRelationship == TypeRelationship.LavoroDipendente)
            {
                employeesAttendance.Add(employee);
            }
        }

        if (employeesAttendance.Count != 0)
        {
            foreach (var employe in employeesAttendance)
            {
                if (employe.TypePosition == TypePosition.InServizio && employe.Mail != "")
                {
                    _emailService.SendEmailNoAttendanceEmployee(employe, employe.Mail);
                }
            }
            var users = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            _emailService.SendEmployeeAttendance(employeesAttendance, users.Select(s => s.Email).ToList());
        }
    }
    
    public async Task UpdateAttendanceEmployee(int month, int year, int EmployeeId)
    {
        const string companyId = "29c78fb8-8e97-4a2b-9df2-302ace7481ce";
        const string apiKey = "0575b429-d35a-4e83-bee6-f02149997cf2";
        DateTime toDate;
        DateTime fromDate;
        string toDateAPI;
        string fromDateAPI;
        if (month == 0 && year == 0)
        {
            toDate = DateTime.Today;
            fromDate = toDate.AddDays(-7);
        }else
        {
            fromDate = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            toDate = lastDayOfMonth;    
        }

        toDateAPI = toDate.ToString("yyyy-MM-dd");
        fromDateAPI = fromDate.ToString("yyyy-MM-dd");
        
        string url = $"https://api.fluida.io/api/v1/stampings/{companyId}/daily_clock_records?from_date={fromDateAPI}&to_date={toDateAPI}";

        HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-fluida-app-uuid", apiKey);

        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseContent);

            if (data != null && data.data != null)
            {
                List<dynamic> records = data.data.ToObject<List<dynamic>>();
                List<string> userIdList = new List<string>();
                List<string> userClockList = new List<string>();

                foreach (dynamic record in records)
                {
                    string userId = record.contract_id;
                    List<dynamic> days = record.days.ToObject<List<dynamic>>();

                    foreach (dynamic day in days)
                    {
                        List<dynamic> clockRecords = day.clock_records.ToObject<List<dynamic>>();
                        foreach (dynamic clockRecord in clockRecords)
                        {
                            string clockAt = clockRecord.clock_at;
                            userIdList.Add($"{userId}");
                            userClockList.Add($"{clockAt}");
                        }
                    }
                }

                string FluidaId = _employeeService.GetEmployeeById(EmployeeId).FluidaId;
                
                await _attendanceService.UpdateAttendanceListWithPresenceEmployee(userIdList, userClockList, FluidaId);

            }
        }
    }
}