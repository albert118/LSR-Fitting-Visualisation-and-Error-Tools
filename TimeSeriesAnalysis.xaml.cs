using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MachineLearnerWPF
{
    /// <summary>
    /// Interaction logic for TimeSeriesAnalysis.xaml
    /// </summary>
    public partial class TimeSeriesAnalysis : UserControl
    {
        public TimeSeriesAnalysis()
        {
            InitializeComponent();
            Constants.RawData data = Constants.Item.GetRawData();
            int n = Constants.Item.GetCount();
            var Ewma = DataPreparer.Weighter(data, 0.5);

            YFormatter = value => value.ToString("C");

            List<double> BuyQ = data.BuyQuantity.Select<int, double>(i => i).ToList();
            List<double> BuyP = data.BuyPrice.Select<int, double>(i => i).ToList();
            List<double> SellQ = data.SellQuantity.Select<int, double>(i => i).ToList();
            List<double> SellP = data.SellPrice.Select<int, double>(i => i).ToList();
           
            for (int i = 0; i<BuyQ.Count(); i++)
            {
                EWMAPrice.Add(new ObservablePoint(i, Ewma._strdBuyPrice.ElementAt(i)));
                Price.Add(new ObservablePoint(i, BuyP.ElementAt(i)));
            }

            DataContext = this;
        }
        public ChartValues<ObservablePoint> EWMAPrice { get; set; }
        public ChartValues<ObservablePoint> Price { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
