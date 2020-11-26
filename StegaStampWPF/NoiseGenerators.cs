using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;


namespace StegaStampWPF
{
    class NoiseGenerators
    {
        public BitmapSource AddGaussianNoise(BitmapSource inputImg, float intensity, bool? isColoredNoise)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);
            Mat gaussianMatC1 = Mat.Zeros(mat.Rows, mat.Cols, MatType.CV_8UC1);
            float mean = 0f;
            float stddev = 30f;

            // Generate gaussian noise
            Cv2.Randn(gaussianMatC1, mean, stddev);

            // If image has 1 channel
            if (mat.Channels() == 1)
                // Apply gaussian noise
                mat += (gaussianMatC1 * intensity);

            // If image has 3 channels
            else if (mat.Channels() == 3)
            {
                // Colored noise
                if (isColoredNoise == true)
                {
                    Mat gaussianMatC2 = Mat.Zeros(mat.Rows, mat.Cols, MatType.CV_8UC1);
                    Mat gaussianMatC3 = Mat.Zeros(mat.Rows, mat.Cols, MatType.CV_8UC1);

                    // Generate gaussian noise
                    Cv2.Randn(gaussianMatC2, mean, stddev);
                    Cv2.Randn(gaussianMatC3, mean, stddev);

                    // Split intro B, G, and R
                    Mat[] channels = new Mat[3];
                    Cv2.Split(mat, out channels);

                    // Apply gaussian noise on each channel
                    channels[0] += (gaussianMatC1 * intensity);
                    channels[1] += (gaussianMatC2 * intensity);
                    channels[2] += (gaussianMatC3 * intensity);

                    // Merge channels
                    Cv2.Merge(channels, mat);
                }

                // Monochrome noise
                else if (isColoredNoise == false)
                {
                    Mat ycrcb = Mat.Zeros(mat.Rows, mat.Cols, MatType.CV_8UC3);

                    // Convert RGB to YCrCb
                    Cv2.CvtColor(mat, ycrcb, ColorConversionCodes.BGR2YCrCb);

                    // Split intro Y, Cb, and Cr channels
                    Mat[] channels = new Mat[3];
                    Cv2.Split(ycrcb, out channels);

                    // Apply gaussian noise on Y (luminance) channel
                    channels[0] += (gaussianMatC1 * intensity);

                    // Merge channels
                    Cv2.Merge(channels, ycrcb);

                    // Convert YCrCb to RGB
                    Cv2.CvtColor(ycrcb, mat, ColorConversionCodes.YCrCb2BGR);
                }
            }

            return ImageConversions.MatToImage(mat);
        }
    }
}
