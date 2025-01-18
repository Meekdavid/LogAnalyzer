# Log Analyzer Library

## Overview
The **Log Analyzer Library** is a powerful tool designed to help manage, search, analyze, and manipulate log files across directories. It supports advanced features like recursive directory traversal, filtering logs by size or date, and uploading logs to remote servers via SFTP.

## Features
### User Stories
1. **US-9: Search Logs in Directories**
   - Traverse directories to locate and return log files based on patterns and filters (e.g., name, size, date).

2. **US-10: Count Total Logs in a Period**
   - Count the total number of logs within a specific date range.

3. **US-12: Count Unique Errors**
   - Identify and count unique errors in log files.

4. **US-13: Count Duplicated Errors**
   - Count duplicate occurrences of errors within log files.

5. **US-16: Archive Logs by Period**
   - Archive log files into a ZIP file based on a specified date range.

6. **US-17: Delete Archived Logs**
   - Delete archived logs from a specified period.

7. **US-18: Upload Logs to Remote Server**
   - Upload logs to a remote server's specific directory using SFTP.

## Getting Started
### Prerequisites
- **.NET 6.0 or higher**
- **NuGet Packages**:
  - `SSH.NET` (for SFTP operations)
  - `Serilog` (for logging)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/log-analyzer-library.git
   ```

2. Navigate to the project directory:
   ```bash
   cd log-analyzer-library
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Build the project:
   ```bash
   dotnet build
   ```

## Usage
### Example: Search Logs in Directories
```csharp
ILogSearchService logSearchService = new LogSearchRepository();
var logs = logSearchService.SearchLogsInDirectories("C:\\Logs", "*.log", true);
foreach (var log in logs)
{
    Console.WriteLine(log);
}
```

### Example: Upload Logs to a Remote Server
```csharp
ILogUploadService logUploadService = new LogUploadRepository();
await logUploadService.UploadLogsToRemoteServerAsync(
    "C:\\Logs\\sample.log",
    "sftp.remoteserver.com",
    22,
    "username",
    "password",
    "/remote/logs"
);
```

### Running the API
1. Run the application:
   ```bash
   dotnet run
   ```

2. Use tools like Postman or cURL to interact with the API endpoints.

## Project Structure
```
log-analyzer-library/
├── Controllers/           # API controllers
├── Models/                # Data models
├── Services/              # Business logic
├── Repositories/          # Data handling logic
├── Program.cs             # Entry point
└── README.md              # Project documentation
```

## Contributing
1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add your message here"
   ```
4. Push to the branch:
   ```bash
   git push origin feature/your-feature-name
   ```
5. Create a pull request.

