using Newtonsoft.Json;

public class AdParameter
{
    [JsonProperty("pl")]
    public string Label { get; set; } = "";

    [JsonProperty("vl")]
    public object Value { get; set; } = new object();


    [JsonProperty("p")]
    public string PropertyName { get; set; } = "";


    [JsonProperty("v")]
    public object PropertyValue { get; set; } = new object();


    [JsonProperty("pu")]
    public string PropertyUniqueName { get; set; } = "";

}

public class FlatSearchResponse
{
    [JsonProperty("ads")]
    public Ad[] Ads { get; set; } = Array.Empty<Ad>();
}

public class Ad
{
    [JsonProperty("account_id")]
    public string? AccountId { get; set; }

    [JsonProperty("account_parameters")]
    public AdParameter[]? AccountParameters { get; set; }

    [JsonProperty("ad_id")]
    public long AdId { get; set; }

    [JsonProperty("ad_link")]
    public string? AdLink { get; set; }

    [JsonProperty("ad_parameters")]
    public AdParameter[] AdParameters { get; set; } = Array.Empty<AdParameter>();

    [JsonProperty("body")]
    public string? Body { get; set; }

    [JsonProperty("body_short")]
    public string? BodyShort { get; set; }

    [JsonProperty("category")]
    public string? Category { get; set; }

    [JsonProperty("company_ad")]
    public bool IsCompanyAd { get; set; }

    [JsonProperty("currency")]
    public string? Currency { get; set; }

    [JsonProperty("images")]
    public object[]? Images { get; set; }

    [JsonProperty("is_mine")]
    public bool IsMine { get; set; }

    [JsonProperty("list_id")]
    public long ListId { get; set; }

    [JsonProperty("list_time")]
    public string? ListTime { get; set; }

    [JsonProperty("message_id")]
    public string? MessageId { get; set; }

    [JsonProperty("paid_services")]
    public PaidServices? PaidServices { get; set; }

    [JsonProperty("phone_hidden")]
    public bool IsPhoneHidden { get; set; }

    [JsonProperty("price_byn")]
    public string? PriceByn { get; set; }

    [JsonProperty("price_usd")]
    public string? PriceUsd { get; set; }

    [JsonProperty("remuneration_type")]
    public string? RemunerationType { get; set; }

    [JsonProperty("show_parameters")]
    public ShowParameters? ShowParameters { get; set; }

    [JsonProperty("subject")]
    public string? Subject { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }
}

public class PaidServices
{
    [JsonProperty("halva")]
    public bool Halva { get; set; }

    [JsonProperty("highlight")]
    public bool Highlight { get; set; }

    [JsonProperty("polepos")]
    public bool PolePos { get; set; }

    [JsonProperty("ribbons")]
    public object? Ribbons { get; set; }
}

public class ShowParameters
{
    [JsonProperty("show_call")]
    public bool ShowCall { get; set; }

    [JsonProperty("show_chat")]
    public bool ShowChat { get; set; }

    [JsonProperty("show_import_link")]
    public bool ShowImportLink { get; set; }

    [JsonProperty("show_web_shop_link")]
    public bool ShowWebShopLink { get; set; }
}