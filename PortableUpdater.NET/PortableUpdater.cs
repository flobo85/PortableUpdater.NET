using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace PortableUpdaterDotNET
{
    /// <summary>
    /// Main class PortableUpdater
    /// </summary>
    public class PortableUpdater
    {
        /// <summary>
        /// URL of the xml file that contains information about latest version of the application.
        /// </summary>
        internal Uri XmlLink;

        /// <summary>
        /// Start checking for new version of application
        /// </summary>
        /// <param name="xmlLink"></param>URL or path of the xml file that contains information about latest version of the application.
        public async void Start(string xmlLink)
        {
            try
            {
                XmlLink = new Uri(xmlLink);
                string xmlString = await GetXmlString();
                var args = CheckForUpdateAsync(xmlString);

                Console.WriteLine($"*** CurrentVersion: {args.CurrentVersion} ***");
                Console.WriteLine($"*** DownloadURL: {args.DownloadURL} ***");
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private async Task<string> GetXmlString()
        {
            try
            {
                if (XmlLink.Scheme.Equals(Uri.UriSchemeHttp) || XmlLink.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    HttpClient client = new HttpClient();
                    using (HttpResponseMessage response = await client.GetAsync(XmlLink))
                    {
                        using (HttpContent content = response.Content)
                        {
                            return await content.ReadAsStringAsync();
                        }
                    }
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
                    // TODO: throw Exception
                    return null;
                }
            }
            catch(Exception exception)
            {
                throw exception;
            }
        }

        private UpdateInfoEventArgs CheckForUpdateAsync(string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xmlString)) { XmlResolver = null };
            return (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);            
        }

        private static void ShowError(Exception exception)
        {
            MessageBox.Show(exception.Message,
                            exception.GetType().ToString(), MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}
