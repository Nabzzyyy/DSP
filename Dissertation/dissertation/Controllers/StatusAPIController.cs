using dissertation.Models;
using dissertation.ObjectModel;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace dissertation.Controllers
{

    /// <summary>
    /// The API determines when the client's PC is live (TURNED ON AND ACTIVE) if clientHash is valid.
    /// </summary>
    /// <param name="statusPost check">Gets the clientHash and passes it through the API to get validated.</param> 
    /// <returns>OK status code if successful or BadRequest if API failed.</returns>
    public class StatusAPIController : ApiController
    {

        [HttpPost]
        public HttpResponseMessage Post(statusPost check)
        {
            if (!string.IsNullOrEmpty(check.ClientHash))
            {
                //Decode ClientHash
                var decodeClientHash = Base64Handler.Decoder(check.ClientHash).Split(',');
                var userID = Int32.Parse(decodeClientHash[0]);
                var clientID = Int32.Parse(decodeClientHash[1]);

                //Check for matching user + client
                if(Client.CheckHash(userID, clientID)){
                    if (Client.Get(clientID).StatusUpdate(DateTime.Now.Round(new TimeSpan(0, 1, 0)))) 
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Status API successful");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Status API failed!");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Status API failed!");
        }

        /// <summary>
        /// status struct allowing necessary data to be stored for the status API to get information
        /// </summary>
        /// <returns></returns>
        public struct statusPost
        {
            public string ClientHash { set; get; }
        }
    }
}
