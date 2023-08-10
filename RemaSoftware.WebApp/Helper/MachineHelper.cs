using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NLog;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Models.MachineViewModel;

namespace RemaSoftware.WebApp.Helper;

public class MachineHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly UserManager<MyUser> _userManager;
    private readonly IMachineService _machineService;


    public MachineHelper(UserManager<MyUser> userManager, IMachineService machineService)
    {
        _userManager = userManager;
        _machineService = machineService;
    }

    public MachineViewModel AutomaticMachine()
    {
        return new MachineViewModel
        {
           MachineOn = _machineService.ConnectMachine()
        };
    }
    
    
}