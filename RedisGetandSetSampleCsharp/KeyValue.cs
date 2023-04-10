using System.Text.Json.Serialization;

namespace RedisGetandSetSampleCsharp;

public class KeyValue
{
    [JsonPropertyName("key")]
    public string Key { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
}