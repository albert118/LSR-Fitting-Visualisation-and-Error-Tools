using System;
using System.Collections.Generic;

namespace MachineLearnerWPF
{
    public const int[] id = { 12429, 19697, 30699, 38131 };
    public class Item
    {
        // private fields
        private int item_id;
        private List<double> sell_quant;
        private List<double> buy_quant;
        private List<double> sell_price;
        private List<double> buy_price;

        // a statistics dictionary
        private Dictionary<string, double> StatisticDataPairs =
        {
            { "mean", 0 },
            { "min", 0},
            { "max", 0},
            { "median", 0},
            { "mode", 0},
            { "set length", 0}
        };

        // fitted data dictionary
        private Dictionary<string, Dictionary<string, double>> FunctionTypeFunctionDataPairs =
        {
            "Supply", new Dictionary<string, double>
            {
                { "Grad", 0 },
                { "x intercept", 0},
            },
            "Demand", new Dictionary<string, double>
            {
                { "Grad", 0 },
                { "x intercept", 0},
            }
        };

        // public constants
        public const Dictionary<string, List<double>> DataList_to_Names =
        {
                { "buy price", buy_price},
                { "buy quantity", buy_quants},
                { "sell price", sell_price},
                { "sell quantity", sell_quant}
        };

        public Item(List<double> buy_price, List<double> buy_quant, List<double> sell_price, List<double> sell_quant, int n) // constructer
        {
            buy_price = this.buy_price;
            buy_quant = this.buy_quant;
            sell_price = this.sell_price;
            sell_quant = this.sell_quant;
            item_id = id[n];
        }

        // public methods, get's & set's
        public List<double> Sell_Quant { get => sell_quant; set => sell_quant = value; }
        public List<double> Buy_Quant { get => buy_quant; set => buy_quant = value; }
        public List<double> Sell_Price { get => sell_price; set => sell_price = value; }
        public List<double> Buy_Price { get => buy_price; set => buy_price = value; }

        // edit the StatisticDataPairs statistics dict
        public void SetDataInfo(Dictionary<string, double> input_DataInfo)
        {
            foreach (KeyValuePair<string, double> input_data in input_DataInfo)
            {
                for (int i = 0; i < StatisticDataPairs.Count(); i++)
                {
                    if (input_data.Key == StatisticDataPairs.Keys.ElementAt(i))
                    {
                        StatisticDataPairs[i] = input_data.Value;
                    }
                }

            }
        }
        public double GetDataInfo(string StatName)
        {
            try
            {
                foreach (KeyValuePair<string, double> stat_data in StatisticDataPairs)
                {
                    if (stat_data.Key == StatName)
                    {
                        return stat_data.Value;
                    }
                }
            }
            catch
            {
                throw new ArgumentException("Statistic: {0} does not exist in dictionary!", StatName);
            }
        }
        public void SetFittedData(string data_key, string Function_Name, double data_value)
        {
            Dictionary<string, double> func_data = FunctionTypeFunctionDataPairs[Function_Name];
            func_data[data_key] = data_value;
        }
    }
}