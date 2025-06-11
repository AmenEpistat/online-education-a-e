using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Для [NotMapped]
using System.Collections.Generic;
using System.Linq;

namespace MathTrainer.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        [Required]
        public string Options { get; set; }

        public int? CourseId { get; set; }

        public Course Course { get; set; }

        [NotMapped]
        public List<string> OptionsList
        {
            get => Options?.Split(';').ToList() ?? new List<string>();
            set => Options = string.Join(";", value);
        }
    }
}
