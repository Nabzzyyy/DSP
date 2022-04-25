using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dissertation.Models
{
    /// <summary>
    /// All settings variables are stored and retrieved in setting struct 
    /// </summary>
    /// <returns></returns>
    public class SettingStruct
    {
        public int ClientID { set; get; }

        public string Name { set; get; }

        public string ComputerName { set; get; }

        public string CurrentIP { set; get; }

        public string OS { set; get; }

        public DateTime Status { set; get; }

        public string ClientHash { set; get; }

        public int SettingID{ set; get; }

        public int DnsID { set; get; }

        public string DnsName { set; get; }

        public string PreferDns { set; get; }

        public string AlternativeDns { set; get; }

        public int TosID { set; get; }

        public int Tos_Start_Minute { set; get; }

        public int Tos_End_Minute { set; get; }

        public DayOfWeek? Tos_Day { set; get; }
        
    }
}