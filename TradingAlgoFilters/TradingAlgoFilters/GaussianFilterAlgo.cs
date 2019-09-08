using System;
using System.ComponentModel.DataAnnotations;
//using cAlgo.API;

// https://ctrader.com/algos/indicators/show/150

namespace TradingAlgoFilters    //cAlgo.Indicators
{
    //[Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class GaussianFilterAlgo //:Indicator
    {
        private double _beta;
        private double _alpha;
        private double _coeff;
        private double _alphaPow;
 
        //[Parameter(DefaultValue = 12, MinValue = 1)]
        [Range(1, int.MaxValue)]
        public int Period { get; set; }
 
        //[Parameter(DefaultValue = 3, MinValue = 1, MaxValue = 4)]
        [Range(1, 4)]
        public int Poles { get; set; }

        //[Parameter]
        //public DataSeries Source { get; set; }
        private double[] _source;
        public double[] Source {
            get { return _source; }
            set { _source = value;  Result = new double[value.Length]; }
        }

        //[Output("Main", Color = Colors.DeepSkyBlue)]
        //public IndicatorDataSeries Result { get; set; }
        public double[] Result { get; set; }

        // Create the object (optionally specifying period and poles), then set the Source
        // property and call CalculateAll function; find results in Result property.
        public GaussianFilterAlgo(int period = 12, int poles = 3)
        {
            Period = period;
            Poles = poles;
            Initialize();
        }

        //protected override void Initialize()
        protected void Initialize()
        {
            _beta = (1 - Math.Cos(2*Math.PI/Period))/(Math.Pow(Math.Sqrt(2.0), 2.0/Poles) - 1);
            _alpha = -_beta + Math.Sqrt(_beta*(_beta + 2));
            _coeff = 1.0 - _alpha;
            _alphaPow = Math.Pow(_alpha, Poles);
        }

        public void CalculateAll()
        {
            for (int i = 0; i < Source.Length; ++i)
            {
                Calculate(i);
            }
        }

        //public override void Calculate(int index)
        public void Calculate(int index)
        {
            if (index < Poles)
            {
                Result[index] = Source[index];
                return;
            }
 
            Result[index] = _alphaPow * Source[index] + Poles * _coeff * Result[index - 1];
             
            switch(Poles)
            {
                case 1:
                    break;
                case 2:
                    Result[index] -= Math.Pow(_coeff, 2.0) * Result[index - 2];
                    break;
                case 3:
                    Result[index] -= 3 * Math.Pow(_coeff, 2.0) * Result[index - 2]
                        - Math.Pow(_coeff,3.0)*Result[index - 3];
                    break;
                case 4:
                    Result[index] -= 6 * Math.Pow(_coeff, 2.0) * Result[index - 2]
                        - 4*Math.Pow(_coeff,3.0)*Result[index - 3]
                        + Math.Pow(_coeff, 4.0)*Result[index - 4];
                    break;
            }            
        }

    } // end of class
} // end of namespace