using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgoFilters
{
    // https://www.eit.lth.se/fileadmin/eit/courses/eit080/InfoTheorySH/InfoTheoryPart2d.pdf
    // https://www.cftc.gov/sites/default/files/idc/groups/public/@economicanalysis/documents/file/oce_algorithmictradingstrateg.pdf
    // https://www.tradingview.com/script/WpVY7GKW-Gaussian-Channel-DW/

    public struct HLC3
    {
        public double High { get; }
        public double Low { get; }
        public double Close { get; }

        public double H { get { return High; } }
        public double L { get { return Low; } }
        public double C { get { return Close; } }

        public HLC3(double high, double low, double close)
        {
            High = high;
            Low = low;
            Close = close;
        }
    }

    public class GaussianChannel
    {
        //@version=3
        //study(title= "Gaussian Channel [DW]", shorttitle= "GC [DW]", overlay= true)
        //by Donovan Wall

        //This study is an experiment utilizing the Ehlers Gaussian Filter technique combined with
        // lag reduction techniques and true range to analyze trend activity.
        //Gaussian filters, as Ehlers explains it, are simply exponential moving averages applied
        // multiple times.
        //First, beta and alpha are calculated based on the sampling period and number of poles
        // specified. The maximum number of poles available in this script is 9.
        //Next, the data being analyzed is given a truncation option for reduced lag, which can
        // be enabled with "Reduced Lag Mode".
        //Then the alpha and source values are used to calculate the filter and filtered true
        // range of the dataset.
        //Filtered true range with a specified multiplier is then added to and subtracted from
        // the filter, generating a channel.
        //Lastly, a one pole filter with a N pole alpha is averaged with the filter to generate
        // a faster filter, which can be enabled with "Fast Response Mode". 

        //Custom bar colors are included.

        //Note: Both the sampling period and number of poles directly affect how much lag the
        //       indicator has, and how smooth the output is.
        //      Larger inputs will result in smoother outputs with increased lag, and smaller
        //       inputs will have noisier outputs with reduced lag.
        //      For the best results, I recommend not setting the sampling period any lower than
        //       the number of poles + 1. Going lower truncates the equation.

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Inputs
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Source
        //src = input(defval= hlc3, title= "Source")
        static double[] src;

        //Poles
        //N = input(defval= 4, minval= 1, maxval= 9, title= "Poles")
        static int N = 4;

        //Sampling Period
        //per = input(defval= 144, minval= 2, title= "Sampling Period")
        static int per = 144;

        //Filtered True Range Multiplier
        //mult = input(defval= 1.414, minval= 0, title= "Filtered True Range Multiplier")
        static double mult = 1.414;

        //Reduced Lag Mode
        //lagreduce = input(defval= false, title= "Reduced Lag Mode")
        static bool lagreduce = false;

        //Fast Response Mode
        //fastresponse = input(defval= false, title= "Fast Response Mode")
        static bool fastresponse = false;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Definitions
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        static double[] tr;     // true range

        //Pi
        const double pi = 3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196442881097566593344612847564823378678316527120190914564856692346034861045432664821339360726024914127372458700660631558817488152092096282;

        //Beta and Alpha Components
        static double beta = (1 - Math.Cos(2 * pi / per)) / (Math.Pow(1.414, 2 / N) - 1);
        static double alpha = -beta + Math.Sqrt(Math.Pow(beta, 2) + 2 * beta);
        static double x = 1 - alpha;

        //Lag Reduction
        static int lag = (per - 1) / (2 * N);


        // https://www.macroption.com/atr-excel/
        // https://kodify.net/tradingview/indicators/average-true-range/
        double TrueRange(HLC3 today, HLC3 yesterday)
        {
            double diff1 = today.High - today.Low;     // traditional range
            double diff2 = today.High - yesterday.Close;
            double diff3 = yesterday.Close - today.Low;
            return Math.Max(Math.Max(diff1, diff2), diff3);
        }

        double nz(double d)
        {
            return double.IsNaN(d) ? 0.0 : d;
        }

        double[] filt1, filt2, filt3, filt4, filt5, filt6, filt7, filt8, filt9;
        double[] filt1tr, filt2tr, filt3tr, filt4tr, filt5tr, filt6tr, filt7tr, filt8tr, filt9tr;
        double[] filt;

        public void Calculate()
        {
            //double srcdata = src[0];
            //double trdata = tr[0];
            double srcdata = lagreduce ? src[0] + (src[0] - src[lag]) : src[0];
            double trdata = lagreduce ? tr[0] + (tr[0] - tr[lag]) : tr[0];

            //Filters (1 - 9 poles)
            filt1[0] = 0.0;
            filt1[0] = alpha * srcdata + x * nz(filt1[1]);   // :=
            filt2[0] = 0.0;
            filt2[0] = Math.Pow(alpha, 2) * srcdata + 2 * x * nz(filt2[1]) - Math.Pow(x, 2) * nz(filt2[2]);
            filt3[0] = 0.0;
            filt3[0] = Math.Pow(alpha, 3) * srcdata + 3 * x * nz(filt3[1]) - 3 * Math.Pow(x, 2) * nz(filt3[2]) + Math.Pow(x, 3) * nz(filt3[3]);
            filt4[0] = 0.0;
            filt4[0] = Math.Pow(alpha, 4) * srcdata + 4 * x * nz(filt4[1]) - 6 * Math.Pow(x, 2) * nz(filt4[2]) + 4 * Math.Pow(x, 3) * nz(filt4[3]) - Math.Pow(x, 4) * nz(filt4[4]);
            filt5[0] = 0.0;
            filt5[0] = Math.Pow(alpha, 5) * srcdata + 5 * x * nz(filt5[1]) - 10 * Math.Pow(x, 2) * nz(filt5[2]) + 10 * Math.Pow(x, 3) * nz(filt5[3]) - 5 * Math.Pow(x, 4) * nz(filt5[4]) + Math.Pow(x, 5) * nz(filt5[5]);
            filt6[0] = 0.0;
            filt6[0] = Math.Pow(alpha, 6) * srcdata + 6 * x * nz(filt6[1]) - 15 * Math.Pow(x, 2) * nz(filt6[2]) + 20 * Math.Pow(x, 3) * nz(filt6[3]) - 15 * Math.Pow(x, 4) * nz(filt6[4]) + 6 * Math.Pow(x, 5) * nz(filt6[5]) - Math.Pow(x, 6) * nz(filt6[6]);
            filt7[0] = 0.0;
            filt7[0] = Math.Pow(alpha, 7) * srcdata + 7 * x * nz(filt7[1]) - 21 * Math.Pow(x, 2) * nz(filt7[2]) + 35 * Math.Pow(x, 3) * nz(filt7[3]) - 35 * Math.Pow(x, 4) * nz(filt7[4]) + 21 * Math.Pow(x, 5) * nz(filt7[5]) - 7 * Math.Pow(x, 6) * nz(filt7[6]) + Math.Pow(x, 7) * nz(filt7[7]);
            filt8[0] = 0.0;
            filt8[0] = Math.Pow(alpha, 8) * srcdata + 8 * x * nz(filt8[1]) - 28 * Math.Pow(x, 2) * nz(filt8[2]) + 56 * Math.Pow(x, 3) * nz(filt8[3]) - 70 * Math.Pow(x, 4) * nz(filt8[4]) + 56 * Math.Pow(x, 5) * nz(filt8[5]) - 28 * Math.Pow(x, 6) * nz(filt8[6]) + 8 * Math.Pow(x, 7) * nz(filt8[7]) - Math.Pow(x, 8) * nz(filt8[8]);
            filt9[0] = 0.0;
            filt9[0] = Math.Pow(alpha, 9) * srcdata + 9 * x * nz(filt9[1]) - 36 * Math.Pow(x, 2) * nz(filt9[2]) + 84 * Math.Pow(x, 3) * nz(filt9[3]) - 126 * Math.Pow(x, 4) * nz(filt9[4]) + 126 * Math.Pow(x, 5) * nz(filt9[5]) - 84 * Math.Pow(x, 6) * nz(filt9[6]) + 36 * Math.Pow(x, 7) * nz(filt9[7]) - 9 * Math.Pow(x, 8) * nz(filt9[8]) + Math.Pow(x, 9) * nz(filt9[9]);

            //Filter Selection
            double filtn = N == 1 ? filt1[0] : N == 2 ? filt2[0] : N == 3 ? filt3[0] : N == 4 ? filt4[0] : N == 5 ? filt5[0] : N == 6 ? filt6[0] : N == 7 ? filt7[0] : N == 8 ? filt8[0] : N == 9 ? filt9[0] : double.NaN;
            double rlfilt = (filtn + filt1[0]) / 2;
            filt[0] = fastresponse ? rlfilt : filtn;

            //Filtered True Range (1 - 9 poles)
            filt1tr[0] = 0.0;
            filt1tr[0] = alpha * trdata + x * nz(filt1tr[1]);
            filt2tr[0] = 0.0;
            filt2tr[0] = Math.Pow(alpha, 2) * trdata + 2 * x * nz(filt2tr[1]) - Math.Pow(x, 2) * nz(filt2tr[2]);
            filt3tr[0] = 0.0;
            filt3tr[0] = Math.Pow(alpha, 3) * trdata + 3 * x * nz(filt3tr[1]) - 3 * Math.Pow(x, 2) * nz(filt3tr[2]) + Math.Pow(x, 3) * nz(filt3tr[3]);
            filt4tr[0] = 0.0;
            filt4tr[0] = Math.Pow(alpha, 4) * trdata + 4 * x * nz(filt4tr[1]) - 6 * Math.Pow(x, 2) * nz(filt4tr[2]) + 4 * Math.Pow(x, 3) * nz(filt4tr[3]) - Math.Pow(x, 4) * nz(filt4tr[4]);
            filt5tr[0] = 0.0;
            filt5tr[0] = Math.Pow(alpha, 5) * trdata + 5 * x * nz(filt5tr[1]) - 10 * Math.Pow(x, 2) * nz(filt5tr[2]) + 10 * Math.Pow(x, 3) * nz(filt5tr[3]) - 5 * Math.Pow(x, 4) * nz(filt5tr[4]) + Math.Pow(x, 5) * nz(filt5tr[5]);
            filt6tr[0] = 0.0;
            filt6tr[0] = Math.Pow(alpha, 6) * trdata + 6 * x * nz(filt6tr[1]) - 15 * Math.Pow(x, 2) * nz(filt6tr[2]) + 20 * Math.Pow(x, 3) * nz(filt6tr[3]) - 15 * Math.Pow(x, 4) * nz(filt6tr[4]) + 6 * Math.Pow(x, 5) * nz(filt6tr[5]) - Math.Pow(x, 6) * nz(filt6tr[6]);
            filt7tr[0] = 0.0;
            filt7tr[0] = Math.Pow(alpha, 7) * trdata + 7 * x * nz(filt7tr[1]) - 21 * Math.Pow(x, 2) * nz(filt7tr[2]) + 35 * Math.Pow(x, 3) * nz(filt7tr[3]) - 35 * Math.Pow(x, 4) * nz(filt7tr[4]) + 21 * Math.Pow(x, 5) * nz(filt7tr[5]) - 7 * Math.Pow(x, 6) * nz(filt7tr[6]) + Math.Pow(x, 7) * nz(filt7tr[7]);
            filt8tr[0] = 0.0;
            filt8tr[0] = Math.Pow(alpha, 8) * trdata + 8 * x * nz(filt8tr[1]) - 28 * Math.Pow(x, 2) * nz(filt8tr[2]) + 56 * Math.Pow(x, 3) * nz(filt8tr[3]) - 70 * Math.Pow(x, 4) * nz(filt8tr[4]) + 56 * Math.Pow(x, 5) * nz(filt8tr[5]) - 28 * Math.Pow(x, 6) * nz(filt8tr[6]) + 8 * Math.Pow(x, 7) * nz(filt8tr[7]) - Math.Pow(x, 8) * nz(filt8tr[8]);
            filt9tr[0] = 0.0;
            filt9tr[0] = Math.Pow(alpha, 9) * trdata + 9 * x * nz(filt9tr[1]) - 36 * Math.Pow(x, 2) * nz(filt9tr[2]) + 84 * Math.Pow(x, 3) * nz(filt9tr[3]) - 126 * Math.Pow(x, 4) * nz(filt9tr[4]) + 126 * Math.Pow(x, 5) * nz(filt9tr[5]) - 84 * Math.Pow(x, 6) * nz(filt9tr[6]) + 36 * Math.Pow(x, 7) * nz(filt9tr[7]) - 9 * Math.Pow(x, 8) * nz(filt9tr[8]) + Math.Pow(x, 9) * nz(filt9tr[9]);

            //Filtered True Range Selection
            double filtntr = N == 1 ? filt1tr[0] : N == 2 ? filt2tr[0] : N == 3 ? filt3tr[0] : N == 4 ? filt4tr[0] : N == 5 ? filt5tr[0] : N == 6 ? filt6tr[0] : N == 7 ? filt7tr[0] : N == 8 ? filt8tr[0] : N == 9 ? filt9tr[0] : double.NaN;
            double rlfilttr = (filtntr + filt1tr[0]) / 2;
            double filttr = fastresponse ? rlfilttr : filtntr;

            //Bands
            double hband = filt[0] + filttr * mult;
            double lband = filt[0] - filttr * mult;

            //Colors
            int red = 0xFF0000;
            int lime = 0x00FF00;
            int orange = 0xFFFF00;
            int fcolor = filt[0] > filt[1] ? lime : filt[0] < filt[1] ? red : orange;
            int barcolor = (src[0] > src[1]) && (src[0] > filt[0]) && (src[0] < hband) ? 0x00FF7F : (src[0] > src[1]) && (src[0] > hband) ? lime : (src[0] < src[1]) && (src[0] > filt[0]) ? 0x006400 : (src[0] < src[1]) && (src[0] < filt[0]) && (src[0] > lband) ? 0xF08080 : (src[0] < src[1]) && (src[0] < lband) ? red : (src[0] > src[1]) && (src[0] < filt[0]) ? 0x8B0000 : orange;

            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
            //Plots
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

            //Filters
            //filtplot = plot(filt, color = fcolor, linewidth = 3, title = "Filter");

            //Bands
            //hbandplot = plot(hband, color = fcolor, title = "Filtered True Range High Band");
            //lbandplot = plot(lband, color = fcolor, title = "Filtered True Range Low Band");

            //Fills
            //fill(hbandplot, lbandplot, color = fcolor, transp = 80, title = "Channel Fill");

            //Bar Color
            //barcolor(barcolor);
        }

    } // end of class
} // end of namespace
