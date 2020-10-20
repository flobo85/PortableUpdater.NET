using System;

namespace PortableUpdaterDotNET
{
    internal class DownloadProgressEventArgs : EventArgs
    {
        public long? TotalFileSize { get; set; }
        public long TotalBytesDownloaded { get; set; }
        public double? ProgressPercentage { get; set; }
    }
}
