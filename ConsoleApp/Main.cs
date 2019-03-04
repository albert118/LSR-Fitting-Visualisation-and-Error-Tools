using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineLearnerWPF
{
    class ConsoleEntry
    {

        public static void Main(string[] args)
        {
            int i = 3;
            Console.WriteLine("Analysing dataset for Item: {0}", Constants.id[i]);
            //get initial data from files
            var RawData = DataPreparer.Splicer(i);
            Console.WriteLine("Getting data from files. ");
            // Create a new item and fit a function for it
            Item.Item item = new Item.Item(buy_price, buy_quant, sell_price, sell_quant, i);
            Console.WriteLine("Created item & now fitting function parameters. ");
            DataPreparer.FittedFunction.FittedFunction(sell_quant, buy_price, item, "supply");
            DataPreparer.FittedFunction.FittedFunction(buy_quant, sell_price, item, "demand");

            // TODO: Finish this!
            /*double[] Corrs = ModelInfo.CorrCoeff(Data, Info);
            //Get prediction lists and convert to List<int> for StrdDev function
            var _DemPreds = FittingFunctions.Predictor(Data.BuyPrice, FitEq[0, 0], FitEq[1, 0]);
            List<int> DemPreds = _DemPreds.Select(x => (int)x).ToList();
            var _SupPreds = FittingFunctions.Predictor(Data.SellPrice, FitEq[0, 1], FitEq[1, 1]);
            List<int> SupPreds = _SupPreds.Select(x => (int)x).ToList();
            //Standard deviations of data, standard deviations of predicted data & correlations
            */
            Console.WriteLine("Printing results to files...");
            DataWriter.FileMaker(Info, i, FitEq, StrdFitEq, FitData);
            Console.Write("Check the desktop!");
            Console.ReadLine();
        }
    }
}
