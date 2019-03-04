using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace MachineLearnerWPF
{
    public class FittedFunction
    {
        // private fields
        private double[] x_vals, y_vals, predictor_vals, error_vals;
        private double avg_x, avg_y, gradient, x_intercept;

        /// <summary>
        /// A Fitted Function Object,
        /// Declare an instance of this object with list data,
        /// This is converted to array structure and allows 
        /// return function to be called in one line.
        /// All operations are internal and make fitting an LSR function
        /// simple & elegant!
        /// </summary>
        /// <param name="X_Vals">List of x values to be regressed on. </param>
        /// <param name="Y_Vals">List of y values to be predicted against. </param>
        public FittedFunction(List<double> X_Vals, List<double> Y_Vals, Item item, string function_name) // contstructor
        {
            using (StreamWriter(fn) as _errStream)
            {
                // activate the logging on instance decleration
                DateTime appStart = DateTime.Now;
                string fn = @"C:\Users\alber\Documents" + appStart.ToString("yyyyMMddHHmm") + ".log";
                // Redirect standard error stream to file
                Console.SetError(_errStream);

                // check list length, apply trimming method (with array conversion) if not, 
                // else convert to arrays, otherwise log error data
                X_list_len = x_vals.Count();
                Y_list_len = y_vals.Count();

                if (X_list_len != Y_list_len)
                {

                    int fnl_count = Trimmer(X_Vals, Y_Vals);
                    //Write output of Trimmer method
                    Console.Error.WriteLine("Trimming data set {0} from {1} values to {2} values...", "X Data List", X_list_len, fnl_count);
                    Console.Error.WriteLine("Trimming data set {0} from {1} values to {2} values...", "Y Data List", Y_list_len, fnl_count);

                    predictor_vals = new double[fnl_count];
                    error_vals = new double[fnl_count];
                }
                else if (X_list_len == Y_list_len)
                {
                    X_DataVals.set = X_Vals.ToArray();
                    Y_DataVals.set = Y_Vals.ToArray();
                    // arbitrary choice on name for array def'
                    predictor_vals = new double[X_list_len];
                    error_vals = new double[X_list_len];
                }
                else
                {
                    // TODO: Logging methods seperated!!
                    var stack_trace = new StackTrace();
                    var stack_frame = stack_trace.GetFrame(0);
                    var method_err = ("Current Method: {0} \n", stack_frame.GetMethod());
                    Console.Error.WriteLine(method_err);
                    Console.Error.WriteLine("Stack Frame: {0}\n", stack_frame);
                    throw new ArgumentException(method_err);
                }

                // get avg's then find gradient & x intercept
                avg_x = x_vals.Average();
                avg_y = y_vals.Average();
                gradient = GradSolver(x_vals, y_vals, avg_x, avg_y);
                x_intercept = InterceptCalc(gradient, avg_x, avg_y);
                for (int i = 0; i < y_vals.Count(); i++) { error_vals[i] = y_vals[i] - predictor_vals[i]; } // find the errors
                Predictor(x_vals, gradient, x_intercept); // find the predictions

                // now finalise all the data in the items dictionaries
                item.SetFittedData("x intercept", function_name, x_intercept);
                item.SetFittedData("Grad", function_name, gradient);
            }
        }

        // private methods
        private Func<Grad, avg_x, avg_y> InterceptCalc = x_int => { return avg_y - Grad * avg_x; }; // much simpler as a Lambda expr.
        private double GradSolver(double[] x_vals, double[] y_vals, double avg_x, double avg_y)
        {
            double Denom;
            double SumNom = 0, SumDenom = 0;
            int x_val_len = x_vals.Length();
            int y_val_len = y_vals.Length();

            for (int i = 0; i < n; i++)
            {
                var x = x_vals[i]; // single lookup rather than two, prob' infentesimal difference but cleaner
                SumNom += (x - AvgX) * (y_vals[i] - AvgY);
                Denom = (x - AvgX);
                SumDenom += Math.Pow(Denom, 2);
            }
            Grad = SumNom / SumDenom; // this could be done in a single line, but more readable this way
            return Grad;
        }
        private int Trimmer(List<double> x_vals, List<double> y_vals)
        {

            int X_MAX = x_vals.Count();
            int Y_MAX = y_vals.Count();

            if (X_MAX > Y_MAX)
            {
                int diff = X_MAX - Y_MAX;
                x_vals.RemoveRange(y_vals(Y_MAX - 1), diff);
                // now convert here to array
                X_DataVals.set = x_vals.ToArray();
                Y_DataVals.set = y_vals.ToArray();
                return Y_MAX;
            }
            else if (X_MAX < Y_MAX)
            {
                int diff = Y_MAX - X_MAX;
                y_vals.RemoveRange(x_vals(X_MAX - 1), diff);
                // now convert here to array
                X_DataVals.set = x_vals.ToArray();
                Y_DataVals.set = y_vals.ToArray();
                return X_MAX;
            }
            else
            {
                throw new Exception("Unknown comparison between data list lengths.");
            }

        }
        private void Predictor(double[] x_vals, double Grad, double x_intercept) // edits the list defined in private fields
        {
            for (int i = 0; i < Price.Count(); i++)
            {
                double pred = Int + Grad * Price.ElementAt(i);
                predictor_vals.Add(pred);
            }
            return;
        }

        // public methods, get's & set's
        public double[] X_DataVals { get => x_vals; set => x_vals = value; }
        public double[] Y_DataVals { get => y_vals; set => y_vals = value; }
        public double[] Predicted_Values { get => predicor_vals; }
        public double[] Prediction_Errors { get => error_vals;  }
    }

    public class ModelInfo
    {
        public double Correlation_Coefficient(double[] x_vals, double[] y_vals, double x_avg, double y_avg)
        {
            int count = x_vals.Length();
            if (count == y_vals.Length())
            {
                var sum_x = x_vals.Sum();
                var sum_y = y_vals.Sum();
                double Correlation = 0.0;
                double denominator, denom_LHS, denom_RHS = 0.0;
                double nominator, nom_LHS, nom_RHS = 0.0;

                nom_RHS = sum_x * sum_y;

                for (int i = 0; i < count; i++)
                {
                    nom_LHS += x_vals[i] * y_vals[i];
                    denom_LHS += Math.Pow(x_vals[i], 2);
                    denom_RHS += Math.Pow(y_vals[i], 2);
                }
                denom_LHS = count * denom_LHS - sum_x;
                denom_RHS = count * denom_RHS - sum_y;
                denominator = Math.Sqrt(denom_LHS * denom_RHS);
                nominator = nom_LHS - nom_RHS;

                Correlation = nominator / denominator;
                return Correlation;
            }
            else
            {
                throw new ArgumentException("Array data must be equal length, trim the data first!");
            }
            
        }    
        public double Standard_Deviation (double[] x_vals, double x_avg, int N)
        {
            
            double mean_err, strd_dev = 0.0;
            foreach (double x in x_vals)
            {
                mean_err += Math.Pow((x- x_avg), 2);
            }
            strd_dev = Math.Sqrt((mean_err / N));
            return strd_dev;
        }    
        public double RMSE(double Err, int N) => (Math.Round(Math.Sqrt(Err / N)));
    }
    public class DataWriter
    {
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
    class DataPreparer
    {
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
    }
}