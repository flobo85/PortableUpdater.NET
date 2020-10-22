using System;

namespace PortableUpdaterDotNET
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public long? TotalFileSize { get; set; }
        public long TotalBytesDownloaded { get; set; }
        public double? ProgressPercentage { get; set; }
        public string Filename { get; set; }
    }
}
