using System;
using System.Xml.Serialization;

namespace PortableUpdaterDotNET
{
    /// <summary>
    ///   Object of this class contains all update information through the xml file  
    /// </summary>
    [XmlRoot("item")]
    public class UpdateInfoEventArgs : EventArgs
    {
        private string _downloadURL;

        /// <summary>
        /// Filename from the update package 
        /// </summary>
        public string DownloadFileName { get; set; }

        /// <summary>
        ///     If new update is available then returns true otherwise false.
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        ///     Returns version of the application currently installed on the user's PC.
        /// </summary>
        public Version InstalledVersion { get; set; }

        /// <summary>
        ///     Download URL of the update file.
        /// </summary>
        [XmlElement("url")]
        public string DownloadURL
        {
            get => CheckStringToUri(_downloadURL);
            set => _downloadURL = value;
        }
         
        /// <summary>
        ///     Returns newest version of the application available to download.
        /// </summary>
        [XmlElement("version")]
        public string CurrentVersion { get; set; }

        internal string CheckStringToUri(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                if (!uri.IsAbsoluteUri)
                {
                    throw new Exception($"Der Download-URL darf kein relativer Pfad sein");
                }
                DownloadFileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                return uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
