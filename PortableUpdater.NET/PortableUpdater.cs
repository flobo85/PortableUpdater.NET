using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
        public void Start(string xmlLink)
        {
            try
            {
                XmlLink = new Uri(xmlLink);
                if(XmlLink.Scheme.Equals(Uri.UriSchemeHttp) || XmlLink.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    _ = CheckForUpdateAsync();
                }
                else if(XmlLink.Scheme.Equals(Uri.UriSchemeFile))
                {
                    UpdateInfoEventArgs args;
                    string xml = File.ReadAllText(XmlLink.LocalPath, System.Text.Encoding.UTF8);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
                    XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xml)) { XmlResolver = null };
                    args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);
                    Console.WriteLine($"*** CurrentVersion: {args.CurrentVersion} ***");
                    Console.WriteLine($"*** DownloadURL: {args.DownloadURL} ***");
                }
            }
            catch (ArgumentNullException)
            {
                // TODO: Error keine gültige URI für XML-Datei
            }
            catch(UriFormatException)
            {
                // TODO: Error keine gültige URI für XML-Datei
            }
        }

        private async Task CheckForUpdateAsync()
        {
            HttpClient client = new HttpClient();
            UpdateInfoEventArgs args;
            using (HttpResponseMessage response = await client.GetAsync(XmlLink))
            {
                using (HttpContent content = response.Content)
                {
                    string xml = await content.ReadAsStringAsync();
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
                    XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xml)) { XmlResolver = null };
                    args = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);                    
                }
            }
            Console.WriteLine($"*** CurrentVersion: {args.CurrentVersion} ***");
            Console.WriteLine($"*** DownloadURL: {args.DownloadURL} ***");
        }
    }
}
