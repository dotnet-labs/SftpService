using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace SFTPService
{
    public interface ISftpService
    {
        IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".");
        void UploadFile(string localFilePath, string remoteFilePath);
        void DownloadFile(string remoteFilePath, string localFilePath);
        void DeleteFile(string remoteFilePath);
    }

    public class SftpService : ISftpService
    {
        private readonly ILogger<SftpService> _logger;
        private readonly SftpConfig _config;

        public SftpService(ILogger<SftpService> logger, SftpConfig sftpConfig)
        {
            _logger = logger;
            _config = sftpConfig;
        }

        public IEnumerable<SftpFile> ListAllFiles(string remoteDirectory = ".")
        {
            using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
            try
            {
                client.Connect();
                return client.ListDirectory(remoteDirectory);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed in listing files under [{remoteDirectory}]", remoteDirectory);
                return new List<SftpFile>();
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void UploadFile(string localFilePath, string remoteFilePath)
        {
            using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
            try
            {
                client.Connect();
                using var s = File.OpenRead(localFilePath);
                client.UploadFile(s, remoteFilePath);
                _logger.LogInformation("Finished uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed in uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void DownloadFile(string remoteFilePath, string localFilePath)
        {
            using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
            try
            {
                client.Connect();
                using var s = File.Create(localFilePath);
                client.DownloadFile(remoteFilePath, s);
                _logger.LogInformation("Finished downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed in downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void DeleteFile(string remoteFilePath)
        {
            using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.UserName, _config.Password);
            try
            {
                client.Connect();
                client.DeleteFile(remoteFilePath);
                _logger.LogInformation("File [{remoteFilePath}] is deleted.", remoteFilePath);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed in deleting thefile [{remoteFilePath}]", remoteFilePath);
            }
            finally
            {
                client.Disconnect();
            }
        }
    }
}
