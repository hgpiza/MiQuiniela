using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Newtonsoft.Json.Linq;

namespace MiQuiniela.Controllers
{
    public class FbUser 
    {
        public string Id { set; get; }
        public string Name { set; get; }
    }

    public class FacebookController : Controller
    {
        //
        // GET: /Facebook/

        public ActionResult Users()
        {
            string myAccessToken = Session["AccessToken"].ToString();
            FacebookClient client = new FacebookClient(myAccessToken);
            
            var friendListData = client.Get("/me/friends");
            JObject friendListJson = JObject.Parse(friendListData.ToString());

            List<FbUser> fbUsers = new List<FbUser>();
            foreach (var friend in friendListJson["data"].Children())
            {
                FbUser fbUser = new FbUser();
                fbUser.Id = friend["id"].ToString().Replace("\"", "");
                fbUser.Name = friend["name"].ToString().Replace("\"", "");
                fbUsers.Add(fbUser);
            }
            return View(fbUsers);
        }

    }
}
