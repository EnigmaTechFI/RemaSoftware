using System.Threading.Tasks;
using RemaSoftware.UtilityServices.Implementation;

namespace RemaSoftware.UtilityServices.Interface
{
    public interface IMachineService
    {
        public Task<MachineViewModel> ConnectMachine();
    }
}