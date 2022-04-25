using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dissertation.Models
{
    /// <summary>
    /// homepage information gets stored in the structs
    /// </summary>
    /// <returns></returns>
    public struct viewDevicesStruct
    {
        public string ClientID { set; get; }
        public string Name { set; get; }
        public DateTime LastStatus { set; get; }
        public int TotalViolations { set; get; }

    }
}