using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using static QRColor.Source.Enums;
using static QRColor.Source.Constants;

namespace QRColor.Source
{
    class QRCodeOperations
    {
        private Mat qrMatMono;
        private List<Point> matSearchPixels;
        private Dictionary<string, BitmapSource> outputImages;
        private Dictionary<string, Mat> outputMats;
        private Dictionary<string, string> outputMessages;
        private string inputMessage;

        public QRCodeOperations()
        {
            qrMatMono = new Mat();
            matSearchPixels = new List<Point>();
            outputMats = new Dictionary<string, Mat>();
            outputImages = new Dictionary<string, BitmapSource>();
            outputMessages = new Dictionary<string, string>();
        }

        public Dictionary<string, BitmapSource> GenerateQRCore(string inputMessage, QRCodeProperties qrCodeProps, QRColorMode colorMode)
        {
            outputImages = new Dictionary<string, BitmapSource>();
            outputMats = new Dictionary<string, Mat>();
            matSearchPixels = new List<Point>();
            this.inputMessage = inputMessage;

            CalculateSearchPoint(qrCodeProps);

            if (colorMode == QRColorMode.Grayscale)
            {
                // Generate empty mat and set all pixels to white
                qrMatMono = Mat.Zeros(new Size(qrCodeProps.ImgSize.Width, qrCodeProps.ImgSize.Height), MatType.CV_8UC1);
                //qrMat.SetTo(Scalar.White);

                // Get mat indexer
                MatOfByte mob1 = new MatOfByte(qrMatMono);
                MatIndexer<byte> indexerByte = mob1.GetIndexer();

                int stringIndex = -1;
                for (int y = 0; y < qrCodeProps.CellsPerDim * qrCodeProps.CellSize; y += qrCodeProps.CellSize)
                    for (int x = 0; x < qrCodeProps.CellsPerDim * qrCodeProps.CellSize; x += qrCodeProps.CellSize)
                    {
                        // If message is done reading
                        if (++stringIndex + 1 > inputMessage.Length)
                            break;

                        // If bit is 0, skip this cell
                        if (inputMessage[stringIndex].Equals('0'))
                            continue;

                        // If bit is 1, color the cell 
                        else if (inputMessage[stringIndex].Equals('1'))
                        {

                            for (int i = y; i < y + qrCodeProps.CellSize; i++)
                                for (int j = x; j < x + qrCodeProps.CellSize; j++)
                                {
                                    indexerByte[i, j] = LUM_INTENSITY_MAX;
                                }
                        }
                    }

                // Add image and mat to output lists
                outputImages.Add(QR_TYPE_MONOCHROME, Utils.MatToImage(qrMatMono));
                outputMats.Add(QR_TYPE_MONOCHROME, qrMatMono);

                // Return images to UI
                return outputImages;
            }
            else if (colorMode == QRColorMode.Color)
            {
                // Generate empty mats and fill with white
                Mat qrRedMat = Mat.Zeros(new Size(qrCodeProps.ImgSize.Width, qrCodeProps.ImgSize.Height), MatType.CV_8UC3);
                Mat qrGreenMat = Mat.Zeros(new Size(qrCodeProps.ImgSize.Width, qrCodeProps.ImgSize.Height), MatType.CV_8UC3);
                Mat qrBlueMat = Mat.Zeros(new Size(qrCodeProps.ImgSize.Width, qrCodeProps.ImgSize.Height), MatType.CV_8UC3);
                //qrCyanMat.SetTo(Scalar.White);
                //qrMagentaMat.SetTo(Scalar.White);
                //qrYellowMat.SetTo(Scalar.White);

                // Get mat indexers
                MatOfByte3 mobRed = new MatOfByte3(qrRedMat);
                MatOfByte3 mobGreen = new MatOfByte3(qrGreenMat);
                MatOfByte3 mobBlue = new MatOfByte3(qrBlueMat);

                MatIndexer<Vec3b> indexerMobRed = mobRed.GetIndexer();
                MatIndexer<Vec3b> indexerMobGreen = mobGreen.GetIndexer();
                MatIndexer<Vec3b> indexerMobBlue = mobBlue.GetIndexer();

                // Split message thrice
                int bitsPerChar = 7;
                int messageChars = inputMessage.Length / bitsPerChar;
                string messageForRed = string.Empty;
                string messageForGreen = string.Empty;
                string messageForBlue = string.Empty;

                for (int i = 0; i < messageChars; i++)
                {
                    if (i % 3 == 0)
                        for (int j = 0; j < bitsPerChar; j++)
                            messageForRed += inputMessage[(i * bitsPerChar) + j];

                    else if (i % 3 == 1)
                        for (int j = 0; j < bitsPerChar; j++)
                            messageForGreen += inputMessage[(i * bitsPerChar) + j];

                    else if (i % 3 == 2)
                        for (int j = 0; j < bitsPerChar; j++)
                            messageForBlue += inputMessage[(i * bitsPerChar) + j];
                }

                indexerMobRed = WriteColorComponent(messageForRed, qrCodeProps, indexerMobRed, COLOR_RED);
                indexerMobGreen = WriteColorComponent(messageForGreen, qrCodeProps, indexerMobGreen, COLOR_GREEN);
                indexerMobBlue = WriteColorComponent(messageForBlue, qrCodeProps, indexerMobBlue, COLOR_BLUE);

                Mat combinedMat = qrRedMat + qrGreenMat + qrBlueMat;

                // Add image and mats to output lists
                outputImages.Add(QR_TYPE_COMBINED, Utils.MatToImage(combinedMat));
                outputImages.Add(QR_TYPE_RED, Utils.MatToImage(qrRedMat));
                outputImages.Add(QR_TYPE_GREEN, Utils.MatToImage(qrGreenMat));
                outputImages.Add(QR_TYPE_BLUE, Utils.MatToImage(qrBlueMat));
                outputMats.Add(QR_TYPE_COMBINED, combinedMat);
                outputMats.Add(QR_TYPE_RED, qrRedMat);
                outputMats.Add(QR_TYPE_GREEN, qrGreenMat);
                outputMats.Add(QR_TYPE_BLUE, qrBlueMat);

                return outputImages;
            }

            return null;
        }

        private void CalculateSearchPoint(QRCodeProperties qrCodeProps)
        {
            // Add center pixel of cell to search points (for image decoding). Could optimise
            for (int y = 0; y < qrCodeProps.CellsPerDim * qrCodeProps.CellSize; y += qrCodeProps.CellSize)
                for (int x = 0; x < qrCodeProps.CellsPerDim * qrCodeProps.CellSize; x += qrCodeProps.CellSize)
                    matSearchPixels.Add(new Point(x + (qrCodeProps.CellSize / 2), y + (qrCodeProps.CellSize / 2)));
        }

        MatIndexer<Vec3b> WriteColorComponent(string message, QRCodeProperties qrCodeProps, MatIndexer<Vec3b> indexerComponent, Vec3b color)
        {
            int stringIndex = -1;
            for (int y = 0; y < qrCodeProps.CellsPerDim * qrCodeProps.CellSize; y += qrCodeProps.CellSize)
                for (int x = 0; x < qrCodeProps.CellsPerDim * qrCodeProps.CellSize; x += qrCodeProps.CellSize)
                {
                    // If message is done reading
                    if (++stringIndex + 1 > message.Length)
                        break;

                    // If bit is 0, skip this cell
                    if (message[stringIndex].Equals('0'))
                        continue;

                    // If bit is 1, color the cell 
                    else if (message[stringIndex].Equals('1'))
                    {
                        for (int i = y; i < y + qrCodeProps.CellSize; i++)
                            for (int j = x; j < x + qrCodeProps.CellSize; j++)
                            {
                                indexerComponent[i, j] = color;
                            }
                    }
                }

            return indexerComponent;
        }

        public Dictionary<string, string> DecodeQRCode(QRCodeProperties qrCodeProps, QRColorMode colorMode)
        {
            outputMessages = new Dictionary<string, string>();

            if (colorMode == QRColorMode.Grayscale)
            {
                string decodedBinary = string.Empty;
                outputMats.TryGetValue(QR_TYPE_MONOCHROME, out Mat qrMatMono);

                // Get mat indexer
                MatOfByte mob1 = new MatOfByte(qrMatMono);
                MatIndexer<byte> indexerByte = mob1.GetIndexer();

                // Read decoded strings
                int bitsPerChar = 7;
                int messageLength = inputMessage.Length;
                decimal messageChars = messageLength / bitsPerChar;

                // Read decoded binary
                for (int i = 0; i < messageChars; i++)
                {
                    decodedBinary += ReadMonochromePixels(indexerByte, i, bitsPerChar);
                }

                // Add decoded messag to output list
                outputMessages.Add(QR_TYPE_MONOCHROME, decodedBinary);

                return outputMessages;
            }
            else if (colorMode == QRColorMode.Color)
            {
                string decodedBinaryRed = string.Empty;
                string decodedBinaryGreen = string.Empty;
                string decodedBinaryBlue = string.Empty;
                string decodedBinaryCombined = string.Empty;

                outputMats.TryGetValue(QR_TYPE_COMBINED, out Mat qrMatCombined);

                // Get mat indexer
                MatOfByte3 mobComb = new MatOfByte3(qrMatCombined);
                MatIndexer<Vec3b> indexerMobComb = mobComb.GetIndexer();

                // Read decoded strings
                int bitsPerChar = 7;
                int messageLength = inputMessage.Length;
                decimal messageChars = messageLength / bitsPerChar;
                int coloredMessageLength = (int)Math.Ceiling(messageChars / 3);

                for (int i = 0; i < coloredMessageLength; i++)
                {
                    string tempRed = ReadColorPixels(indexerMobComb, QR_TYPE_RED, i, bitsPerChar);
                    string tempGreen = ReadColorPixels(indexerMobComb, QR_TYPE_GREEN, i, bitsPerChar);
                    string tempBlue = ReadColorPixels(indexerMobComb, QR_TYPE_BLUE, i, bitsPerChar);

                    decodedBinaryRed += tempRed;
                    decodedBinaryGreen += tempGreen;
                    decodedBinaryBlue += tempBlue;

                    decodedBinaryCombined += tempRed;
                    decodedBinaryCombined += tempGreen;
                    decodedBinaryCombined += tempBlue;
                }

                // Add output messages
                outputMessages.Add(QR_TYPE_RED, decodedBinaryRed);
                outputMessages.Add(QR_TYPE_GREEN, decodedBinaryGreen);
                outputMessages.Add(QR_TYPE_BLUE, decodedBinaryBlue);
                outputMessages.Add(QR_TYPE_COMBINED, decodedBinaryCombined);

                return outputMessages;
            }

            return null;
        }

        private string ReadMonochromePixels(MatIndexer<byte> indexer, int charCounter, int bitsPerChar)
        {
            string tempStr = string.Empty;

            for (int i = charCounter * bitsPerChar; i < (charCounter * bitsPerChar) + bitsPerChar; i++)
            {
                if (i < matSearchPixels.Count)
                {
                    // Get pixel value
                    float pixelIntensity = indexer[matSearchPixels[i].Y, matSearchPixels[i].X];

                    // If pixel contains blue
                    if (pixelIntensity == LUM_INTENSITY_MAX)
                        tempStr += '1';

                    else if (pixelIntensity == LUM_INTENSITY_MIN)
                        tempStr += '0';
                }
            }

            // If nothing was read
            if (tempStr == CHAR_ZEROS_7)
                return "";

            return tempStr;
        }

        private string ReadColorPixels(MatIndexer<Vec3b> indexer, string qrType, int charCounter, int bitsPerChar)
        {
            string tempStr = string.Empty;

            switch (qrType)
            {
                case QR_TYPE_RED:
                    for (int i = charCounter * bitsPerChar; i < (charCounter * bitsPerChar) + bitsPerChar; i++)
                    {
                        if (i < matSearchPixels.Count)
                        {
                            // Get pixel value
                            Vec3b pixelValue = indexer[matSearchPixels[i].Y, matSearchPixels[i].X];

                            // If pixel contains red
                            if (pixelValue.Item2 == LUM_INTENSITY_MAX)
                                tempStr += '1';

                            else if (pixelValue.Item2 == LUM_INTENSITY_MIN)
                                tempStr += '0';
                        }
                    }
                    break;

                case QR_TYPE_GREEN:
                    for (int i = charCounter * bitsPerChar; i < (charCounter * bitsPerChar) + bitsPerChar; i++)
                    {
                        if (i < matSearchPixels.Count)
                        {
                            // Get pixel value
                            Vec3b pixelValue = indexer[matSearchPixels[i].Y, matSearchPixels[i].X];

                            // If pixel contains green
                            if (pixelValue.Item1 == LUM_INTENSITY_MAX)
                                tempStr += '1';

                            else if (pixelValue.Item1 == LUM_INTENSITY_MIN)
                                tempStr += '0';
                        }
                    }
                    break;
                case QR_TYPE_BLUE:
                    for (int i = charCounter * bitsPerChar; i < (charCounter * bitsPerChar) + bitsPerChar; i++)
                    {
                        if (i < matSearchPixels.Count)
                        {
                            // Get pixel value
                            Vec3b pixelValue = indexer[matSearchPixels[i].Y, matSearchPixels[i].X];

                            // If pixel contains blue
                            if (pixelValue.Item0 == LUM_INTENSITY_MAX)
                                tempStr += '1';

                            else if (pixelValue.Item0 == LUM_INTENSITY_MIN)
                                tempStr += '0';
                        }
                    }
                    break;

                default: break;
            }

            // If nothing was read
            if (tempStr == CHAR_ZEROS_7)
                return "";

            return tempStr;
        }

        public Dictionary<string, BitmapSource> SplitColoredQR(Mat combinedMat)
        {
            Dictionary<string, BitmapSource> outputSplitImages = new Dictionary<string, BitmapSource>();
            Size size = new Size(combinedMat.Width, combinedMat.Height);
            int depth = combinedMat.Depth();

            Mat redComponent = Mat.Zeros(size, MatType.CV_8UC1);
            Mat grnComponent = Mat.Zeros(size, MatType.CV_8UC1);
            Mat bluComponent = Mat.Zeros(size, MatType.CV_8UC1);

            // Get mat indexers
            MatOfByte3 mobComb = new MatOfByte3(combinedMat);
            MatOfByte mobRed = new MatOfByte(redComponent);
            MatOfByte mobGrn = new MatOfByte(grnComponent);
            MatOfByte mobBlu = new MatOfByte(bluComponent);

            MatIndexer<Vec3b> indexerComb = mobComb.GetIndexer();
            MatIndexer<byte> indexerRed = mobRed.GetIndexer();
            MatIndexer<byte> indexerGrn = mobGrn.GetIndexer();
            MatIndexer<byte> indexerBlu = mobBlu.GetIndexer();

            for (int y = 0; y < combinedMat.Height; y++)
                for (int x = 0; x < combinedMat.Width; x++)
                {
                    // Assign intensity of red channel from the combined mat to the red component mat
                    indexerRed[y, x] = indexerComb[y, x].Item2;

                    // Assign intensity of green channel from the combined mat to the green component mat
                    indexerGrn[y, x] = indexerComb[y, x].Item1;

                    // Assign intensity of blue channel from the combined mat to the blue component mat
                    indexerBlu[y, x] = indexerComb[y, x].Item0;
                }

            outputSplitImages.Add(QR_TYPE_RED_OUT, Utils.MatToImage(redComponent));
            outputSplitImages.Add(QR_TYPE_GREEN_OUT, Utils.MatToImage(grnComponent));
            outputSplitImages.Add(QR_TYPE_BLUE_OUT, Utils.MatToImage(bluComponent));

            return outputSplitImages;
        }

        public Mat GetColoredQR()
        {
            outputMats.TryGetValue(QR_TYPE_COMBINED, out Mat comb);
            return comb;
        }
    }
}
