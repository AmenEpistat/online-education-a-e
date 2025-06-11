using System.ComponentModel.DataAnnotations;

namespace MathTrainer.Models
{
    public class CourseProgress
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string CourseName { get; set; }

        public int ProgressPercent { get; set; }

        public bool EasyTestPassed { get; set; }

        public bool MediumTestPassed { get; set; }

        public bool HardTestPassed { get; set; }

        public bool CertificateIssued { get; set; }

        public User User { get; set; }
    }
}