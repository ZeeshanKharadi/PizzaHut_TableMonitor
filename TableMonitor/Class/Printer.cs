using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableMonitor.Class
{
    class Printer
    {
        public int Printer_Id { get; set; }
        public string PrinterName { get; set; }

        public bool PrinterStatus { get; set; }
        public int TransactionType { get; set; }
    }
}
