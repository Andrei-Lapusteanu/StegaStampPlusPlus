using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace QRColor.Source
{
    public class Constants
    {
        public const string QR_TYPE_MONOCHROME = "monochrome";
        public const string QR_TYPE_RED = "red";
        public const string QR_TYPE_GREEN = "green";
        public const string QR_TYPE_BLUE = "blue";
        public const string QR_TYPE_RED_OUT = "redOut";
        public const string QR_TYPE_GREEN_OUT = "greenOut";
        public const string QR_TYPE_BLUE_OUT = "blueOut";
        public const string QR_TYPE_CYAN = "cyan";
        public const string QR_TYPE_MAGENTA = "magenta";
        public const string QR_TYPE_YELLOW = "yellow";
        public const string QR_TYPE_COMBINED = "combined";

        public const string CHAR_ZEROS_7 = "0000000";
        public const string CHAR_ZEROS_8 = "00000000";

        public static byte LUM_INTENSITY_MIN = 0;
        public static byte LUM_INTENSITY_MAX = 255;

        // Vec3i (B, G, R)
        public static Vec3b COLOR_WHITE = new Vec3b(255, 255, 255);
        public static Vec3b COLOR_BLACK = new Vec3b(0, 0, 0);
        public static Vec3b COLOR_RED = new Vec3b(0, 0, 255);
        public static Vec3b COLOR_GREEN = new Vec3b(0, 255, 0);
        public static Vec3b COLOR_BLUE = new Vec3b(255, 0, 0);
        public static Vec3b COLOR_CYAN = new Vec3b(255, 255, 0);
        public static Vec3b COLOR_MAGENTA = new Vec3b(255, 0, 255);
        public static Vec3b COLOR_YELLOW = new Vec3b(0, 255, 255);
    }
}
