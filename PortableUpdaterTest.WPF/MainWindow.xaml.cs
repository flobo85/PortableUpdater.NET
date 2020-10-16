using PortableUpdaterDotNET;
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
            TestHttp();
            //TestFile();
        }

        private void TestHttp()
        {
            var updatemanager = new PortableUpdater();
            updatemanager.Start(@"https://raw.githubusercontent.com/flobo85/PortableUpdater.NET/main/Test.xml");
        }

        private void TestFile()
        {
            var updatemanager = new PortableUpdater();
            updatemanager.Start(@"C:\Temp\Test.xml");
        }
    }
}
