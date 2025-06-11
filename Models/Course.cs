using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathTrainer.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();
    }
}