namespace Radar.Common.NetworkModels
{
    using Radar.Models;

    public class Host
    {
        public string IP { get; set; }
        public string MAC { get; set; }
        public string Vendor { get; set; }
        public string HostName { get; set; }

        public LoginCredential LoginCredentials { get; set; }
    }
}
