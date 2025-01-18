using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Helpers.ConfigurationSettings.ConfigModels
{
    public class ApplicationSettings
    {
        public string[] NotAllowedSearchFolders { get; set; }
        public string PathForPermission { get; set; }

        public SFTPCredential SFTPCredential { get; set; }
    }

    public class SFTPCredential
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
