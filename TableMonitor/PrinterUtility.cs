using iTextSharp.text.pdf.qrcode;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using TableMonitor.Class;
using QRCoder;
using QRCode = QRCoder.QRCode;
using System.Windows.Forms;

namespace TableMonitor
{
    /// <summary>
    /// Printer utility is custom code for genrating the Tharmal printer receipt
    /// and perform selection of the printer and print the recipt according to pool
    /// </summary>
    internal class PrinterUtility
    {
        // This List Contains Database Items for Receipt
        private static TransactionData transactionData;

        private static Graphics graphics;
        private static string Pool;
        public static string PoolName;



        /// <summary>
        /// Print function get the data in Doc_PrintPage1 and get Poolid and printerName send to printer.
        /// Purpose of this function is to get doc , Printer name and Poolid and send doc to associate printer to print recipt.
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="pool"></param>
        /// <param name="td"></param>
        public static void Print(string printerName, string pool, TransactionData td, bool finalized = false)
        {
            Pool = pool;

            //transactionData = new TransactionData
            //{
            //    CREATEDDATETIME = td.CREATEDDATETIME,
            //    ReceiptID = td.ReceiptID,
            //    StaffId = td.StaffId,
            //    TransactionID = td.TransactionID,
            //    TableNumber = td.TableNumber,
            //    EmployeName = td.EmployeName,
            //    Server = td.Server,
            //    Information = td.Information,
            //    SUSPENDEDTRANSACTIONID = td.SUSPENDEDTRANSACTIONID,
            //    CHANNEL = td.CHANNEL,
            //    Floor = td.Floor,
            //    ThirdPartyOrderId = td.ThirdPartyOrderId,
            //    StoreId = td.StoreId,
            //    StoreName = td.StoreName,
            //    TaxReg = td.TaxReg,
            //    DiscountAmount = td.DiscountAmount,
            //    AmountExcl = td.AmountExcl,
            //    TaxAmount = td.TaxAmount,
            //    AmountIncl = td.AmountIncl,
            //    SurveyUrl = td.SurveyUrl,
            //    QrCode = td.QrCode,
            //    PaymentAmount = td.PaymentAmount,
            //    FBRInvoiceNo = td.FBRInvoiceNo,
            //    SalesLines = td.SalesLines,
            //    PaymentLines = td.PaymentLines
            //};


            var doc = new PrintDocument();
            var custdoc = new PrintDocument();


            if (finalized == true)
            {
                transactionData = new TransactionData
                {
                    CREATEDDATETIME = td.CREATEDDATETIME,
                    ReceiptID = td.ReceiptID,
                    StaffId = td.StaffId,
                    TransactionID = td.TransactionID,
                    TableNumber = td.TableNumber,
                    EmployeName = td.EmployeName,
                    Server = td.Server,
                    Information = td.Information,
                    SUSPENDEDTRANSACTIONID = td.SUSPENDEDTRANSACTIONID,
                    CHANNEL = td.CHANNEL,
                    Floor = td.Floor,
                    ThirdPartyOrderId = td.ThirdPartyOrderId,
                    StoreId = td.StoreId,
                    StoreName = td.StoreName,
                    TaxReg = td.TaxReg,
                    DiscountAmount = td.DiscountAmount,
                    AmountExcl = td.AmountExcl,
                    TaxAmount = td.TaxAmount,
                    AmountIncl = td.AmountIncl,
                    SurveyUrl = td.SurveyUrl,
                    QrCode = td.QrCode,
                    PaymentAmount = td.PaymentAmount,
                    FBRInvoiceNo = td.FBRInvoiceNo,
                    SalesLines = td.SalesLines,
                    PaymentLines = td.PaymentLines,
                    Terminal = td.Terminal,
                    TaxRatePercent = td.TaxRatePercent
                    
                };
                custdoc.DefaultPageSettings.PrinterSettings.PrinterName = printerName;
                custdoc.PrintPage += Doc_CustPrintPage;
                //ShowPrintPreview();
                custdoc.Print();
            }

            else
            {

                switch (pool)
                {
                    case "Master":
                        PoolName = "CUTT";
                        break;
                    case "01":
                        PoolName = "MAKE";
                        break;
                    case "02":
                        PoolName = "Pasta";
                        break;
                    case "03":
                        PoolName = "Fried";
                        break;
                    case "04":
                        PoolName = "BEVERAGES";
                        break;
                    default:
                        // Handle unknown pool values, if needed
                        PoolName = "CUTT"; // or any default value
                        break;
                }


                transactionData = new TransactionData
                {
                    CREATEDDATETIME = td.CREATEDDATETIME,
                    ReceiptID = td.ReceiptID,
                    StaffId = td.StaffId,
                    TransactionID = td.TransactionID,
                    TableNumber = td.TableNumber,
                    EmployeName = td.EmployeName,
                    Server = td.Server,
                    Information = td.Information,
                    SUSPENDEDTRANSACTIONID = td.SUSPENDEDTRANSACTIONID,
                    CHANNEL = td.CHANNEL,
                    Floor = td.Floor,
                    ThirdPartyOrderId = td.ThirdPartyOrderId,
                    StoreId = td.StoreId,
                    StoreName = td.StoreName,
                    TaxReg = td.TaxReg,
                    DiscountAmount = td.DiscountAmount,
                    AmountExcl = td.AmountExcl,
                    TaxAmount = td.TaxAmount,
                    AmountIncl = td.AmountIncl,
                    SurveyUrl = td.SurveyUrl,
                    QrCode = td.QrCode,
                    PaymentAmount = td.PaymentAmount,
                    FBRInvoiceNo = td.FBRInvoiceNo,
                    DeliveryNumber = td.DeliveryNumber
                };
                if (pool != "Master")
                {
                    foreach (var item in td.SalesLines)
                    {
                        if (item.PoolId == pool)
                        {
                            transactionData.SalesLines.Add(item);
                        }
                    }
                    doc.DefaultPageSettings.PrinterSettings.PrinterName = printerName;
                    doc.PrintPage += Doc_PrintPage1;
                    doc.Print();

                    //return
                    return;
                }


                foreach (var item in td.SalesLines)
                {
                    transactionData.SalesLines.Add(item);
                }

                doc.DefaultPageSettings.PrinterSettings.PrinterName = printerName;
                doc.PrintPage += Doc_PrintPage1;
                doc.Print();
            }

        }

        /// <summary>
        /// This event function is to design receipt and place data form transaction data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Doc_PrintPage1(object sender, PrintPageEventArgs e)
        {
            graphics = e.Graphics;
            var receipt = transactionData;
            var minifont = new Font("Arial", 7);
            var itemfont = new Font("Arial", 12);
            var smallfont = new Font("Arial", 13);// 8 10 12
            var mediumfont = new Font("Arial", 12);
            var largefont = new Font("Arial", 16);
            var Offset = 2;
            // int smallinc = 14, mediuminc = 16, largeinc = 18;
            int smallinc = 20, mediuminc = 15, largeinc = 30;
            var startX = 10;
            var startY = 0;
            /*

                        graphics.DrawString("MAITRE CHOUX ARTISTE", largefont,
                              new SolidBrush(Color.Black), 5, startY);

                        Offset = Offset + largeinc + 10;
                        graphics.DrawString("           PATISSIER    ", largefont,
                              new SolidBrush(Color.Black), 5, startY + Offset);
            */
            // Offset = Offset + largeinc + 10;

            String underLine = "-----------------------------------------------";
            // DrawLine(underLine, smallfont, Offset, 5);

            //Offset = Offset + mediuminc;

            //For OrderNo
            //DrawAtStartForTable($"Order No: {transactionData.TableNumber}", Offset);
            //Offset = Offset + mediuminc;

            if (transactionData.SUSPENDEDTRANSACTIONID != null)
            {
                // Get the last 4 characters of SUSPENDEDTRANSACTIONID
                string last6Digits = transactionData.SUSPENDEDTRANSACTIONID.Substring(transactionData.SUSPENDEDTRANSACTIONID.Length - 6);
                DrawAtStartForHeader($"OrderNo: {last6Digits}", Offset);
                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
            }
            if (transactionData.ThirdPartyOrderId != null)
            {

                //DrawAtStartForHeader($"OrderNo: {transactionData.ThirdPartyOrderId}", Offset);
                string last6Digits = transactionData.ThirdPartyOrderId.Substring(transactionData.ThirdPartyOrderId.Length - 6);
                DrawAtStartForHeader($"OrderNo: {last6Digits}", Offset);
                Offset = Offset + mediuminc;
                Offset = Offset + mediuminc;
            }

            DrawAtStart("Date: " + DateTime.Now, Offset);

            Offset = Offset + mediuminc;
            DrawAtStart($"Receipt Id: {transactionData.ReceiptID}", Offset);

            Offset = Offset + mediuminc;
            DrawAtStart($"Transaction: {transactionData.TransactionID}", Offset);

            //Offset = Offset + mediuminc;
            //DrawAtStart($"ThirdPartyOrderId: {transactionData.ThirdPartyOrderId}", Offset);
            if (transactionData.StaffId != null)
            {
                Offset = Offset + mediuminc;
                DrawAtStart($"Employee Id: {transactionData.StaffId}", Offset);
            }
            //if (transactionData.EmployeName != null)
            //{
            //    Offset = Offset + mediuminc;
            //    DrawAtStart($"Employee: {transactionData.EmployeName}", Offset);
            //}

            if (transactionData.SUSPENDEDTRANSACTIONID != null)
            {
                Offset = Offset + mediuminc;
                DrawAtStart($"Susp. Id: {transactionData.SUSPENDEDTRANSACTIONID}", Offset);
            }
            if (transactionData.Server != null && transactionData.Server != "")
            {
                Offset = Offset + mediuminc;
                DrawAtStartForTable($"ServerName: {transactionData.Server}", Offset);
            }
            if (transactionData.DeliveryNumber != null && transactionData.DeliveryNumber != "")
            {
                Offset = Offset + mediuminc;
                DrawAtStartForTable($"Delivery Number: {transactionData.DeliveryNumber}", Offset);
            }
            if (transactionData.CHANNEL != null)
            {
                Offset = Offset + mediuminc;
                DrawAtStartForTable($"Channel: {transactionData.CHANNEL}", Offset);
            }

            if (transactionData.Floor != null)
            {
                Offset = Offset + mediuminc;
                DrawAtStartForTable($"Floor: {transactionData.Floor}", Offset);
            }

            //Offset = Offset + mediuminc;
            //DrawAtStartForTable($"Pool : {Pool}", Offset);

            Offset = Offset + mediuminc;
            DrawAtStartForTable($"PoolName : {PoolName}", Offset);

            //if (transactionData.ThirdPartyOrderId != null)
            //{
            //    Offset = Offset + mediuminc;
            //    DrawAtStartForTable($"OrderNo: {transactionData.ThirdPartyOrderId}", Offset);
            //}

            if (transactionData.SUSPENDEDTRANSACTIONID != null)
            {
                // Get the last 4 characters of SUSPENDEDTRANSACTIONID
                Offset = Offset + mediuminc;
                string last4Digits = transactionData.SUSPENDEDTRANSACTIONID.Substring(transactionData.SUSPENDEDTRANSACTIONID.Length - 4);
                DrawAtStartForTable($"OrderNo: {last4Digits}", Offset);
                Offset = Offset + mediuminc;

            }
            if (transactionData.ThirdPartyOrderId != null)
            {
                Offset = Offset + mediuminc;
                DrawAtStartForTable($"OrderNo: {transactionData.ThirdPartyOrderId}", Offset);
                Offset = Offset + mediuminc;
            }
            if (transactionData.TableNumber != null)
            {
                Offset = Offset + mediuminc;
                DrawAtStartForTable($"Table No: {transactionData.TableNumber}", Offset);
            }

            Offset = Offset + smallinc;
            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + largeinc;

            InsertHeaderStyleItem("QTY", "", "", "ITEM NAME", Offset);
            Offset = Offset + smallinc;
            var srNo = 0;
            foreach (var item in receipt.SalesLines)
            {
                srNo++;
                InsertItem(srNo, item, ref Offset);
                //Offset = Offset + 55;

            }
            Offset = Offset + largeinc;

            Offset = Offset + 7;
            underLine = "-------------------------------------";
            DrawLine(underLine, largefont, Offset, 0);
        }

        private static void Doc_CustPrintPage(object sender, PrintPageEventArgs e)
        {
            graphics = e.Graphics;
            var receipt = transactionData;
            var minifont = new Font("Arial", 7);
            var itemfont = new Font("Arial", 12);
            var smallfont = new Font("Arial", 14);// 8 10 12
            var mediumfont = new Font("Arial", 12);
            var totalfont = new Font("Arial", 9);
            var largefont = new Font("Arial", 16);
            var linefont = new Font("Arial", 16, FontStyle.Bold);
            var Offset = 2;
            int smallinc = 10, mediuminc = 20, largeinc = 30;
            var startX = 10;
            var startY = 5;
            decimal changeback = 0.00M;


            graphics.DrawString($"Welcome To {transactionData.StoreName}", totalfont,
                              new SolidBrush(Color.Black), 50, startY);

            Offset = Offset + smallinc + 10;
            graphics.DrawString($"Store ID:         {transactionData.StoreId}", totalfont,
                    new SolidBrush(Color.Black), 50, Offset);

            Offset = Offset + smallinc + 10;
            graphics.DrawString($"PNTN Number:  4563773-4", totalfont,
                    new SolidBrush(Color.Black), 50, Offset);

            Offset = Offset + smallinc + 10;
            graphics.DrawString($"CHEEZIOUS", largefont,
                             new SolidBrush(Color.Black), 70, Offset);

          

            if (transactionData.ReceiptID != null || transactionData.ReceiptID != "")
            {
                Offset = Offset + largeinc + 10;
                graphics.DrawString($"ORDER NUMBER: {transactionData.ReceiptID.Substring(transactionData.ReceiptID.Length - 4)}", largefont,
                              new SolidBrush(Color.Black), 20, Offset);
            }
            Offset = Offset + largeinc + 15;
            DrawAtStart("Date: " + DateTime.Now, Offset);

            Offset = Offset + smallinc + 5;
            DrawAtStart($"Receipt Number: {transactionData.ReceiptID}", Offset);

            Offset = Offset + smallinc + 10;
            DrawAtStart($"Tax Invoice: {transactionData.FBRInvoiceNo}", Offset);

            Offset = Offset + smallinc + 5;
            DrawAtStart($"Transaction No: {transactionData.TransactionID}", Offset);

            String underLine = "--------------------------------";

            Offset = Offset + smallinc + 10;
            DrawAtStart($"Terminal: {transactionData.Terminal}", Offset);
            Offset = Offset + smallinc + 5;
            DrawAtStart($"Employee: {transactionData.StaffId}", Offset);
            Offset = Offset + smallinc + 5;
            DrawAtStart($"Table No: {transactionData.TableNumber}", Offset);
            Offset = Offset + smallinc + 5;
            DrawAtStart($"Server Name: {transactionData.Server}", Offset);


            Offset = Offset + smallinc + 10;
            DrawAtStart($"Channel Id: {transactionData.CHANNEL}", Offset);

            Offset = Offset + smallinc;
            underLine = "-----------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + mediuminc;
            InsertHeaderStyleItemCustPrint("Item Name", "", "", "Qty", "", "Excl Price", "", "Tax", "", "Total", Offset);

            Offset = Offset + smallinc;
            underLine = "-----------------------------------";
            DrawLine(underLine, largefont, Offset, 0);

            Offset = Offset + mediuminc;
            var srNo = 0;
            foreach (var item in receipt.SalesLines)
            {
                srNo++;
                InsertItemCustPrint(srNo, item, Offset);
                Offset = Offset + 60;

            }

            Offset = Offset + smallinc;
            underLine = "-----------------------------------";
            DrawLine(underLine, largefont, Offset, 0);


            Offset = Offset + largeinc;
            graphics.DrawString($"Channel ", totalfont,
                              new SolidBrush(Color.Black), startX, Offset);
            graphics.DrawString($"{transactionData.CHANNEL}", totalfont,
                              new SolidBrush(Color.Black), 200, Offset);
            if (transactionData.DiscountAmount > 0)
            {
                Offset = Offset + mediuminc;
                graphics.DrawString($"Discount: ", totalfont,
                                  new SolidBrush(Color.Black), startX, Offset);
                graphics.DrawString($"{transactionData.DiscountAmount}", totalfont,
                                  new SolidBrush(Color.Black), 200, Offset);
            }

            Offset = Offset + mediuminc;
            graphics.DrawString($"Total Exc. Tax: ", totalfont,
                              new SolidBrush(Color.Black), startX, Offset);
            graphics.DrawString($"{transactionData.AmountExcl}", totalfont,
                              new SolidBrush(Color.Black), 200, Offset);

            Offset = Offset + mediuminc;
            graphics.DrawString($"Total Tax: " + transactionData.TaxRatePercent + "%" , totalfont,
                              new SolidBrush(Color.Black), startX, Offset);
            graphics.DrawString($"{transactionData.TaxAmount}", totalfont,
                              new SolidBrush(Color.Black), 200, Offset);

            Offset = Offset + mediuminc;
            graphics.DrawString($"Total: ", totalfont,
                              new SolidBrush(Color.Black), startX, Offset);
            graphics.DrawString($"{transactionData.AmountIncl}", totalfont,
                              new SolidBrush(Color.Black), 200, Offset);


            Offset = Offset + smallinc + 5;
            graphics.DrawString($"Payment", totalfont,
                              new SolidBrush(Color.Black), startX, Offset);

            foreach (var item in receipt.PaymentLines)
            {
                Offset = Offset + mediuminc;
                graphics.DrawString($"{item.Tendertype}", totalfont,
                                  new SolidBrush(Color.Black), startX, Offset);

                graphics.DrawString($"{item.Amount}", totalfont,
                                  new SolidBrush(Color.Black), 200, Offset);
            }


            Offset = Offset + smallinc;
            underLine = "......................................";
            DrawLine(underLine, linefont, Offset, 0);

            if (transactionData.ReceiptID != "")
            {
                Offset = Offset + largeinc + 25;
                graphics.DrawString($"SCAN BELOW TO TRACK THIS ORDER", minifont,
                              new SolidBrush(Color.Black), 40, Offset);
                //string invoiceNumber = "125555"; // Replace with actual data
                //transactionData.QrCode = invoiceNumber;
                GenerateQRCodeWithGraphicsCustom(graphics, transactionData.ReceiptID, Offset);
            }

            if (transactionData.FBRInvoiceNo != "")
            {
                Offset = Offset + mediuminc + 100;
                graphics.DrawString($"SCAN BELOW TO VERIFY TAX INVOICE", minifont,
                              new SolidBrush(Color.Black), 40, Offset);
                //string invoiceNumber = "125555"; // Replace with actual data
                //transactionData.QrCode = invoiceNumber;
                GenerateQRCodeWithGraphicsCustom(graphics, transactionData.FBRInvoiceNo, Offset);
            }

            Offset = Offset + mediuminc + 100;
            graphics.DrawString($"CONNECT WITH CHEEZIOUS", totalfont, new SolidBrush(Color.Black), 60, Offset);

            Offset = Offset + mediuminc;
            graphics.DrawString($"www.cheezious.com  |  111-44-66-99", totalfont,
                              new SolidBrush(Color.Black), 40, Offset);

        }

        /// <summary>
        ///This function is use to draw string text on receipt.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Offset"></param>
        private static void DrawAtStart(string text, int Offset)
        {
            int startX = 0;
            int startY = 5;
            Font minifont = new Font("Arial", 9);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 5, startY + Offset);
        }
        private static void DrawAtStartForTable(string text, int Offset)
        {
            int startX = 0;
            int startY = 5;
            Font minifont = new Font("Arial", 12);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 5, startY + Offset);
        }
        private static void DrawAtStartForHeader(string text, int Offset)
        {
            int startX = 0;
            int startY = 5;
            Font minifont = new Font("Arial", 12);

            graphics.DrawString(text, minifont,
                     new SolidBrush(Color.Black), startX + 75, startY + Offset);
        }

        /// <summary>
        /// This function is use to draw "Serial No.","description","Comment","Info Code" in receipt.
        /// In this function we also check Empty or Null Comments or InfoCode
        /// </summary>
        /// <param name="srNo"></param>
        /// <param name="item"></param>
        /// <param name="Offset"></param>
        private static void InsertItem_bkup(int srNo, SalesLine item, int Offset)
        {
            using (Font minifont = new Font("Arial", 10))
            {
                int startX = 5;//10
                int startY = 10;

                //  graphics.DrawString($"{srNo})", new Font("Arial", 10), new SolidBrush(Color.Black), startX, startY + Offset + 4);

                RectangleF rect1 = new RectangleF(startX + 70, startY + 2 + Offset, 200, 80);
                rect1.Size = new Size(220, ((int)graphics.MeasureString(item.Description, minifont, 220, StringFormat.GenericTypographic).Height));

                graphics.DrawString(item.Description, minifont, new SolidBrush(Color.Black), rect1);
                graphics.DrawString($"{item.QTY}", minifont, new SolidBrush(Color.Black), startX + 20, startY + Offset);

                if (!String.IsNullOrEmpty(item.Comment) || !string.IsNullOrEmpty(item.ItemInfoCode))
                {
                    Offset += 50;
                    var rect2 = new RectangleF(startX + 70, startY + 10 + Offset, 200, 80);
                    graphics.DrawString($"{item.ItemInfoCode} / {item.Comment}", new Font("Arial", 10), new SolidBrush(Color.Black), rect2);
                    //   graphics.DrawString($"{item.Comment})", new Font("Arial", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                    Offset += 80;
                    var rect3 = new RectangleF(startX + 15, startY + 2 + Offset, 200, 80);
                    graphics.DrawString("  ", new Font("Arial", 10), new SolidBrush(Color.Black), rect3);
                }
            }
        }

        private static void InsertItem(int srNo, SalesLine item, ref int Offset)
        {
            using (Font minifont = new Font("Arial", 10))
            {
                int startX = 5; // Starting X position
                int startY = 10; // Starting Y position

                // Determine if we need to strike through the item description
                Font itemFont = new Font("Arial", 10, item.QTY < 0 ? FontStyle.Strikeout : FontStyle.Regular);

                // Measure the height of the item description dynamically based on the text length
                SizeF descriptionSize = graphics.MeasureString(item.Description, itemFont, 220, StringFormat.GenericTypographic);
                int descriptionHeight = (int)descriptionSize.Height;

                // Drawing the item description with possible strikeout
                RectangleF rect1 = new RectangleF(startX + 70, startY + 2 + Offset, 220, 0);
                graphics.DrawString(item.Description, itemFont, new SolidBrush(Color.Black), rect1);

                // Draw the quantity
                graphics.DrawString($"{item.QTY}", minifont, new SolidBrush(Color.Black), startX + 20, startY + Offset);

                // Update the Offset based on the height of the item description
                Offset += descriptionHeight + 10;  // Add some padding after the description

                if (!string.IsNullOrEmpty(item.Comment) || !string.IsNullOrEmpty(item.ItemInfoCode) || !string.IsNullOrEmpty(item.ItemSize))
                {
                    Offset += 20;

                    // Measure and draw the comment and item info code
                    var rect2 = new RectangleF(startX + 70, startY + 10 + Offset, 200, 80);
                    graphics.DrawString($" {item.ItemSize} / {item.ItemInfoCode} / {item.Comment}", new Font("Arial", 10), new SolidBrush(Color.Black), rect2);
                    Offset += 50;

                    // Draw some space between the items if needed
                    var rect3 = new RectangleF(startX + 15, startY + 2 + Offset, 200, 80);
                    graphics.DrawString("  ", new Font("Arial", 10), new SolidBrush(Color.Black), rect3);
                }
            }
        }

        //private static void InsertItem(int srNo, SalesLine item, int Offset)
        //{
        //    using (Font minifont = new Font("Arial", 10))
        //    {
        //        int startX = 5; // 10
        //        int startY = 10;

        //        // Determine if we need to strike through the item description
        //        Font itemFont = new Font("Arial", 10, item.QTY < 0 ? FontStyle.Strikeout : FontStyle.Regular);
        //        //Font itemFont = new Font("Arial", 10, item.QTY < 0 ? FontStyle.Strikeout | FontStyle.Bold : FontStyle.Regular);

        //        // Drawing the item description with possible strikeout
        //        RectangleF rect1 = new RectangleF(startX + 70, startY + 2 + Offset, 200, 80);
        //        rect1.Size = new Size(220, ((int)graphics.MeasureString(item.Description, itemFont, 220, StringFormat.GenericTypographic).Height));

        //        graphics.DrawString(item.Description, itemFont, new SolidBrush(Color.Black), rect1);
        //        graphics.DrawString($"{item.QTY}", minifont, new SolidBrush(Color.Black), startX + 20, startY + Offset);

        //        if (!String.IsNullOrEmpty(item.Comment) || !string.IsNullOrEmpty(item.ItemInfoCode))
        //        {
        //            Offset += 20;
        //            var rect2 = new RectangleF(startX + 70, startY + 10 + Offset, 200, 80);
        //            graphics.DrawString($"{item.ItemInfoCode} / {item.Comment}", new Font("Arial", 10), new SolidBrush(Color.Black), rect2);
        //            Offset += 50;
        //            var rect3 = new RectangleF(startX + 15, startY + 2 + Offset, 200, 80);
        //            graphics.DrawString("  ", new Font("Arial", 10), new SolidBrush(Color.Black), rect3);
        //        }
        //    }
        //}


        /// <summary>
        /// This function is use to draw Header in recipt "ITEM NAME","Qty"
        /// Purpose of this function is to setup header on receipt.
        /// </summary>
        /// <param name="col1">Item Name</param>
        /// <param name="col2">Blank</param>
        /// <param name="col3">Blank</param>
        /// <param name="col4">Qty</param>
        /// <param name="Offset"></param>
        private static void InsertHeaderStyleItem(string col1, string col2, string col3, string col4, int Offset)
        {
            int startX = 10;
            int startY = 5;
            Font itemfont = new Font("Arial", 8, FontStyle.Bold);

            graphics.DrawString(col1, itemfont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);

            graphics.DrawString(col4, itemfont,
                     new SolidBrush(Color.Black), startX + 70, startY + Offset);
        }

        /// <summary>
        /// This function is use to draw line in receipt.
        /// Purpose of this function is use to decorate or separate data with in receipt 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="Offset"></param>
        /// <param name="xOffset"></param>
        private static void DrawLine(string text, Font font, int Offset, int xOffset)
        {
            int startX = 10;
            int startY = 5;
            graphics.DrawString(text, font,
                     new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
        }

        /// <summary>
        /// This function is user to draw smiple string.
        /// </summary>
        /// <param name="text">String</param>
        /// <param name="font">Set font</param>
        /// <param name="Offset">Set Alignment</param>
        /// <param name="xOffset"></param>
        private static void DrawSimpleString(string text, Font font, int Offset, int xOffset)
        {
            int startX = 10;
            int startY = 5;
            graphics.DrawString(text, font,
                     new SolidBrush(Color.Black), startX + xOffset, startY + Offset);
        }

        private static void InsertItemCustPrint(int srNo, SalesLine item, int Offset)
        {
            var largefont = new Font("Arial", 16);
            using (Font minifont = new Font("Arial", 6))
            {
                int startX = 5;//10
                int startY = 10;

                //  graphics.DrawString($"{srNo})", new Font("Arial", 10), new SolidBrush(Color.Black), startX, startY + Offset + 4);

                RectangleF rect1 = new RectangleF(startX + 15, startY + 2 + Offset, 120, 80);
                rect1.Size = new Size(100, ((int)graphics.MeasureString(item.Description, minifont, 100, StringFormat.GenericTypographic).Height));

                graphics.DrawString(item.Description, minifont, new SolidBrush(Color.Black), rect1);
                graphics.DrawString($"{item.QTY}", minifont, new SolidBrush(Color.Black), startX + 130, startY + Offset);
                graphics.DrawString($"{item.Unitprice}", minifont, new SolidBrush(Color.Black), startX + 155, startY + Offset);
                graphics.DrawString($"{item.Taxprice}", minifont, new SolidBrush(Color.Black), startX + 195, startY + Offset);
                graphics.DrawString($"{item.Totalprice}", minifont, new SolidBrush(Color.Black), startX + 230, startY + Offset);

                //if (!String.IsNullOrEmpty(item.Comment) || !string.IsNullOrEmpty(item.ItemInfoCode))
                //{
                //    Offset += 50;
                //    var rect2 = new RectangleF(startX + 70, startY + 10 + Offset, 200, 80);
                //    graphics.DrawString($"{item.ItemInfoCode} / {item.Comment}", new Font("Arial", 10), new SolidBrush(Color.Black), rect2);
                //    //   graphics.DrawString($"{item.Comment})", new Font("Arial", 10), new SolidBrush(Color.Black), startX, startY + Offset);
                //    Offset += 80;
                //    var rect3 = new RectangleF(startX + 15, startY + 2 + Offset, 200, 80);
                //    graphics.DrawString("  ", new Font("Arial", 10), new SolidBrush(Color.Black), rect3);
                //}

            }
            Offset = Offset + 40;
            String underLine = "----------------------------------";
            DrawLine(underLine, largefont, Offset, 0);
        }
        private static void InsertHeaderStyleItemCustPrint(string col1, string col2, string col3, string col4, string col5, string col6, string col7, string col8, string col9, string col10, int Offset)
        {
            int startX = 10;
            int startY = 5;
            Font itemfont = new Font("Arial", 8, FontStyle.Regular);

            graphics.DrawString(col1, itemfont,
                         new SolidBrush(Color.Black), startX + 5, startY + Offset);

            graphics.DrawString(col4, itemfont,
                     new SolidBrush(Color.Black), startX + 120, startY + Offset);

            graphics.DrawString(col6, itemfont,
         new SolidBrush(Color.Black), startX + 145, startY + Offset);

            graphics.DrawString(col8, itemfont,
         new SolidBrush(Color.Black), startX + 200, startY + Offset);

            graphics.DrawString(col10, itemfont,
         new SolidBrush(Color.Black), startX + 220, startY + Offset);

        }

        static void GenerateQRCodeWithGraphicsCustom(Graphics graphics, string qrText, int Offset)
        {
            // QR code size
            int qrCodeSize = 100;

            // Generate QR code using QRCoder
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20)) // Generate QR
                        {
                            Offset += 20; // Adjust offset for QR placement

                            // Draw QR Code directly on the receipt
                            graphics.DrawImage(qrCodeImage, new Rectangle(80, Offset, qrCodeSize, qrCodeSize));

                            //// Draw Invoice Number Below QR Code
                            //using (Font totalFont = new Font("Arial", 12, FontStyle.Bold))
                            //{
                            //    graphics.DrawString(qrText, totalFont, new SolidBrush(Color.Black), 70, Offset + qrCodeSize + 5);
                            //}
                        }
                    }
                }
            }
        }

        private static PrintDocument printDocument = new PrintDocument();
        static public void ShowPrintPreview()
        {

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            printDocument.PrintPage += Doc_CustPrintPage;
            previewDialog.Document = printDocument;
            previewDialog.ShowDialog();
        }
    }
}