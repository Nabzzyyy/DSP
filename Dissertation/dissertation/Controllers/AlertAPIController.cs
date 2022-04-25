using dissertation.Models;
using dissertation.ObjectModel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace dissertation.Controllers
{
    /// <summary>
    /// This API retrieves violation that occur and store into the database if the clientHash is valid. 
    /// </summary>
    /// <param name="HttpRequestMessage requestData">data about violation is requested.</param> 
    /// <returns>OK status code if successful or BadRequest and InternalServerError if API failed.</returns>
    public class AlertAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(HttpRequestMessage requestData)
        {
            try
            {

                string requestString = requestData.Content.ReadAsStringAsync().Result;
                alertPost check = JsonConvert.DeserializeObject<alertPost>(requestString);
                Logging.Log.Debug("AlertAPI", requestString);

                if (!string.IsNullOrEmpty(check.ClientHash))
                {
                    Logging.Log.Error("AlertAPI", "starting to decode clientHash");
                    //Decode ClientHash
                    var decodeClientHash = Base64Handler.Decoder(check.ClientHash).Split(',');
                    var userID = Int32.Parse(decodeClientHash[0]);
                    var clientID = Int32.Parse(decodeClientHash[1]);
                    Logging.Log.Error("AlertAPI", "finished decoding clientHash");
                    //Check for matching user + client
                    if (Client.CheckHash(userID, clientID))
                    {
                        Logging.Log.Error("AlertAPI", "Retrieveing client information");
                        //Retrieve Client Information
                        var individual_client = Client.GetClientHash(check.ClientHash);
                        Logging.Log.Error("AlertAPI", "client information finished retrieveing");
                        if (!string.IsNullOrEmpty(check.Keyword) && !string.IsNullOrEmpty(check.Screenshot) && !string.IsNullOrEmpty(check.Location)
                            && check.AlertTypeID >= 0 && !string.IsNullOrEmpty(check.Keyword))
                        {
                            Logging.Log.Error("AlertAPI", "writing to text file");
                            //Stores screenshot value and DateTime to make it unique 
                            string newTextFilePath = @"C:\Program Files\DspWebsite\DSP\Logs\screenshot" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
                            
                            using (StreamWriter sw = File.CreateText(newTextFilePath))
                            {
                                sw.Write(check.Screenshot);

                            }
                            Logging.Log.Error("AlertAPI", "finished writing to a text file");

                            Logging.Log.Error("AlertAPI", "Inserting to DB");
                            var violation = Client.AlertNotification(newTextFilePath, check.Keyword, check.Location, DateTime.Now.Round(new TimeSpan(0, 1, 0)), check.AlertTypeID, clientID);
                            Logging.Log.Error("AlertAPI", "Finished inerting to DB");
                            if (violation)
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, "Alert API successful");
                                
                            }
                            
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Alert API failed!");
            }
            catch (Exception ex)
            {
                Logging.Log.Error("AlertAPI", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Alert API failed!");
            }
        }
    }

    /// <summary>
    /// Alert struct allowing necessary data to be passed into the Alert API.
    /// </summary>
    /// <returns></returns>
    public struct alertPost
    {
        public string Screenshot { set; get; }
        public string Keyword { set; get; }
        public string Location { set; get; }
        public int AlertTypeID { set; get; }
        public string ClientHash { set; get; }
    }
}