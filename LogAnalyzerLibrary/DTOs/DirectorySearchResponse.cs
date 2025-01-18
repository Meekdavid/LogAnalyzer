using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.DTOs
{
    public class DirectorySearchResponse
    {
        public string message { get; set; }
        public string FullFilePath { get; set; }
        public int filesCount {  get; set; }
    }
}
