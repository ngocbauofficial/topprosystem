using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace TopProSystem.Extension
{
    public class PrinterWithAcrobatOrFoxitRender
    {
        public void printer(string pathPdf)
        {
            ProcessStartInfo infoPrintPdf = new ProcessStartInfo();
            infoPrintPdf.FileName = pathPdf;
            // The printer name is hardcoded here, but normally I get this from a combobox with all printers
            string printerName = @"\\TIGERPC\Godex G500";
            string driverName = "printqueue.inf";
            string portName = "USB001";
            infoPrintPdf.FileName = @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            infoPrintPdf.Arguments = string.Format("/t {0} \"{1}\" \"{2}\" \"{3}\"",
            pathPdf, printerName, driverName, portName);
            infoPrintPdf.CreateNoWindow = true;
            infoPrintPdf.UseShellExecute = false;
            infoPrintPdf.WindowStyle = ProcessWindowStyle.Hidden;
            Process printPdf = new Process();
            printPdf.StartInfo = infoPrintPdf;
            printPdf.Start();

            // This time depends on the printer, but has to be long enough to
            // let the printer start printing
            System.Threading.Thread.Sleep(10000);

            if (!printPdf.CloseMainWindow())              // CloseMainWindow never seems to succeed
                printPdf.Kill(); printPdf.WaitForExit();  // Kill AcroRd32.exe
            printPdf.Close();
        }
    }
}