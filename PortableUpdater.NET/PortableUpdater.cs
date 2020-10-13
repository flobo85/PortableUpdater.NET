using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace PortableUpdaterDotNET
{
    public class PortableUpdater
    {        
        public Uri XmlLink { get; private set; }

        public void Start(string xmlLink)
        {
            try
            {
                XmlLink = new Uri(xmlLink);
                if(XmlLink.Scheme.Equals(Uri.UriSchemeHttp) || XmlLink.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    CheckForUpdateAsync();
                }
            }
            catch (ArgumentNullException)
            {

            }
            catch(UriFormatException)
            {

            }
        }

        private async Task CheckForUpdateAsync()
        {
            HttpClient client = new HttpClient();
            using (HttpResponseMessage response = await client.GetAsync(XmlLink))
            {
                using (HttpContent content = response.Content)
                {
                    string result = await content.ReadAsStringAsync();
                    Console.WriteLine($"*** {result} ***");
                }
            }
        }
    }
}
