using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RemaSoftware.Controllers
{
    public class WebSocketController : Controller
    {
        private readonly MyHub _myHub;

        public WebSocketController(MyHub myHub)
        {
            _myHub = myHub;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> TriggerWS()
        {
            // da capire come mandare oggetti complessi, forse un json
            _myHub.Send("nuovo messaggio", "corpo del messaggio");
            return RedirectToAction("Index", "Home");
        }
    }
}