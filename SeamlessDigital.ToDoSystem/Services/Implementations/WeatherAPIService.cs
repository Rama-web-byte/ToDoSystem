using Newtonsoft.Json;
using SeamlessDigital.ToDoSystem.ViewModels;
using SeamlessDigital.ToDoSystem.Models;
using SeamlessDigital.ToDoSystem.Services.Contracts;
namespace SeamlessDigital.ToDoSystem.Services.Implementations
{
    public class WeatherAPIService:IWeatherAPIRepo
    {
        private readonly string _weatherApiKey;
        private readonly HttpClient _httpClient;
        private string URL = "http://api.weatherapi.com/v1/current.json?key=";
        public WeatherAPIService(string weatherApiKey, HttpClient httpClient)
        {
            _weatherApiKey = weatherApiKey;
            _httpClient = httpClient;
        }

        public async Task<WeatherData> GetWeatherAsync(double latitude, double longitude)
        {
            var weatherApiUrl = $"{URL}{_weatherApiKey}&q={latitude},{longitude}";

            var response = await _httpClient.GetStringAsync(weatherApiUrl);
            var weatherResponse = JsonConvert.DeserializeObject<RootResponse>(response);

            if (weatherResponse?.Current != null)
            {
                return new WeatherData
                {
                    Temperature = weatherResponse.Current.Temp_C,
                    Condition = weatherResponse.Current.Condition.Text
                };
            }

            return null;
        }
    }
}

