using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableMonitor.Class
{
    [Table("dbo.RetailTransactionSalesTrans")]
    public class MiddlewareSALESTRANSACTION
    {
        [Key]
        public long Id { get; set; }  // Maps to 'Id', and the primary key
        public Guid? GlobalId { get; set; }  // Maps to 'GlobalId' (nullable GUID)

        [Required]
        [MaxLength(60)]
        public string TransactionId { get; set; }  // Maps to 'TransactionId' (nvarchar(60))

        [MaxLength(50)]
        public string Store { get; set; }  // Maps to 'Store' (nvarchar(50))

        [MaxLength(50)]
        public string ItemId { get; set; }  // Maps to 'ItemId' (nvarchar(50))

        [MaxLength(250)]
        public string ItemName { get; set; }  // Maps to 'ItemName' (nvarchar(250))

        public decimal? Linenum { get; set; }  // Maps to 'Linenum' (numeric(18, 0))

        public decimal? Quantity { get; set; }  // Maps to 'Quantity' (numeric(18, 0))

        public decimal? TaxAmount { get; set; }  // Maps to 'TaxAmount' (numeric(18, 0))

        public decimal? NetAmount { get; set; }  // Maps to 'NetAmount' (numeric(18, 0))

        public decimal? NetAmountInclTax { get; set; }  // Maps to 'NetAmountInclTax' (numeric(18, 0))

        public DateTime? TransdDate { get; set; }  // Maps to 'TransdDate' (datetime2(7))

        public decimal? Price { get; set; }  // Maps to 'Price' (numeric(18, 0))

        public decimal? NetPrice { get; set; }  // Maps to 'NetPrice' (numeric(18, 0))

        [MaxLength(250)]
        public string Comment { get; set; }  // Maps to 'Comment' (nvarchar(250))

        public bool? IsActive { get; set; }  // Maps to 'IsActive' (bit)

        public bool? IsDeleted { get; set; }  // Maps to 'IsDeleted' (bit)

        public DateTime? CreatedOn { get; set; }  // Maps to 'CreatedOn' (datetime2(7))

        [MaxLength(25)]
        public string CreatedBy { get; set; }  // Maps to 'CreatedBy' (nvarchar(25))

        public DateTime? ModifiedOn { get; set; }  // Maps to 'ModifiedOn' (datetime2(7))

        [MaxLength(25)]
        public string ModifiedBy { get; set; }  // Maps to 'ModifiedBy' (nvarchar(25))

        public string LineComment { get; set; }  // Maps to 'LineComment' (nvarchar(max))

        public float? DiscAmount { get; set; }  // Maps to 'DiscAmount' (float)

        public float? DiscAmountWithOutTax { get; set; }  // Maps to 'DiscAmountWithOutTax' (float)

        public float? PeriodicDiscAmount { get; set; }  // Maps to 'PeriodicDiscAmount' (float)

        public float? PeriodicPercentaGeDisc { get; set; }  // Maps to 'PeriodicPercentaGeDisc' (float)

        public int? Version { get; set; }  // Maps to 'version' (int)
    }
}
