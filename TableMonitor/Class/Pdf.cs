using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace TableMonitor.Class
{
    class Pdf
    {

        public void Pdfexprot(string logText)
        {
            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream("C:\\Users\\Administrator\\source\\repos\\TableMonitor\\TableMonitor\\TableMonitor\\bin\\Debug\\v.pdf", FileMode.Create));
            document.Open();

            Paragraph para = new Paragraph(logText);
            document.Add(para);




            document.Close();
        }



    }
}
    
