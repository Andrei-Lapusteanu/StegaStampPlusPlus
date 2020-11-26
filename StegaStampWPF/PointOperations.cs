using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace StegaStampWPF
{
    class PointOperations
    {
        int currentBrightness = 0;
        float currentContrast = 1.0f;

        public int CurrentBrightness { get => currentBrightness; set => currentBrightness = value; }
        public float CurrentContrast { get => currentContrast; set => currentContrast = value; }

        public BitmapSource Threshold(BitmapSource inputImg, int thresholdValue, Enums.ThreshTypes threshTypes)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);

            // Apply threshold
            Cv2.Threshold(mat, mat, thresholdValue, 255, ThresholdTypes.Binary);

            return ImageConversions.MatToImage(mat);
        }

        public BitmapSource Grayscale(BitmapSource inputImg)
        {
            // Convert bitmap to matrix
            Mat mat = ImageConversions.ImageToMat(inputImg);

            // Create empty grayscale mat
            Mat grayMat = Mat.Zeros(mat.Size(), MatType.CV_8UC1);

            // Get rgb matrix indexer
            MatOfByte3 mat3 = new MatOfByte3(mat);
            MatIndexer<Vec3b> indexerB3 = mat3.GetIndexer();

            // Get grayscale matrix indexer
            MatOfByte mat1 = new MatOfByte(grayMat);
            MatIndexer<byte> indexerB1 = mat1.GetIndexer();

            for (int y = 0; y < mat.Height; y++)
                for (int x = 0; x < mat.Width; x++)
                {
                    // Apply grayscale per-pixel using indexers (fastest way!)
                    Vec3b color = indexerB3[y, x];
                    indexerB1[y, x] = (byte)(indexerB3[y, x].Item2 * 0.3f +
                                             indexerB3[y, x].Item1 * 0.59f +
                                             indexerB3[y, x].Item0 * 0.11f);

                }

            return ImageConversions.MatToImage(grayMat);
        }

        public BitmapSource Invert(BitmapSource inputImg)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);

            // Apply invert
            Cv2.BitwiseNot(mat, mat);

            return ImageConversions.MatToImage(mat);
        }

        public BitmapSource Sepia(BitmapSource inputImg)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);

            // If image is in BGR format
            if (mat.Channels() == 3)
            {
                var indexer = mat.GetGenericIndexer<Vec3b>();

                for (int y = 0; y < mat.Height; y++)
                {
                    for (int x = 0; x < mat.Width; x++)
                    {
                        // Get each channel of pixel
                        Vec3b color = indexer[y, x];
                        byte B = color.Item0;
                        byte G = color.Item1;
                        byte R = color.Item2;

                        // Apply sepia formula per pixel of image
                        int outputR = (int)((R * .493) + (G * .769) + (B * .189));
                        int outputG = (int)((R * .349) + (G * .686) + (B * .168));
                        int outputB = (int)((R * .272) + (G * .534) + (B * .131));

                        // Limit max and min values
                        if (outputR > 255)
                            outputR = 255;
                        else if (outputR < 0)
                            outputR = 0;

                        if (outputG > 255)
                            outputG = 255;
                        else if (outputG < 0)
                            outputG = 0;

                        if (outputB > 255)
                            outputB = 255;
                        else if (outputB < 0)
                            outputB = 0;

                        color = new Vec3b((byte)outputB, (byte)outputG, (byte)outputR);

                        // Set pixel with calculated value in matrix
                        mat.Set<Vec3b>(y, x, color);
                    }
                }
                return ImageConversions.MatToImage(mat);
            }
            else if (mat.Channels() == 1)
            {
                var indexer = mat.GetGenericIndexer<byte>();
                Mat outMat = new Mat(mat.Rows, mat.Cols, MatType.CV_8UC3);

                for (int y = 0; y < mat.Height; y++)
                {
                    for (int x = 0; x < mat.Width; x++)
                    {
                        // Get channel of pixel
                        byte value = indexer[y, x];

                        // Apply sepia formula per pixel of image
                        int outputR = (int)((value * .393) + (value * .769) + (value * .189));
                        int outputG = (int)((value * .349) + (value * .686) + (value * .168));
                        int outputB = (int)((value * .272) + (value * .534) + (value * .131));

                        // Limit max and min values
                        if (outputR > 255)
                            outputR = 255;
                        else if (outputR < 0)
                            outputR = 0;

                        if (outputG > 255)
                            outputG = 255;
                        else if (outputG < 0)
                            outputG = 0;

                        if (outputB > 255)
                            outputB = 255;
                        else if (outputB < 0)
                            outputB = 0;

                        Vec3b color = new Vec3b((byte)outputB, (byte)outputG, (byte)outputR);

                        // Set pixel with calculated value in matrix
                        outMat.Set<Vec3b>(y, x, color);
                    }
                }
                return ImageConversions.MatToImage(outMat);
            }

            return ImageConversions.MatToImage(mat);
        }

        public BitmapSource SetBrightness(BitmapSource inputImg, int brightnessValue)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);

            // If image only has one channel
            if (mat.Channels() == 1)
                mat.ConvertTo(mat, MatType.CV_8UC1, 1, brightnessValue - CurrentBrightness);

            // If image has three channels
            else if (mat.Channels() == 3)
                mat.ConvertTo(mat, MatType.CV_8UC3, 1, brightnessValue - CurrentBrightness);

            CurrentBrightness = brightnessValue;

            return ImageConversions.MatToImage(mat);
        }

        public BitmapSource SetContrast(BitmapSource inputImg, float contrastValue)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);
            MatType mType = new MatType();

            // If image only has one channel
            if (mat.Channels() == 1)
                mType = MatType.CV_8UC1;

            // If image has three channels
            else if (mat.Channels() == 3)
                mType = MatType.CV_8UC3;

            // Apply contrast change
            if (contrastValue >= currentContrast)
                mat.ConvertTo(mat, mType, contrastValue - CurrentContrast + 1, 0);
            else
                mat.ConvertTo(mat, mType, 1 - (1 - (contrastValue / CurrentContrast)), 0);

            CurrentContrast = contrastValue;

            return ImageConversions.MatToImage(mat);
        }

        public BitmapSource ContrastLimitedAdaptiveHistEq(BitmapSource inputImg)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);
            Mat ycrcb = new Mat(mat.Rows, mat.Cols, MatType.CV_8UC3);
            CLAHE clahe = Cv2.CreateCLAHE(2.0, new Size(8, 8));

            // If grayscale
            if (mat.Channels() == 1)
                clahe.Apply(mat, mat);

            else if (mat.Channels() == 3)
            {
                // Convert RGB to YCrCb
                Cv2.CvtColor(mat, ycrcb, ColorConversionCodes.BGR2YCrCb);

                // Split intro Y, Cb, and Cr channels
                Mat[] channels = new Mat[3];
                Cv2.Split(ycrcb, out channels);

                // Apply CLAHE on Y (luminance) channel
                clahe.Apply(channels[0], channels[0]);

                // Merge channels
                Cv2.Merge(channels, ycrcb);

                // Convert YCrCb to RGB
                Cv2.CvtColor(ycrcb, mat, ColorConversionCodes.YCrCb2BGR);
            }

            return ImageConversions.MatToImage(mat);
        }

        public BitmapSource HistEqualization(BitmapSource inputImg)
        {
            Mat mat = ImageConversions.ImageToMat(inputImg);
            Mat ycrcb = new Mat(mat.Rows, mat.Cols, MatType.CV_8UC3);

            // If grayscale
            if (mat.Channels() == 1)
                Cv2.EqualizeHist(mat, mat);

            else if (mat.Channels() == 3)
            {
                // Convert RGB to YCrCb
                Cv2.CvtColor(mat, ycrcb, ColorConversionCodes.BGR2YCrCb);

                // Split intro Y, Cb, and Cr channels
                Mat[] channels = new Mat[3];
                Cv2.Split(ycrcb, out channels);

                // Equalize only Y (luminance) componenst
                Cv2.EqualizeHist(channels[0], channels[0]);

                // Merge channels
                Cv2.Merge(channels, ycrcb);

                // Convert YCrCb to RGB
                Cv2.CvtColor(ycrcb, mat, ColorConversionCodes.YCrCb2BGR);
            }

            return ImageConversions.MatToImage(mat);
        }
    }
}
