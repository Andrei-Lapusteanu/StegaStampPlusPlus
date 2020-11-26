using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            StegaStampDecoder();
        }

        private static void StegaStampEncoder()
        {
            string result = string.Empty;
            string baseDir = @"C:/Users/lapus/Desktop/Implementation/StegaStamp/";
            string pyScriptPath = baseDir + "encode_image.py";

            string argModel = baseDir + "saved_models/saved_models/stegastamp_pretrained" + " ";
            string argImage = "--image" + " " + baseDir + "test_img.png" + " ";
            string argSaveDir = "--save_dir" + " " + baseDir + "out/" + " ";
            string argSecret = "--secret" + " " + "Python";
            string args = argModel + argImage + argSaveDir + argSecret;

            ProcessStartInfo psi = new ProcessStartInfo(@"C:\Python\Python37\python.exe");
            psi.Arguments = pyScriptPath + " " + args;

            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using (Process p = new Process())
            {
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();

                if (p.ExitCode == 0)
                    result = p.StandardOutput.ReadToEnd();
            }
        }

        private static void StegaStampDecoder()
        {
            string result = string.Empty;
            string baseDir = @"C:/Users/lapus/Desktop/Implementation/StegaStamp/";
            string pyScriptPath = baseDir + "decode_image.py";

            string argModel = baseDir + "saved_models/saved_models/stegastamp_pretrained" + " ";
            string argImage = "--image" + " " + baseDir + "out/test_img_hidden.png";
            string args = argModel + argImage;

            ProcessStartInfo psi = new ProcessStartInfo(@"C:\Python\Python37\python.exe");
            psi.Arguments = pyScriptPath + " " + args;

            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using (Process p = new Process())
            {
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();

                if (p.ExitCode == 0)
                    result = p.StandardOutput.ReadToEnd();
            }
        }
    }
}
