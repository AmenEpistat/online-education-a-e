using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MathTrainer.Data;
using MathTrainer.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTrainer.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> Easy()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id == userId); // ✅ прямо int

            if (userId == 0) return Unauthorized();

            var progress = await _context.CourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == "Easy");
            if (progress == null)
            {
                progress = new CourseProgress
                {
                    UserId = userId,
                    CourseName = "Easy",
                    ProgressPercent = 0,
                    EasyTestPassed = false,
                    MediumTestPassed = false,
                    HardTestPassed = false,
                    CertificateIssued = false
                };
                _context.CourseProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }

            var questions = await _context.Questions
                .Where(q => q.Level == "easy")
                .ToListAsync();
            return View("Easy", questions);
        }

        [HttpGet]
        public async Task<IActionResult> Medium()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id == userId); // ✅ прямо int

            if (userId == 0) return Unauthorized();
            var progress = await _context.CourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == "Medium");
            if (progress == null)
            {
                progress = new CourseProgress
                {
                    UserId = userId,
                    CourseName = "Medium",
                    ProgressPercent = 0,
                    EasyTestPassed = false,
                    MediumTestPassed = false,
                    HardTestPassed = false,
                    CertificateIssued = false
                };
                _context.CourseProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }

            var questions = await _context.Questions
                .Where(q => q.Level == "medium")
                .ToListAsync();
            return View("Medium", questions);
        }

        [HttpGet]
        public async Task<IActionResult> Hard()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id == userId); // ✅ прямо int

            if (userId == 0) return Unauthorized();

            var progress = await _context.CourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == "Hard");
            if (progress == null)
            {
                progress = new CourseProgress
                {
                    UserId = userId,
                    CourseName = "Hard",
                    ProgressPercent = 0,
                    EasyTestPassed = false,
                    MediumTestPassed = false,
                    HardTestPassed = false,
                    CertificateIssued = false
                };
                _context.CourseProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }

            var questions = await _context.Questions
                .Where(q => q.Level == "hard")
                .ToListAsync();
            return View("Hard", questions);
        }

        [HttpGet]
        public async Task<IActionResult> Exam()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id == userId); // ✅ прямо int

            if (userId == 0) return Unauthorized();

            var attempt = await _context.ExamAttempts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (attempt == null)
            {
                attempt = new ExamAttempt
                {
                    UserId = userId,
                    AttemptsUsed = 0,
                    LastAttempt = DateTime.UtcNow
                };
                _context.ExamAttempts.Add(attempt);

                var examProgress = await _context.CourseProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == "Exam");
                if (examProgress == null)
                {
                    examProgress = new CourseProgress
                    {
                        UserId = userId,
                        CourseName = "Exam",
                        ProgressPercent = 0,
                        EasyTestPassed = false,
                        MediumTestPassed = false,
                        HardTestPassed = false,
                        CertificateIssued = false
                    };
                    _context.CourseProgresses.Add(examProgress);
                }
                await _context.SaveChangesAsync();
            }

            if (attempt.AttemptsUsed >= 3)
            {
                ViewBag.Message = "Вы исчерпали все 3 попытки на экзамен.";
                return View("ExamDenied");
            }

            attempt.LastAttempt = DateTime.UtcNow;
            attempt.AttemptsUsed += 1;
            await _context.SaveChangesAsync();

            ViewBag.ExamStartTime = attempt.LastAttempt;
            var questions = await _context.Questions
                .Where(q => q.Level == "exam")
                .ToListAsync();

            if (!questions.Any())
            {
                ViewBag.Message = "Вопросы для экзамена не найдены.";
                return View("ExamDenied");
            }

            return View("Exam", questions);
        }

        [HttpPost]
        //     public async Task<IActionResult> SubmitExam(List<string> answers)
        //     {
        //         int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //         var user = await _context.Users
        //             .Include(u => u.CourseProgresses)
        //             .FirstOrDefaultAsync(u => u.Id == userId); // ✅ прямо int

        //         if (userId == 0) return Unauthorized();

        //         var questions = await _context.Questions
        //             .Where(q => q.Level == "exam")
        //             .Take(Math.Min(answers.Count, 5))
        //             .ToListAsync();

        //         if (!questions.Any())
        //             return BadRequest("Вопросы для экзамена не найдены");

        //         int correct = 0;
        //         for (int i = 0; i < answers.Count && i < questions.Count; i++)
        //         {
        //             if (answers[i] == questions[i].CorrectAnswer)
        //                 correct++;
        //         }

        //         double score = correct / (double)questions.Count;
        //         int progressPercent = (int)(score * 100);

        //         var examProgress = await _context.CourseProgresses
        //             .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == "Exam");
        //         if (examProgress == null)
        //         {
        //             examProgress = new CourseProgress
        //             {
        //                 UserId = userId,
        //                 CourseName = "Exam",
        //                 ProgressPercent = progressPercent,
        //                 EasyTestPassed = false,
        //                 MediumTestPassed = false,
        //                 HardTestPassed = false,
        //                 CertificateIssued = score >= 0.8
        //             };
        //             _context.CourseProgresses.Add(examProgress);
        //         }
        //         else
        //         {
        //             examProgress.ProgressPercent = progressPercent;
        //             examProgress.CertificateIssued = score >= 0.8;
        //         }

        //         try
        //         {
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (Exception ex)
        //         {
        //             return StatusCode(500, $"Ошибка при сохранении прогресса: {ex.Message}");
        //         }

        //         return RedirectToAction("Index", "Dashboard");
        //     }
        // }
        [HttpPost]
        public async Task<IActionResult> SubmitExam(List<List<string>> answers)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (userId == 0) return Unauthorized();

            var questions = await _context.Questions
                .Where(q => q.Level == "exam")
                .OrderBy(q => q.Id) 
                .Take(Math.Min(answers.Count, 5))
                .ToListAsync();

            if (!questions.Any())
                return BadRequest("Вопросы для экзамена не найдены");

            int correctCount = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var userAnswers = answers[i] ?? new List<string>();

                var correctAnswers = question.CorrectAnswer.Split(',')
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToHashSet();

                var userAnswerSet = userAnswers
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToHashSet();

                if (correctAnswers.SetEquals(userAnswerSet))
                {
                    correctCount++;
                }
            }

            double score = correctCount / (double)questions.Count;
            int progressPercent = (int)(score * 100);

            var examProgress = await _context.CourseProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseName == "Exam");

            if (examProgress == null)
            {
                examProgress = new CourseProgress
                {
                    UserId = userId,
                    CourseName = "Exam",
                    ProgressPercent = progressPercent,
                    EasyTestPassed = false,
                    MediumTestPassed = false,
                    HardTestPassed = false,
                    CertificateIssued = score >= 0.8
                };
                _context.CourseProgresses.Add(examProgress);
            }
            else
            {
                examProgress.ProgressPercent = progressPercent;
                examProgress.CertificateIssued = score >= 0.8;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при сохранении прогресса: {ex.Message}");
            }

            return RedirectToAction("Index", "Dashboard");
        }

    }
}