using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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

    public AttendanceHelper(IAttendanceService attendanceService, UserManager<MyUser> userManager, IEmployeeService employeeService, IEmailService emailService)
    {
        _attendanceService = attendanceService;
        _employeeService = employeeService;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task DeleteAttendance(int attendanceId)
    {
        var attendance = _attendanceService.getOneAttendanceById(attendanceId);
        _attendanceService.DeleteAttendanceById(attendance);
    }

    public void ModifyAttendance(ModifyAttendanceDTO model)
    {
        _attendanceService.ModifyAttendance(model.AttendanceId, model.InId, model.OutId);
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
        const string companyId = "29c78fb8-8e97-4a2b-9df2-302ace7481ce";
        const string apiKey = "0575b429-d35a-4e83-bee6-f02149997cf2";
        
        if (month == 0)
            month = DateTime.Today.Month;

        if (year == 0)
            year = DateTime.Today.Year;
        
        
        string fromDate = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
        DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        string toDate = lastDayOfMonth.ToString("yyyy-MM-dd");        
        string url = $"https://api.fluida.io/api/v1/stampings/{companyId}/daily_clock_records?from_date={fromDate}&to_date={toDate}";

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

            }
        }
        await ControlFirstAttendance();
    }

    public void SendAttendance(int month, int year, string mail)
    {
        string filePath = CreateTxtAttendance();
        
        if (!string.IsNullOrEmpty(filePath))
        {
            _emailService.SendEmailAttendance("14/07/2023 - 15/07/2023", "alessio.corsi.lv@gmail.com", filePath);
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

    static string CreateTxtAttendance()
    {
        string fileText = "00000000000000101010101010000000000000000000000000000000000000010101010000";
        string filePath = Path.GetTempFileName() + ".txt";

        try
        {
            File.WriteAllText(filePath, fileText);
            return filePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Si è verificato un errore durante la creazione del file: " + ex.Message);
            return null;
        }
    }

    public async Task ControlFirstAttendance()
    {
        List<Attendance> attendances = _attendanceService.getAllAttendanceForDay();
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

            if (!hasAttendance && DateTime.Now.TimeOfDay > new TimeSpan(7, 30, 0) && DateTime.Now.TimeOfDay < new TimeSpan(9, 30, 0))
            {
                employeesAttendance.Add(employee);
            }
        }

        if (employeesAttendance.Count != 0)
        {
            var users = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            _emailService.SendEmployeeAttendance(employeesAttendance, users.Select(s => s.Email).ToList());   
        }
    }
}