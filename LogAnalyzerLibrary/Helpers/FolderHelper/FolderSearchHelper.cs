using LogAnalyzerLibrary.Helpers.ConfigurationSettings.ConfigManager;
using LogAnalyzerLibrary.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Helpers.FolderHelper
{
    public class FolderSearchHelper
    {

        private static string _foundDirectoryPath = string.Empty;

        /// <summary>
        /// Returns the folder path associated with a specific log folder enum value.
        /// </summary>
        /// <param name="logFolder">The enum value representing the log folder.</param>
        /// <returns>
        /// A string representing the folder path corresponding to the specified enum value.
        /// If the enum value does not match any predefined values, null is returned.
        /// </returns>
        /// <remarks>
        /// This method takes a <see cref="LogFolder"/> enum value and returns a string representing the corresponding folder path.
        /// If the provided enum value does not match any of the defined cases, the method returns null.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown if an invalid value is passed as the <paramref name="logFolder"/> parameter.
        /// </exception>
        public static string GetFolderPathFromEnum(LogFolder logFolder)
        {
            switch (logFolder)
            {
                case LogFolder.AmadeoLogs:
                    return "AmadeoLogs";
                case LogFolder.AWIErrors:
                    return "AWIErrors";
                case LogFolder.Loggings:
                    return "Loggings";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Searches for a specific directory within a root directory, recursively, and counts the files within it that fall within a given date range.
        /// </summary>
        /// <param name="rootDirectory">The path of the root directory in which to start the search.</param>
        /// <param name="directoryName">The name of the directory to search for.</param>
        /// <param name="startDate">The start date for filtering files based on their creation date. If null, no start date filter is applied.</param>
        /// <param name="endDate">The end date for filtering files based on their creation date. If null, no end date filter is applied.</param>
        /// <returns>
        /// The total count of files within the matching directories that meet the date range criteria.
        /// </returns>
        /// <remarks>
        /// This method recursively searches for directories matching the specified directory name within the given root directory. For each matching directory,
        /// it counts the files within it that fall within the specified date range. The method uses parallel processing to handle multiple directories efficiently.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if access to a directory is denied during the search.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if an error occurs during the search or while accessing directories or files.
        /// </exception>
        public static async Task<int> SearchDirectoryAndReturnCount(string rootDirectory, string directoryName, DateTime? startDate, DateTime? endDate)
        {
            int fileCount = 0;

            try
            {
                var restrictedDirectories = ConfigSettings.ApplicationSetting.NotAllowedSearchFolders;

                var directories = Directory.EnumerateDirectories(rootDirectory, "*");
                Console.WriteLine($"The current directory is: {JsonConvert.SerializeObject(directories)} \r\n");

                var allowedDirectories = directories
                    .Where(directory => !restrictedDirectories
                    .Any(restricted => directory.Contains(restricted, StringComparison.OrdinalIgnoreCase)));

                Console.WriteLine($"The current allowed directory is: {JsonConvert.SerializeObject(allowedDirectories)} \r\n");

                Parallel.ForEach(allowedDirectories, async directory =>
                {
                    try
                    {
                        // Check if the directory matches the target directory name
                        if (directory.EndsWith(directoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"The found directory is: {directory}");
                            int count = CountFiles(directory, startDate, endDate);
                            Interlocked.Add(ref fileCount, count); // Thread-safe addition
                        }
                        else
                        {
                            // Recursively search the subdirectory
                            int count = await SearchDirectoryAndReturnCount(directory, directoryName, startDate, endDate);
                            Interlocked.Add(ref fileCount, count); // Thread-safe addition
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Unable to process directory {directory} because {ex.Message}", ex);
                        Console.WriteLine($"Unable to process directory {directory} because {ex.Message}", ex);
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to some directories. Skipping... {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return fileCount;
        }

        /// <summary>
        /// Counts the number of files in a specified directory that fall within a given date range.
        /// </summary>
        /// <param name="directory">The path of the directory in which to count files.</param>
        /// <param name="startDate">The start date for the date range. If null, no start date filter is applied.</param>
        /// <param name="endDate">The end date for the date range. If null, no end date filter is applied.</param>
        /// <returns>
        /// The total count of files that meet the date range criteria.
        /// </returns>
        /// <remarks>
        /// This method iterates through all files in the specified directory, checks the creation time of each file, and compares it with the provided date range.
        /// If the file's creation time falls within the range, it is counted.
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown if an error occurs while accessing the directory or processing files.
        /// </exception>
        private static int CountFiles(string directory, DateTime? startDate, DateTime? endDate)
        {
            int count = 0;

            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    // Get the creation time of the file
                    DateTime creationTime = File.GetCreationTime(file);

                    // Check if the file falls within the date range (if specified)
                    if ((!startDate.HasValue || creationTime >= startDate.Value) &&
                        (!endDate.HasValue || creationTime <= endDate.Value))
                    {
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while counting files: {ex.Message}");
            }

            return count;
        }

        /// <summary>
        /// Searches for a directory by name within the specified folder and its subdirectories, excluding restricted directories.
        /// </summary>
        /// <param name="currentFolder">The path of the folder in which to begin the search.</param>
        /// <param name="directoryName">The name of the directory to search for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the path of the found directory, or an empty string if not found.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="Directory.EnumerateDirectories"/> to iterate through directories and excludes those that are in the restricted directories list.
        /// It searches recursively in subdirectories and stops further searching once the directory is found.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when access to a directory is denied during the search process.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if any error occurs during the search process, such as an unexpected failure in the directory enumeration or while processing directories.
        /// </exception>
        public static async Task<string> FindDirectoryPathFormer(string currentFolder, string directoryName)
        {
            string fullFilePath = "";

            try
            {
                var restrictedDirectories = ConfigSettings.ApplicationSetting.NotAllowedSearchFolders;

                var options = new EnumerationOptions();
                options.AttributesToSkip = FileAttributes.Hidden;

                var directories = Directory.EnumerateDirectories(currentFolder, "*", options);
                Console.WriteLine($"The current directory is: {JsonConvert.SerializeObject(directories)} \r\n");

                var allowedDirectories = directories
                    .Where(directory => !restrictedDirectories
                    .Any(restricted => directory.Contains(restricted, StringComparison.OrdinalIgnoreCase)));

                Console.WriteLine($"The current allowed directory is: {JsonConvert.SerializeObject(allowedDirectories)} \r\n");

                Parallel.ForEach(allowedDirectories, (directory, state) =>
                {
                    try
                    {
                        // Check if the directory matches the target directory name
                        if (directory.EndsWith(directoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"The found directory is: {directory}");
                            fullFilePath = directory;
                            state.Stop();
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(fullFilePath))
                            {
                                // Recursively search the subdirectory
                                FindDirectoryPath(directory, directoryName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Unable to process directory {directory} because {ex.Message}", ex);
                        Console.WriteLine($"Unable to process directory {directory} because {ex.Message}", ex);
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to some directories. Skipping... {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return fullFilePath;
        }

        /// <summary>
        /// Searches for a directory by name within the specified folder and its subdirectories, excluding restricted directories.
        /// </summary>
        /// <param name="currentFolder">The path of the folder in which to begin the search.</param>
        /// <param name="directoryName">The name of the directory to search for.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the path of the found directory, or an empty string if not found.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="Directory.EnumerateDirectories"/> to iterate through directories and excludes those that are in the restricted directories list.
        /// It also supports cancellation if the directory is found before the entire search completes.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when access to a directory is denied during the search process.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if any error occurs during the search process, such as an unexpected failure in the directory enumeration or while processing directories.
        /// </exception>
        public static async Task<string> FindDirectoryPath(string currentFolder, string directoryName)
        {
            try
            {
                // Reset the found directory path before each search
                _foundDirectoryPath = string.Empty;

                var restrictedDirectories = ConfigSettings.ApplicationSetting.NotAllowedSearchFolders;
                var options = new EnumerationOptions { AttributesToSkip = FileAttributes.Hidden };

                var directories = Directory.EnumerateDirectories(currentFolder, "*", options);
                var allowedDirectories = directories
                    .Where(directory => !restrictedDirectories
                        .Any(restricted => directory.Contains(restricted, StringComparison.OrdinalIgnoreCase)));

                await Parallel.ForEachAsync(allowedDirectories, async (directory, token) =>
                {
                    if (_foundDirectoryPath != string.Empty)
                    {
                        token.ThrowIfCancellationRequested(); // Cancel the remaining iterations
                        return;
                    }

                    try
                    {
                        if (directory.EndsWith(directoryName, StringComparison.OrdinalIgnoreCase))
                        {
                            _foundDirectoryPath = directory;
                            token.ThrowIfCancellationRequested(); // Cancel the remaining iterations
                        }
                        else
                        {
                            // Recursively search the subdirectory
                            await FindDirectoryPath(directory, directoryName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Unable to process directory {directory} because {ex.Message}", ex);
                        Console.WriteLine($"Unable to process directory {directory} because {ex.Message}", ex);
                    }
                });

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to some directories. Skipping... {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return _foundDirectoryPath;
        }

        /// <summary>
        /// Grants full control permissions to "Everyone" for the specified folder.
        /// </summary>
        /// <param name="folderPath">The path of the folder to modify permissions for.</param>
        public static void GrantFullControlToFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Console.WriteLine($"Invalid folder path: {folderPath}");
                return;
            }

            try
            {
                var directoryInfo = new DirectoryInfo(folderPath);
                var directorySecurity = directoryInfo.GetAccessControl();

                // Add a new rule granting Everyone full control
                var rule = new FileSystemAccessRule(
                    "Everyone",
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                directorySecurity.AddAccessRule(rule);

                // Apply the new access rules
                directoryInfo.SetAccessControl(directorySecurity);

                Console.WriteLine($"Permissions granted successfully to 'Everyone' for the folder: {folderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while granting permissions: {ex.Message}");
            }
        }

        public static async Task<string> RetrieveFolderLocation(string folder)
        {
            string fullFilePath = string.Empty;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!string.IsNullOrEmpty(fullFilePath)) break;

                if (drive.IsReady)
                {
                    fullFilePath = await FolderSearchHelper
                    .FindDirectoryPath(drive.Name, folder);
                }
            }

            return fullFilePath;
        }

    }

}
