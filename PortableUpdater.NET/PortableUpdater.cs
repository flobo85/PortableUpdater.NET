using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

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

        internal static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Prüft anhand der XML - Datei, ob eine neue Version verfügbar ist 
        /// </summary>
        /// <param name="xmlLink"></param> URL or path of the xml file that contains information about latest version of the application.
        public static async void Start(string xmlLink)
        {
            try
            {
                XmlLink = new Uri(xmlLink);
                string xmlString = await GetXmlString();
                var updateinfo = ReadXmlFileAsync(xmlString);

                bool IsUpdateAvailable = CheckForUpdate(updateinfo);
                DownloadFile(updateinfo);
                Console.WriteLine($"***** {updateinfo.DownloadURL} *****");
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private static async Task<string> GetXmlString()
        {
            try
            {
                if (XmlLink.Scheme.Equals(Uri.UriSchemeHttp) || XmlLink.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
                    HttpResponseMessage response = await client.GetAsync(XmlLink);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }

                else if (XmlLink.Scheme.Equals(Uri.UriSchemeFile))
                {
                    return File.ReadAllText(XmlLink.LocalPath, System.Text.Encoding.UTF8);
                }

                else if (XmlLink.Scheme.Equals(Uri.UriSchemeFtp))
                {
                    // TODO: get xml string via ftp
                    return null;
                }

                else
                {
                    // TODO: Protokoll nicht unterstützt
                    return null;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private static UpdateInfoEventArgs ReadXmlFileAsync(string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xmlString)) { XmlResolver = null };
            UpdateInfoEventArgs updateInfo = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);

            if (string.IsNullOrEmpty(updateInfo.CurrentVersion) || string.IsNullOrEmpty(updateInfo.DownloadURL))
            {
                throw new MissingFieldException();
            }

            return updateInfo;
        }

        private static bool CheckForUpdate(UpdateInfoEventArgs args)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            args.InstalledVersion = assembly.GetName().Version;
            args.IsUpdateAvailable = new Version(args.CurrentVersion) > args.InstalledVersion;
            return args.IsUpdateAvailable;
        }

        private static async void DownloadFile(UpdateInfoEventArgs updateInfo)
        {
            if (XmlLink.Scheme.Equals(Uri.UriSchemeHttp) || XmlLink.Scheme.Equals(Uri.UriSchemeHttps))
            {
                using (var httpClient = client)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, updateInfo.DownloadURL))
                    {
                        using (
                            Stream contentStream = await (await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                            stream = new FileStream($"Testtest.xml", FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                        {
                            await contentStream.CopyToAsync(stream);
                        }
                    }
                }
            }

            else if (XmlLink.Scheme.Equals(Uri.UriSchemeFile))
            {
                // TODO: Download Update via File
            }

            else if (XmlLink.Scheme.Equals(Uri.UriSchemeFtp))
            {
                // TODO: Download Update via ftp                
            }
        }

        internal static void ShowError(Exception exception)
        {
            MessageBox.Show(exception.Message,
                            exception.GetType().ToString(), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}
