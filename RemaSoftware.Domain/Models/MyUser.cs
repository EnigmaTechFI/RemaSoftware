using Microsoft.AspNetCore.Identity;

namespace RemaSoftware.Domain.Models
{
    public class MyUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public int? MachineId { get; set; }
        public virtual UserClient UserClient { get; set; }
    }
}
