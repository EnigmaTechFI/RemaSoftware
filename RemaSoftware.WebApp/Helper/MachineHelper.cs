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
        // Connessione OPC UA e lettura dei valori
        var machine = await _machineService.ConnectMachine();

        // Crea una nuova istanza di MachineViewModel e assegna i valori appropriati
        var machineViewModel = new MachineViewModel
        {
            MachineOn = machine.MachineOn,
            Brush1_On = machine.Brush1_On,
            Brush2_On = machine.Brush2_On,
            Brush3_On = machine.Brush3_On,
            Brush4_On = machine.Brush4_On,
            Brush5_On = machine.Brush5_On
        };

        return machineViewModel;
    }
    
    
}