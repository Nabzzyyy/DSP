using dissertation.Models;
using dissertation.ObjectModel;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace dissertation.Controllers
{
    /// <summary>
    ///  The API gets the information about the PC information and stores into the database if clientHash is valid.
    /// </summary>
    /// <param name="ClientStruct client"> structs that stores all of client's information.</param> 
    /// <returns>OK status code if successful or unauthorised if API failed.</returns>
    public class CurrentSystemAPIController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Post(ClientStruct client)
        {
            client.CurrentIP = HttpContext.Current.Request.UserHostAddress;

            var currentIP = client.CurrentIP;

            if (!string.IsNullOrEmpty(client.ClientHash))
            {
                //Decode ClientHash
                var decodeClientHash = Base64Handler.Decoder(client.ClientHash).Split(',');
                var userID = Int32.Parse(decodeClientHash[0]);
                var clientID = Int32.Parse(decodeClientHash[1]);

                //Check for matching user + client
                if (Client.CheckHash(userID, clientID))
                {
                    //Retrieve Client Information
                    var individual_client = Client.GetClientHash(client.ClientHash);
                    //Update Client Information
                    individual_client.UpdateClientInformation(client.ComputerName, currentIP, client.OS, DateTime.Now.Round(new TimeSpan(0, 1, 0)));
                    return Request.CreateResponse(HttpStatusCode.OK, "CurrentSystem API successful");
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, "CurrentSystem API failed!");
        }
    }
}
