using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MLPredictHousing
{
    public static class HelperReflection
    {
        static void PrintCustomAttributes(PropertyInfo property)
        {
            Console.WriteLine("PropertyInfo: '{0}'   {1}", property.Name, property.PropertyType);
            var attrs = property.GetCustomAttributes(typeof(System.Attribute));  // property.Attributes;
            foreach (var attr in attrs)
            {
                Console.WriteLine("....{0}", attr.ToString());
            }
            Console.WriteLine();
        }

        /*
        // Add input data
        //ModelInput input = null;
        ModelInput input = new ModelInput();
        //var input = new ModelInput();
        //while (true)
        //{
        //    Console.Write("Enter line # in data file (ENTER to quit): ");
        //    int lineNumber;
        //    var success = int.TryParse(Console.ReadLine(), out lineNumber);
        //    if (!success) break;

        //    input = MLHelper.ParseDataLine(lineNumber, oname);
        //}

        //foreach (MemberInfo member in type.GetMembers())
        //{
        //    Console.WriteLine(member.Name + ":" + member.MemberType);
        //}

        foreach (PropertyInfo property in typeof(ModelInput).GetProperties())
        {
            Console.WriteLine(property.Name + ":" + property.PropertyType); // property.MemberType);

            var attr1 = property.GetCustomAttribute(typeof(Microsoft.ML.Data.ColumnNameAttribute)) as Microsoft.ML.Data.ColumnNameAttribute;
            var attr2 = property.GetCustomAttribute(typeof(Microsoft.ML.Data.LoadColumnAttribute)) as Microsoft.ML.Data.LoadColumnAttribute;
            Console.WriteLine("        {0}", attr1, "        {1}", attr2);

        }

        input = MLHelper.ParseDataLine(5, csvFilepath);
        Console.WriteLine(input);

        // Load model and predict output of sample data
        ModelOutput result = ConsumeModel.Predict(input);
        //Console.WriteLine($"Vendor:{input.Vendor_id} RateCode:{input.Rate_code} Passengers:{input.Passenger_count} TripTime:{input.Trip_time_in_secs} Distance:{input.Trip_distance} PaymentType:{input.Payment_type}");
        Console.WriteLine($"Median Home Value: {result.Score:N2}\n");
        */
    }
}
