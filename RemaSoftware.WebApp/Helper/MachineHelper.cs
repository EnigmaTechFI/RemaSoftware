using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
    
    public async Task<MachineViewModel> GetMachineViewModelAsync()
    {
        MachineViewModel machineViewModel = new MachineViewModel();

        try
        {
            var machine = await _machineService.ConnectMachine();
        
            if (machine != null)
            {
                machineViewModel.MachineOn = machine.MachineOn;
                machineViewModel.Brush1_On = machine.Brush1_On;
                machineViewModel.Brush2_On = machine.Brush2_On;
                machineViewModel.Brush3_On = machine.Brush3_On;
                machineViewModel.Brush4_On = machine.Brush4_On;
                machineViewModel.Brush5_On = machine.Brush5_On;
            }
            else
            {
                machineViewModel.MachineOn = false;
                machineViewModel.Brush1_On = false;
                machineViewModel.Brush2_On = false;
                machineViewModel.Brush3_On = false;
                machineViewModel.Brush4_On = false;
                machineViewModel.Brush5_On = false;
            }
        }
        catch (Exception ex)
        {
            machineViewModel.MachineOn = false;
            machineViewModel.Brush1_On = false;
            machineViewModel.Brush2_On = false;
            machineViewModel.Brush3_On = false;
            machineViewModel.Brush4_On = false;
            machineViewModel.Brush5_On = false;
        }

        return machineViewModel;
    }
    
}