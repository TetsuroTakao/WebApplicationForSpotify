using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NetCore21Utilities;
using System;
using System.Diagnostics;
using WebApplicationForSpotify.Models;
using System.Linq;

namespace WebApplicationForSpotify.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(Nullable<bool> signin,Nullable<bool> isSignin)
        {
            var message = string.Empty;
            StringValues codeText = string.Empty;
            StringValues stateText = string.Empty;
            var clientID = new Encryptor().Decrypt(new Configuration().Get("ClientId"));
            var clientSec = new Encryptor().Decrypt(new Configuration().Get("ClientSecret"));
            var runServer = "http://" + new Configuration().Get("LocalWeb");
            #region ユーザーがログインをクリックして認証画面から戻ってきた時の動作
            if (Request.Query.TryGetValue("state", out stateText))
            {
                signin = null;
                object val = null;
                if (TempData.TryGetValue("AuthState", out val))
                {
                    if (stateText.ToString() == val.ToString())
                    {
                        if (!Request.Query.TryGetValue("code", out codeText)) message = "Auth Codeを取得できなかった";
                    }
                    else
                    {
                        message = "Request元を検証できなかった";
                    }
                }
                else 
                {
                    message = "Request元を検証できなかった";
                }
            }
            var username = string.Empty;
            if (!string.IsNullOrEmpty(codeText)) 
            {
                var token = new Facade().GetToken(clientID, clientSec, runServer, codeText);
                username = new Facade().GetSpotifyObject("currentUserName", token);
            }
            #endregion
            #region ユーザーがログインをクリックした時の動作
            if (signin.HasValue)
            {
                if (signin.Value)
                {
                    var callbackurl = new Facade().SpotifyAuthCallUrl(clientID, clientSec, runServer);
                    var state = new Uri(callbackurl).Query.Split('&').Where(q => q.StartsWith("state")).FirstOrDefault();
                    TempData["AuthState"] = string.IsNullOrEmpty(state) ? "" : state.Split("=").LastOrDefault();
                    Response.Redirect(callbackurl);
                }
            }
            #endregion
            ViewBag.SigninUser = username;
            ViewBag.Message = message;
            ViewBag.IsSignin = isSignin;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
