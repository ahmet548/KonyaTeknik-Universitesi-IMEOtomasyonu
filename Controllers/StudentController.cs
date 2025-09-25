using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

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

        [HttpGet]
        public IActionResult StudentLogin()
        {
            // Yeni captcha oluştur ve session'a kaydet
            var rand = new Random();
            int num1 = rand.Next(1, 10);
            int num2 = rand.Next(1, 10);

            HttpContext.Session.SetInt32("CaptchaNum1", num1);
            HttpContext.Session.SetInt32("CaptchaNum2", num2);

            ViewBag.CaptchaNum1 = num1;
            ViewBag.CaptchaNum2 = num2;

            return View();
        }

        [HttpPost]
        public IActionResult StudentLogin(string email, string password, string captchaInput)
        {
            try
            {
                // 1. Captcha kontrolü (BACKEND'DE)
                int? num1 = HttpContext.Session.GetInt32("CaptchaNum1");
                int? num2 = HttpContext.Session.GetInt32("CaptchaNum2");

                if (num1 == null || num2 == null)
                {
                    ViewBag.Hata = "Güvenlik kodu süresi dolmuş. Lütfen tekrar deneyin.";
                    GenerateNewCaptcha();
                    return View();
                }

                int captchaResult = num1.Value + num2.Value;

                if (string.IsNullOrEmpty(captchaInput) || !int.TryParse(captchaInput, out int userCaptcha) || userCaptcha != captchaResult)
                {
                    ViewBag.Hata = "Güvenlik kodu yanlış";
                    GenerateNewCaptcha();
                    return View();
                }

                // 2. E-posta ve şifre boş kontrolü
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Hata = "E-posta ve şifre boş olamaz.";
                    GenerateNewCaptcha();
                    return View();
                }

                // 3. Kullanıcı doğrulama
                var student = _databaseService.GetStudentsData()
                    .FirstOrDefault(s => s.Email == email);

                if (student == null)
                {
                    ViewBag.Hata = "Hatalı kullanıcı adı veya şifre.";
                    GenerateNewCaptcha();
                    return View();
                }

                bool isAuthenticated = false;
                string storedPassword = student.Password ?? "";

                // Şifre doğrulama
                if (storedPassword.StartsWith("$2a$") || storedPassword.StartsWith("$2b$") || storedPassword.StartsWith("$2y$"))
                {
                    // BCrypt hash
                    isAuthenticated = BCrypt.Net.BCrypt.Verify(password, storedPassword);
                }
                else
                {
                    // Düz metin
                    isAuthenticated = password == storedPassword;
                }

                if (isAuthenticated)
                {
                    // Session'a kullanıcı bilgilerini kaydet
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
                    HttpContext.Session.SetInt32("StudentID", student.UserID);

                    return RedirectToAction("CheckMissingInfo", "Student");
                }
                else
                {
                    ViewBag.Hata = "Hatalı kullanıcı adı veya şifre.";
                    GenerateNewCaptcha();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Giriş işlemi sırasında bir hata oluştu: " + ex.Message;
                GenerateNewCaptcha();
                return View();
            }
        }

        private void GenerateNewCaptcha()
        {
            var rand = new Random();
            int num1 = rand.Next(1, 10);
            int num2 = rand.Next(1, 10);

            HttpContext.Session.SetInt32("CaptchaNum1", num1);
            HttpContext.Session.SetInt32("CaptchaNum2", num2);

            ViewBag.CaptchaNum1 = num1;
            ViewBag.CaptchaNum2 = num2;
        }

        [HttpGet]
        public IActionResult RefreshCaptcha()
        {
            var rand = new Random();
            int num1 = rand.Next(1, 10);
            int num2 = rand.Next(1, 10);

            HttpContext.Session.SetInt32("CaptchaNum1", num1);
            HttpContext.Session.SetInt32("CaptchaNum2", num2);

            return Json(new
            {
                num1 = num1,
                num2 = num2,
                question = $"{num1} + {num2} = ?"
            });
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
        public IActionResult SaveProfile(
            string firstName, string lastName, int academicYear, string nationalID, DateTime birthDate,
            string schoolNumber, string phoneNumber, string address, string department, string email,
            string supervisorFirstName, string supervisorLastName, DateTime startDate, DateTime endDate,
            string[] workDays, string internshipTitle, string password)
        {
            var sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("StudentLogin", "Student");

            var student = _databaseService.GetStudentsData().FirstOrDefault(s => s.Email == sessionEmail);

            // Supervisor ve Company bul
            var supervisor = _databaseService.GetSupervisorsData()
                .FirstOrDefault(s => s.FirstName == supervisorFirstName && s.LastName == supervisorLastName);

            if (supervisor == null)
            {
                ViewBag.Hata = "Girilen sorumlu personel bulunamadı.";
                return View("Register", student);
            }

            var companyName = HttpContext.Request.Form["CompanyName"];
            var company = _databaseService.GetCompaniesData()
                .FirstOrDefault(c => c.CompanyName == companyName);

            if (company == null)
            {
                ViewBag.Hata = "Girilen iş yeri bulunamadı.";
                return View("Register");
            }

            int studentId;

            if (student == null)
            {
                var user = new User
                {
                    UserName = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    RoleID = 2
                };
                int userId = _databaseService.AddUser(user);

                student = new Student
                {
                    UserID = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    AcademicYear = academicYear,
                    NationalID = nationalID,
                    BirthDate = birthDate,
                    SchoolNumber = schoolNumber,
                    Department = department,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    Address = address,
                    Password = password
                };
                studentId = _databaseService.AddStudentAndReturnId(student);
            }
            else
            {
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

                try
                {
                    _databaseService.UpdateStudent(student);
                }
                catch (Exception ex)
                {
                    ViewBag.Hata = "Bilgiler kaydedilirken bir hata oluştu: " + ex.Message;
                    return View("Register", student);
                }
                studentId = student.UserID;
            }

            _databaseService.AddInternshipDetails(
                studentId,
                supervisor.SupervisorID,
                company.CompanyID,
                startDate,
                endDate,
                workDays,
                internshipTitle
            );

            return RedirectToAction("StudentLogin", "Student");
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

        [HttpPost]
        public IActionResult AddNote(string Title, string SubTitle, string Content)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            var note = new Note
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

        [HttpGet]
        public IActionResult DeleteNote(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Student");

            _databaseService.DeleteUserNote(email, id);
            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult UploadVideo(IFormFile videoFile, string Title, string Description)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email) || videoFile == null || videoFile.Length == 0)
                return RedirectToAction("StudentPage");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "videos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                videoFile.CopyTo(stream);
            }

            var video = new Video
            {
                Title = Title,
                Description = Description,
                FilePath = "/uploads/videos/" + uniqueFileName,
                UploadDate = DateTime.Now
            };

            _databaseService.AddUserVideo(email, video);

            return RedirectToAction("StudentPage");
        }

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

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("KonyaTecnicalUnivercityIMEAutomation", "Home");
        }
    }
}