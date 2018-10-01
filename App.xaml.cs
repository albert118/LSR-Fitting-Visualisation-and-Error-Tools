using System.Windows;

namespace MachineLearnerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            int i = 3;
            var RawData = DataPreparer.Splicer(i);
            Constants.Item item = Constants.Item.GetItem(RawData, 4000);
            Constants.RawData raw = Constants.Item.GetRawData();
            //series.InitializeComponent();
            MainWindow window = new MainWindow();
            window.InitializeComponent();
        }
    }
}
