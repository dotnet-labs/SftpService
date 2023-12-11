using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace SFTPService
{
    public interface ISftpService
    {
        IEnumerable<ISftpFile> ListAllFiles(string remoteDirectory = ".");
        void UploadFile(string localFilePath, string remoteFilePath);
        void DownloadFile(string remoteFilePath, string localFilePath);
        void DeleteFile(string remoteFilePath);
    }

    public class SftpService(ILogger<SftpService> logger, SftpConfig sftpConfig) : ISftpService
    {
        public IEnumerable<ISftpFile> ListAllFiles(string remoteDirectory = ".")
        {
            using var client = new SftpClient(sftpConfig.Host, sftpConfig.Port == 0 ? 22 : sftpConfig.Port, sftpConfig.UserName, sftpConfig.Password);
            try
            {
                client.Connect();
                return client.ListDirectory(remoteDirectory);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed in listing files under [{remoteDirectory}]", remoteDirectory);
                return new List<ISftpFile>();
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void UploadFile(string localFilePath, string remoteFilePath)
        {
            using var client = new SftpClient(sftpConfig.Host, sftpConfig.Port == 0 ? 22 : sftpConfig.Port, sftpConfig.UserName, sftpConfig.Password);
            try
            {
                client.Connect();
                using var s = File.OpenRead(localFilePath);
                client.UploadFile(s, remoteFilePath);
                logger.LogInformation("Finished uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed in uploading the file [{localFilePath}] to [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void DownloadFile(string remoteFilePath, string localFilePath)
        {
            using var client = new SftpClient(sftpConfig.Host, sftpConfig.Port == 0 ? 22 : sftpConfig.Port, sftpConfig.UserName, sftpConfig.Password);
            try
            {
                client.Connect();
                using var s = File.Create(localFilePath);
                client.DownloadFile(remoteFilePath, s);
                logger.LogInformation("Finished downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed in downloading the file [{localFilePath}] from [{remoteFilePath}]", localFilePath, remoteFilePath);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public void DeleteFile(string remoteFilePath)
        {
            using var client = new SftpClient(sftpConfig.Host, sftpConfig.Port == 0 ? 22 : sftpConfig.Port, sftpConfig.UserName, sftpConfig.Password);
            try
            {
                client.Connect();
                client.DeleteFile(remoteFilePath);
                logger.LogInformation("File [{remoteFilePath}] is deleted.", remoteFilePath);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed in deleting the file [{remoteFilePath}]", remoteFilePath);
            }
            finally
            {
                client.Disconnect();
            }
        }
    }
}
