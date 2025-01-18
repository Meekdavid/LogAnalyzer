using LogAnalyzerLibrary.Helpers.FolderHelper;
using LogAnalyzerLibrary.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Repositories
{
    public class LogManagementRepository : ILogManagementService
    {
        /// <summary>
        /// Asynchronously deletes log files in the specified directory within the given date range.
        /// </summary>
        /// <param name="directoryPath">The path to the directory where log files are stored.</param>
        /// <param name="startDate">The start date of the period within which log files will be deleted.</param>
        /// <param name="endDate">The end date of the period within which log files will be deleted.</param>
        /// <remarks>
        /// This method searches the specified directory for log files whose last write time falls within the specified period
        /// and deletes them. It logs each deleted file and provides information on successful deletion completion.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to access the directory or delete the files.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing or deleting the files in the directory.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        public async Task ArchiveLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            try
            {
                Log.Information("Deleting logs in directory: {DirectoryPath} for period {StartDate} - {EndDate}", logFolder, startDate, endDate);

                // Find the folder location in drive
                string fullFilePath = await FolderSearchHelper.RetrieveFolderLocation(logFolder);

                var filesToDelete = Directory.EnumerateFiles(fullFilePath)
                    .Where(file => File.GetLastWriteTime(file) >= startDate && File.GetLastWriteTime(file) <= endDate);

                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                    Log.Information("Deleted file: {FilePath}", file);
                }

                Log.Information("Logs deletion completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting logs in directory: {DirectoryPath}", logFolder);
            }
        }


        /// <summary>
        /// Asynchronously deletes an archived log file in the specified directory within the given date range.
        /// </summary>
        /// <param name="directoryPath">The path to the directory where the archived log file is stored.</param>
        /// <param name="startDate">The start date of the period for which the archive is named.</param>
        /// <param name="endDate">The end date of the period for which the archive is named.</param>
        /// <remarks>
        /// This method constructs the archive file name based on the specified date range (in the format "dd_MM_yyyy-dd_MM_yyyy.zip"),
        /// checks if the archive exists, and deletes it. It logs the deletion of the archive or warns if the archive is not found.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to delete the archive file.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing or deleting the archive file.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        public async Task DeleteArchivedLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            try
            {
                Log.Information("Deleting archived logs in directory: {DirectoryPath} for period {StartDate} - {EndDate}", logFolder, startDate, endDate);

                var archiveName = $"{logFolder}\\{startDate:dd_MM_yyyy}-{endDate:dd_MM_yyyy}.zip";
                if (File.Exists(archiveName))
                {
                    File.Delete(archiveName);
                    Log.Information("Deleted archive: {ArchiveName}", archiveName);
                }
                else
                {
                    Log.Warning("Archive not found: {ArchiveName}", archiveName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting archived logs in directory: {DirectoryPath}", logFolder);
            }
        }

        /// <summary>
        /// Asynchronously deletes log files in the specified directory within the given date range.
        /// </summary>
        /// <param name="directoryPath">The path to the directory where the log files are located.</param>
        /// <param name="startDate">The start date of the period for which the log files will be deleted.</param>
        /// <param name="endDate">The end date of the period for which the log files will be deleted.</param>
        /// <remarks>
        /// This method enumerates through the log files in the specified directory, checks their last write time, and deletes files
        /// that fall within the specified date range. The method logs each deleted file and the completion of the deletion process.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is a lack of permission to delete the log files.
        /// </exception>
        /// <exception cref="IOException">
        /// Thrown if an error occurs while accessing or deleting the log files.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// Thrown if the specified directory does not exist.
        /// </exception>
        public async Task DeleteLogsByPeriodAsync(string logFolder, DateTime startDate, DateTime endDate)
        {
            try
            {
                Log.Information("Deleting logs in directory: {DirectoryPath} for period {StartDate} - {EndDate}", logFolder, startDate, endDate);

                // Find the folder location in drive
                string fullFilePath = await FolderSearchHelper.RetrieveFolderLocation(logFolder);

                var filesToDelete = Directory.EnumerateFiles(fullFilePath)
                    .Where(file => File.GetLastWriteTime(file) >= startDate && File.GetLastWriteTime(file) <= endDate);

                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                    Log.Information("Deleted file: {FilePath}", file);
                }

                Log.Information("Logs deletion completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting logs in directory: {DirectoryPath}", logFolder);
            }
        }
    }
}
