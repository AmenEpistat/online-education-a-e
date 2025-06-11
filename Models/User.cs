using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathTrainer.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }


        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [RegularExpression(@"^[\w\.\-]+@mail\.ru$", ErrorMessage = "Email должен быть с доменом mail.ru")]
        public string Email { get; set; }
        public int ExamProgress { get; set; }

        public List<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
    }
}