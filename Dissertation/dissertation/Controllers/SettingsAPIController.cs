using dissertation.Models;
using dissertation.ObjectModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace dissertation.Controllers
{
    /// <summary>
    /// The settings API allows the variable setted for time on screen and DNS settings to be sent to the client's PC if the client hash is valid.
    /// </summary>
    /// <param name="SettingStruct settings">Allows collection of setting variables to be called to set and get data</param> 
    /// <returns>DNS Settings and time on screen settings</returns>
    public class SettingsAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(SettingStruct settings)
        {
            if (!string.IsNullOrEmpty(settings.ClientHash))
            {
                //Decode ClientHash
                var decodeClientHash = Base64Handler.Decoder(settings.ClientHash).Split(',');
                var userID = Int32.Parse(decodeClientHash[0]);
                var clientID = Int32.Parse(decodeClientHash[1]);
                if (Client.CheckHash(userID, clientID))
                {
                    //Send Settings to client 
                    
                    var client = Client.Get(clientID);
                    var clientSettings = client.GetSettings(clientID);
                    Setting s = new Setting();
                    if (clientSettings.Any())
                    {
                        s.PreferDns = clientSettings.First().PreferDns;
                        s.AlternativeDns = clientSettings.First().AlternativeDns;
                        s.TosSetting = new TOS[7];
                        var list = clientSettings.ToList();
                        for (int i = 0; i < list.Count; i++)
                        {
                            s.TosSetting[i] = new TOS()
                            {
                                Tos_Day = list[i].Tos_Day.Value,
                                Tos_End_Minute = list[i].Tos_End_Minute,
                                Tos_Start_Minute = list[i].Tos_Start_Minute
                            };
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, s);
                    
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Setting API failed!");
        }
    }
}

/// <summary>
/// Setting struct allowing necessary data to be stored for the settings API to get information.
/// </summary>
/// <returns></returns>
public struct Setting
{
    public string PreferDns { set; get; }
    public string AlternativeDns { set; get; }
    public TOS[] TosSetting { set; get; }
}

/// <summary>
/// setting time on screen variables and data gets called in Setting struct which data gets stored in array.
/// </summary>
/// <returns></returns>
public struct TOS
{
    public int Tos_Start_Minute;
    public int Tos_End_Minute;
    public DayOfWeek Tos_Day;
}


