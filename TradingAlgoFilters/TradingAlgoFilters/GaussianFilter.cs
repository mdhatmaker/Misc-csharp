using System;

// https://www.mql5.com/en/code/16776

namespace TradingAlgoFilters
{
    // INDICATOR_DATA --> Data to draw
    // INDICATOR_COLOR_INDEX --> Color
    // INDICATOR_CALCULATIONS --> Auxiliary buffers for intermediate calculations
    public enum EnumIndexBufferType { IndicatorData, IndicatorColorIndex, IndicatorCalculations }

    public class GaussianFilter
    {
        const double Pi = 3.141592653589793238462643;

        int GPeriod = 14;   // Calculation period
        int GOrder = 2;    // Order
        double[] gf;
        double[] colorBuffer;

        public GaussianFilter()
        {
            SetIndexBuffer(0, gf, EnumIndexBufferType.IndicatorData);
            SetIndexBuffer(1, colorBuffer, EnumIndexBufferType.IndicatorColorIndex);
        }

        public void SetIndexBuffer(int index, double[] buffer, EnumIndexBufferType dataType)
        {
            
        }

        public void Calculate()
        {
            Console.WriteLine("CALCULATE!");
        }

        int OnCalculate(int rates_total, int prev_calculated, int begin, double[] price)
        {
            for (int i = (int)Math.Max(prev_calculated - 1, 0); i < rates_total; i++)
            {
                gf[i] = iGFilter(price[i], GPeriod, GOrder, rates_total, i);
                if (i > 0)
                {
                    colorBuffer[i] = colorBuffer[i - 1];
                    if (gf[i] > gf[i - 1]) colorBuffer[i] = 0;
                    if (gf[i] < gf[i - 1]) colorBuffer[i] = 1;
                }
            }
            return (rates_total);
        }

        int[] periods = new int[1];     // int periods[1];
        double[,] coeffs;               // double coeffs[][3];
        double[,] filters;              // double filters[][1];
        double iGFilter(double price, int period, int order, int bars, int i, int instanceNo = 0)
        {
            //if (ArrayRange(filters, 0) != bars) ArrayResize(filters, bars);
            //if (ArrayRange(coeffs, 0) < order + 1) ArrayResize(coeffs, order + 1);
            if (filters.GetLength(0) != bars)
                filters = new double[bars, 1];
            if (coeffs.GetLength(0) < order + 1)
            {
                int newSize = Math.Max(coeffs.GetLength(0), order + 1);
                //Array.Resize<double>(ref coeffs, newSize);
                coeffs = new double[newSize, 3];                
            }

            if (periods[instanceNo] != period)
            {
                periods[instanceNo] = period;
                double b = (1.0 - Math.Cos(2.0 * Pi / period)) / (Math.Pow(Math.Sqrt(2.0), 2.0 / order) - 1.0);
                double a = -b + Math.Sqrt(b * b + 2.0 * b);
                for (int r = 0; r <= order; r++)
                {
                    coeffs[r, instanceNo * 3 + 0] = fact(order) / (fact(order - r) * fact(r));
                    coeffs[r, instanceNo * 3 + 1] = Math.Pow(a, r);
                    coeffs[r, instanceNo * 3 + 2] = Math.Pow(1.0 - a, r);
                }
            }

            //if (price == EMPTY_VALUE) price = 0;
            if (double.IsNaN(price)) price = 0;
            filters[i, instanceNo] = price * coeffs[order, instanceNo * 3 + 1];
            double sign = 1;
            for (int r = 1; r <= order && (i - r) >= 0; r++, sign *= -1.0)
                filters[i, instanceNo] += sign * coeffs[r, instanceNo * 3 + 0] * coeffs[r, instanceNo * 3 + 2] * filters[i - r, instanceNo];
            return (filters[i, instanceNo]);
        }

        double fact(int n)
        {
            double a = 1;
            for (int i = 1; i <= n; i++) a *= i;
            return (a);
        }

    } // end of class GaussianFilter
} // end of namespace