using LogAnalyzerLibrary.Interfaces;
using LogAnalyzerLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzerAPI.Controllers
{
    /// <summary>
    /// Controller to manage and analyze log files.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LogsAnalyzerController : ControllerBase
    {
        private readonly ILogCountService _logCountService;
        private readonly ILogManagementService _logManagementService;
        private readonly ILogSearchService _logSearchService;
        private readonly ILogUploadService _logUploadService;

        public LogsAnalyzerController(
            ILogCountService logCountService,
            ILogManagementService logManagementService,
            ILogSearchService logSearchService,
            ILogUploadService logUploadService)
        {
            _logCountService = logCountService;
            _logManagementService = logManagementService;
            _logSearchService = logSearchService;
            _logUploadService = logUploadService;
        }

        /// <summary>
        /// Counts the total logs in a specified directory within a date range.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <param name="startDate">Start date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        /// <param name="endDate">End date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        /// <returns>The total number of logs.</returns>
        [HttpGet("count/total")]
        public async Task<IActionResult> CountTotalLogsAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            var count = await _logCountService.CountTotalLogsAsync(logFolder, startDate, endDate);
            return Ok(count);
        }

        /// <summary>
        /// Counts unique errors in a specified log file.
        /// </summary>
        /// <param name="logFolder">The path of the log file.</param>
        /// <returns>The number of unique errors.</returns>
        [HttpGet("count/unique-errors")]
        public async Task<IActionResult> CountUniqueErrorsAsync(string logFolder)
        {
            var count = await _logCountService.CountUniqueErrorsAsync(logFolder);
            return Ok(count);
        }

        /// <summary>
        /// Counts duplicated errors in a specified log file.
        /// </summary>
        /// <param name="logFolder">The path of the log file.</param>
        /// <returns>The number of duplicated errors.</returns>
        [HttpGet("count/duplicated-errors")]
        public async Task<IActionResult> CountDuplicatedErrorsAsync(string logFolder)
        {
            var count = await _logCountService.CountDuplicatedErrorsAsync(logFolder);
            return Ok(count);
        }

        /// <summary>
        /// Deletes logs in a directory within a date range.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <param name="startDate">Start date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        /// <param name="endDate">End date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            await _logManagementService.DeleteLogsByPeriodAsync(logFolder, startDate, endDate);
            return NoContent();
        }

        /// <summary>
        /// Archives logs in a directory within a date range.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <param name="startDate">Start date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        /// <param name="endDate">End date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        [HttpPost("archive")]
        public async Task<IActionResult> ArchiveLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            await _logManagementService.ArchiveLogsByPeriodAsync(logFolder, startDate, endDate);
            return NoContent();
        }

        /// <summary>
        /// Deletes archived logs in a directory within a date range.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <param name="startDate">Start date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        /// <param name="endDate">End date of the range. Format is <b>2025-01-15T14:30:00</b></param>
        [HttpDelete("delete-archived")]
        public async Task<IActionResult> DeleteArchivedLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            await _logManagementService.DeleteArchivedLogsByPeriodAsync(logFolder, startDate, endDate);
            return NoContent();
        }

        /// <summary>
        /// Searches logs in directories with optional subdirectory inclusion.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <param name="searchPattern">The search pattern (e.g., "*.log").</param>
        /// <param name="includeSubdirectories">Whether to include subdirectories.</param>
        /// <returns>A list of matching log file paths.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchLogsInDirectoriesAsync(string logFolder, string searchPattern, bool includeSubdirectories)
        {
            var results = await _logSearchService.SearchLogsInDirectoriesAsync(logFolder, searchPattern, includeSubdirectories);
            return Ok(results);
        }

        /// <summary>
        /// Searches logs in a directory by size range.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <param name="minSizeKb">Minimum size in kilobytes.</param>
        /// <param name="maxSizeKb">Maximum size in kilobytes.</param>
        /// <returns>A list of matching log file paths.</returns>
        [HttpGet("search/size")]
        public async Task<IActionResult> SearchLogsBySizeAsync(string logFolder, long minSizeKb, long maxSizeKb)
        {
            var results = await _logSearchService.SearchLogsBySizeAsync(logFolder, minSizeKb, maxSizeKb);
            return Ok(results);
        }

        /// <summary>
        /// Searches logs in a specific directory.
        /// </summary>
        /// <param name="logFolder">The directory to search.</param>
        /// <returns>A list of log file paths.</returns>
        [HttpGet("search/directory")]
        public async Task<IActionResult> SearchLogsByDirectoryAsync(string logFolder)
        {
            var results = await _logSearchService.SearchLogsByDirectoryAsync(logFolder);
            return Ok(results);
        }

        /// <summary>
        /// Uploads logs to a remote server via API.
        /// </summary>
        /// <param name="request">The upload request details.</param>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadLogsToRemoteServerAsync([FromBody] UploadRequest request)
        {
            await _logUploadService.UploadLogsToRemoteServerAsync(request);
            return NoContent();
        }
    }
}
