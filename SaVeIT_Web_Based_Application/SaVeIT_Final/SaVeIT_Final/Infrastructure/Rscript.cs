using System;
using System.Diagnostics;
using System.IO;

namespace SaVeIT_Final.Infrastructure
{
    public static class Rscript
    {
        public static bool Run(string filename)
        {
            var rCodeFilePath = $"C:\\Users\\COMNET\\source\\repos\\SaVeIT_Project\\SaVeIT_Final\\RProject\\{filename}.R";



            var rScriptExecutablePath = @"C:\Program Files\R\R-3.4.3\bin\x64\Rscript.exe";

            var result = string.Empty;

            try
            {
                var info = new ProcessStartInfo
                {
                    FileName = rScriptExecutablePath,
                    WorkingDirectory = Path.GetDirectoryName(rScriptExecutablePath),
                    Arguments = rCodeFilePath,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };


                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    proc.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception("R Script failed: " + result, ex);

            }
        }
    }
}