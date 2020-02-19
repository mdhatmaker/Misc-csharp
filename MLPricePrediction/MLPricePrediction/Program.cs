using System;
using System.IO;
using MLPricePredictionML.Model;

namespace myMLApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Add input data
            var input = new ModelInput();

            while (true)
            {
                Console.Write("Enter line # in data file (ENTER to quit): ");
                int lineNumber;
                var success = int.TryParse(Console.ReadLine(), out lineNumber);
                if (!success) break;

                var dataPath = @"C:\GitHub\Misc-csharp\MLPricePrediction\taxi-fare-train.txt";
                string line = "";
                using (var f = new StreamReader(dataPath))
                {
                    for (int i = 0; i < lineNumber; ++i)
                        line = f.ReadLine();
                }

                var items = line.Split(',');

                input.Vendor_id = items[0];
                input.Rate_code = int.Parse(items[1]);
                input.Passenger_count = int.Parse(items[2]);
                input.Trip_time_in_secs = int.Parse(items[3]);
                input.Trip_distance = float.Parse(items[4]);
                input.Payment_type = items[5];

                /*input.Vendor_id = "CMT";        // CMT, VTS
                input.Rate_code = 1;
                input.Passenger_count = 1;
                input.Trip_time_in_secs = 386;
                input.Trip_distance = 1.3f;
                input.Payment_type = "CRD";     // CSH, CRD*/


                // Load model and predict output of sample data
                ModelOutput result = ConsumeModel.Predict(input);
                Console.WriteLine($"Vendor:{input.Vendor_id} RateCode:{input.Rate_code} Passengers:{input.Passenger_count} TripTime:{input.Trip_time_in_secs} Distance:{input.Trip_distance} PaymentType:{input.Payment_type}");
                Console.WriteLine($"Fare: {result.Score:N2}\n");
            }
        }
    }
}



/*
// Add input data
var input = new ModelInput();

// Load model and predict output of sample data
ModelOutput result = ConsumeModel.Predict(input);
*/
