namespace SeamlessDigital.ToDoSystem.ViewModels
{
    public class CreateTaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public bool Completed { get; set; }

        public int UserId { get; set; }


        public int Priority { get; set; } = 3;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime? DueDate { get; set; }

        public int? CategoryId { get; set; }
    }
}
