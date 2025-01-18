using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Models
{
    public class UploadRequest
    {
        public UploadRequest()
        {
            RemoteFolderPath = "";
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogFolder LocalFolder { get; set; }
        public string RemoteFolderPath { get; set; }
    }

    public enum LogFolder
    {
        AmadeoLogs,
        AWIErrors,
        Loggings
    }
}
