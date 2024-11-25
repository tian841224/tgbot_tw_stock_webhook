using System.Text.Json.Serialization;

namespace TGBot_TW_Stock_Webhook.Model.DTOs
{
    /// <summary>
    /// 台灣證交所API回傳格式
    /// </summary>
    public class TWSEApiResponse
    {
        [JsonPropertyName("stat")]
        public string? Status { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("tables")]
        public List<Tables>? Tables { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("fields")]
        public List<string>? Fields { get; set; }

        [JsonPropertyName("data")]
        public List<List<object>?>? Data { get; set; }

        [JsonPropertyName("hints")]
        public string? Hints { get; set; } = default!;

        [JsonPropertyName("notes")]
        public List<string>? Notes { get; set; }

        [JsonPropertyName("params")]
        public ParamsInfo? Params { get; set; }

        [JsonPropertyName("total")]
        public int? Total { get; set; }
    }

    public class ParamsInfo
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("controller")]
        public string? Controller { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("lang")]
        public string? Lang { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }
    }

    public class Tables
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("fields")]
        public List<string>? Fields { get; set; }

        [JsonPropertyName("data")]
        public List<List<object>?>? Data { get; set; }
    }
}
