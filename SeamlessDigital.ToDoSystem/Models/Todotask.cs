using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace SeamlessDigital.ToDoSystem.Models
{
    public class Todotask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }

        public bool Completed { get; set; }

        public int UserId { get; set; }
       

        public int Priority { get; set; } = 3;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime? DueDate { get; set; }

        public int? CategoryId {  get; set; }

        public virtual Category? Category { get; set; }


    }
}
