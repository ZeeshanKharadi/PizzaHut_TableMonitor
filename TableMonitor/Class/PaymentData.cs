using System.Text.Json.Serialization;

public class PaymentData
{

    [JsonPropertyName("TenderType")]
    public string Tendertype { get; set; } // ok

    public string Amount { get; set; }
}