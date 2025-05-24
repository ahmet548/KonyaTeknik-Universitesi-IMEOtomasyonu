using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace IMEAutomationDBOperations.Controllers
{
    public class StudentController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly ApplicationDbContext _context;

        public StudentController(DatabaseService databaseService, ApplicationDbContext context)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _context = context;
        }

        public IActionResult StudentLogin()
        {
            var students = _databaseService.GetStudentsData();
            return View(students);
        }

        [HttpPost]
        public IActionResult StudentLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Hata = "E-posta ve şifre boş olamaz.";
                return View();
            }

            var student = _databaseService.GetStudentsData()
                .FirstOrDefault(s => s.Email == email && s.Password == password);

            if (student != null)
            {
                HttpContext.Session.SetString("Email", student.Email);
                HttpContext.Session.SetString("UserName", student.FirstName);
                HttpContext.Session.SetString("UserSurname", student.LastName);
                HttpContext.Session.SetString("NationalID", student.NationalID);
                HttpContext.Session.SetInt32("AcademicYear", student.AcademicYear);
                HttpContext.Session.SetString("Department", student.Department);
                HttpContext.Session.SetString("SchoolNumber", student.SchoolNumber);
                HttpContext.Session.SetString("PhoneNumber", student.PhoneNumber);
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetString("Address", student.Address);
                return RedirectToAction("CheckMissingInfo", "Student");
            }

            ViewBag.Hata = "Hatalı kullanıcı adı veya şifre.";
            return View();
        }

        public IActionResult StudentPage()
        {
            var email = HttpContext.Session.GetString("Email");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            var videoUploadDays = _databaseService.GetUserVideos(email);
            ViewBag.VideoUploadDays = videoUploadDays.Select(v => v.UploadDate.ToString("yyyy-MM-dd")).ToList();

            var notes = _databaseService.GetUserNotes(email);
            ViewBag.Notes = notes;

            var leaveDetails = _databaseService.GetLeaveDetailsByEmail(email);
            ViewBag.LeaveDetails = leaveDetails;

            // Mazaretli günleri dizi olarak hazırla
            var excuseDays = leaveDetails?
                .Where(l => l.LeaveStatus == "Onaylandı" || l.LeaveStatus == "Onay Bekliyor")
                .SelectMany(l =>
                {
                    var days = new List<string>();
                    for (var date = l.LeaveStart.Date; date <= l.LeaveEnd.Date; date = date.AddDays(1))
                        days.Add(date.ToString("yyyy-MM-dd"));
                    return days;
                })
                .Distinct()
                .ToList() ?? new List<string>();

            ViewBag.ExcuseDays = excuseDays;

            var userNotesDict = notes
                .GroupBy(n => n.CreatedAt.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        title = g.First().Title,
                        subTitle = g.First().SubTitle,
                        content = g.First().Content
                    }
                );
            ViewBag.UserNotes = userNotesDict;

            var videos = _databaseService.GetUserVideos(email);
            ViewBag.Videos = videos;
            ViewBag.VideoUploadDays = videos.Select(v => v.UploadDate.ToString("yyyy-MM-dd")).ToList();
            ViewBag.VideoTitles = videos.ToDictionary(
                v => v.UploadDate.ToString("yyyy-MM-dd"),
                v => v.Title
            );

            ViewBag.Email = email;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            return View();
        }

        public IActionResult CheckMissingInfo()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("StudentLogin", "Student");
            }

            var student = _databaseService.GetStudentsData().FirstOrDefault(s => s.Email == email);
            if (student == null)
            {
                return RedirectToAction("StudentLogin", "Student");
            }

            var missingFields = new List<string>();
            if (string.IsNullOrEmpty(student.FirstName)) missingFields.Add("FirstName");
            if (string.IsNullOrEmpty(student.LastName)) missingFields.Add("LastName");
            if (student.AcademicYear == 0) missingFields.Add("AcademicYear");
            if (string.IsNullOrEmpty(student.NationalID)) missingFields.Add("NationalID");
            if (student.BirthDate == DateTime.MinValue) missingFields.Add("BirthDate");
            if (string.IsNullOrEmpty(student.SchoolNumber)) missingFields.Add("SchoolNumber");
            if (string.IsNullOrEmpty(student.PhoneNumber)) missingFields.Add("PhoneNumber");
            if (string.IsNullOrEmpty(student.Address)) missingFields.Add("Address");
            if (string.IsNullOrEmpty(student.Department)) missingFields.Add("Department");
            if (string.IsNullOrEmpty(student.Email)) missingFields.Add("Email");

            if (missingFields.Any())
            {
                ViewBag.MissingFields = missingFields;
                return View("Register", student);
            }

            return RedirectToAction("StudentPage", "Student");
        }

        [HttpPost]
        public IActionResult SaveProfile(string firstName, string lastName, int academicYear, string nationalID, DateTime birthDate,
                                   string schoolNumber, string phoneNumber, string address, string department, string email,
                                   string supervisorFirstName, string supervisorLastName, DateTime startDate, DateTime endDate,
                                   string[] workDays, string internshipTitle)
        {
            var sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail))
            {
                return RedirectToAction("StudentLogin", "Student");
            }

            var student = _databaseService.GetStudentsData().FirstOrDefault(s => s.Email == sessionEmail);
            if (student == null)
            {
                return RedirectToAction("StudentLogin", "Student");
            }

            student.FirstName = firstName ?? student.FirstName;
            student.LastName = lastName ?? student.LastName;
            student.AcademicYear = academicYear != 0 ? academicYear : student.AcademicYear;
            student.NationalID = nationalID ?? student.NationalID;
            student.BirthDate = birthDate != DateTime.MinValue ? birthDate : student.BirthDate;
            student.SchoolNumber = schoolNumber ?? student.SchoolNumber;
            student.PhoneNumber = phoneNumber ?? student.PhoneNumber;
            student.Address = address ?? student.Address;
            student.Department = department ?? student.Department;
            student.Email = email ?? student.Email;

            var supervisor = _databaseService.GetSupervisorsData()
                .FirstOrDefault(s => s.FirstName == supervisorFirstName && s.LastName == supervisorLastName);

            if (supervisor == null)
            {
                ViewBag.Hata = "Girilen sorumlu personel bulunamadı.";
                return View("Register", student);
            }

            _databaseService.AddInternshipDetails(student.UserID, supervisor.SupervisorID, startDate, endDate, workDays, internshipTitle);

            try
            {
                _databaseService.UpdateStudent(student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                ViewBag.Hata = "Bilgiler kaydedilirken bir hata oluştu.";
                return View("Register", student);
            }

            return RedirectToAction("StudentPage", "Student");
        }

        [HttpGet]
        public IActionResult InternshipDiary()
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Student");

            var notes = _context.Notes
                .Where(n => n.StudentID == studentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            var videos = _context.Videos
                .Where(v => v.StudentID == studentId)
                .OrderByDescending(v => v.UploadDate)
                .ToList();

            ViewBag.Notes = notes;
            ViewBag.Videos = videos;
            return View();
        }

        [HttpPost]
        public IActionResult UploadVideo(IFormFile videoFile, string Title, string Description)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email) || videoFile == null || videoFile.Length == 0)
                return RedirectToAction("StudentPage");

            // Videoları kaydedeceğin klasör
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Dosya adı ve yolu
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                videoFile.CopyTo(stream);
            }

            // Veritabanına kaydet
            var video = new IMEAutomationDBOperations.Models.Video
            {
                Title = Title,
                Description = Description,
                FilePath = "/uploads/videos/" + uniqueFileName,
                UploadDate = DateTime.Now
            };

            _databaseService.AddUserVideo(email, video);

            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult AddNote(string Title, string SubTitle, string Content)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            var note = new IMEAutomationDBOperations.Models.Note
            {
                Title = Title,
                SubTitle = SubTitle,
                Content = Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _databaseService.AddUserNote(email, note);

            return RedirectToAction("StudentPage");
        }

        // Not Silme
        [HttpGet]
        public IActionResult DeleteNote(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            _databaseService.DeleteUserNote(email, id);
            return RedirectToAction("StudentPage");
        }

        // Video Silme
        [HttpGet]
        public IActionResult DeleteVideo(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            _databaseService.DeleteUserVideo(email, id);
            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult EditNote(int NoteID, string Title, string SubTitle, string Content)
        {
            var email = HttpContext.Session.GetString("Email");
            _databaseService.UpdateUserNote(email, NoteID, Title, SubTitle, Content);
            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult SubmitExcuse(DateTime LeaveStart, DateTime LeaveEnd, string LeaveReason, string ReasonDetail, string AddressDuringLeave)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            _databaseService.AddLeaveDetail(email, LeaveStart, LeaveEnd, LeaveReason, ReasonDetail, AddressDuringLeave);

            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult DeleteLeave(int LeaveID)
        {
            var email = HttpContext.Session.GetString("Email");
            _databaseService.DeleteLeaveDetail(email, LeaveID);
            return RedirectToAction("StudentPage");
        }


    }
}