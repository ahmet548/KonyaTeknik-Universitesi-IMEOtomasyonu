namespace IMEAutomationDBOperations.Controllers
{
    using IMEAutomationDBOperations.Models;
    using IMEAutomationDBOperations.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using System;

    public class SupervisorController : Controller
    {
        private readonly SupervisorService _supervisorService;
        private readonly StudentService _studentService;
        private readonly InternshipOperationsService _internshipOperationsService;
        private readonly StudentDashboardService _studentDashboardService;

        public SupervisorController(
            SupervisorService supervisorService,
            StudentService studentService,
            InternshipOperationsService internshipOperationsService,
            StudentDashboardService studentDashboardService)
        {
            _supervisorService = supervisorService ?? throw new ArgumentNullException(nameof(supervisorService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _internshipOperationsService = internshipOperationsService ?? throw new ArgumentNullException(nameof(internshipOperationsService));
            _studentDashboardService = studentDashboardService ?? throw new ArgumentNullException(nameof(studentDashboardService));
        }

        public IActionResult Index()
        {
            // Login view'ına yönlendirme, AuthController'a taşındı.
            // Bu Index, SupervisorPage'e yönlendirebilir veya boş kalabilir.
            return RedirectToAction("SupervisorPage");
        }

        // SupervisorLogin (GET ve POST) AuthController'a taşındı.

        public IActionResult SupervisorPage()
        {
            var email = HttpContext.Session.GetString("Email");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("SupervisorLogin", "Auth"); // AuthController'a yönlendir

            var supervisor = _supervisorService.GetSupervisorByEmail(email);
            if (supervisor == null)
            {
                // Handle case where supervisor is not found, perhaps redirect to an error page or login
                return RedirectToAction("SupervisorLogin", "Auth");
            }

            ViewBag.Email = email;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.Supervisor = supervisor; // Set the supervisor object in ViewBag

            var students = _supervisorService.GetStudentsBySupervisorEmail(email);
            return View(students);
        }



        public IActionResult EvaluateStudent(int studentId)
        {
            var student = _studentService.GetStudentById(studentId);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            ViewBag.SupervisorID = HttpContext.Session.GetInt32("SupervisorID");
            return View(student);
        }

        [HttpPost]
        public IActionResult SaveEvaluation(EvaluationPersonel evaluation)
        {
            _internshipOperationsService.SaveEvaluation(evaluation);
            return RedirectToAction("SupervisorPage");
        }

        public IActionResult ViewStudentDetails(int studentId)
        {
            var supervisorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(supervisorEmail))
            {
                return RedirectToAction("SupervisorLogin", "Auth");
            }

            var student = _studentService.GetStudentById(studentId);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            var notes = _studentDashboardService.GetUserNotes(student.Email);
            var videos = _studentDashboardService.GetUserVideos(student.Email);
            var internshipDetails = _internshipOperationsService.GetInternshipDetailsByStudentId(studentId);
            var evaluation = _internshipOperationsService.GetEvaluationByStudentId(studentId);

            ViewBag.Notes = notes;
            ViewBag.Videos = videos;
            ViewBag.InternshipDetails = internshipDetails;
            ViewBag.Evaluation = evaluation;

            return View("SupervisorStudentDetails", student);
        }

        public IActionResult ShowNote(int noteId)
        {
            var supervisorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(supervisorEmail))
            {
                return RedirectToAction("SupervisorLogin", "Auth");
            }

            var note = _studentDashboardService.GetNoteById(noteId);
            if (note == null)
            {
                return NotFound("Not bulunamadı.");
            }

            var student = _studentService.GetStudentById(note.StudentID);
            if (student == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            var internshipDetails = _studentDashboardService.GetInternshipDetailsByStudentEmail(student.Email);
            Company company = null;
            if (internshipDetails != null)
            {
                company = _supervisorService.GetCompanyById(internshipDetails.CompanyID.GetValueOrDefault());
            }

            ViewBag.Note = note;
            ViewBag.CurrentNote = note; // Set the current note for highlighting in the sidebar
            ViewBag.StudentName = $"{student.FirstName} {student.LastName}";
            ViewBag.StudentId = student.StudentID; // Add StudentID to ViewBag
            ViewBag.Department = student.Department;
            ViewBag.InternshipTitle = internshipDetails?.InternshipTitle;
            ViewBag.Supervisor = _supervisorService.GetSupervisorByEmail(supervisorEmail); // Assuming current supervisor is viewing
            ViewBag.Company = company;

            // Fetch all notes for the student to display in the sidebar
            var allStudentNotes = _studentDashboardService.GetUserNotes(student.Email);
            ViewBag.Notes = allStudentNotes;

            return View(note);
        }

        public IActionResult SearchStudent(string searchTerm)
        {
            var supervisorEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(supervisorEmail))
            {
                return RedirectToAction("SupervisorLogin", "Auth");
            }

            var student = _supervisorService.SearchStudentByName(searchTerm, supervisorEmail);
            if (student == null)
            {
                return PartialView("_SupervisorStudentDetails", null);
            }

            var notes = _studentDashboardService.GetUserNotes(student.Email);
            var videos = _studentDashboardService.GetUserVideos(student.Email);

            ViewBag.Notes = notes;
            ViewBag.Videos = videos;

            return PartialView("_SupervisorStudentDetails", student);
        }
        public IActionResult SupervisorProfile()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("SupervisorLogin", "Auth");
            }

            var supervisor = _supervisorService.GetSupervisorByEmail(email);
            if (supervisor == null)
            {
                return NotFound("Supervisor not found.");
            }

            var company = _supervisorService.GetCompanyById(supervisor.CompanyID);

            ViewBag.Supervisor = supervisor;
            ViewBag.Company = company;

            return View();
        }

        [HttpPost]
        public IActionResult UpdateProfile(InternshipSupervisor updatedSupervisor)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("SupervisorLogin", "Auth");
            }

            var existingSupervisor = _supervisorService.GetSupervisorByEmail(email);
            if (existingSupervisor == null)
            {
                return NotFound("Supervisor not found.");
            }

            // Update only the editable fields
            existingSupervisor.FirstName = updatedSupervisor.FirstName;
            existingSupervisor.LastName = updatedSupervisor.LastName;
            existingSupervisor.ContactPhone = updatedSupervisor.ContactPhone;
            existingSupervisor.Expertise = updatedSupervisor.Expertise;
            existingSupervisor.Email = updatedSupervisor.Email; // Allow email update if desired, or keep existing

            _supervisorService.UpdateSupervisor(existingSupervisor);

            // Update session variables if email or name changed
            HttpContext.Session.SetString("Email", existingSupervisor.Email);
            HttpContext.Session.SetString("UserName", existingSupervisor.FirstName);
            HttpContext.Session.SetString("UserSurname", existingSupervisor.LastName);

            return RedirectToAction("SupervisorProfile");
        }
    }
}