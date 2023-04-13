using System;

namespace RemaSoftware.UtilityServices.Interface;

public interface IUtilityService
{
    string GetDifferenceBetweenDate(DateTime firstDate, DateTime secondDate);
}