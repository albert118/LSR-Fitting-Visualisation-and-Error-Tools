using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineLearnerWPF
{
    class Visualiser
    {
        /// <summary>
        /// Input data of item for direct supply and demand scatterplot data. Note: yields inverse relation
        /// demand - negative correlation (inverse is shown in equation with equation plotted as is)
        /// supply - positive correlation (inverse is plotted in eco with q as x, but equation uses p as x)
        /// </summary>
        /// <param name="BQuant">Buy Quantity</param>
        /// <param name="BPrice">Buy Price</param>
        /// <param name="SQuant">Sell Quantity</param>
        /// <param name="SPrice">Sell Price</param>
        /// <param name="Range">Size of data set to be used</param>
        /// <returns>Obserable Point chart values for Demand and Supply returned in array, demand[0] and supply[1]</returns>
        public static Object[] ScatterListBuilder(List<double> BQuant, List<double> BPrice, List<double> SQuant, List<double> SPrice, int Range)
        {
            Object[] Scatters = new Object[2];
            ChartValues<ObservablePoint> Demand = new ChartValues<ObservablePoint>();
            ChartValues<ObservablePoint> Supply = new ChartValues<ObservablePoint>();
            //S(p)=SellQuantity = b + m*BuyPrice
            //D(p)=BuyQuantity = b + m*SellPrice
            //dependent variable (Quantity) on Y-Axis and independent on X-Axis
            for (int i = 0; i < Range; i++)
            {
                double DemY = BQuant.ElementAt(i);
                double DemX = SPrice.ElementAt(i);
                double SupY = SQuant.ElementAt(i);
                double SupX = BPrice.ElementAt(i);

                Demand.Add(new ObservablePoint(DemX, DemY));
                Supply.Add(new ObservablePoint(SupX, SupY));
            }
            Scatters[0] = Demand;
            Scatters[1] = Supply;
            return Scatters;

        }
        /// <summary>
        /// Series Initialiser for data analysis
        /// </summary>
        /// <param name="Residuals">Data error on regression</param>
        /// <param name="StrdResid">Standardised Residuals</param>
        /// <param name="Predicted">Regression estimates</param>
        /// <param name="itemId">Item being analysed</param>
        /// <returns>Analysis Series</returns>
        public static ChartValues<ObservablePoint> SeriesBuilder(List<double> Residuals)
        {
            ChartValues<ObservablePoint> AnalysisDis = new ChartValues<ObservablePoint>();
            //calculate the residual data frequencies and add to X, using LINQ
            foreach (var grp in Residuals.GroupBy(i => i))
            {
                AnalysisDis.Add(new ObservablePoint(grp.Key, grp.Count()));
            }
            return AnalysisDis;
        }
    }
}
