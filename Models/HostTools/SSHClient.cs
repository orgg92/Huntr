namespace Radar.Models.HostTools
{
    using Chilkat;
    using Radar.Common.Config;
    using Radar.Common.NetworkModels;
    using Radar.Services;
    using Radar.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SSHClient
    {
        private readonly ILoggingService _loggingService;
        public SSHClient(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public bool AttemptConnection(Host host)
        {

            Ssh ssh = new Ssh();

            var loginCredentials = Config.LOGIN_CREDENTIALS.Where(x => x.Vendor.Contains(host.Vendor));

            if (loginCredentials.Any())
            {
                foreach (var credential in loginCredentials)
                {

                    int port = 22;
                    bool success = ssh.Connect(host.IP, port);
                    if (success != true)
                    {
                        credential.Attempted = true;
                        Debug.WriteLine(ssh.LastErrorText);
                        return false;
                    }

                    success = ssh.AuthenticatePw(credential.Username, credential.Password);
                    if (success != true)
                    {
                        credential.Attempted = true;
                        Debug.WriteLine(ssh.LastErrorText);
                        return false;
                    }

                    host.LoginCredentials = credential;
                    credential.Attempted = true;
                    credential.Successful = true;

                    return true;

                }
            }


            return true;
        }


    }
}
