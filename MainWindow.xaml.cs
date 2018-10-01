namespace MachineLearnerWPF
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Constants.RawData Data = Constants.Item.GetRawData();
            
            ScatterPlotWindow.ScatterPlots scatterPlots = new ScatterPlotWindow.ScatterPlots();
            scatterPlots.InitializeComponent();
        }
    }
}
//construct demand and supply equations from data (economics)
            //S(p)=SellQuantity = b + m*BuyPrice
            //D(p)=BuyQuantity = b + m*SellPrice
            //dependent variable (Quantity) on Y-Axis and independent on X-Axis