using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Util;

namespace TableMonitor.Class
{
    [Table("crt.SALESTRANSACTION")]
    public class SALESTRANSACTION
    {
        public string TRANSACTIONID { get; set; }
        public long CHANNELID { get; set; }
        public string TERMINALID { get; set; }
        public string CUSTOMERID { get; set; }
        public string NAME { get; set; }
        public int BYTELENGTH { get; set; }
        public Byte[] TRANSACTIONDATA { get; set; }
        public DateTime CREATEDDATETIME { get; set; }
        public DateTime MODIFIEDDATETIME { get; set; }
        public DateTime DELETEDDATETIME { get; set; }
        public TimeSpan ROWVERSION { get; set; }
        public bool ISSUSPENDED { get; set; }
        public string COMMENT { get; set; }
        public int TYPE { get; set; }
        public string STAFF { get; set; }
        public double AMOUNT { get; set; }
        public int STORAGETYPE { get; set; }
        public int READONLYREASON { get; set; }
      

    }
}

