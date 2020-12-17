using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using static System.Net.Mime.MediaTypeNames;

namespace QRColor.Source
{
    public static class Utils
    {
        static Mat result;

        public static Mat ImageToMat(BitmapSource imgSrc)
        {
            result = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(imgSrc);

            if (result.Channels() == 4)
                Cv2.CvtColor(result, result, ColorConversionCodes.BGRA2BGR);

            return result;
        }

        public static BitmapSource MatToImage(Mat mat, bool contrastFlag = false)
        {
            return OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(mat);
        }


        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static string ToBinary(string message)
        {
            byte[] data = ConvertToByteArray(message, Encoding.ASCII);
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2)));
        }

        public static string ASCIIToBinary(this string data, bool formatBits = false)
        {
            char[] buffer = new char[(((data.Length * 7) + (formatBits ? (data.Length - 1) : 0)))];
            int index = 0;
            for (int i = 0; i < data.Length; i++)
            {
                string binary = Convert.ToString(data[i], 2).PadLeft(7, '0');
                for (int j = 0; j < 7; j++)
                {
                    buffer[index] = binary[j];
                    index++;
                }
                if (formatBits && i < (data.Length - 1))
                {
                    buffer[index] = ' ';
                    index++;
                }
            }
            return new string(buffer);
        }

        public static string BinaryToASCII(this string data)
        {
            var list = new List<Byte>();

            for (int i = 0; i < data.Length; i += 7)
            {
                try
                {
                    String t = data.Substring(i, 7);

                    list.Add(Convert.ToByte(t, 2));
                }
                catch(Exception ex) {  }
            }

            return new string(Encoding.ASCII.GetChars(list.ToArray()));
        }
    }
}
