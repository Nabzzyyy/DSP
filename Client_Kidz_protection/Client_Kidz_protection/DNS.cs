using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace Client_Kidz_protection
{
    internal class DNS
    {
        public static void SetDNS(string[] dns)
        {
            if (dns == null || dns.Count() > 2) return;
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface ip set dns \"Ethernet\" static {dns[0]}");
            p.StartInfo = psi;
            p.Start();


            p = new Process();
            psi = new ProcessStartInfo("netsh", $"interface ip add dns \"Ethernet\" {dns[1]}");
            p.StartInfo = psi;
            p.Start();


            p = new Process();
            psi = new ProcessStartInfo("netsh", $"interface ip set dns \"WiFi\" static {dns[0]}");
            p.StartInfo = psi;
            p.Start();


            p = new Process();
            psi = new ProcessStartInfo("netsh", $"interface ip add dns \"WiFi\" {dns[1]}");
            p.StartInfo = psi;
            p.Start();

        }
    }
}
