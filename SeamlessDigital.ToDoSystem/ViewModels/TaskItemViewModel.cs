using SeamlessDigital.ToDoSystem.Models;

namespace SeamlessDigital.ToDoSystem.ViewModels
{
    public class TaskItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public bool Completed { get; set; }

        public int UserId { get; set; }


        public int Priority { get; set; } 

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime? DueDate { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public WeatherDto? Weather { get; set; } // Weather information
    }

    public class WeatherDto
    {
        public double? Temperature { get; set; }
        public string? Condition { get; set; }
    }
}
