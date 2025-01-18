using LogAnalyzerLibrary.DTOs;
using LogAnalyzerLibrary.Helpers.FolderHelper;
using LogAnalyzerLibrary.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Repositories
{
    public class LogCountRepository : ILogCountService
    {
        /// <summary>
        /// Asynchronously counts the number of duplicated errors in a specified log file.
        /// </summary>
        /// <param name="logFilePath">The path to the log file to analyze for duplicated errors.</param>
        /// <returns>
        /// The number of duplicated errors in the log file. If an error occurs during the operation, 0 is returned.
        /// </returns>
        /// <remarks>
        /// This method reads the lines from the log file, compares them to identify duplicated errors, 
        /// and returns the count of the duplicated entries.
        /// </remarks>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while reading the log file.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if the caller does not have permission to access the log file.
        /// </exception>
        public async Task<int> CountDuplicatedErrorsAsync(string logFolder)
        {
            try
            {
                Log.Information("Counting duplicated errors in log file: {LogFilePath}", logFolder);

                // Find the folder location in drive
                string fullFilePath = await FolderSearchHelper.RetrieveFolderLocation(logFolder);

                var lines = await File.ReadAllLinesAsync(fullFilePath);

                var count = lines.Length - lines.Distinct().Count();

                Log.Information("Duplicated errors counted: {Count}", count);

                return count;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while counting duplicated errors in log file: {LogFilePath}", logFolder);
                return 0;
            }
        }

        /// <summary>
        /// Asynchronously counts the total number of log files in the specified directory within the given date range.
        /// </summary>
        /// <param name="directoryName">The name of the directory to search for log files.</param>
        /// <param name="startDate">The start date of the range to search for log files.</param>
        /// <param name="endDate">The end date of the range to search for log files.</param>
        /// <returns>
        /// A <see cref="DirectorySearchResponse"/> object containing the count of the log files and the full file path of the found directory.
        /// If no logs are found, the message will indicate "Log Not Found"; otherwise, it will indicate the total count of logs found.
        /// </returns>
        /// <remarks>
        /// This method searches through all drives, retrieves directories matching the specified name, and counts the log files created
        /// within the provided date range. It breaks the loop once the directory with log files is found.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to access any directories during the search.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing or reading files during the search process.
        /// </exception>
        public async Task<DirectorySearchResponse> CountTotalLogsAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            var response = new DirectorySearchResponse();
            try
            {
                Log.Information("Counting total logs in directory: {DirectoryPath} for period {StartDate} - {EndDate}", logFolder, startDate, endDate);

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        response.filesCount = await FolderSearchHelper.SearchDirectoryAndReturnCount(drive.RootDirectory.FullName, logFolder, startDate, endDate);
                        response.FullFilePath = await FolderSearchHelper.FindDirectoryPath(drive.Name, logFolder);
                        if (response.filesCount > 0) break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while counting total logs in directory: {DirectoryPath}", logFolder);
                return response;
            }
            response.message = response.filesCount == 0 ? "Log Not Found" : $"Log Folder Found with total of {response.filesCount} Logs";
            return response;
        }

        /// <summary>
        /// Asynchronously counts the unique errors in the specified log file.
        /// </summary>
        /// <param name="logFilePath">The path to the log file where errors are to be counted.</param>
        /// <returns>
        /// The number of unique errors found in the log file.
        /// </returns>
        /// <remarks>
        /// This method searches through all available drives to locate the specified log file, reads its contents,
        /// and counts the number of unique error entries by removing duplicates.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to access any directories or files during the search or reading process.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing or reading the log file during the search process.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown if the log file is not found during the directory search process.
        /// </exception>
        public async Task<int> CountUniqueErrorsAsync(string logFolder)
        {
            try
            {
                Log.Information("Counting unique errors in log file: {LogFilePath}", logFolder);

                // Find the folder location in drive
                string fullFilePath = await FolderSearchHelper.RetrieveFolderLocation(logFolder);

                var lines = await File.ReadAllLinesAsync(fullFilePath);
                var count = lines.Distinct().Count();

                //int count = 0;
                //using (var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                //using (var reader = new StreamReader(fileStream))
                //{
                //    var content = await reader.ReadToEndAsync();
                //    var lines = content.Split(Environment.NewLine);
                //    count = lines.Distinct().Count();
                //}

                Log.Information("Unique errors counted: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while counting unique errors in log file: {LogFilePath}", logFolder);
                return 0;
            }
        }
    }
}
