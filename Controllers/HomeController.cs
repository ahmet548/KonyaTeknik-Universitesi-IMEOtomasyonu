using Microsoft.AspNetCore.Mvc;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using System;
using System.Linq;

namespace IMEAutomationDBOperations.Controllers
{
    public class HomeController : Controller
    {
        private readonly InternshipOperationsService _internshipOperationsService;
        private readonly StudentService _studentService;

        public HomeController(
            InternshipOperationsService internshipOperationsService,
            StudentService studentService)
        {
            _internshipOperationsService = internshipOperationsService ?? throw new ArgumentNullException(nameof(internshipOperationsService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        }

        public IActionResult KonyaTecnicalUnivercityIMEAutomation()
        {
            return View();
        }

        public IActionResult aboutme()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Auth"); // AuthController'a yönlendir

            var (supervisor, company, internshipDetail, evaluationPersonel) = _internshipOperationsService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(email);

            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "Misafir";
            ViewBag.UserSurname = HttpContext.Session.GetString("UserSurname") ?? "";
            ViewBag.NationalID = HttpContext.Session.GetString("NationalID") ?? "-";
            ViewBag.AcademicYear = HttpContext.Session.GetInt32("AcademicYear") ?? 0;
            ViewBag.Department = HttpContext.Session.GetString("Department") ?? "-";
            ViewBag.SchoolNumber = HttpContext.Session.GetString("SchoolNumber") ?? "-";
            ViewBag.PhoneNumber = HttpContext.Session.GetString("PhoneNumber") ?? "-";
            ViewBag.Email = HttpContext.Session.GetString("Email") ?? "-";
            ViewBag.Address = HttpContext.Session.GetString("Address") ?? "-";
            ViewBag.Supervisor = supervisor;
            ViewBag.Company = company;
            ViewBag.InternshipDetail = internshipDetail;

            return View();
        }

        [HttpPost]
        public IActionResult UpdateAboutMe(string tcNo, string fullName, int academicYear, string department, string schoolNo, string phoneNumber, string email, string address)
        {
            if (string.IsNullOrEmpty(tcNo))
            {
                return BadRequest("T.C. Kimlik No boş olamaz.");
            }

            var student = _studentService.GetStudentsData().FirstOrDefault(s => s.NationalID == tcNo);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            var nameParts = fullName.Split(' ', 2);
            student.FirstName = nameParts.Length > 0 ? nameParts[0] : student.FirstName;
            student.LastName = nameParts.Length > 1 ? nameParts[1] : student.LastName;

            student.AcademicYear = academicYear;
            student.Department = department;
            student.SchoolNumber = schoolNo;
            student.PhoneNumber = phoneNumber;
            student.Email = email;
            student.Address = address;

            _studentService.UpdateStudent(student);

            // Session bilgilerini güncelle
            HttpContext.Session.SetString("UserName", student.FirstName ?? string.Empty);
            HttpContext.Session.SetString("UserSurname", student.LastName ?? string.Empty);
            HttpContext.Session.SetInt32("AcademicYear", student.AcademicYear);
            HttpContext.Session.SetString("Department", student.Department);
            HttpContext.Session.SetString("SchoolNumber", student.SchoolNumber);
            HttpContext.Session.SetString("PhoneNumber", student.PhoneNumber);
            HttpContext.Session.SetString("Email", student.Email);
            HttpContext.Session.SetString("Address", student.Address);

            return RedirectToAction("aboutme");
        }

        public IActionResult KtunIMEPage()
        {
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("StudentLogin", "Auth"); // AuthController'a yönlendir
            }

            ViewBag.Role = role;
            return View();
        }

        // Bu metot artık kullanılmıyor gibi görünüyor, AuthController'da BCrypt kullanılıyor.
        // private string HashPassword(string password)
        // {
        //     using (var sha256 = System.Security.Cryptography.SHA256.Create())
        //     {
        //         var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        //         var hash = sha256.ComputeHash(bytes);
        //         return Convert.ToBase64String(hash);
        //     }
        // }
    }
}