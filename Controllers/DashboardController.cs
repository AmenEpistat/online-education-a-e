using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MathTrainer.Data;
using MathTrainer.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using MathTrainer.Services;

namespace MathTrainer.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public DashboardController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.CourseProgresses)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User updatedUser)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Age = updatedUser.Age;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Certificate(bool sendByEmail = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Now.ToString("dd.MM.yyyy", new CultureInfo("ru-RU"));

            var document = Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(40);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(16).FontColor(Colors.Black));

        page.Content().Padding(20).Border(5)
            .BorderColor(Colors.Indigo.Medium)
            .Background(Colors.Grey.Lighten3)
            .Column(column =>
        {
            column.Spacing(20);

           
            column.Item().AlignCenter().Text("АКАДЕМИЯ Amen&Els")
                .FontFamily("Georgia")
                .FontSize(18)
                .FontColor(Colors.Grey.Darken2)
                .Italic();
                
            
            column.Item().AlignCenter().Text("СЕРТИФИКАТ")
                .FontSize(42)
                .Bold()
                .FontColor(Colors.Indigo.Darken2)
                .Underline();

            
            column.Item().AlignCenter().Text(text =>
            {
                text.Span("Настоящий сертификат подтверждает, что ").FontSize(18);
                text.Span(user.Name).Bold().FontSize(20).FontColor(Colors.Blue.Medium);
                text.Span(" успешно прошёл экзамен с результатом 100%.").FontSize(18);
            });

       
            column.Item().AlignCenter().Text($"Дата выдачи: {today}")
                .FontSize(14)
                .Italic()
                .FontColor(Colors.Grey.Darken1);

      
            column.Item().PaddingTop(60).Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(text =>
{
    
    text.Line("Секретарь: Хакиева").FontSize(19);
    
});

                row.RelativeItem().AlignRight().Text(text =>
                {
                
                    text.Line("Секретарь: Иргалиева").FontSize(19);
                });

            });

            // Нижнее декоративное оформление
            column.Item().PaddingTop(30).AlignCenter().Text("Поздравляем с достижением!")
                .FontColor(Colors.Green.Darken2)
                .FontSize(16)
                .Italic();
        });
    });
});


            var pdfStream = new MemoryStream();
            document.GeneratePdf(pdfStream);
            pdfStream.Position = 0;

            if (sendByEmail)
            {
                await _emailService.SendCertificateAsync(user.Email, user.Name, pdfStream);
                TempData["Message"] = "Сертификат отправлен на почту!";
                return RedirectToAction("Index");
            }

            return File(pdfStream, "application/pdf", "Сертификат.pdf");
        }



    }
}