using IPAddress_Location_MVC.Models;
using System;

namespace dissertation.Models
{
    /// <summary>
    /// ClientStruct stores information about client's PC and violation also retrieves data.
    /// </summary>
    /// <returns></returns>
    public struct ClientStruct
    {
        public string Name { set; get; }
        public string ComputerName { set; get; }
        public string CurrentIP { set; get; }
        public string OS { set; get; }
        public DateTime LastStatus { set; get; }
        public string Screenshot { set; get; }
        public string Keyword { set; get; }
        public string Location { set; get; }
        public DateTime AlertTime { set; get; }
        public string ClientHash { set; get; }
        public int Alert { set; get; }
    }
}
