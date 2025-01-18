using LogAnalyzerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Interfaces
{
    public interface ILogUploadService
    {
        Task UploadLogsToRemoteServerAsync(UploadRequest request);
    }
}
