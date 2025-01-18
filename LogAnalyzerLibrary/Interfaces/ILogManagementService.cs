using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Interfaces
{
    public interface ILogManagementService
    {
        Task DeleteLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate);
        Task ArchiveLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate);
        Task DeleteArchivedLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate);
    }
}
