using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using YamlDotNet.RepresentationModel;
using System.Linq;
using Newtonsoft.Json;

namespace NetCore21Utilities
{
    public class SpotifyTypeResolver
    {
        public Tuple<string, List<Tuple<string, string>>> GetType(string objectName) 
        {
            Tuple<string, List<Tuple<string, string>>> result = null;
            var client = new WebClient();
            var yamlAddress = new Configuration().Get("SpotifyYaml");
            string yamlText = client.DownloadString("https://" + yamlAddress);
            var input = new StringReader(yamlText);
            var yaml = new YamlStream();
            yaml.Load(input);
            var mapping =(YamlMappingNode)yaml.Documents[0].RootNode;
            var spotifyObjects = new List<Tuple<string, List<Tuple<string,string>>>>();
            foreach (var entry in mapping.Children)
            {
                if (entry.Key.ToString()== "definitions") 
                {
                    var classes = (entry.Value as YamlMappingNode).Children.Keys;
                    foreach (var n in (entry.Value as YamlMappingNode).Children) 
                    {
                        List <Tuple<string, string>> propertiesTemp = new List<Tuple<string, string>>();
                        foreach (var cn in (n.Value as YamlMappingNode).Children)
                        {
                            if (cn.Key.ToString() == "properties")
                            {
                                var properties = (cn.Value as YamlMappingNode).Children.Keys;
                                Tuple<string, string> temp = null;
                                foreach (var gcn in (cn.Value as YamlMappingNode).Children)
                                {
                                    var tempVal = string.Empty;
                                    if ((gcn.Value as YamlMappingNode).Children.Where(c => c.Key.ToString() == "type").Count() > 0)
                                    {
                                        tempVal = (gcn.Value as YamlMappingNode).Children.Where(c => c.Key.ToString() == "type").FirstOrDefault().Value.ToString();
                                        temp = new Tuple<string, string>(gcn.Key.ToString(), tempVal);
                                        propertiesTemp.Add(temp);
                                    }
                                    else if ((gcn.Value as YamlMappingNode).Children.Where(c => c.Key.ToString() == "$ref").Count() > 0)
                                    {
                                        tempVal = (gcn.Value as YamlMappingNode).Children.Where(c => c.Key.ToString() == "$ref").FirstOrDefault().Value.ToString();
                                        tempVal = tempVal.Replace("#/definitions/", "");
                                        tempVal = tempVal.Replace("'", "");
                                        temp = new Tuple<string, string>(gcn.Key.ToString(), tempVal);
                                        propertiesTemp.Add(temp);
                                    }
                                }
                                break;
                            }
                        }
                        spotifyObjects.Add(new Tuple<string, List<Tuple<string, string>>>(n.Key.ToString(), propertiesTemp));
                    }
                }
            }
            result = spotifyObjects.Where(r => r.Item1 == objectName).FirstOrDefault();
            var resultText = JsonConvert.SerializeObject(result);
            if (!new Configuration().Set("SpotifyClasses", resultText)) 
            {
                result = null;
            }
            return result;
        }

    }
}
