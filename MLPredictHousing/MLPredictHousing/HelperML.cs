using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection; 
using MLPredictHousingML.Model;
using Microsoft.ML;
using System.Linq;
using System.Diagnostics;

namespace MLPredictHousing
{
    public static class HelperML
    {
        #region STRUCTS
        // Immutable struct for storing results of a prediction
        public struct PredictionResults
        {
            public double Actual { get; }
            public double Predicted { get; }
            public double PctDiff { get; }

            public PredictionResults(double actual, double predicted, double pctDiff)
            {
                Actual = actual;
                Predicted = predicted;
                PctDiff = pctDiff;
            }
        }

        // Immutable struct for storing elapsed time (nanos, micros, millis)
        public struct ElapsedTime
        {
            public ulong Milliseconds { get; }
            public ulong Microseconds { get; }
            public ulong Nanoseconds { get; }

            public ElapsedTime(Stopwatch timer)
            {
                Milliseconds = GetMilliseconds(timer);
                Microseconds = GetMicroseconds(timer);
                Nanoseconds = GetNanoseconds(timer);
            }

            public override string ToString()
            {
                string rv = $"Elapsed Time (nanos): {this.Nanoseconds:#,000}"
                            + $"   (micros): {this.Microseconds:#,000}"
                            + $"   (millis): {this.Milliseconds:#,000}";
                return rv;
            }

            public void Print(string pre = "", string post = "")
            {
                Console.WriteLine(pre + $"Elapsed Time (nanoseconds) : {this.Nanoseconds,15:#,000}");
                Console.WriteLine($"Elapsed Time (microseconds): {this.Microseconds,15:#,000}");
                Console.WriteLine($"Elapsed Time (milliseconds): {this.Milliseconds,15:#,000}" + post);
            }
        }
        #endregion


        // dataPath = @"C:\GitHub\Misc-csharp\MLPricePrediction\taxi-fare-train.txt";

        // Given a .txt file (no column headers, tab-separated)
        // Convert to a .csv file (with column headers, comma-separated)
        static void ReformatDataFile(string txtFilePath, string csvFilePath)
        {
            using (var instream = new StreamReader(txtFilePath))
            using (var outstream = new StreamWriter(csvFilePath))
            {
                outstream.WriteLine("CRIM,ZN,INDUS,CHAS,NOX,RM,AGE,DIS,RAD,TAX,PTRATIO,B,LSTAT,MEDV");
                string line;
                while ((line = instream.ReadLine()) != null)
                {
                    var cols = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var formatted = string.Join(',', cols);
                    outstream.WriteLine(formatted);
                }
            }
        }

        // TODO: this method isn't finished
        // Generalized method to read a line from a .csv file and populate a ModelInput object
        public static ModelInput ParseDataLine(int lineNumber, string dataFilePath)
        {
            string line = "";
            using (var f = new StreamReader(dataFilePath))
            {
                for (int i = 0; i < lineNumber; ++i)
                    line = f.ReadLine();
            }

            var items = line.Split(',');

            var input = new ModelInput();

            /*input.Vendor_id = items[0];
            input.Rate_code = int.Parse(items[1]);
            input.Passenger_count = int.Parse(items[2]);
            input.Trip_time_in_secs = int.Parse(items[3]);
            input.Trip_distance = float.Parse(items[4]);
            input.Payment_type = items[5];*/

            return input;
        }

        // Given a single ModelInput entry and resulting ModelOutput
        // Evaluate the quality of the prediction and display
        public static void EvaluatePrediction(ModelInput sampleData, ModelOutput predictionResult)
        {
            Console.WriteLine("Using model to make single prediction -- Comparing actual MEDV with predicted MEDV from sample data...\n");
            Console.WriteLine($"CRIM: {sampleData.CRIM}");
            Console.WriteLine($"ZN: {sampleData.ZN}");
            Console.WriteLine($"INDUS: {sampleData.INDUS}");
            Console.WriteLine($"CHAS: {sampleData.CHAS}");
            Console.WriteLine($"NOX: {sampleData.NOX}");
            Console.WriteLine($"RM: {sampleData.RM}");
            Console.WriteLine($"AGE: {sampleData.AGE}");
            Console.WriteLine($"DIS: {sampleData.DIS}");
            Console.WriteLine($"RAD: {sampleData.RAD}");
            Console.WriteLine($"TAX: {sampleData.TAX}");
            Console.WriteLine($"PTRATIO: {sampleData.PTRATIO}");
            Console.WriteLine($"B: {sampleData.B}");
            Console.WriteLine($"LSTAT: {sampleData.LSTAT}");
            Console.WriteLine($"\nActual MEDV: {sampleData.MEDV} \nPredicted MEDV: {predictionResult.Score}\n\n");
        }

        public static IEnumerable<ModelInput> GetModelData(string dataFilePath)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load dataset
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: dataFilePath,
                                            hasHeader: true,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Use first line of dataset as model input
            // You can replace this with new test data (hardcoded or from end-user application)
            //ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false).First();
            IEnumerable<ModelInput> modelData = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false);
            return modelData;
        }

        // Change this code to create your own sample data
        #region CreateSingleDataSample
        // Method to load single row of dataset to try a single prediction
        public static ModelInput CreateSingleDataSample(string dataFilePath, int index = 0)
        {
            var modelData = GetModelData(dataFilePath);
            ModelInput sampleForPrediction = modelData.ElementAt(index);
            return sampleForPrediction;
        }
        #endregion

        public static ulong GetMilliseconds(Stopwatch timer)
        {
            double frequency = Stopwatch.Frequency;
            double millisecPerTick = (1000) / frequency;
            // return the value in nanosec
            double durMillisec = timer.ElapsedTicks * millisecPerTick;
            return (ulong)durMillisec;
        }

        public static ulong GetMicroseconds(Stopwatch timer)
        {
            double frequency = Stopwatch.Frequency;
            double microsecPerTick = (1000 * 1000) / frequency;
            // return the value in nanosec
            double durMicrosec = timer.ElapsedTicks * microsecPerTick;
            return (ulong)durMicrosec;
        }

        public static ulong GetNanoseconds(Stopwatch timer)
        {
            double frequency = Stopwatch.Frequency;
            double nanosecPerTick = (1000 * 1000 * 1000) / frequency;
            // return the value in nanosec
            double durNanosec = timer.ElapsedTicks * nanosecPerTick;
            return (ulong)durNanosec;
        }

    } // end of class MLHelper

} // end of namespace
