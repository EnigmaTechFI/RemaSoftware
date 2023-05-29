using System;
using RemaSoftware.UtilityServices.Interface;

namespace RemaSoftware.UtilityServices.Implementation;

public class UtilityService : IUtilityService
{
    public string GetDifferenceBetweenDate(DateTime firstDate, DateTime secondDate)
    {
        TimeSpan span = secondDate - firstDate;
        return $"{span.Hours}:{span.Minutes}:{span.Seconds}";
    }
}