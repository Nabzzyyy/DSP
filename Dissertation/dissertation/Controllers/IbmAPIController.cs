using dissertation.Models;
using dissertation.ObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace dissertation.Controllers
{
    /// <summary>
    /// The IBM API allows the client emotion to be scored with a tone name if the client hash is valid.
    /// </summary>
    /// <param name="ibmPost check">Allows collection of IBM variables to be called to set and get tone name and score.</param> 
    /// <returns>OK status code if successful or BadRequest if API failed.</returns>
    public class IbmAPIController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post(ibmPost check)
        {
            if (!string.IsNullOrEmpty(check.ClientHash))
            {
                //Decode ClientHash
                var decodeClientHash = Base64Handler.Decoder(check.ClientHash).Split(',');
                var userID = Int32.Parse(decodeClientHash[0]);
                var clientID = Int32.Parse(decodeClientHash[1]);
                //Check for matching user + client
                if (Client.CheckHash(userID, clientID))
                {
                    HttpClient client = new HttpClient();
                    string baseURL;
                    string apikey;
                    //Calls IBM API
                    baseURL = "https://api.eu-gb.tone-analyzer.watson.cloud.ibm.com/instances/916ab65f-adfb-49a4-9d96-b0eb507076da/v3/tone?version=2017-09-21";
                    apikey = "_Ua8K2z-G2LLisXyhsdi42mfU_FPwWPb7JIwz8hrPMPx";

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("apikey:" + apikey)));
                    //Message gets analysed
                    string postData = "{\"text\": \"" + check.message + "\"}";
                    //Result of message
                    var response = client.PostAsync(baseURL, new StringContent(postData, Encoding.UTF8, "application/json")).Result; 
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Root ibmEmotions = JsonConvert.DeserializeObject<Root>(responseContent);
                    var tones = ibmEmotions.document_tone.tones;

                    var IbmAnalyst = false;
                    if (responseContent != null)
                    {
                        //Insert Emotions into database
                        foreach (var tone in tones)
                        {
                            Client.InsertClientEmotions(tone.tone_name, tone.score, clientID, userID);
                        }

                        IbmAnalyst = true;
                    }
                    else
                    {
                        IbmAnalyst = false;
                    }

                    if (IbmAnalyst)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "IBM API successful");
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "IBM API failed");
        }

        /// <summary>
        /// IBM struct allowing necessary data to be passed into the IBM API.
        /// </summary>
        /// <returns></returns>
        public struct ibmPost
        {
            public String message { set; get; }
            public String ClientHash { set; get; }
        }

        //Structs that allows the JSON result returned from API to be stored in classes.
        public class Tone
        {
            public decimal score { get; set; }
            public string tone_id { get; set; }
            public string tone_name { get; set; }
        }

        public class DocumentTone
        {
            public List<Tone> tones { get; set; }
        }

        public class Root
        {
            public DocumentTone document_tone { get; set; }
        }
    }
}