using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableMonitor.Class
{
    [Table("dbo.RetailTransaction")]
    public class MiddlewareTransaction
    {
        public long Id { get; set; } // Primary key, auto-incremented
        public Guid? GlobalId { get; set; }
        public string TransactionId { get; set; } // NVARCHAR(60)
        public string DataAreaId { get; set; } // NVARCHAR(50)
        public string Currency { get; set; } // NVARCHAR(50)
        public decimal? GrossAmount { get; set; } // NUMERIC(18, 0)
        public decimal? NetAmount { get; set; } // NUMERIC(18, 0)
        public decimal? NetPrice { get; set; } // NUMERIC(18, 0)
        public DateTime? TransDate { get; set; } // DATETIME2(7)
        public int? PaymentMode { get; set; } // INT
        public string Store { get; set; } // NVARCHAR(10)
        public string TenderTypeId { get; set; } // NVARCHAR(50)
        public string AmountCur { get; set; } // NVARCHAR(50)
        public string ThirdPartyOrderId { get; set; } // NVARCHAR(50)
        public string Json { get; set; } // NVARCHAR(MAX)
        public bool? IsPaid { get; set; } // BIT
        public bool? IsActive { get; set; } // BIT
        public bool? IsDeleted { get; set; } // BIT
        public DateTime? CreatedOn { get; set; } // DATETIME2(7)
        public string CreatedBy { get; set; } // NVARCHAR(25)
        public DateTime? ModifiedOn { get; set; } // DATETIME2(7)
        public string ModifiedBy { get; set; } // NVARCHAR(25)
        public string Comment { get; set; } // NVARCHAR(100)
        public string Floor { get; set; } // NVARCHAR(255)
        public string Table { get; set; } // NVARCHAR(255)
        public string Server { get; set; } // NVARCHAR(255)
        public string Person { get; set; } // NVARCHAR(255)
        public decimal? DiscAmount { get; set; } // DECIMAL(18, 2)
        public decimal? DiscAmountWithoutTax { get; set; } // DECIMAL(18, 2)
        public bool? IsFinalize { get; set; } // BIT
        public bool? IsOrderLock { get; set; } // BIT
        public string TaxGroup { get; set; } // NVARCHAR(60)
        public bool? IsFinalizeUpdate { get; set; } // BIT
        public string DiscountOfferId { get; set; } // NVARCHAR(100)
    }
}
