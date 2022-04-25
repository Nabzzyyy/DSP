using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Kidz_protection
{

    public class TosSetting
    {
        public int Tos_Start_Minute { get; set; }
        public int Tos_End_Minute { get; set; }
        public int Tos_Day { get; set; }
    }

    public class SettingsResponse
    {
        public string PreferDns { get; set; }
        public string AlternativeDns { get; set; }
        public List<TosSetting> TosSetting { get; set; }
    }

}
