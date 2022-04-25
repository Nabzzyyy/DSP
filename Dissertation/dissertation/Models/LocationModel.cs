using dissertation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IPAddress_Location_MVC.Models
{
    /// <summary>
    /// Location model storing geographical location of client.
    /// </summary>
    /// <returns></returns>
    public class LocationModel
    {
        public string IP { set; get; }
        public string Country_Code { set; get; }
        public string Country_Name { set; get; }
        public string Region_Code { set; get; }
        public string Region_Name { set; get; }
        public string City { set; get; }
        public string Zip_Code { set; get; }
        public string Time_Zone { set; get; }
        public string Latitude { set; get; }
        public string Longitude { set; get; }
        public string Metro_Code { set; get; }
    }
}