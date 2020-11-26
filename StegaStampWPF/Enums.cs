using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StegaStampWPF
{
    public static class Enums
    {
        public enum ThreshTypes
        {
            Binary, Otsu
        }

        public enum EdgeDetectType
        {
            Sobel, Canny
        }

        public enum RotateDir
        {
            ClockWise, CounterClockWise
        }
    }
}
