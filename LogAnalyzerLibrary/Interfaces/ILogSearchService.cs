using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Interfaces
{
    public interface ILogSearchService
    {
        Task<IEnumerable<string>> SearchLogsInDirectoriesAsync(string logFolder, string searchPattern, bool includeSubdirectories);
        Task<IEnumerable<string>> SearchLogsBySizeAsync(string logFolder, long minSizeKb, long maxSizeKb);
        Task<IEnumerable<string>> SearchLogsByDirectoryAsync(string logFolder);
    }
}
