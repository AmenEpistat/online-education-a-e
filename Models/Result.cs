using System.ComponentModel.DataAnnotations;

namespace MathTrainer.Models
{
    public class Result
    {
        public int Id { get; set; }

        public int Score { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}