using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace NetCore21Utilities
{
    /// <summary>
    /// Configuration Utility
    /// This class use settings.json
    /// or settings{_Your Version_}.json eg.) settings1.1.json, settingsBata.json and so on.
    /// </summary>
    public class Configuration
    {
        IConfigurationRoot config { get; set; }
        public string Get(string key,string ver="Current") 
        {
            string result = string.Empty;
            if (ver == "Current")
            {
                result = config.GetSection(key).Value;
            }
            else 
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("settings" + ver + ".json");
                config = builder.Build();
                result = config.GetSection(key).Value;
            }
            return result;
        }
        public bool Set(string key,string val, string ver = "Current")
        {
            bool result = false;
            try 
            {
                if (ver == "Current")
                {
                    config.GetSection(key).Value = val;
                }
                else
                {
                    var builder = new ConfigurationBuilder();
                    builder.AddJsonFile("settings" + ver + ".json");
                    config = builder.Build();
                    config.GetSection(key).Value = val;
                }
                result = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return result;
        }
        public List<T> Read<T>(string key)
        {
            var configKey = "LogFilePath";
            List<T> result = new List<T>();
            switch (typeof(T).Name)
            {
                case "LogModel":
                    configKey = "LogFilePath";
                    break;
            }
            FileInfo file = new FileInfo(configKey);
            if (file.Exists)
            {
                var json = File.ReadAllText(file.FullName);
                if (!string.IsNullOrEmpty(json))
                {
                    result = JsonConvert.DeserializeObject<List<T>>(json);
                    switch (typeof(T).Name)
                    {
                        case "LogModel":
                            configKey = "LogFilePath";
                            break;
                    }
                }
            }
            return result;
        }
        public bool Write<T>(T val)
        {
            bool result = false;
            var configKey = "LogFilePath";
            switch (typeof(T).Name) 
            {
                case "LogModel":
                    configKey = "LogFilePath";
                    break;
            }
            FileInfo targetFile = new FileInfo(Get(configKey));
            if (!targetFile.Exists)
            {
                if (!targetFile.Directory.Exists)
                {
                    var drive = targetFile.Directory.FullName.Split(':').FirstOrDefault() + ":";
                    var pathroot = targetFile.Directory.FullName.Replace(drive + @"\", "");
                    foreach (var dir in pathroot.Split('\\'))
                    {
                        var current = new DirectoryInfo(drive + @"\" + dir);
                        if (!current.Exists) current.Create();
                    }
                }
                targetFile.Create();
            }
            var keyValue = File.ReadAllText(targetFile.FullName);
            var logs = new List<T>();
            if (!string.IsNullOrEmpty(keyValue))
            {
                logs = JsonConvert.DeserializeObject<List<T>>(keyValue);
            }
            logs.Add(val);
            var json = JsonConvert.SerializeObject(logs);
            using (StreamWriter sw = File.CreateText(targetFile.FullName))
            {
                sw.Write(json);
                result = true;
            }
            return result;
        }
        public Configuration() 
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("settings.json");
            config = builder.Build();
        }
    }
}
