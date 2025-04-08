﻿using System.Text.Json.Serialization;

namespace SeamlessDigital.ToDoSystem.Models
{
    public class Category
    {
        public int? Id { get; set; }

        public string? Title { get; set; }

        public int? ParentCategoryId { get; set; }

        
        [JsonIgnore]
        public virtual Category? ParentCategory { get; set; }
    }
}
