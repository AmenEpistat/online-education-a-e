using MathTrainer.Data;
using MathTrainer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTrainer.Controllers
{
    public class TestController : Controller
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        // Показывает страницу с тестом по выбранному уровню
        [HttpGet]
        [HttpGet]
        public IActionResult Start(string level)
        {
            if (string.IsNullOrEmpty(level))
            {
                return BadRequest("Уровень не указан");
            }

            string actionName = level.ToLower() switch
            {
                "easy" => "Easy",
                "medium" => "Medium",
                "hard" => "Hard",
                "exam" => "Exam",
                _ => null
            };

            if (actionName == null)
            {
                return BadRequest("Неверный уровень");
            }

            return RedirectToAction(actionName, "Courses");
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Submit(string level, List<string> answers)
        {
            if (answers == null || answers.Count == 0)
                return BadRequest("Ответы не получены");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            string courseName = level.ToLower() switch
            {
                "easy" => "Easy",
                "medium" => "Medium",
                "hard" => "Hard",
                _ => throw new ArgumentException("Неверный уровень")
            };

            var questions = await _context.Questions
                .Where(q => q.Level.ToLower() == level.ToLower())
                .Take(Math.Min(answers.Count, 5))
                .ToListAsync();

            if (!questions.Any())
                return BadRequest("Вопросы не найдены");

            int correct = 0;
            for (int i = 0; i < answers.Count && i < questions.Count; i++)
            {
                if (answers[i] == questions[i].CorrectAnswer)
                    correct++;
            }

            double score = correct / (double)questions.Count;
            int progressPercent = (int)(score * 100);

            var progress = await _context.CourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == courseName);

            if (progress == null)
            {
                progress = new CourseProgress
                {
                    UserId = userId,
                    CourseName = courseName,
                    ProgressPercent = progressPercent,
                    EasyTestPassed = courseName == "Easy" && score >= 0.8,
                    MediumTestPassed = courseName == "Medium" && score >= 0.8,
                    HardTestPassed = courseName == "Hard" && score >= 0.8,
                    CertificateIssued = false
                };
                _context.CourseProgresses.Add(progress);
            }
            else
            {
                progress.ProgressPercent = progressPercent;
                if (courseName == "Easy") progress.EasyTestPassed = score >= 0.8;
                if (courseName == "Medium") progress.MediumTestPassed = score >= 0.8;
                if (courseName == "Hard") progress.HardTestPassed = score >= 0.8;
            }

            await _context.SaveChangesAsync();

            
            ViewBag.Answers = answers;
            ViewBag.Score = score;
            ViewBag.ShowResults = true;

            string viewName = level.ToLower() switch
            {
                "easy" => "Easy",
                "medium" => "Medium",
                "hard" => "Hard",
                "exam" => "Exam",
                _ => null
            };

            if (viewName == null)
            {
                return BadRequest("Неверный уровень");
            }

            return View($"~/Views/Courses/{viewName}.cshtml", questions);
        }

    }
}