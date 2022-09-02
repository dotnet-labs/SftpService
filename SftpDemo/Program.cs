using Microsoft.Extensions.Logging;
using SFTPService;

var config = new SftpConfig
{
    Host = "test.rebex.net",
    Port = 22,
    UserName = "demo",
    Password = "password"
};

var logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<SftpService>();
var sftpService = new SftpService(logger, config);

// list files
Console.WriteLine("List files in the SFTP directory:");
var files = sftpService.ListAllFiles("/pub/example");
foreach (var file in files)
{
    if (file.IsDirectory)
    {
        Console.WriteLine($"\t Directory: [{file.FullName}]");
    }
    else if (file.IsRegularFile)
    {
        Console.WriteLine($"\t File: [{file.FullName}]");
    }
}
Console.WriteLine();

// download a file
Console.WriteLine("Download a file from the SFTP directory:");
const string pngFile = @"hi.png";
File.Delete(pngFile);
sftpService.DownloadFile(@"/pub/example/imap-console-client.png", pngFile);
if (File.Exists(pngFile))
{
    Console.WriteLine($"\t file {pngFile} downloaded");
}
Console.WriteLine();

// upload a file // not working for this demo SFTP server due to readonly permission
Console.WriteLine("Upload a file to the SFTP directory:");
var testFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
sftpService.UploadFile(testFile, @"/pub/test.txt");
sftpService.DeleteFile(@"/pub/test.txt");
Console.WriteLine();
