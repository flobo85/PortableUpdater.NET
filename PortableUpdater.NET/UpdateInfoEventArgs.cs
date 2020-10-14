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
        ///     If new update is available then returns true otherwise false.
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        ///     Download URL of the update file.
        /// </summary>
        [XmlElement("url")]
        public string DownloadURL
        {
            get => GetURI(_downloadURL);
            set => _downloadURL = value;
        }

        /// <summary>
        ///     Returns newest version of the application available to download.
        /// </summary>
        [XmlElement("version")]
        public string CurrentVersion { get; set; }

        internal static string GetURI(string url)
        {
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                Uri uri = new Uri(url);
            }
            else
            {
                // TODO: Error Message ausgeben, "Downloadstring kein gültiger Uri-Pfad "
            }
            return url;
        }
    }
}
