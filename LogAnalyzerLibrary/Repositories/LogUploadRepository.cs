using LogAnalyzerLibrary.Helpers.ConfigurationSettings.ConfigManager;
using LogAnalyzerLibrary.Helpers.FolderHelper;
using LogAnalyzerLibrary.Interfaces;
using LogAnalyzerLibrary.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Repositories
{
    public class LogUploadRepository : ILogUploadService
    {
        /// <summary>
        /// Asynchronously uploads log files from a local directory to a remote server via SFTP.
        /// </summary>
        /// <param name="request">An <see cref="UploadRequest"/> object containing the details of the SFTP upload request, such as the server address, port, username, password, local folder, and remote folder.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method connects to an SFTP server using the provided credentials, ensures that the specified remote directory exists (creating it if necessary),
        /// and uploads log files from the local directory to the remote server. It logs each step of the process, including the creation of the directory and the uploading of each file.
        /// If an error occurs, it is logged, and the operation is rethrown.
        /// </remarks>
        /// <exception cref="Renci.SshNet.SshConnectionException">
        /// Thrown if there is an issue establishing the SFTP connection, such as authentication failure or network issues.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown if there is an issue accessing files or directories on the local system or the remote server.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if there is insufficient permission to access files or directories either locally or remotely.
        /// </exception>
        /// <exception cref="System.Exception">
        /// Any other errors that may occur during the file upload process.
        /// </exception>
        public async Task UploadLogsToRemoteServerAsync(UploadRequest request)
        {
            var sftpCredentials = ConfigSettings.ApplicationSetting.SFTPCredential;
            using var sftp = new Renci.SshNet.SftpClient(sftpCredentials.Host, sftpCredentials.Port, sftpCredentials.Username, sftpCredentials.Password);

            try
            {
                // Connect to the SFTP server
                sftp.Connect();
                Log.Information("Connected to the SFTP server: {ServerAddress}", sftpCredentials.Host);

                // Ensure the remote directory exists
                if (!sftp.Exists(request.RemoteFolderPath))
                {
                    sftp.CreateDirectory(request.RemoteFolderPath);
                    Log.Information("Created remote directory: {RemoteFolderPath}", request.RemoteFolderPath);
                }

                // Find the folder location in drive
                string fullFilePath = await FolderSearchHelper.RetrieveFolderLocation(request.LocalFolder.ToString());

                // Upload files from the local folder
                var files = Directory.GetFiles(fullFilePath, "*");

                foreach (var file in files)
                {
                    using var fileStream = File.OpenRead(file);

                    var remoteFilePath = string.IsNullOrEmpty(request.RemoteFolderPath)
                        ? Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(file))
                        : Path.Combine(request.RemoteFolderPath, Path.GetFileName(file));

                    sftp.UploadFile(fileStream, remoteFilePath);
                    Log.Information("Uploaded file: {FilePath} to {RemoteFilePath}", file, remoteFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during SFTP upload.");
                throw;
            }
            finally
            {
                // Disconnect from the SFTP server
                sftp.Disconnect();
                Log.Information("Disconnected from the SFTP server.");
            }
        }
    }
}
