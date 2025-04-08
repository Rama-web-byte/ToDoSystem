using SeamlessDigital.ToDoSystem.Models;


namespace SeamlessDigital.ToDoSystem.Services.Contracts
{
    public interface IWeatherAPIRepo
    {
        Task<WeatherData> GetWeatherAsync(double latitude, double longitude);
    }
}
