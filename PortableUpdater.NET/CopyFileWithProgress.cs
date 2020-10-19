using System;
using System.Net;

namespace PortableUpdaterDotNET
{
    internal class CopyFileWithProgress
    {
        private readonly string _source;
        private readonly string _destination;

        public delegate void IntDelegate(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

        public event IntDelegate FileCopyProgress;

        public CopyFileWithProgress(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

        public void StartDownload()
        {
            var webClient = new WebClient();
            webClient.DownloadProgressChanged += DownloadProgress;
            webClient.DownloadFileAsync(new Uri(_source), _destination);
        }

        public void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            FileCopyProgress?.Invoke(e.TotalBytesToReceive, e.BytesReceived, e.ProgressPercentage);
        }

    }
}
