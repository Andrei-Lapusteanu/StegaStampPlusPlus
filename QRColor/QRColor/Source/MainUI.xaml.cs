using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QRColor.Source;
using static QRColor.Source.Enums;
using static QRColor.Source.Constants;

namespace QRColor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainUI : Window
    {
        private QRCodeOperations qrCodeOps;
        private QRCodeProperties qrCodeProps;
        private Dictionary<string, BitmapSource> outputImages;
        private Dictionary<string, string> outputMessages;

        private int cellSize;
        private bool isColored;
        private bool isECC;
        private QRColorMode qrColorMode;

        public MainUI()
        {
            Init();
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputTextChangeEventHandler();
        }

        private void CheckBoxColor_Click(object sender, RoutedEventArgs e)
        {
            isColored = checkBoxColor.IsChecked.Value ? true : false;

            if (isColored == true)
                qrColorMode = QRColorMode.Color;
            else
                qrColorMode = QRColorMode.Grayscale;

            InputTextChangeEventHandler();
        }

        private void CheckBoxECC_Click(object sender, RoutedEventArgs e)
        {
            isECC = checkBoxECC.IsChecked.Value ? true : false;
        }

        private void ComboBoxCellSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxCellSize.Text != "")
                cellSize = int.Parse((comboBoxCellSize.Items[comboBoxCellSize.SelectedIndex] as ComboBoxItem).Content.ToString());

            if (cellSize > 0)
            {
                qrCodeProps = new QRCodeProperties(400, 400, cellSize);
                InputTextChangeEventHandler();
            }
        }

        private void Init()
        {
            InitializeComponent();

            qrCodeOps = new QRCodeOperations();

            outputImages = new Dictionary<string, BitmapSource>();
            outputMessages = new Dictionary<string, string>();

            cellSize = int.Parse(comboBoxCellSize.SelectionBoxItem.ToString());
            isColored = checkBoxColor.IsChecked.Value ? true : false;
            isECC = checkBoxECC.IsChecked.Value ? true : false;

            // DIvisors of 400: 1, 2, 4, 5, 8, 10, 16, 20, 25, 40, 50, 80, 100, 200, 400
            qrCodeProps = new QRCodeProperties(400, 400, cellSize);

            qrColorMode = new QRColorMode();
            if (isColored == true)
                qrColorMode = QRColorMode.Color;
            else
                qrColorMode = QRColorMode.Grayscale;
        }

        private void InputTextChangeEventHandler()
        {
            outputImages.Clear();
            outputMessages.Clear();

            // Get textbox string
            string textBoxMessage = textBoxMain.Text;

            // Binary string from ASCII message
            string binaryMessage = textBoxMessage.ASCIIToBinary(false);

            // Generate QR code and show it
            outputImages = qrCodeOps.GenerateQRCore(binaryMessage, qrCodeProps, qrColorMode);

            foreach (KeyValuePair<string, BitmapSource> outputImage in outputImages)
            {
                switch (outputImage.Key)
                {
                    case QR_TYPE_COMBINED: masterImage.Source = outputImage.Value; break;
                    case QR_TYPE_MONOCHROME: masterImage.Source = outputImage.Value; break;
                    case QR_TYPE_RED: imageInputRed.Source = outputImage.Value; break;
                    case QR_TYPE_GREEN: imageInputGrn.Source = outputImage.Value; break;
                    case QR_TYPE_BLUE: imageInputBlu.Source = outputImage.Value; break;
                    case QR_TYPE_RED_OUT: break;
                    case QR_TYPE_GREEN_OUT: break;
                    case QR_TYPE_BLUE_OUT: break;
                    default: break;
                }
            }

            // Process QR code and return encoded message
            outputMessages = qrCodeOps.DecodeQRCode(qrCodeProps, qrColorMode);

            foreach (KeyValuePair<string, string> outputMessage in outputMessages)
            {
                switch (outputMessage.Key)
                {
                    case QR_TYPE_MONOCHROME: textBoxDecoded.Text = outputMessage.Value.BinaryToASCII(); break;
                    case QR_TYPE_COMBINED: textBoxDecoded.Text = outputMessage.Value.BinaryToASCII(); break;
                    case QR_TYPE_RED: textBoxDecodedRed.Text = outputMessage.Value.BinaryToASCII(); break;
                    case QR_TYPE_GREEN: textBoxDecodedGrn.Text = outputMessage.Value.BinaryToASCII(); break;
                    case QR_TYPE_BLUE: textBoxDecodedBlu.Text = outputMessage.Value.BinaryToASCII(); break;
                    default: break;
                }
            }

            // Split color QR code into R,G,B and show it on screen
            if(qrColorMode == QRColorMode.Color)
            {
                Dictionary<string, BitmapSource> outputSplitImages = qrCodeOps.SplitColoredQR(qrCodeOps.GetColoredQR());

                foreach(KeyValuePair<string, BitmapSource> outputSplitImage in outputSplitImages)
                {
                    switch (outputSplitImage.Key)
                    {
                        case QR_TYPE_RED_OUT: imageOutputRed.Source = outputSplitImage.Value; break;
                        case QR_TYPE_GREEN_OUT: imageOutputGrn.Source = outputSplitImage.Value; break;
                        case QR_TYPE_BLUE_OUT: imageOutputBlu.Source = outputSplitImage.Value; break;
                        default: break;
                    }
                }
            }
        }
    }
}
