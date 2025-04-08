using System.Text.Json.Serialization;

namespace SeamlessDigital.ToDoSystem.Models
{
    public class WeatherData
    {
        public double Temperature { get; set; }
        public string Condition { get; set; }
    }

    // WeatherApiResponse model (for deserialization)
    public class RootResponse
    {
        public CurrentWeather Current { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("temp_c")]
        public double Temp_C { get; set; }
        public Condition Condition { get; set; }
    }

    public class Condition
    {
        public string Text { get; set; }
    }
   
}
