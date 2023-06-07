using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemaSoftware.Domain.Services;
using RemaSoftware.WebApp.DTOs;

namespace RemaSoftware.WebApp.Helper;

public class AttendanceHelper
{
    private readonly IAttendanceService _attendanceService;
    private readonly IEmployeeService _employeeService;

    public AttendanceHelper(IAttendanceService attendanceService, IEmployeeService employeeService)
    {
        _attendanceService = attendanceService;
        _employeeService = employeeService;
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
    }
    
    public void ExportAttendance()
    {
        CreateTxtAttendance();

        //Download il file
    }
    
    public void SendAttendance()
    {
        CreateTxtAttendance();
        
        //Invio diretto al coso del lavoro
    }
    
    private void CreateTxtAttendance()
    {
        var ciao = 0;
        //Creazione del file
    }

}