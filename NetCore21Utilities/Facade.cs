using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace NetCore21Utilities
{
    public class Facade
    {
        WebAPICommunicator api { get; set; }
        public Facade() 
        {
            api = new WebAPICommunicator();
        }
        public string SpotifyAuthCallUrl(string clientID, string clientSec, string redirect) 
        {
            string result = string.Empty;
            var message = api.CreateRequestMessage(clientID, clientSec, redirect, "Spotify", "Auth");
            result = message.RequestUri.AbsoluteUri + "?" + message.Content.ReadAsStringAsync().Result;
            return result;
        }
        public string GetToken(string clientID, string clientSec, string redirect, string codeText, string refresh="")
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(codeText))
            {
                //リフレッシュトークンで取得
                result = api.GetTokenResult(clientID, clientSec, redirect);
            }
            else 
            {
                result = api.GetTokenResult(clientID, clientSec, redirect, codeText);
            }
            return result;
        }
        public string GetSpotifyObject(string valueName,string token) 
        {
            string result = string.Empty;
            SpotifyToken tokenObj = null;
            if (string.IsNullOrEmpty(token)) 
            {
                string json = new Configuration().Get("SpotifyToken");
                tokenObj = JsonConvert.DeserializeObject<SpotifyToken>(json);
                //TODO when it expire, call GetToken

                if (string.IsNullOrEmpty(tokenObj.expires_in))
                { 
                }
                token = tokenObj.access_token;
            }
            switch (valueName) 
            {
                case "currentUserName":
                    Tuple<string, List<Tuple<string, string>>> resultObject = api.GetQueryResult(token, "QueryMe");
                    var data = resultObject.Item2.Where(prop => prop.Item1 == "displayName").FirstOrDefault();
                    if (data != null) result = data.Item2;
                    break;
            }
            return result;
        }
    }
}
