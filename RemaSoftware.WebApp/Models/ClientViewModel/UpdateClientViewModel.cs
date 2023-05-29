using System.Collections.Generic;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.ClientViewModel;

public class UpdateClientViewModel
{
    public Client Client { get; set; }
    public List<Ddt_Template> Ddt_Templates { get; set; }
}