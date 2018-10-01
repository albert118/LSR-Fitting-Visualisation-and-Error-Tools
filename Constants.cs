using System;
using System.Collections.Generic;

namespace MachineLearnerWPF
{
    public class Constants
    {
        /// <summary>
        /// Iteration order for rawdata collection, always order of lists if unknown
        /// </summary>
        public static readonly string[] StringToMatch = { "buy price", "buy quantity", "sell price", "sell quantity" };
        /// <summary>
        /// Program iteration order
        /// </summary>
        public static readonly int[] id = { 12429, 19697, 30699, 38131 };
        /// <summary>
        /// Item Object
        /// </summary>
        public class Item
        {
            private static Item _instance;
            //lock sync of object
            private static object syncLock = new object();
            private static List<int> Squant = new List<int>();
            private static List<int> Bquant = new List<int>();
            private static List<int> Sprice = new List<int>();
            private static List<int> Bprice = new List<int>();
            private static RawData rawData = new RawData(Sprice, Squant, Bprice, Bquant);
            private static int DataCount = 0;

            //protecting constructer
            protected Item(RawData Data,int n)
            {
                DataCount += n;
                rawData.BuyPrice.AddRange(Data.BuyPrice);
                rawData.SellPrice.AddRange(Data.SellPrice);
                rawData.BuyQuantity.AddRange(Data.BuyQuantity);
                rawData.SellQuantity.AddRange(Data.SellQuantity);  
            }
            public static RawData GetRawData()
            {
                return rawData;
            }
            public static int GetCount()
            {
                return DataCount;
            }
            public static Item GetItem(RawData rawData, int n)
            {
                if (_instance == null)
                {
                    lock (syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Item(rawData, n);
                        }
                    }
                }

                return _instance;
            }
            public static void ClearItem(Item item)
            {
                DataCount = 0;
                rawData.BuyPrice.Clear();
                rawData.SellPrice.Clear();
                rawData.BuyQuantity.Clear();
                rawData.SellQuantity.Clear();
            }
        }
        /// <summary>
        /// Input Data
        /// </summary>
        public class RawData
        {
            //Raw data
            public List<int> BuyPrice { get; set; }
            public List<int> BuyQuantity { get; set; }           
            public List<int> SellPrice { get; set; }           
            public List<int> SellQuantity { get; set; }
            private List<int>[] ListArray { get; set; } = new List<int>[4];

            public RawData(List<int> SPrice, List<int> SQuant, List<int> BPrice, List<int> BQuant)
            {
                BuyPrice = BPrice;
                ListArray[0] = BuyPrice;
                BuyQuantity = BQuant;
                ListArray[1] = BuyQuantity;
                SellPrice = SPrice;
                ListArray[2] = SellPrice;
                SellQuantity = SQuant;
                ListArray[3] = SellQuantity;
            }
            //returns rawdata based on title of complist
            public List<int> GetRawData(string title)
            {
                foreach(string match in StringToMatch)
                {
                    if (match == title)
                    {
                        int index = Array.IndexOf(StringToMatch, match);
                        return ListArray[index];
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Standardised Data Structures
        /// </summary>
        public class StrdData
        {
            //Standardised data
            public List<double> _strdBuyQuant { get; set; }
            public List<double> _strdBuyPrice { get; set; }
            public List<double> _strdSellQuant { get; set; }
            public List<double> _strdSellPrice { get; set; }

            public StrdData(List<double> StrdBPrice, List<double> StrdBQuant, List<double> StrdSPrice, List<double> StrdSQuant)
            {
                StrdBPrice = _strdBuyPrice;
                StrdBQuant = _strdBuyQuant;
                StrdSPrice = _strdSellPrice;
                StrdSQuant = _strdSellQuant;
            }
        }
        /// <summary>
        /// Information on Raw Dataset
        /// </summary>
        public class DataInfo
        {
            //Info on datasets
            public Object[,] BuyPriceInfo { get; set; }
            public Object[,] BuyQuantInfo { get; set; }
            public Object[,] SellPriceInfo { get; set; }
            public Object[,] SellQuantInfo { get; set; }

            /// <summary>
            /// Structure for storing information on rawdata
            /// </summary>
            /// <param name="SPriceInfo"></param>
            /// <param name="SQuantInfo"></param>
            /// <param name="BPriceInfo"></param>
            /// <param name="BQuantInfo"></param>
            public DataInfo(Object[,] SPriceInfo, Object[,] SQuantInfo, Object[,] BPriceInfo, Object[,] BQuantInfo)
            {
                SPriceInfo = SellPriceInfo;
                SQuantInfo = SellQuantInfo;
                BPriceInfo = BuyPriceInfo;
                BQuantInfo = BuyQuantInfo;
            }
        }
    }
}