using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableMonitor.Class
{
    class InitPrinter
    {
        public bool PrintPDF(string printer, string paperName, string filename,  int copies)
        {
            try
            {
                // Create the printer settings for our printer
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printer,
                    Copies = (short)copies,
                };


                // Now print the PDF document
                using (var document = PdfDocument.Load(filename))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        // printDocument.DefaultPageSettings = pageSettings;
                        printDocument.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom", 285, 600);
                       // printDocument.DefaultPageSettings.PaperSize.RawKind = 119;
                        printDocument.PrinterSettings.DefaultPageSettings.PaperSize.RawKind = 119;
                        printDocument.DefaultPageSettings.Landscape = false;

                        //printDocument.DefaultPageSettings.PaperSize = (new PaperSize("Roll Paper", (int)(80 * 0.254), (int)(297 * 0.254))); //PaperSize is taken from the Printer Settings multiplied with a hundreth of an inch

                        printDocument.PrintController = new StandardPrintController();
                        Font font = new Font("calibri", 15);



                        printDocument.Print();
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                //printDocument.Print();
                MessageBox.Show(ex.Message);
                return false;

            }
        }

      
    }
}
