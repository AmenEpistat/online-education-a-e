// using MathTrainer.Data;
// using MathTrainer.Models;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using System.Linq;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity;


// namespace MathTrainer.Controllers
// {
//     public class AccountController : Controller
//     {
//         private readonly AppDbContext _context;

//         public AccountController(AppDbContext context)
//         {
//             _context = context;
//         }

//         [HttpPost]
//         // public async Task<IActionResult> Login(string username, string password)
//         // {
//         //     var user = await _context.Users
//         //         .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

//         //     if (user == null)
//         //     {
//         //         return BadRequest("Неверный логин или пароль");
//         //     }

//         //     HttpContext.Session.SetInt32("UserId", user.Id);
//         //     return RedirectToAction("Index", "Dashboard");
//         // }
//         public async Task<IActionResult> Login(string username, string password)
//         {
//             var user = await _context.Users
//                 .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

//             if (user == null)
//             {
//                 return BadRequest("Неверный логин или пароль");
//             }

//             var claims = new List<Claim>
//     {
//         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//         new Claim(ClaimTypes.Name, user.Username)
//     };

//             var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//             var principal = new ClaimsPrincipal(identity);

//             await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

//             return RedirectToAction("Index", "Dashboard");
//         }

//         [HttpPost]
//         public async Task<IActionResult> Register(string name, string username, string email, string password)
//         {
//             if (_context.Users.Any(u => u.Email == email))
//             {
//                 return BadRequest("Такой email уже зарегистрирован");
//             }

//             var user = new User
//             {
//                 Name = name,
//                 Username = username,
//                 Email = email,
//                 Password = password
//             };

//             _context.Users.Add(user);
//             await _context.SaveChangesAsync();

//             HttpContext.Session.SetInt32("UserId", user.Id);
//             return RedirectToAction("Index", "Dashboard"); // или на любую нужную страниц

//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Logout()
//         {
//             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//             return RedirectToAction("Login", "Account");
//         }

//         [HttpGet]
//         public IActionResult Login()
//         {
//             return View();
//         }

//         [HttpGet]
//         public IActionResult Register()
//         {
//             return View();
//         }

//     }
// }
using MathTrainer.Data;
using MathTrainer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathTrainer.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string name, string username, string email, string password)
        {
            // Простейшая валидация email через регулярку — можно заменить на ModelState при передаче модели
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[\w\.\-]+@mail\.ru$"))
            {
                ModelState.AddModelError("Email", "Email должен быть в домене mail.ru");
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("Email", "Такой email уже зарегистрирован");
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("Username", "Такой логин уже занят");
                return View();
            }

            var user = new User
            {
                Name = name,
                Username = username,
                Email = email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return BadRequest("Неверный логин или пароль");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result != PasswordVerificationResult.Success)
            {
                return BadRequest("Неверный логин или пароль");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
