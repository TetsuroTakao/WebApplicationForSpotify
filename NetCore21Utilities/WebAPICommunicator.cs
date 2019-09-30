using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Net;

namespace NetCore21Utilities
{
    public class WebAPICommunicator
    {
        public HttpRequestMessage CreateRequestMessage(string clientID, string clientSec, string redirect, string destination = "Spotify", string urlType = "Auth", string codeText = "", string token="")
        {
            HttpRequestMessage result = new HttpRequestMessage();
            string url = string.Empty;
            string scope = "user-read-private user-read-email";
            var message = new HttpRequestMessage();
            StringContent content = null;
            message.Headers.Add("Connection", "Keep-Alive");
            message.Headers.Add("Host", "accounts.spotify.com");
            message.Headers.Add("Accept", "*/*");
            message.Headers.Add("Cache-Control", "no-cache");
            string parameter = string.Empty;
            switch (destination)
            {
                case "Spotify":
                    if (urlType == "Auth")
                    {
                        url = "https://accounts.spotify.com/authorize";
                        message.Method = HttpMethod.Get;
                        parameter = "scope=" + scope;
                        parameter += "&response_type=code";
                        parameter += "&client_id=" + clientID;
                        parameter += "&redirect_uri=" + redirect;
                        var state = new Random().Next(111111, 999999).ToString();
                        parameter += "&state=" + state;
                        content = new StringContent(parameter, Encoding.UTF8, "application/x-www-form-urlencoded");
                        message.Content = (HttpContent)content;
                    }
                    else if (urlType == "Token")
                    {
                        url = "https://accounts.spotify.com/api/token";
                        message.Method = HttpMethod.Post;
                        parameter = "grant_type=authorization_code";
                        parameter += "&client_id=" + clientID;
                        parameter += "&client_secret=" + clientSec;
                        parameter += "&code=" + codeText;
                        parameter += "&redirect_uri=" + redirect;
                        content = new StringContent(parameter, Encoding.UTF8, "application/x-www-form-urlencoded");
                        message.Content = (HttpContent)content;
                    }
                    else if (urlType == "Refresh")
                    {
                        //未実装
                        url = "https://accounts.spotify.com/api/token";
                        message.Method = HttpMethod.Post;
                        parameter = "grant_type=authorization_code";
                        parameter += "&client_id=" + clientID;
                        parameter += "&client_secret=" + clientSec;
                        parameter += "&code=" + codeText;
                        parameter += "&redirect_uri=" + redirect;
                        content = new StringContent(parameter, Encoding.UTF8, "application/x-www-form-urlencoded");
                        message.Content = (HttpContent)content;
                    }
                    else if (urlType == "QueryMe")
                    {
                        url = "https://api.spotify.com/v1/me";
                        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        message.Method = HttpMethod.Get;
                    }
                    message.RequestUri = new Uri(url);
                    break;
            }
            return message;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="clientSec"></param>
        /// <param name="redirect">if null it might run use default value http://localhost:8888</param>
        /// <param name="codeText"></param>
        /// <returns></returns>
        public string GetTokenResult(string clientID, string clientSec, string redirect, string codeText="")
        {
            string result = string.Empty;
            HttpRequestMessage message = null;
            if (!string.IsNullOrEmpty(codeText))
            {
                message = CreateRequestMessage(clientID, clientSec, (string.IsNullOrEmpty(redirect) ? null : redirect), "Spotify", "Token", codeText);
            }
            else 
            {
                message = CreateRequestMessage(clientID, clientSec, (string.IsNullOrEmpty(redirect) ? null : redirect), "Spotify", "Refresh");
            }
            using (var client = new HttpClient())
            {
                using (var response = client.SendAsync(message).Result)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var token = JsonConvert.DeserializeObject<SpotifyToken>(json);
                        result = token.access_token;
                        new Configuration().Set("SpotifyToken", json);
                    }
                }
            }
            return result;
        }

        public Tuple<string, List<Tuple<string, string>>> GetQueryResult(string token = "",string type="")
        {
            Tuple<string, List<Tuple<string, string>>> result = null;
             var message = CreateRequestMessage("", "", "", "Spotify", type, "", token);
            var values = new List<Tuple<string, string>>();
            using (var client = new HttpClient())
            {
                using (var res = client.SendAsync(message).Result)
                {
                    var meText = res.Content.ReadAsStringAsync().Result;
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        //var currentUser = JsonConvert.DeserializeObject<Tuple<string, List<Tuple<string, string>>>>(meText);
                        var currentUser = JsonConvert.DeserializeObject<SpotifyUser>(meText);
                        //result = new Tuple<string, List<Tuple<string, string>>>("current-user-profile", currentUser);
                        List<Tuple<string, string>> data = new List<Tuple<string, string>>();
                        data.Add(new Tuple<string, string>("displayName", currentUser.display_name));
                        result = new Tuple<string, List<Tuple<string, string>>>("current-user-profile", data);
                    }
                }
            }
            return result;
        }
    }
}
