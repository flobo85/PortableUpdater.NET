using System;
using System.Threading.Tasks;


namespace PortableUpdaterDotNET
{
    interface IConnectionType
    {
        event EventHandler<DownloadProgressEventArgs> DownloadProgressChanged;

        Task StartDownloadAsync(string downloadUrl, string destinationFilePath);

        Task<UpdateInfoEventArgs> ReadXmlFileAsync(Uri XmlUri);

        UpdateInfoEventArgs GetUpdateInfo();
    }
}
