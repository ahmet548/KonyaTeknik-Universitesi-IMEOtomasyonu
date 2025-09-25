using Microsoft.AspNetCore.Mvc;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;

namespace IMEAutomationDBOperations.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseService _databaseService;

        public HomeController(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public IActionResult KonyaTecnicalUnivercityIMEAutomation()
        {
            return View();
        }

        public IActionResult aboutme()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Home");

            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(email);

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

            var student = _databaseService.GetStudentsData().FirstOrDefault(s => s.NationalID == tcNo);
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

            _databaseService.UpdateStudent(student);

            HttpContext.Session.SetString("UserName", student.FirstName);
            HttpContext.Session.SetString("UserSurname", student.LastName);
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
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Role = role;
            return View();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
