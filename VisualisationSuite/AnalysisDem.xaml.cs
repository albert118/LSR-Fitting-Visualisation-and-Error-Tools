using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MachineLearnerWPF.Analysis
{
    /// <summary>
    /// Interaction logic for AnalysisDem.xaml
    /// </summary>
    public partial class AnalysisDem : UserControl
    {
        public AnalysisDem()
        {
            InitializeComponent();

            Constants.RawData Data = Constants.Item.GetRawData();
            int n = Constants.Item.GetCount();
            var PredictedBP = new List<double>();
            var ResidSup = new List<double>();
            double ResidBP = 0;
            double[,] Regressors = FittingFunctions.FitLin(Data, n);
            for (var i = 0; i < n; i++)
            {
                var Qs = Data.SellQuantity.ElementAt(i);
                double BP = (Qs - Regressors[1, 1]) / Regressors[0, 1];
                ResidBP = Qs - BP;
                PredictedBP.Add(BP);
                ResidSup.Add(ResidBP);
            }
            var StrdResid = DataPreparer.ListStrd(ResidSup);
            //create observable points for graphing
            AnalysisDis = Visualiser.SeriesBuilder(PredictedBP);
            DataContext = this;
        }
        public ChartValues<ObservablePoint> AnalysisDis { get; set; }
    }
}
