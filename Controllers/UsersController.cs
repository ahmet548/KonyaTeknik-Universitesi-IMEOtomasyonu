using IMEAutomationDBOperations.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMEAutomationDBOperations.Models;

namespace IMEAutomationDBOperations.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult StudentPage()
        {
            string? email = HttpContext.Session.GetString("Email");
            string? userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("StudentLogin");
            }

            ViewBag.Email = email;
            ViewBag.UserName = string.IsNullOrEmpty(userName) ? "Misafir" : userName;
            return View();
        }

        [HttpPost]
        public IActionResult StudentLogin(string email, string password)
        {
            var student = _context.Students.FirstOrDefault(s => s.Email == email && s.Password == password);

            if (student != null)
            {
                HttpContext.Session.SetString("Email", student.Email);
                HttpContext.Session.SetString("UserName", student.FirstName ?? "Misafir");
                return RedirectToAction("StudentPage", "Home");
            }

            ViewBag.Hata = "Hatalı kullanıcı adı veya şifre ya da bilgileriniz sistemde mevcut değil.";
            return View("StudentLogin");
        }

        public IActionResult aboutme()
        {
            return View();
        }

        public IActionResult IsletmedeMeslekiEgitimSozlesmesi()
        {
            return View();
        }
    }
}
