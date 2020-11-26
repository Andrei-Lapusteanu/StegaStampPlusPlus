using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace StegaStampWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum PyScript
        {
            Encode, Decode, Detect
        }

        public enum OpenImageType
        {
            InputImg, OutputImg
        }

        public string inputImagePath = string.Empty;
        public string outputImagePath = string.Empty;
        public string residualImagePath = string.Empty;

        public string inputImageSaveName = string.Empty;
        public string outputImageSaveName = string.Empty;
        public string residualImageSaveName = string.Empty;

        public string pyScriptsBaseDir = string.Empty;
        public string pyStart = string.Empty;
        public string argModel = string.Empty;
        public string argEncImage = string.Empty;
        public string argDecImage = string.Empty;
        public string argSaveDir = string.Empty;
        public string argSecret = string.Empty;

        PointOperations pointOps;
        NoiseGenerators noiseGens;

        Dictionary<PyScript, string> pyScriptsDict = new Dictionary<PyScript, string>();

        public MainWindow()
        {
            InitializeComponent();

            MyInit();
        }

        private void MyInit()
        {
            pyScriptsDict.Add(PyScript.Encode, "encode_image.py");
            pyScriptsDict.Add(PyScript.Decode, "decode_image.py");
            pyScriptsDict.Add(PyScript.Detect, "detector.py");

            pyScriptsBaseDir = @"C:/Users/lapus/Desktop/Implementation/StegaStamp/";
            pyStart = @"C:\Python\Python37\python.exe";
            argModel = pyScriptsBaseDir + "saved_models/saved_models/stegastamp_pretrained";
            argEncImage = "--image" + " ";
            argDecImage = "--image" + " " + pyScriptsBaseDir + "out/";
            argSaveDir = "--save_dir" + " " + pyScriptsBaseDir + "out/";
            argSecret = "--secret" + " ";

            outputImagePath = pyScriptsBaseDir + "out/";

            pointOps = new PointOperations();
            noiseGens = new NoiseGenerators();
        }

        private void ButtonOpenInputImage_Click(object sender, RoutedEventArgs e)
        {
            string openResult = OpenImage(OpenImageType.InputImg);

            // Save path
            inputImagePath = openResult;

            if (openResult != null)
            {
                imageOutput.Source = null;
                imageInput.Source = CreateBitmapImage(openResult);
            }
            else
                MessageBox.Show("Failed to open file!", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void ButtonOpenOutputImage_Click(object sender, RoutedEventArgs e)
        {
            string openResult = OpenImage(OpenImageType.OutputImg);

            // Save path
            outputImagePath = openResult;

            if (openResult != null)
            {
                imageOutput.Source = null;
                imageOutput.Source = CreateBitmapImage(openResult);
            }
            else
                MessageBox.Show("Failed to open file!", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ButtonEncodeImage_Click(object sender, RoutedEventArgs e)
        {
            // Set current script
            pyScriptsDict.TryGetValue(PyScript.Encode, out string script);
            string pyScript = pyScriptsBaseDir + script;

            // Update args
            inputImagePath = inputImagePath.Replace('\\', '/');
            string tempArgImage = argEncImage + inputImagePath;
            string tempArgSecret = argSecret + textBoxSecretMessage.Text;

            // Create arguments
            string args = argModel + " " + tempArgImage + " " + argSaveDir + " " + tempArgSecret;

            // Run process
            ExecutePythonScript(pyScript, args, PyScript.Encode);
        }

        private void ButtonDecodeImage_Click(object sender, RoutedEventArgs e)
        {
            // Set current script
            pyScriptsDict.TryGetValue(PyScript.Decode, out string script);
            string pyScript = pyScriptsBaseDir + script;

            // Update args
            string tempArgDecImg = argDecImage + outputImageSaveName;

            // Create arguments
            string args = argModel + " " + tempArgDecImg;

            // Run process
            ExecutePythonScript(pyScript, args, PyScript.Decode);
        }

        private void ButtonDetectImage_Click(object sender, RoutedEventArgs e)
        {
            pyScriptsDict.TryGetValue(PyScript.Detect, out string pyScript);

        }

        private void ApplyBrightness_Click(object sender, EventArgs e)
        {
            // Apply modification
            imageOutput.Source = pointOps.SetBrightness(imageOutput.Source as BitmapSource, (int)controlBrightness.SliderValue);

            // Save image (necessary in this prototype in order to send it as a param to python)
            SaveImageAfterMods(imageOutput.Source as BitmapSource);
        }

        private void ApplyContrast_Click(object sender, EventArgs e)
        {
            // Apply modification
            imageOutput.Source = pointOps.SetContrast(imageOutput.Source as BitmapSource, (float)controlContrast.SliderValue);

            // Save image (necessary in this prototype in order to send it as a param to python)
            SaveImageAfterMods(imageOutput.Source as BitmapSource);
        }

        private void ApplyGaussianNoise_Click(object sender, EventArgs e)
        {
            // Apply modifications
            imageOutput.Source = noiseGens.AddGaussianNoise(imageOutput.Source as BitmapSource, (float)controlAddGaussianNoise.SliderValue, controlAddGaussianNoise.GetCheckBoxState);

            // Save image (necessary in this prototype in order to send it as a param to python)
            SaveImageAfterMods(imageOutput.Source as BitmapSource);
        }

        // --- METHODS

        public string OpenImage(OpenImageType openImageType)
        {
            // Create an open file dialog
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            // Set dialog open file path
            string tempPath = System.IO.Path.GetDirectoryName(
                System.IO.Path.GetDirectoryName(
                    System.IO.Path.GetDirectoryName(
                        Environment.CurrentDirectory)));

            // Set dialog properties
            dialog.InitialDirectory = tempPath;
            dialog.DefaultExt = ".jpg";
            dialog.Filter = ("All Images (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp" +
                            "|JPEG Images (*.jpg;*.jpeg)|*.jpg;*.jpeg" +
                            "|PNG Images (*.png)|*.png" +
                            "|Bitmap Images (*.bmp)|*.bmp");

            // Get dialog result
            bool? result = dialog.ShowDialog();

            if (openImageType == OpenImageType.InputImg)
                inputImageSaveName = dialog.SafeFileName.Substring(0, dialog.SafeFileName.Length - 4);
            else if (openImageType == OpenImageType.OutputImg)
                outputImageSaveName = dialog.SafeFileName;//.Substring(0, dialog.SafeFileName.Length - 4);

            // Return file path or null if result is false
            if (result == true)
                return dialog.FileName;
            else
                return null;
        }

        public void SaveImageAfterMods(BitmapSource img)
        {
            // Declare an image encoder
            BitmapEncoder encoder;

            encoder = new PngBitmapEncoder();

            // Write to file using encoder
            using (FileStream fileStream = new FileStream(outputImagePath, FileMode.Create))
            {
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(fileStream);
            }
        }


        private void ExecutePythonScript(string script, string args, PyScript pyScript)
        {
            string result = string.Empty;

            ProcessStartInfo psi = new ProcessStartInfo(pyStart);
            psi.Arguments = script + " " + args;

            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using (Process p = new Process())
            {
                // Run python script
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();

                // Check status
                if (p.ExitCode == 0)
                    result = p.StandardOutput.ReadToEnd();
                else
                {
                    MessageBox.Show("Pytyhon script " + script + " exited with return code " + p.ExitCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Show failed message and change color
                    textBlockRecoveredMessage.Foreground = new SolidColorBrush(Colors.Red);
                    textBlockRecoveredMessage.Text = "Failed!";
                    return;
                }

                if (pyScript == PyScript.Encode)
                {
                    Thread.Sleep(1000);

                    // Clear recovered message (of any)
                    textBlockRecoveredMessage.Text = "";

                    // Open output image
                    outputImageSaveName = inputImageSaveName + "_hidden.png";
                    outputImagePath = pyScriptsBaseDir + "out/" + outputImageSaveName;
                    imageOutput.Source = CreateBitmapImage(outputImagePath).Clone();

                    // Open residual image
                    residualImagePath = inputImageSaveName + "_residual.png";
                    residualImagePath = pyScriptsBaseDir + "out/" + residualImagePath;
                    imageResidual.Source = CreateBitmapImage(residualImagePath).Clone();
                }
                else if (pyScript == PyScript.Decode)
                {
                    // Check if failed to decode
                    var a = result.ToString().Substring(0, result.Length - 2);
                    if (result.ToString().Substring(0, result.Length - 2).Equals("Failed to decode!"))
                    {
                        // Show failed to decde message and change color
                        textBlockRecoveredMessage.Foreground = new SolidColorBrush(Colors.Red);
                        textBlockRecoveredMessage.Text = "Failed!";
                    }
                    else
                    {
                        // Show return message
                        textBlockRecoveredMessage.Foreground = new SolidColorBrush(Colors.GreenYellow);
                        textBlockRecoveredMessage.Text = result;
                    }
                }
            }
        }

        private BitmapImage CreateBitmapImage(string imgPath)
        {
            BitmapImage bmpImg = new BitmapImage();
            bmpImg.BeginInit();
            /// !!! - need
            bmpImg.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmpImg.UriSource = new Uri(imgPath, UriKind.Absolute);
            bmpImg.CacheOption = BitmapCacheOption.OnLoad;
            bmpImg.EndInit();

            return bmpImg;
        }
    }
}
