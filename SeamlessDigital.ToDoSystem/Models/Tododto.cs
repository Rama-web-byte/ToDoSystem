using System.Text.Json.Serialization;

namespace SeamlessDigital.ToDoSystem.Models
{
    public class Tododto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("todo")]
        public string? Todo { get; set; }

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("todos")]
        public List<Tododto>? Todos { get; set; }
    }
}

