using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PortableUpdaterDotNET
{
    internal class ConnectionTypeWeb : IConnectionType
    {
        public event EventHandler<DownloadProgressEventArgs> DownloadProgressChanged;

        private string _downloadUrl;
        private string _destinationFilePath;
        private static Uri _xmlUri;
        private UpdateInfoEventArgs _updateInfo;

        public UpdateInfoEventArgs GetUpdateInfo() { return _updateInfo; }

        public async Task<UpdateInfoEventArgs> ReadXmlFileAsync(Uri xmlUri)
        {
            _xmlUri = xmlUri;
            HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };
            string xmlString = await GetXmlString(_httpClient);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateInfoEventArgs));
            XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xmlString)) { XmlResolver = null };
            _updateInfo = (UpdateInfoEventArgs)xmlSerializer.Deserialize(xmlTextReader);
            Dispose(_httpClient);

            if (string.IsNullOrEmpty(_updateInfo.CurrentVersion) || string.IsNullOrEmpty(_updateInfo.DownloadURL))
            {
                throw new MissingFieldException();
            }

            return _updateInfo;
        }

        private static async Task<string> GetXmlString(HttpClient _httpClient)
        {
            _httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
            HttpResponseMessage response = await _httpClient.GetAsync(_xmlUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task StartDownloadAsync(string downloadUrl, string destinationFilePath)
        {
            _downloadUrl = downloadUrl;
            _destinationFilePath = destinationFilePath;
            HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };

            using (var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                await DownloadFileFromHttpResponseMessage(response, _httpClient);
        }

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response, HttpClient _httpClient)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            using (var contentStream = await response.Content.ReadAsStreamAsync())
                await ProcessContentStream(totalBytes, contentStream, _httpClient);
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream, HttpClient _httpClient)
        {
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead, _httpClient);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    readCount += 1;

                    if (readCount % 100 == 0)
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead, _httpClient);
                }
                while (isMoreToRead);
            }
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead, HttpClient _httpClient)
        {
            if (DownloadProgressChanged == null)
                return;

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

            var e = new DownloadProgressEventArgs() { TotalFileSize = totalDownloadSize, TotalBytesDownloaded = totalBytesRead, ProgressPercentage = progressPercentage };
            OnDownloadProgressChanged(e);

            if (totalBytesRead == totalDownloadSize)
                Dispose(_httpClient);
        }

        protected virtual void OnDownloadProgressChanged(DownloadProgressEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        private void Dispose(HttpClient _httpClient)
        {
            _httpClient?.Dispose();
        }
    }
}