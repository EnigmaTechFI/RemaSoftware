using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemaSoftware.Domain.Models
{
    public class UserClient
    {
        public int ClientID { get; set; }
        public string MyUserID { get; set; }
        public virtual Client Client { get; set; }
        public virtual MyUser MyUser { get; set; }
    }
}
