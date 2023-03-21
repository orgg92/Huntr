namespace Radar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LoginCredential
    {
        public string Vendor { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Attempted { get; set; }
        public bool Successful { get; set; }
    }
}
