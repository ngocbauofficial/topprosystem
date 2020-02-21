
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace TopProSystem.Extension.Printer
{
    public class PrinterMachine
    {


        public static bool DetectPrinterMachine(string printerName)
        {
            var printerlist = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
            foreach (var printer in printerlist)
            {
                if (printer.ToString().Equals(printerName))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> GetAllPrinterName()
        {
            List<string> PrinterNames = new List<string>();
            var printerlist = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
            foreach (var printer in printerlist)
            {
                PrinterNames.Add(printer.ToString());
            }
            return PrinterNames;
        }

        public static List<string> GetAllPaperName()
        {
            PaperSize pkSize;
            PrintDocument printDoc = new PrintDocument();
            List<string> pageSize = new List<string>();
            for (int i = 0; i < printDoc.PrinterSettings.PaperSizes.Count; i++)
            {
                pkSize = printDoc.PrinterSettings.PaperSizes[i];
                pageSize.Add(pkSize.PaperName);
            }
            return pageSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printer">printer name</param>
        /// <param name="paperName">paper type</param>
        /// <param name="filename">url file</param>
        /// <param name="copies">copies of files</param>
        /// <param name="isduplex">is duplex</param>
        /// <param name="isHorizontal">is horizontal</param>
        public static void PrintPDF(string printer, string paperName, string filename, int copies, bool isduplex = false, bool isHorizontal = false, bool printLabel = false)
        {
            try
            {    // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printer,
                    Copies = (short)copies,
                    Duplex = Duplex.Simplex,
                    
                };

                if (isduplex && printerSettings.CanDuplex && isHorizontal)
                {
                    printerSettings.Duplex = Duplex.Horizontal;
                }

                if (isduplex && printerSettings.CanDuplex && isHorizontal == false)
                {
                    printerSettings.Duplex = Duplex.Vertical;
                }
                //// Create our page settings for the paper size selected
                var pageSettings = new PageSettings(printerSettings);
                
                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.PaperName == paperName)
                    {
                        pageSettings.PaperSize = paperSize;
                        pageSettings.Margins = new Margins(0, 0, 200, 0);
                        break;
                    }
                }


                // Now print the PDF document
                if (printerSettings.IsValid)
                {
                    using (var document = PdfiumViewer.PdfDocument.Load(filename))
                    {
                        using (var printDocument = document.CreatePrintDocument())
                        {
                            printDocument.PrintController = new StandardPrintController();
                            printDocument.PrinterSettings = printerSettings;
                            printDocument.DefaultPageSettings = pageSettings;
                            printDocument.Print();
                        }
                    }
                }
            }
            catch 
            {
                throw;
            }
        }

    }
}