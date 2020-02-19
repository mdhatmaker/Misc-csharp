using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.ML.Data;
using MLPredictHousingML.Model;

namespace MLPredictHousing
{
    class Program
    {
        static string csvFilePath = @"C:\GitHub\Misc-csharp\MLPredictHousing\data\Housing_Data.csv";

        static void Main(string[] args)
        {
            Console.WriteLine("Predict Boston Housing Prices with ML\n");

            // Only need to call this once to create our usable .csv file
            //var txtFilePath = @"C:\GitHub\Misc-csharp\MLPredictHousing\data\housing.data.txt";
            //HelperML.ReformatDataFile(txtFilePath, csvFilePath);

            const int nSamplePredictions = 50;
            int[] int_arr = new int[nSamplePredictions];

            // Create array of random integers
            Random rnd = new Random();
            int min = 0, max = 500;
            for (int i = 0; i < int_arr.Length; ++i)
                int_arr[i] = rnd.Next(min, max);

            var int_list = int_arr.ToList();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            //foreach (int dataRowIndex in int_list)
            //    PredictForDataRow(dataRowIndex);
            //int_list.ForEach(ix => PredictForDataRow(ix));
            //int numEven = int_list.Aggregate(0, (total, next) => next % 2 == 0 ? total + 1 : total);
            //int_list.ForEach(ix => PredictForDataRow(ix));

            /*double sumPctDiff = int_list.Aggregate(0.0, (total, next) => total + Math.Abs(PredictForDataRow(next).PctDiff));
            double avg = sumPctDiff / int_list.Count();*/

            /*var results = from ix in int_list
                          select PredictForDataRow(ix);
            var diffs = from r in results
                        select Math.Abs(r.PctDiff);*/

            var modelData = HelperML.GetModelData(csvFilePath);

            var diffs = from ix in int_list
                        select Math.Abs(PredictForDataRow(modelData, ix).PctDiff);
            double avg = diffs.Average();

            timer.Stop();

            Console.WriteLine($"\nAverage % Diff of Predictions: {avg:n2}%");

            var elapsed = new HelperML.ElapsedTime(timer);
            Console.WriteLine("\n" + elapsed);
            //elapsed.Print("\n", "\n");

            Console.WriteLine("\n\n=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }

        

        /// Get sample model input data from a single row in the .csv file 
        static HelperML.PredictionResults PredictForDataRow(int dataRowIndex)
        {
            // Create sample model input data from a single line in the specified .csv file
            ModelInput sampleData = HelperML.CreateSingleDataSample(csvFilePath, dataRowIndex);

            // Make a single prediction on the sample data and print results
            ModelOutput predictionResult = ConsumeModel.Predict(sampleData);

            var pctDiff = (predictionResult.Score - sampleData.MEDV) / sampleData.MEDV * 100.0;

            //HelperML.EvaluatePrediction(sampleData, predictionResult);
            Console.WriteLine(sampleData + $"  PredictedMEDV:{predictionResult.Score,-7:n2}     ({pctDiff,5:n1}%)");

            return new HelperML.PredictionResults(sampleData.MEDV, predictionResult.Score, pctDiff);
        }

        /// Like PredictionResults method above, but uses the HelperML.GetModelData
        /// method to create the data enumerable just once
        static HelperML.PredictionResults PredictForDataRow(IEnumerable<ModelInput> modelData, int dataRowIndex)
        {
            // Create sample model input data from a single line in the specified .csv file
            ModelInput sampleData = modelData.ElementAt(dataRowIndex);

            // Make a single prediction on the sample data and print results
            ModelOutput predictionResult = ConsumeModel.Predict(sampleData);

            var pctDiff = (predictionResult.Score - sampleData.MEDV) / sampleData.MEDV * 100.0;

            //HelperML.EvaluatePrediction(sampleData, predictionResult);
            Console.WriteLine(sampleData + $"  PredictedMEDV:{predictionResult.Score,-7:n2}     ({pctDiff,5:n1}%)");

            return new HelperML.PredictionResults(sampleData.MEDV, predictionResult.Score, pctDiff);
        }

    } // end of class Program


} // end of namespace
