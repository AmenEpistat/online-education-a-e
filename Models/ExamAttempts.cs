using System;
using System.ComponentModel.DataAnnotations;

namespace MathTrainer.Models
{
    public class ExamAttempt
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int AttemptsUsed { get; set; }

        public DateTime LastAttempt { get; set; }

        public User User { get; set; }
    }
}