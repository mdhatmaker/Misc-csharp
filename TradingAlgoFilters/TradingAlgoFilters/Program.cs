using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace TradingAlgoFilters
{
    public class CoinbaseCsvType
    {
        public string Date { get; set; }
        public string Symbol { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        [Name("Volume BTC")]
        public double VolumeBTC { get; set; }
        [Name("Volume USD")]
        public double VolumeUSD { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /*if (args.Length < 1)
            {
                Console.WriteLine("Please provide an image file name as an argument");
                return;
            }

            var fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} does not exist");
                return;
            }*/

            
            var sw = Stopwatch.StartNew();

            //var gf = new GaussianFilter();
            //gf.Calculate();

            double[] source;
            string[] sourceDate;
            string pathname = @"D:\Git\Misc-python\data\Coinbase_BTCUSD_d.csv";
            using (var reader = new StreamReader(pathname))
            using (var csv = new CsvReader(reader))
            {
                //csv.Configuration.PrepareHeaderForMatch =
                //    header => Regex.Replace(header, @"\s", string.Empty);

                var li = csv.GetRecords<CoinbaseCsvType>();
                //int n = li.Count();
                var liSource = new List<double>();
                var liSourceDate = new List<string>();
                foreach (var rec in li.Reverse())
                {
                    liSource.Add(rec.Close);
                    liSourceDate.Add(rec.Date);
                }
                source = liSource.ToArray();
                sourceDate = liSourceDate.ToArray();
                //sourceDate = li.Select(x => x.Date).ToArray();  // .ToString()).Reverse().ToArray();
                //source = li.Select(x => x.Close).Reverse().ToArray();                

                var gf = new GaussianFilterAlgo(7, 3);
                gf.Source = source;
                gf.CalculateAll();

                for (int i = 0; i < source.Length; ++i)
                {
                    string ind = " ";
                    if (i > 0)
                    {
                        if (gf.Result[i] > gf.Result[i - 1])
                            ind = "+";
                        else if (gf.Result[i] < gf.Result[i - 1])
                            ind = "-";
                    }
                    Console.WriteLine("{0} {1,10:0.00} {2,10:0.00}   {3}", sourceDate[i], source[i], gf.Result[i], ind);
                }

                Console.WriteLine(source.Length);
            }


            Console.WriteLine($"Finished in: {sw.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }
    }
}
