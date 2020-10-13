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
            Test();
            
        }

        private async void Test()
        {
            var updatemanager = new PortableUpdater();
            updatemanager.Start(@"https://raw.githubusercontent.com/flobo85/PortableUpdater.NET/main/Test.txt");
        }
    }
}
