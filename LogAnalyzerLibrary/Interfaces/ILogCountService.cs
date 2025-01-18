using LogAnalyzerLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Interfaces
{
    public interface ILogCountService
    {
        Task<DirectorySearchResponse> CountTotalLogsAsync(string logFolder, DateTime startDate, DateTime endDate);
        Task<int> CountUniqueErrorsAsync(string logFolder);
        Task<int> CountDuplicatedErrorsAsync(string logFolder);
    }
}
