using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using static System.Net.Mime.MediaTypeNames;

namespace StegaStampWPF
{
    public static class ImageConversions
    {
        //static Mat result;

        public static Mat ImageToMat(BitmapSource imgSrcImg)
        {
            Mat result = OpenCvSharp.Extensions.BitmapSourceConverter.ToMat(imgSrcImg);

            if (result.Channels() == 4)
                Cv2.CvtColor(result, result, ColorConversionCodes.BGRA2BGR);

            return result;
        }

        public static BitmapSource MatToImage(Mat mat, bool contrastFlag = false)
        {
            return OpenCvSharp.Extensions.BitmapSourceConverter.ToBitmapSource(mat);
        }
    }
}