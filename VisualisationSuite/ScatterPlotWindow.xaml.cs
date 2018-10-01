using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;

namespace MachineLearnerWPF.ScatterPlotWindow
{
    /// <summary>
    /// Interaction logic for ScatterPlotWindow
    /// </summary>
    public partial class ScatterPlots : UserControl
    {
        /// <summary>
        /// scatter plots of EWMA vs. raw
        /// </summary>
        public ScatterPlots()
        {
            InitializeComponent();
            Constants.RawData data = Constants.Item.GetRawData();
            int n = Constants.Item.GetCount();
            var Ewma = DataPreparer.Weighter(data, 0.5);
            //LINQ, $ formatting
            PriceFormatter = value => value.ToString("C");

            List<double> BuyQ = data.BuyQuantity.Select<int, double>(i => i).ToList();
            List<double> BuyP = data.BuyPrice.Select<int, double>(i => i).ToList();
            List<double> SellQ = data.SellQuantity.Select<int, double>(i => i).ToList();
            List<double> SellP = data.SellPrice.Select<int, double>(i => i).ToList();

            //~3500 data points possible with current config = 7000 total
            Object[] ScatterPoints = Visualiser.ScatterListBuilder(BuyQ, BuyP, SellQ, SellP, n);
            Object[] WScatterPoints = Visualiser.ScatterListBuilder(Ewma._strdBuyQuant, Ewma._strdBuyPrice, Ewma._strdSellQuant, Ewma._strdSellPrice, n);
            //note explicit conversion of types
            Supply = (ChartValues<ObservablePoint>)ScatterPoints[0];
            Demand = (ChartValues<ObservablePoint>)ScatterPoints[1];
            EwmaSupply = (ChartValues<ObservablePoint>)WScatterPoints[0];
            EwmaDemand = (ChartValues<ObservablePoint>)WScatterPoints[1];
            DataContext = this;
        }
        public ChartValues<ObservablePoint> Demand { get; set; }
        public ChartValues<ObservablePoint> Supply { get; set; }
        public ChartValues<ObservablePoint> EwmaDemand { get; set; }
        public ChartValues<ObservablePoint> EwmaSupply { get; set; }
        public Func<double, string> PriceFormatter { get; set; }
    }
}