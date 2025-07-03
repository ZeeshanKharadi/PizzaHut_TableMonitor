using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TableMonitor.Class
{
    public class TransactionData

    {
        //public TransactionData() => SalesLines = new List<SalesLine>();

        public TransactionData()
        {
            SalesLines = new List<SalesLine>();
            PaymentLines = new List<PaymentData>();
        }


        public string CREATEDDATETIME { get; set; } //ok

        [JsonPropertyName("NetPrice")]
        public string ReceiptID { get; set; } //ok

        public string TransactionID { get; set; } //ok
        public string CUSTNAME { get; set; } //  
        public string Address {  get; set; } //  
        public string Phone { get; set; } // 
      

        [JsonPropertyName("SalesLines")]
        public List<SalesLine> SalesLines { get; set; }

        public string StaffId { get; set; } //ok

        public string EmployeName { get; set; }

        public string Information { get; set; }
        public string TableNumber { get; set; }
        public string SUSPENDEDTRANSACTIONID { get; set; }

        public string CHANNEL { get; set; }
        public string IsPaid { get; set; } // 
        public string paymode { get; set; }
        public string ThirdPartyOrderId { get; set; }
        public string Floor { get; set; }

        public string StoreId { get; set; }

        public string StoreName { get; set; }

        public string TaxReg { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal AmountExcl { get; set; }

        public decimal AmountIncl { get; set; }

        public string SurveyUrl { get; set; }

        public string QrCode { get; set; }

        [JsonPropertyName("PaymentLines")]
        public List<PaymentData> PaymentLines { get; set; }

        public decimal PaymentAmount { get; set; }

        public string FBRInvoiceNo { get; set; }
        public string Server { get; set; }
        public string DeliveryNumber { get; set; }
        public bool IsFinalize { get; set; }
        public string Terminal { get; set; }
        public string Size { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public decimal TaxRatePercent { get; set; }

    }
}
