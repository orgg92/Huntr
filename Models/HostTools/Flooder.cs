namespace Radar.Common.HostTools
{
    using Radar.Common.NetworkModels;
    using System.Diagnostics;
    using System.Net.NetworkInformation;

    public class Flooder : HostTool
    {
        public Flooder()
        {
            this.Name = "Flooder";
            this.Id = 1;

        }

        public void GenerateTraffic(Host targetHost)
        {
            var numberOfThreads = Process.GetCurrentProcess().Threads.Count;

            var threads = new List<Thread>();

            Thread.Sleep(500);

            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < TimeSpan.FromMinutes(5))
            {
                Task.Run(() => PingHost(targetHost));
            }

        }

        private static void CreatePingThread(Host targetHost)
        {

        }

        private static void PingHost(Host targetHost)
        {


            try
            {
                Ping ping = new Ping();
                byte[] packet = new byte[32000];

                var reply = ping.Send(targetHost.IP, 10000, packet);

                if (reply.Status == IPStatus.Success)
                {
                    //ConsoleTools.WriteToConsole(Thread.CurrentThread.ManagedThreadId.ToString(), ConsoleColor.Yellow);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
