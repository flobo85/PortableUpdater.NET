using PortableUpdaterDotNET;
using System;
using System.Windows;


namespace PortableUpdaterTest.WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PortableUpdater.DownloadProgress += DownloadProgressEvent;
            //TestHttp();
            TestFile();
        }

        private void DownloadProgressEvent(object sender, DownloadProgressEventArgs e)
        {
            Console.WriteLine($"{e.Filename} wird heruntergeladen. {e.ProgressPercentage}% ({e.TotalBytesDownloaded}/{e.TotalFileSize})");
        }

        private void TestHttp()
        {
            PortableUpdater.Start(@"https://raw.githubusercontent.com/flobo85/PortableUpdater.NET/main/Test.xml");
        }

        private void TestFile()
        {
            PortableUpdater.Start(@"C:\Temp\Test.xml");
        }
    }
}
