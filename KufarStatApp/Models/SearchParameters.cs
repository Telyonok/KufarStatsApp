namespace KufarStatApp.Models
{
    public class KufarSearchQueryParameters
    {
        public string Category { get; set; } = "1010";
        public string Currency { get; set; } = "USD";
        public string GeoTarget { get; set; } = "country-belarus~province-minsk~locality-minsk";
        public string Language { get; set; } = "ru";
        public int Size { get; set; } = 5000;
        public string Type { get; set; } = "sell";
    }

    public class PropertyBookingParameters
    {
        public string Area { get; set; } = "";
        public DateTime EntryDate { get; set; }
        public DateTime ExitDate { get; set; }
    }
}
