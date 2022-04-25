using dissertation.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls.WebParts;

namespace dissertation.Models
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool authorise = false;
            var cookie = filterContext.HttpContext.Request.Cookies.Get("Authorisation");
            
            if (cookie != null)
            {
                var token = cookie.Value;
                if (token != null) {

                    //Does token exist in DB and is it still valid
                    var decodedToken = Base64Handler.Decoder(token).Split(',');
                    var userID = Int32.Parse(decodedToken[2]);

                    if (Token.getInstance().ValidateLoggingin(token, userID)) {
                        //Checks token to see if its a login procedure
                        var tokenProcedure = decodedToken[1];
                        if (tokenProcedure.Trim() == "LOGIN") {
                            authorise = true;
                        }
                    }
                }
            }
            if (authorise != true) {
                filterContext.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
            }

        }
    }
}