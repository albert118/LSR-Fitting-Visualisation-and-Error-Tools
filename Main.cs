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
            Console.WriteLine("Operation on dataset: Item ({0})...", Constants.id[i]);
            //get initial data from files
            var RawData = DataPreparer.Splicer(i);
            Console.WriteLine("Getting data from files");
            //subset length, total set size is 4*n (buy/sell quant, buy/sell price)
            //trim dataset to n before singleton implemented!
            int n = 4000;
            Console.WriteLine("trimmng datset...");
            DataPreparer.Trimmer(RawData, n);
            //get item specific data for raw dataset as a singleton class
            Console.WriteLine("Getting item...");
            Constants.Item item = Constants.Item.GetItem(RawData, n);
            Constants.RawData Data = Constants.Item.GetRawData();
            //standardised dataset as well as weighted and standardised weighted data
            Console.WriteLine("Standardising dataset");
            Constants.StrdData strdData = DataPreparer.Z_Score(Data);
            //LSR fitting solutions to each set (standardised & non-standardised)
            Console.WriteLine("Fitting function parameters");
            double[,] FitEq = FittingFunctions.FitLin(Data, n);
            double[,] StrdFitEq = FittingFunctions.StrdFitLin(strdData, n);
            Constants.DataInfo Info = DataPreparer.DataInfo();  
            Console.WriteLine("Getting model information");
            double[] Corrs = ModelInfo.CorrCoeff(Data, Info);
            //Get prediction lists and convert to List<int> for StrdDev function
            var _DemPreds = FittingFunctions.Predictor(Data.BuyPrice, FitEq[0, 0], FitEq[1, 0]);
            List<int> DemPreds = _DemPreds.Select(x => (int)x).ToList();
            var _SupPreds = FittingFunctions.Predictor(Data.SellPrice, FitEq[0, 1], FitEq[1, 1]);
            List<int> SupPreds = _SupPreds.Select(x => (int)x).ToList();
            //Standard deviations of data, standard deviations of predicted data & correlations
            double[] FitData = {
                ModelInfo.StrdDev(Data.SellQuantity, Info.SellQuantInfo, n) ,
                ModelInfo.StrdDev(Data.BuyQuantity, Info.BuyQuantInfo , n),
                ModelInfo.StrdDev(DemPreds, Info.BuyQuantInfo, n),
                ModelInfo.StrdDev(SupPreds, Info.BuyQuantInfo, n),
                ModelInfo.RMSE(FitEq[2, 0], n), ModelInfo.RMSE(FitEq[2, 1], n),
                Corrs[0] , Corrs[1]
            };
            Console.WriteLine("Printing results to files...");
            DataWriter.FileMaker(Info, i, FitEq, StrdFitEq, FitData);
            Console.Write("Check the desktop!");
            Console.ReadLine();
        }
    }
}
