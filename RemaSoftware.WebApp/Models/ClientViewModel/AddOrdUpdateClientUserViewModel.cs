namespace RemaSoftware.WebApp.Models.ClientViewModel;

public class AddOrdUpdateClientUserViewModel
{
    public int ParentClientId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}