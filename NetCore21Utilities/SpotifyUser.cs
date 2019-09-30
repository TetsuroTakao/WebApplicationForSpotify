using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore21Utilities
{
    public class SpotifyUser
    {
        public string country { get; set; }
        public string display_name { get; set; }
        public string email { get; set; }
        public explicit_content_Type explicit_content { get; set; }
        public external_urls_type external_urls { get; set; }
        public followers_type followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<image_type> images { get; set; }
        public string product { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }
    public class explicit_content_Type
    {
        public bool filter_enabled { get; set; }
        public bool filter_locked { get; set; }
    }
    public class external_urls_type
    {
        public string spotify { get; set; }
    }
    public class followers_type
    {
        public string href { get; set; }
        public int total { get; set; }
    }
    public class image_type
    {
        public Nullable<int> height { get; set; }
        public string url { get; set; }
        public Nullable<int> width { get; set; }
    }
}