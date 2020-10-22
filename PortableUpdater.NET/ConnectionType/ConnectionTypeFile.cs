using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PortableUpdaterDotNET
{
    class ConnectionTypeFile : IConnectionType
    {
        public event EventHandler<DownloadProgressEventArgs> DownloadProgressChanged;

        private UpdateInfoEventArgs _updateInfo;

        public UpdateInfoEventArgs GetUpdateInfo() { return _updateInfo; }

        public async Task<UpdateInfoEventArgs> ReadXmlFileAsync(Uri xmlUri)
        {
            byte[] result;
            using (FileStream SourceStream = File.Open(xmlUri.LocalPath, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }
            string xmlString = System.Text.Encoding.UTF8.GetString(result);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xmlString)) { XmlResolver = null };
            _updateInfo = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);

            if (string.IsNullOrEmpty(_updateInfo.CurrentVersion) || string.IsNullOrEmpty(_updateInfo.DownloadURL))
            {
                throw new MissingFieldException();
            }

            return _updateInfo;
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task StartDownloadAsync(string downloadUrl, string destinationFilePath)
        {
            var webClient = new WebClient();
            webClient.DownloadProgressChanged += OnDownloadProgressChanged;
            webClient.DownloadFileAsync(new Uri(downloadUrl), destinationFilePath);
        }

        public void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var args = new DownloadProgressEventArgs() 
            { 
                TotalFileSize = e.TotalBytesToReceive, TotalBytesDownloaded = e.BytesReceived, ProgressPercentage = e.ProgressPercentage, Filename = _updateInfo.DownloadFileName
            };
            DownloadProgressChanged?.Invoke(this, args);
        }
    }
}
