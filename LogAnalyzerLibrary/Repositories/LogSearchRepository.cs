using LogAnalyzerLibrary.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Repositories
{
    public class LogSearchRepository : ILogSearchService
    {
        /// <summary>
        /// Asynchronously searches for log files in the specified directory.
        /// </summary>
        /// <param name="directoryPath">The path to the directory where the log files will be searched.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of strings representing the paths of the log files found in the specified directory.
        /// If an error occurs, an empty sequence will be returned.
        /// </returns>
        /// <remarks>
        /// This method enumerates through the files in the specified directory and returns the file paths as a sequence.
        /// The method logs the search process and any errors that occur.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to access the specified directory.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing the directory or its files.
        /// </exception>
        public async Task<IEnumerable<string>> SearchLogsByDirectoryAsync(string logFolder)
        {
            try
            {
                Log.Information("Searching logs in directory: {DirectoryPath}", logFolder);

                var result = Directory.EnumerateFiles(logFolder);

                Log.Information("Search in directory completed successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while searching logs in directory: {DirectoryPath}", logFolder);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Asynchronously searches for log files in the specified directory by size range.
        /// </summary>
        /// <param name="logFolder">The path to the directory where the log files will be searched.</param>
        /// <param name="minSizeKb">The minimum file size in kilobytes (KB).</param>
        /// <param name="maxSizeKb">The maximum file size in kilobytes (KB).</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of strings representing the paths of the log files found in the specified directory
        /// that match the specified size range.
        /// If an error occurs, an empty sequence will be returned.
        /// </returns>
        /// <remarks>
        /// This method enumerates through the files in the specified directory, checking if their size falls within the specified
        /// range and returns the file paths as a sequence.
        /// The method logs the search process and any errors that occur.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to access the specified directory.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing the directory or its files.
        /// </exception>
        public async Task<IEnumerable<string>> SearchLogsBySizeAsync(string logFolder, long minSizeKb, long maxSizeKb)
        {
            try
            {
                Log.Information("Searching logs by size in directory: {DirectoryPath}", logFolder);

                var result = Directory.EnumerateFiles(logFolder)
                    .Where(file =>
                    {
                        var sizeInKb = new FileInfo(file).Length / 1024;
                        return sizeInKb >= minSizeKb && sizeInKb <= maxSizeKb;
                    });

                Log.Information("Search by size completed successfully.");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while searching logs by size in directory: {DirectoryPath}", logFolder);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Asynchronously searches for log files in the specified directory and its subdirectories (if specified) using a search pattern.
        /// </summary>
        /// <param name="directoryPath">The path to the directory where the log files will be searched.</param>
        /// <param name="searchPattern">The search pattern to match the log files. Use '*' for all files, or specify a pattern like '*.log'.</param>
        /// <param name="includeSubdirectories">A boolean value indicating whether subdirectories should be included in the search.
        /// If true, all subdirectories will be included in the search; otherwise, only the top-level directory is searched.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of strings representing the paths of the log files found in the specified directory
        /// and its subdirectories (if applicable), matching the search pattern.
        /// If an error occurs, an empty sequence will be returned.
        /// </returns>
        /// <remarks>
        /// This method performs a file search in the specified directory based on the given search pattern and directory depth (including or excluding subdirectories).
        /// The method logs the search process and any errors that occur.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to access the specified directory or its contents.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the search pattern is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing the directory or its files.
        /// </exception>
        public async Task<IEnumerable<string>> SearchLogsInDirectoriesAsync(string logFolder, string searchPattern, bool includeSubdirectories)
        {
            try
            {
                Log.Information("Searching logs in directories: {DirectoryPath}", logFolder);

                var option = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                var result = Directory.GetFiles(logFolder, searchPattern, option);

                Log.Information("Search completed successfully.");
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while searching logs in directories: {DirectoryPath}", logFolder);
                return Enumerable.Empty<string>();
            }
        }
    }
}
