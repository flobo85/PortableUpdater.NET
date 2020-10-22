using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PortableUpdaterDotNET
{
    /// <summary>
    /// PortableUpdater Hauptklasse
    /// </summary>
    public class PortableUpdater
    {
        /// <summary>
        /// <see cref="Uri"/> - Pfad der XML - Datei
        /// </summary>
        internal static Uri XmlLink;

        internal static IConnectionType connectionType;

        public static event EventHandler<DownloadProgressEventArgs> DownloadProgress;

        /// <summary>
        /// Prüft anhand der XML - Datei, ob eine neue Version verfügbar ist 
        /// </summary>
        /// <param name="xmlLink"></param> URL or path of the xml file that contains information about latest version of the application.
        public static async void Start(string xmlLink)
        {
            try
            {
                XmlLink = new Uri(xmlLink);
                UpdateInfoEventArgs updateInfo = null;
                if (XmlLink.Scheme.Equals(Uri.UriSchemeHttp) || XmlLink.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    connectionType = new ConnectionTypeWeb();                    
                }
                else if (XmlLink.Scheme.Equals(Uri.UriSchemeFile))
                {
                    connectionType = new ConnectionTypeFile();
                }

                updateInfo = await connectionType.ReadXmlFileAsync(XmlLink);
                bool IsUpdateAvailable = CheckForUpdate(updateInfo);

                // TODO: Wollen Sie das Update durchführen?

                Uri downloadLink = new Uri(updateInfo.DownloadURL);
                if (downloadLink.Scheme.Equals(Uri.UriSchemeHttp) || downloadLink.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    connectionType = new ConnectionTypeWeb();
                }
                else if (downloadLink.Scheme.Equals(Uri.UriSchemeFile))
                {
                    connectionType = new ConnectionTypeFile();
                }
                
                connectionType.DownloadProgressChanged += DownloadUpdateProgressChanged;

                await connectionType.StartDownloadAsync(updateInfo.DownloadURL, Path.GetFullPath(updateInfo.DownloadFileName));
                Console.WriteLine($"***** {updateInfo.DownloadURL} *****");
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private static void DownloadUpdateProgressChanged(object sender, DownloadProgressEventArgs e)
        {
            DownloadProgress?.Invoke(sender, e);            
        }

        private static bool CheckForUpdate(UpdateInfoEventArgs args)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            args.InstalledVersion = assembly.GetName().Version;
            args.IsUpdateAvailable = new Version(args.CurrentVersion) > args.InstalledVersion;
            return args.IsUpdateAvailable;
        }        

        internal static void ShowError(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show(exception.Message,
                            exception.GetType().ToString(), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}
