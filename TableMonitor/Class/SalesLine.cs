using System.Text.Json.Serialization;

public class SalesLine
{

    [JsonPropertyName("Description")]
    public string Description { get; set; } // ok

    [JsonPropertyName("ProductId")]
    public string ProductId { get; set; } //ok

    public string PoolId { get; set; }
    public int QTY { get; set; }
    public string Comment { get; set; }

    public string ItemInfoCode { get; set; }
    public decimal Unitprice { get; set; }
    public decimal Taxprice { get; set; }

    public decimal Totalprice { get; set; }
}