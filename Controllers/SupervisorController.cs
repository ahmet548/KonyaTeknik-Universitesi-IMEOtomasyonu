namespace IMEAutomationDBOperations.Controllers
{
    using IMEAutomationDBOperations.Data;
    using IMEAutomationDBOperations.Models;
    using IMEAutomationDBOperations.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using System.Linq;

    public class SupervisorController : Controller
    {
        private readonly DatabaseService _databaseService;

        public SupervisorController(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SupervisorLogin()
        {
            var supervisors = _databaseService.GetSupervisorsData();
            return View(supervisors);
        }

        [HttpPost]
        public IActionResult SupervisorLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Hata = "E-posta ve şifre boş olamaz.";
                return View();
            }

            var supervisor = _databaseService.GetSupervisorsData()
                .FirstOrDefault(s => s.Email == email && s.Password == password);

            if (supervisor != null)
            {
                HttpContext.Session.SetString("Email", supervisor.Email);
                HttpContext.Session.SetString("UserName", supervisor.FirstName);
                HttpContext.Session.SetString("UserSurname", supervisor.LastName);
                HttpContext.Session.SetString("Expertise", supervisor.Expertise);
                HttpContext.Session.SetString("Role", "Supervisor");
                return RedirectToAction("SupervisorPage", "Supervisor");
            }

            ViewBag.Hata = "Hatalı personel adı veya şifre.";
            return View();
        }

        public IActionResult SupervisorPage()
        {
            var email = HttpContext.Session.GetString("Email");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Supervisor");

            ViewBag.Email = email;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            return View();
        }

        public IActionResult SupervisorStudentList()
        {
            var supervisorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(supervisorEmail))
            {
                return RedirectToAction("SupervisorLogin", "Supervisor");
            }

            var students = _databaseService.GetStudentsBySupervisorEmail(supervisorEmail);
            return View(students);
        }

        public IActionResult EvaluateStudent(int studentId)
        {
            var student = _databaseService.GetStudentById(studentId);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            ViewBag.SupervisorID = HttpContext.Session.GetString("SupervisorID");
            return View(student);
        }

        [HttpPost]
        public IActionResult SaveEvaluation(EvaluationPersonel evaluation)
        {
            _databaseService.SaveEvaluation(evaluation);
            return RedirectToAction("SupervisorStudentList");
        }
    }
}