using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MachineLearnerWPF
{  
    /// <summary>
    /// Set of functions for regression and output data
    /// </summary>
    public class FittingFunctions
    {
        /// <summary>
        /// For univariate regression
        /// </summary>
        /// <param name="X">Independent Variable</param>
        /// <param name="Y">Dependent Variable</param>
        /// <param name="n">Size of set</param>
        /// <param name="AvgX"></param>
        /// <param name="AvgY"></param>
        /// <returns>b1 regressor</returns>
        private static double GradSolver(List<int> X, List<int> Y, int n, double AvgX, double AvgY)
        {
            double Grad;
            double SumNom = 0;
            double SumDenom = 0;
            double Denom;
            for(var i = 0; i < n; i++)
            {
                SumNom += (X.ElementAt(i)-AvgX) * (Y.ElementAt(i)-AvgY);
                Denom = (X.ElementAt(i) - AvgX);
                SumDenom += Math.Pow(Denom, 2);
            }
            Grad = SumNom / SumDenom;
            return Grad;
        }
        private static double StrdGradSolver(List<double> X, List<double> Y, int n, double AvgX, double AvgY)
        {
            double Grad;
            double SumNom = 0;
            double SumDenom = 0;
            double Denom;
            for (var i = 0; i < n; i++)
            {
                SumNom += (X.ElementAt(i) - AvgX) * (Y.ElementAt(i) - AvgY);
                Denom = (X.ElementAt(i) - AvgX);
                SumDenom += Math.Pow(Denom, 2);
            }
            Grad = SumNom / SumDenom;
            return Grad;
        }
        /// <summary>
        /// Solve for y intercept of LSR minimised function
        /// </summary>
        /// <param name="Grad">Function Gradient</param>
        /// <param name="Ymean">Series y mean value</param>
        /// <param name="Xmean">Series x mean value</param>
        /// <returns>double Intercept value</returns>
        private static double IntSolver(double Grad, double Ymean, double Xmean)
        {
            double Int;
            return Int = Ymean - Grad * Xmean;       
        }
        /// <summary>
        /// Error function of LSR
        /// </summary>
        /// <returns>R-squared error value</returns>
        private static double ErrorSolver(List<int> Quant, List<double> Prediction)
        {
            double Err = 0;
            for (int i = 0; i< Quant.Count(); i++)
            {
                var PredErr = Quant.ElementAt(i) - Prediction.ElementAt(i);
                Err += Math.Pow(PredErr, 2);
            }
            return Math.Round(Err);
        }
        private static double StrdErrorSolver(List<double> Quant, List<double> Prediction)
        {
            double Err = 0;
            List<double> deStrd = DataPreparer.DeStrd(Prediction);
            for (int i = 0; i < Quant.Count(); i++)
            {     
                var PredErr = Quant.ElementAt(i) - deStrd.ElementAt(i);
                Err += Math.Pow(PredErr, 2);
            }
            return Err;
        }
        /// <summary>
        /// solves fitted equation for estimated data
        /// </summary>
        /// <param name="Price"></param>
        /// <param name="Grad"></param>
        /// <param name="Int"></param>
        /// <returns></returns>
        public static List<double> Predictor (List<int> Price, double Grad, double Int)
        {
            var Pred = new List<double>();
            for (int i = 0; i<Price.Count(); i++)
            {
                double pred = Int + Grad * Price.ElementAt(i);
                Pred.Add(pred);
            }
            return Pred;
        }
        private static List<double> StrdPredictor(List<double> Price, double Grad, double Int)
        {
            var Pred = new List<double>();
            for (int i = 0; i < Price.Count(); i++)
            {
                double pred = Int + Grad * Price.ElementAt(i);
                Pred.Add(pred);
            }
            return Pred;
        }
        /// <summary>
        /// Fits a line in a univariate analysis to data
        /// </summary>
        /// <param name="data">Input</param>
        /// <param name="n">Range of Input</param>
        /// <returns>Fit Param's, first column is gradients</returns>
        public static double[,] FitLin(Constants.RawData data, int n)
        {
            //S(p)=BuyQuantity = b + m*SellPrice
            //D(p)=SellQuantity = b - m*BuyPrice
            double[,] Params = new double[3, 2];
            var BQuant = data.BuyQuantity;
            var BPrice = data.BuyPrice;
            var SQuant = data.SellQuantity;
            var SPrice = data.SellPrice;

            //array location 0, 0 is mean value
            var InfoObject = DataPreparer.DataInfo();          
            var BPMean = (double)InfoObject.BuyPriceInfo[0, 0];
            var BQMean = (double)InfoObject.BuyQuantInfo[0, 0];
            var SPMean = (double)InfoObject.SellPriceInfo[0, 0];
            var SQMean = (double)InfoObject.SellQuantInfo[0, 0];

            //return B1/Grad for demand and supply
            var GradDem = GradSolver(BPrice, SQuant, n, BPMean, SQMean);
            var GradSup = GradSolver(SPrice, BQuant, n, SPMean, BQMean);
            //return B0/intercept for demand and supply
            var DemInt = IntSolver(GradDem, SQMean, BPMean);
            var SupInt = IntSolver(GradSup, BQMean, SPMean);
            //create predictions of data and add to predictions lists
            var PredDem = Predictor(BPrice, GradDem, DemInt);
            var PredSup = Predictor(SPrice, GradSup, SupInt);
            //return error on predicitons
            var DemErr = ErrorSolver(SQuant, PredDem);
            var SupErr = ErrorSolver(BQuant, PredSup);
            //add to Params and return
            Params[0, 0] = Math.Round(GradDem);
            Params[0, 1] = Math.Round(GradSup);
            Params[1, 0] = Math.Round(DemInt);
            Params[1, 1] = Math.Round(SupInt);
            Params[2, 0] = Math.Round(DemErr);
            Params[2, 1] = Math.Round(SupErr);
            Console.WriteLine(Params[2,1]);
            return Params;
        }
        /// <summary>
        /// Fit to a standardised data set then return de-standardized values
        /// </summary>
        /// <param name="strdData"></param>
        /// <param name="n">Dataset length</param>
        /// <returns></returns>
        public static double[,] StrdFitLin(Constants.StrdData strdData, int n)
        {
            //S(p)=SellQuantity = b + m*BuyPrice
            //D(p)=BuyQuantity = b + m*SellPrice
            double[,] Params = new double[3, 2];
            var BQuant = strdData._strdBuyQuant;
            var BPrice = strdData._strdBuyPrice;
            var SQuant = strdData._strdSellQuant;
            var SPrice = strdData._strdSellPrice;

            var InfoObject = DataPreparer.StrdDataInfo(strdData);
            //array location 0, 0 is mean value
            var BPMean = (double)InfoObject.BuyPriceInfo[0, 0];
            var BQMean = (double)InfoObject.BuyQuantInfo[0, 0];
            var SPMean = (double)InfoObject.SellPriceInfo[0, 0];
            var SQMean = (double)InfoObject.SellQuantInfo[0, 0];

            //return B1/Grad for demand and supply
            var GradDem = StrdGradSolver(SPrice, BQuant, n, SPMean, BQMean);
            var GradSup = StrdGradSolver(BPrice, SQuant, n, BPMean, SQMean);
            //return B0/intercept for demand and supply
            var DemInt = IntSolver(GradDem, BQMean, SPMean);
            var SupInt = IntSolver(GradSup, SQMean, BPMean);
            //create predictions of data and add to predictions lists
            var PredDem = StrdPredictor(SPrice, GradSup, SupInt);
            var PredSup = StrdPredictor(BPrice, GradDem, DemInt);
            //return error on predicitons
            var DemErr = StrdErrorSolver(BQuant, PredDem);
            var SupErr = StrdErrorSolver(SQuant, PredSup);
            //add to Params and return
            Params[0, 0] = GradDem;
            Params[0, 1] = GradSup;
            Params[1, 0] = DemInt;
            Params[1, 1] = SupInt;
            Params[2, 0] = DemErr;
            Params[2, 1] = SupErr;
            return Params;
        }
    }
    /// <summary>
    /// Fitted model information
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        /// Calculates the Correlation Coefficient (pearson's)
        /// </summary>
        /// <param name="x">independent variable</param>
        /// <param name="x_avg">mean</param>
        /// <param name="y">dependent variable</param>
        /// <param name="y_avg">mean</param>
        /// <returns>Correlation Coefficient</returns>
        public static double[] CorrCoeff (Constants.RawData data, Constants.DataInfo info)
        {
            List<int> demquant = data.SellQuantity;
            List<int> supquant = data.BuyQuantity;
            List<int> demprice = data.BuyPrice;
            List<int> supprice = data.SellPrice;
            double demprice_avg = (double)info.BuyPriceInfo[ 0, 0 ];
            double supprice_avg = (double)info.SellPriceInfo[ 0, 0 ];
            double demquant_avg = (double)info.SellQuantInfo[ 0, 0 ];
            double supquant_avg = (double)info.BuyQuantInfo[ 0, 0 ];
            double[] Corrs = new double[2];
            double nominator = 0;
            double denominator = 0;
            double Denomxx = 0;
            double Denomyy = 0;

            for (int i = 0; i < demquant.Count(); i++)
            {
                nominator += (demprice.ElementAt(i) - demprice_avg) * (demquant.ElementAt(i) - demquant_avg);
                Denomxx += Math.Pow(demprice.ElementAt(i) - demprice_avg, 2); 
                Denomyy += Math.Pow(demquant.ElementAt(i) - demquant_avg, 2);
            }
            denominator = Math.Sqrt(Denomxx * Denomyy);
            Corrs[0] = nominator / denominator;
            for (int i = 0; i < demquant.Count(); i++)
            {
                nominator += (supprice.ElementAt(i) - supprice_avg) * (supquant.ElementAt(i) - supquant_avg);
                Denomxx += Math.Pow(supprice.ElementAt(i) - supprice_avg, 2);
                Denomyy += Math.Pow(supquant.ElementAt(i) - supquant_avg, 2);
            }
            denominator = Math.Sqrt(Denomxx * Denomyy);
            Corrs[1] = nominator / denominator;
            return Corrs;
        }
        /// <summary>
        /// Get the standard deviation of a fitted model
        /// </summary>
        /// <param name="Err">Error of model, a squared value</param>
        /// <param name="N">number of x, y pairs</param>
        /// <returns>sigma, standard deviation</returns>
        public static double StrdDev (List<int> Quant, Object[,] SetInfo, int N)
        {
            double Err = 0;
            foreach(double n in Quant)
            {
                Err += Math.Pow((n - (double)SetInfo[0, 0]), 2);
            }
            var StrdDev = Math.Sqrt((Err / N));
            return Math.Round(StrdDev);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Err">Squared Error prediction of linear fitted formula</param>
        /// <param name="N">Size of fitted equations datset (no. (x,y) pairs)</param>
        /// <returns>The Root Mean Squared Error of a fitted formula</returns>
        public static double RMSE(double Err, int N) => (Math.Round(Math.Sqrt(Err / N)));
    }
    /// <summary>
    /// Data Writing Tools
    /// </summary>
    public class DataWriter
    {
        /// <summary>
        /// Create a Human Readable File on the desktop. Including analysis of instances and fitted functions.
        /// </summary>
        /// <param name="info">RawData info object</param>
        /// <param name="n">item ID</param>
        /// <param name="RawFit">Fitted Parameters for rawdata</param>
        /// <param name="StrdFit">Fitted Parameters for Standardised Data</param>
        /// <param name="WeightedFit">Fitted Parameters for EWMA data</param>
        /// <param name="WeightedFit">Fitted Parameters for standardised EWMA data</param>
        public static void FileMaker(Constants.DataInfo info, int n, double[,] RawFit, double[,] StrdFit, double[] ModelInfo)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            if (RawFit == null)
            {
                throw new ArgumentNullException(nameof(RawFit));
            }
            if (StrdFit == null)
            {
                throw new ArgumentNullException(nameof(StrdFit));
            }
            if (ModelInfo == null)
            {
                throw new ArgumentNullException(nameof(ModelInfo));
            }

            string FileName = @"c: \Users\iTzEinstein118Xx\Desktop\AnalysisData" + Constants.id[n] + ".txt";
            try {
                    using (StreamWriter sw = new StreamWriter(FileName, false))
                    {
                        sw.WriteLine("Human Readable File: {0}\n", Constants.id[n]);
                        sw.WriteLine("Info for BuyQuant\n");
                        sw.WriteLine("Analysis of Instances:\n");
                        foreach (object inf in info.BuyQuantInfo)
                        {
                            string bqinf = Convert.ToString(inf);
                            sw.WriteLine(" " + bqinf + "\n");
                            sw.Write("\n");
                        }

                        sw.WriteLine("Info for BuyPrice");
                        sw.WriteLine("Analysis of Instances:\n");
                        foreach (object inf in info.BuyPriceInfo)
                        {
                            string binf = Convert.ToString(inf);
                            sw.WriteLine(" " + binf + "\n");
                        }

                        sw.WriteLine("Info for SellQuant");
                        sw.WriteLine("Analysis of Instances:\n");
                        foreach (object inf in info.SellQuantInfo)
                        {
                            string pqinfo = Convert.ToString(inf);
                            sw.WriteLine(" " + pqinfo + "\n");
                        }

                        sw.WriteLine("Info for SellPrice");
                        sw.WriteLine("Analysis of Instances:\n");
                        foreach (object inf in info.SellPriceInfo)
                        {
                            string sinf = Convert.ToString(inf);
                            sw.WriteLine(" " + sinf + "\n");
                        }

                    sw.WriteLine("Demand Function: {0} + {1}p \r R-Squared: {2}\n", RawFit[1, 0], RawFit[0, 0], RawFit[2, 0]);
                    sw.WriteLine("Supply Function: {0} + {1}p \r R-Squared: {2}\n", RawFit[1, 1], RawFit[0, 1], RawFit[2, 1]);
                    sw.WriteLine("\n");
                    sw.WriteLine("Standard Demand Function: {0} + {1}p \r R-Squared: {2}\n", StrdFit[1, 0], StrdFit[0, 0], StrdFit[2, 0]);
                    sw.WriteLine("Standard Demand Function: {0} + {1}p \r R-Squared: {2}\n", StrdFit[1, 1], StrdFit[0, 1], StrdFit[2, 1]);
                    sw.WriteLine("Raw Data Info: \n");
                    sw.WriteLine("Standard Dev, Demand: {0}", ModelInfo[0]);
                    sw.WriteLine("Standard Dev, Supply: {0}", ModelInfo[1]);
                    sw.WriteLine("Correlation, Demand: {0}", ModelInfo[6]);
                    sw.WriteLine("Correlation, Supply: {0}", ModelInfo[7]);
                    sw.WriteLine("Fitted Data Info: \n");
                    sw.WriteLine("Standard Dev, Fitted Demand: {0}", ModelInfo[2]);
                    sw.WriteLine("Standard Dev, Fitted Supply: {0}", ModelInfo[3]);
                    sw.WriteLine("RMSE, Demand: {0}", ModelInfo[4]);
                    sw.WriteLine("RMSE, Supply: {0}", ModelInfo[5]);
                    Console.Write("File Printed!\n");
                    }
            }
            finally
            {
            }
        }
    }
    /// <summary>
    /// Data tools
    /// </summary>
    class DataPreparer
    {
        /// <summary>
        /// de-standardize data
        /// </summary>
        /// <param name="ListStrd"></param>
        /// <returns></returns>
        public static List<double> DeStrd (List<double> ListStrd)
        {
            var Mean = ListStrd.Average();
            var Count = ListStrd.Count();
            double Sum = 0;
            List<double> DeStrd = new List<double>();
            //standard deviation
            for (int i = 0; i < Count; i++)
            {
                double Working = (ListStrd.ElementAt(i) - Mean);
                Sum += Math.Pow(Working, 2);
            }
            double StrdDev = Math.Sqrt(Sum / Count);
            //z-score solve for x
            double x;
            foreach (int i in ListStrd)
            {
                x = i*StrdDev + Mean;
                DeStrd.Add(x);
            }
            return DeStrd;
        }

        /// <summary>
        /// Standardise a single list of data
        /// </summary>
        /// <param name="Input"></param>
        /// <returns>Standardised data</returns>
        public static List<double> ListStrd(List<double> Input)
        {
            //z = (x- avg) / dev 
            var Mean = Input.Average();
            var Count = Input.Count();
            double Prev = 0;
            double Sum = 0;
            double Strd = 0;
            List<double> ListStrd = new List<double>();

            for (int i = 0; i < Count; i++)
            {
                double Current = Input.ElementAt(i);
                double Working = (Current - Mean);
                double Ans = Math.Pow(Working, 2);
                Sum = Prev + Ans;
                Prev = Ans;
            }
            //gets the standard deviation
            double StrdDev = Math.Sqrt(Sum / Count);
            
            foreach (int i in Input)
            {
                Strd = (i - Mean) / StrdDev;
                ListStrd.Add(Strd);
            }
            return ListStrd;
        }

        /// <summary>
        /// Raw Data Z-Score standardiser
        /// </summary>
        /// <param name="rawData">Raw data from item</param>
        /// <returns>Population standardised score of each data</returns>
        public static Constants.StrdData Z_Score(Constants.RawData rawData)
        {
            var strdbquant = new List<double>();
            var strdbprice = new List<double>();
            var strdsprice = new List<double>();
            var strdsquant = new List<double>();
            List<double>[] AccessList = new List<double>[] { strdbprice, strdbquant, strdsprice, strdsquant };
            var DataLists = new List<List<int>> { rawData.BuyPrice, rawData.BuyQuantity, rawData.SellPrice, rawData.SellQuantity };
            Constants.StrdData z_score = new Constants.StrdData(strdbprice, strdbquant, strdsprice, strdsquant);   

            for (int j = 0; j<AccessList.Count(); j++)
            {
                var WorkingList = new List<int>();
                WorkingList.AddRange(DataLists.ElementAt(j));
                double Mean = WorkingList.Average();
                int Count = WorkingList.Count();
                double Prev = 0;
                double Sum = 0;

                for (int i = 0; i < Count; i++)
                {
                    //current is always the next element in the list
                    double Current = WorkingList.ElementAt(i);

                    double Working = (Current - Mean);
                    double Ans = Math.Pow(Working, 2);
                    Sum = Prev + Ans;
                    Prev = Ans;
                }
                //gets the standard deviation
                double StrdDev = Math.Sqrt(Sum / Count);
                double Strd = 0;

                //standardises the input list
                foreach (int i in WorkingList)
                {
                    Strd = (i - Mean) / StrdDev;
                    AccessList.ElementAt(j).Add(Strd);
                }
                WorkingList.Clear();
            }
            z_score._strdBuyPrice = strdbprice;
            z_score._strdBuyQuant = strdbquant;
            z_score._strdSellPrice = strdsprice;
            z_score._strdSellQuant = strdsquant;
            return z_score;
        }

        /// <summary>
        ///  Raw data file reader of item
        /// </summary>
        /// <param name="a">Takes input of which item from Constants.Id[]</param>
        /// <returns>returns list of each of the four parameters</returns>
        public static Constants.RawData Splicer(int a)
        {
            var sprice = new List<int>();
            var squant = new List<int>();
            var bprice = new List<int>();
            var bquant = new List<int>();
            Constants.RawData rawdata = new Constants.RawData(sprice, squant, bprice, bquant);     

            int id = Constants.id[a];
            string file = @"C:\Users\iTzEinstein118Xx\Documents\ItemDataOutput\" + id + ".txt";
            using (TextReader tr = File.OpenText(file))
            {
                int linecount = File.ReadLines(file).Count();
                for (int y = 0; y < linecount; y++)
                {
                    string line = tr.ReadLine();
                    //{ "buy price", "buy quantity", "sell price", "sell quantity" };
                    foreach (string match in Constants.StringToMatch)
                    {
                        if (line.Contains(match))
                        {
                            string[] bits = line.Split(' ');
                            int j = bits.Length;
                            try
                            {
                                //parsing bits, the number is always at the end of the line
                                int num = Int32.Parse(bits[j - 1]);
                                //add the number to dataset
                                //use GetRawData method, Constants.RawData
                                //list title as string, returning the list itself
                                List<int> workingdata = rawdata.GetRawData(match);
                                workingdata.Add(num);
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Bad Format");
                            }
                        }
                    }                              
                }
                return rawdata;
            }
        }

        /// <summary>
        /// Item data, Mean, Min, Max, Median and Set Length
        /// </summary>
        /// <param name="rawdata">Input data object, with four lists</param>
        /// <returns>DataInfo object, Mean, Min, Max, Median and Set Length</returns>
        public static Constants.DataInfo DataInfo()
        {
            Constants.RawData rawdata = Constants.Item.GetRawData();
            Object[,] spriceinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            Object[,] squantinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            Object[,] bpriceinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            Object[,] bquantinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            List<int> sprice = new List<int>();
            List<int> squant = new List<int>();
            List<int> bprice = new List<int>();
            List<int> bquant = new List<int>();

            Constants.DataInfo datainfo = new Constants.DataInfo(spriceinfo, squantinfo, bpriceinfo, bquantinfo);

            bpriceinfo[0,0] = rawdata.BuyPrice.Average();
            bpriceinfo[1,0] = rawdata.BuyPrice.Min();
            bpriceinfo[2,0] = rawdata.BuyPrice.Max();
            bpriceinfo[4,0] = rawdata.BuyPrice.Count();
            bprice.AddRange(rawdata.BuyPrice);
            bprice.Sort();
            int i = (bprice.Count() + 1) / 2;
            bpriceinfo[3,0] = bprice.ElementAt(i);

            spriceinfo[0,0] = rawdata.SellPrice.Average();
            spriceinfo[1,0] = rawdata.SellPrice.Min();
            spriceinfo[2,0] = rawdata.SellPrice.Max();
            spriceinfo[4,0] = rawdata.SellPrice.Count();
            sprice.AddRange(rawdata.SellPrice);
            sprice.Sort();
            int j = (sprice.Count() + 1) / 2;
            spriceinfo[3, 0] = sprice.ElementAt(j);

            bquantinfo[0,0] = rawdata.BuyQuantity.Average();
            bquantinfo[1,0] = rawdata.BuyQuantity.Min();
            bquantinfo[2,0] = rawdata.BuyQuantity.Max();
            bquantinfo[4,0] = rawdata.BuyQuantity.Count();
            bquant.AddRange(rawdata.BuyQuantity);
            bquant.Sort();
            int k = (bquant.Count() + 1) / 2;
            bquantinfo[3, 0] = bquant.ElementAt(k);

            squantinfo[0,0] = rawdata.SellQuantity.Average();
            squantinfo[1,0] = rawdata.SellQuantity.Min();
            squantinfo[2,0] = rawdata.SellQuantity.Max();
            squantinfo[4,0] = rawdata.SellQuantity.Count();
            squant.AddRange(rawdata.SellQuantity);
            squant.Sort();
            int l = (squant.Count() + 1) / 2;
            squantinfo[3,0] = squant.ElementAt(l);
            datainfo.BuyPriceInfo = bpriceinfo;
            datainfo.BuyQuantInfo = bquantinfo;
            datainfo.SellQuantInfo = squantinfo;
            datainfo.SellPriceInfo = spriceinfo;

            return datainfo;
        }

        /// <summary>
        /// Item data, Mean, Min, Max, Median and Set Length
        /// </summary>
        /// <param name="rawdata">Input standard data object</param>
        /// <returns>DataInfo object, Mean, Min, Max, Median and Set Length</returns>
        public static Constants.DataInfo StrdDataInfo(Constants.StrdData strdData)
        {
            Object[,] spriceinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            Object[,] squantinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            Object[,] bpriceinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            Object[,] bquantinfo = new Object[,] { { 0, "Mean" }, { 0, "Min" }, { 0, "Max" }, { 0, "Median" }, { 0, "Set Length" } };
            List<double> sprice = new List<double>();
            List<double> squant = new List<double>();
            List<double> bprice = new List<double>();
            List<double> bquant = new List<double>();

            Constants.DataInfo datainfo = new Constants.DataInfo(spriceinfo, squantinfo, bpriceinfo, bquantinfo);

            bpriceinfo[0, 0] = strdData._strdBuyPrice.Average();
            bpriceinfo[1, 0] = strdData._strdBuyPrice.Min();
            bpriceinfo[2, 0] = strdData._strdBuyPrice.Max();
            bpriceinfo[4, 0] = strdData._strdBuyPrice.Count();
            bprice.AddRange(strdData._strdBuyPrice);
            bprice.Sort();
            int i = (bprice.Count() + 1) / 2;
            bpriceinfo[3, 0] = bprice.ElementAt(i);

            spriceinfo[0, 0] = strdData._strdSellPrice.Average();
            spriceinfo[1, 0] = strdData._strdSellPrice.Min();
            spriceinfo[2, 0] = strdData._strdSellPrice.Max();
            spriceinfo[4, 0] = strdData._strdSellPrice.Count();
            sprice.AddRange(strdData._strdSellPrice);
            sprice.Sort();
            int j = (sprice.Count() + 1) / 2;
            spriceinfo[3, 0] = sprice.ElementAt(j);

            bquantinfo[0, 0] = strdData._strdBuyQuant.Average();
            bquantinfo[1, 0] = strdData._strdBuyQuant.Min();
            bquantinfo[2, 0] = strdData._strdBuyQuant.Max();
            bquantinfo[4, 0] = strdData._strdBuyQuant.Count();
            bquant.AddRange(strdData._strdBuyQuant);
            bquant.Sort();
            int k = (bquant.Count() + 1) / 2;
            bquantinfo[3, 0] = bquant.ElementAt(k);

            squantinfo[0, 0] = strdData._strdSellQuant.Average();
            squantinfo[1, 0] = strdData._strdSellQuant.Min();
            squantinfo[2, 0] = strdData._strdSellQuant.Max();
            squantinfo[4, 0] = strdData._strdSellQuant.Count();
            squant.AddRange(strdData._strdSellQuant);
            squant.Sort();
            int l = (squant.Count() + 1) / 2;
            squantinfo[3, 0] = squant.ElementAt(l);
            datainfo.BuyPriceInfo = bpriceinfo;
            datainfo.BuyQuantInfo = bquantinfo;
            datainfo.SellQuantInfo = squantinfo;
            datainfo.SellPriceInfo = spriceinfo;

            return datainfo;
        }

        /// <summary>
        ///Weight standardised data
        /// </summary>
        /// <param name="strdData">Standard Data object</param>
        /// <param name="Alpha">Higher alpha discards data faster</param>
        /// <returns>Weighted, Standardised data</returns>
        public static Constants.StrdData EWMA(Constants.StrdData strdData, double Alpha)
        {
            var strdbquant = new List<double>();
            var strdbprice = new List<double>();
            var strdsprice = new List<double>();
            var strdsquant = new List<double>();
            List<double>[] AccessList = new List<double>[] { strdbprice, strdbquant, strdsprice, strdsquant};
            List<double>[] DataLists = new List<double>[] { strdData._strdBuyPrice, strdData._strdBuyQuant, strdData._strdSellPrice, strdData._strdSellQuant };
            Constants.StrdData ewma = new Constants.StrdData(strdbprice, strdbquant,strdsprice,strdsquant);

            for (int j = 0; j < AccessList.Count(); j++)
            {
                double[] WeightedList = new double[DataLists.ElementAt(j).Count()];
                List<double> CurrentList = DataLists.ElementAt(j);

                double X_init = CurrentList.ElementAt(0);
                //first X value is not weighted, used to initiallise weighting
                WeightedList[0] = X_init;
                double Weight_prev = X_init;
                double Weight_Current = 0;

                for (int i = 0; i < CurrentList.Count(); i++)
                {
                    Weight_Current = Alpha * CurrentList.ElementAt(i) + (1 - Alpha) * Weight_prev;
                    WeightedList[i] = Weight_Current;
                    Weight_prev = Weight_Current;
                }

                AccessList[j].AddRange(WeightedList);

            }

            ewma._strdBuyPrice = strdbprice;
            ewma._strdBuyQuant = strdbquant;
            ewma._strdSellPrice = strdsprice;
            ewma._strdSellQuant = strdsquant;

            return ewma;
        }

        /// <summary>
        /// weighting function for non-standardised data (raw data)
        /// </summary>
        /// <param name="rawdata">Raw data object</param>
        /// <param name="Alpha">higher alpha discards quicker</param>
        /// <returns>Pure weighted data</returns>
        public static Constants.StrdData Weighter(Constants.RawData rawdata, double Alpha)
        {
            var strdbquant = new List<double>();
            var strdbprice = new List<double>();
            var strdsprice = new List<double>();
            var strdsquant = new List<double>();

            List<double> BuyQ = rawdata.BuyQuantity.Select<int, double>(i => i).ToList();
            List<double> BuyP = rawdata.BuyPrice.Select<int, double>(i => i).ToList();
            List<double> SellQ = rawdata.SellQuantity.Select<int, double>(i => i).ToList();
            List<double> SellP = rawdata.SellPrice.Select<int, double>(i => i).ToList();

            List<double>[] AccessList = new List<double>[] { strdbprice, strdbquant, strdsprice, strdsquant };
            List<double>[] DataLists = new List<double>[] { BuyP, BuyQ, SellP, SellQ };
            Constants.StrdData ewma = new Constants.StrdData(strdbprice, strdbquant, strdsprice, strdsquant);

            for (int j = 0; j < DataLists.Count(); j++)
            {
                double[] WeightedArray = new double[DataLists.ElementAt(j).Count()];
                List<double> CurrentList = DataLists.ElementAt(j);

                double X_init = CurrentList.ElementAt(0);
                //first X value is not weighted, used to initiallise weighting
                WeightedArray[0] = X_init;
                double Weight_prev = X_init;
                double Weight_Current = 0;

                for (int i = 0; i < CurrentList.Count(); i++)
                {
                    Weight_Current = Alpha * CurrentList.ElementAt(i) + (1 - Alpha) * Weight_prev;
                    WeightedArray[i] = Weight_Current;
                    Weight_prev = Weight_Current;
                }

                AccessList[j].AddRange(WeightedArray);
            }

            ewma._strdBuyPrice = strdbprice;
            ewma._strdBuyQuant = strdbquant;
            ewma._strdSellPrice = strdsprice;
            ewma._strdSellQuant = strdsquant;

            return ewma;
        }

        /// <summary>
        /// Trims the raw data object length to desired length, leaves original length - n
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="ListLength">Datapoints to remove</param>
        /// <returns></returns>
        public static void Trimmer (Constants.RawData rawData, int ListLength)
        {
            
            int BQcount = rawData.BuyQuantity.Count - ListLength;
            rawData.BuyQuantity.RemoveRange(ListLength, BQcount);
            int BPcount = rawData.BuyPrice.Count - ListLength;
            rawData.BuyPrice.RemoveRange(ListLength, BPcount);
            int SQcount = rawData.SellQuantity.Count - ListLength;
            rawData.SellQuantity.RemoveRange(ListLength, SQcount);
            int SPcount = rawData.SellPrice.Count - ListLength;
            rawData.SellPrice.RemoveRange(ListLength, SPcount);
        }
    }
}