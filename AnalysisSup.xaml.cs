using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;


namespace MachineLearnerWPF.Analysis
{
    /// <summary>
    /// Interaction logic for AnalysisSup.xaml
    /// </summary>
    public partial class AnalysisSup : UserControl
    {
        public AnalysisSup()
        {
            InitializeComponent();

            Constants.RawData Data = Constants.Item.GetRawData();
            int n = Constants.Item.GetCount();
            var PredictedSP = new List<double>();
            var ResidDem = new List<double>();
            double ResidSP = 0;
            double[,] Regressors = FittingFunctions.FitLin(Data, n);
            for (var i = 0; i < n; i++)
            {
                var Qd = Data.BuyQuantity.ElementAt(i);
                double SP = (Qd - Regressors[1, 0]) / Regressors[0, 0];
                ResidSP = Qd - SP;
                PredictedSP.Add(SP);
                ResidDem.Add(ResidSP);
                
            } 
            var StrdResid = DataPreparer.ListStrd(ResidDem);
            //create observable points for graphing
            AnalysisDis = Visualiser.SeriesBuilder(PredictedSP);
            DataContext = this;
        }
        public ChartValues<ObservablePoint> AnalysisDis { get; set; }
    }
}
