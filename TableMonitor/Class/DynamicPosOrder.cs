using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableMonitor.Class
{
    [Table("dbo.DynamicPosOrders")]
    public class DynamicPosOrders
    {

            public int Id { get; set; }
            public string StoreId { get; set; }
            public string ThirdPartyOrderId { get; set; }
            public DateTime TransDate { get; set; }
            public string OrderSource { get; set; }  // Nullable
            public string OrderStatus { get; set; }
            public string RequestJson { get; set; }
            public string IsPrintRequest { get; set; }
            public DateTime? CreatedDateTime { get; set; }
            public DateTime? ModifiedDateTime { get; set; }

    }
}
