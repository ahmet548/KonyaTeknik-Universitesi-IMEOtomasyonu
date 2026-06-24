using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;

namespace IMEAutomationDBOperations.Controllers
{
    public class AuthController : Controller
    {
        private readonly StudentService _studentService;
        private readonly SupervisorService _supervisorService;
        private readonly CompanyService _companyService;
        private readonly UserService _userService;
        private readonly InternshipOperationsService _internshipOperationsService;

        public AuthController(
            StudentService studentService,
            SupervisorService supervisorService,
            CompanyService companyService,
            UserService userService,
            InternshipOperationsService internshipOperationsService)
        {
            _studentService = studentService;
            _supervisorService = supervisorService;
            _companyService = companyService;
            _userService = userService;
            _internshipOperationsService = internshipOperationsService;
        }

        // --- Student Login ---

        [HttpGet]
        public IActionResult StudentLogin()
        {
            GenerateNewCaptcha();
            return View();
        }

        [HttpPost]
        public IActionResult StudentLogin(string email, string password, string captchaInput)
        {
            try
            {
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

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Hata = "E-posta ve şifre boş olamaz.";
                    GenerateNewCaptcha();
                    return View();
                }

                var student = _studentService.GetStudentsData().FirstOrDefault(s => s.Email == email);

                if (student == null)
                {
                    ViewBag.Hata = "Hatalı kullanıcı adı veya şifre.";
                    GenerateNewCaptcha();
                    return View();
                }

                bool isAuthenticated = false;
                string storedPassword = student.User.PasswordHash ?? "";

                if (storedPassword.StartsWith("$2a$") || storedPassword.StartsWith("$2b$") || storedPassword.StartsWith("$2y$"))
                {
                    isAuthenticated = BCrypt.Net.BCrypt.Verify(password, storedPassword);
                }
                else
                {
                    isAuthenticated = password == storedPassword;
                }

                if (isAuthenticated)
                {
                    HttpContext.Session.SetString("Email", student.User.Email);
                    HttpContext.Session.SetString("UserName", student.User.FirstName ?? string.Empty);
                    HttpContext.Session.SetString("UserSurname", student.User.LastName ?? string.Empty);
                    HttpContext.Session.SetString("NationalID", student.NationalID ?? string.Empty);
                    HttpContext.Session.SetInt32("AcademicYear", student.AcademicYear);
                    HttpContext.Session.SetString("Department", student.Department ?? string.Empty);
                    HttpContext.Session.SetString("SchoolNumber", student.SchoolNumber);
                    HttpContext.Session.SetString("PhoneNumber", student.PhoneNumber);
                    HttpContext.Session.SetString("Role", "Student");
                    HttpContext.Session.SetString("Address", student.Address);
                    HttpContext.Session.SetInt32("StudentID", student.Id);

                    return RedirectToAction("CompleteInternshipInfo", "Auth", new { studentId = student.Id });
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

        // --- Supervisor Login ---

        public IActionResult SupervisorLogin()
        {
            var supervisors = _supervisorService.GetSupervisorsData();
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

            var supervisor = _supervisorService.GetSupervisorsData()
                .FirstOrDefault(s => s.Email == email);

            if (supervisor != null)
            {
                bool isAuthenticated = false;
                string storedPassword = supervisor.PasswordHash ?? "";

                if (storedPassword.StartsWith("$2a$") || storedPassword.StartsWith("$2b$") || storedPassword.StartsWith("$2y$"))
                {
                    isAuthenticated = BCrypt.Net.BCrypt.Verify(password, storedPassword);
                }
                else
                {
                    isAuthenticated = password == storedPassword;
                }

                if (isAuthenticated)
                {
                    HttpContext.Session.SetString("Email", supervisor.User.Email);
                    HttpContext.Session.SetString("UserName", supervisor.User.FirstName);
                    HttpContext.Session.SetString("UserSurname", supervisor.User.LastName);
                    HttpContext.Session.SetString("Expertise", supervisor.Expertise ?? string.Empty);
                    HttpContext.Session.SetString("Role", "Supervisor");
                    HttpContext.Session.SetInt32("SupervisorId", supervisor.SupervisorId);
                    return RedirectToAction("SupervisorPage", "Supervisor");
                }
            }

            ViewBag.Hata = "Hatalı personel adı veya şifre.";
            return View();
        }

        // --- Supervisor Registration ---

        [HttpGet]
        public IActionResult SupervisorRegister()
        {
            return View("~/Views/Auth/SupervisorRegister.cshtml");
        }

        [HttpPost]
        public IActionResult SupervisorRegister(
            string firstName, string lastName, string email, string password, string confirmPassword,
            string contactPhone, // Changed from phoneNumber to contactPhone
            string expertise, string companyName, string taxNumber,
            int? employeeCount, string departments, string address,
            string companyPhoneNumber, string website, string industry, string companyEmail,
            string managerFirstName, string managerLastName, string managerPhone, string managerEmail,
            string bankName, string bankBranch, string bankIbanNo)
        {
            if (password != confirmPassword)
            {
                ViewBag.Hata = "Şifreler uyuşmuyor.";
                return View("~/Views/Auth/SupervisorRegister.cshtml");
            }

            //var existingUser = _userService.GetUserByUsername(email);
            //if (existingUser != null)
            //{
            //    ViewBag.Hata = "Bu e-posta adresi zaten kullanılıyor.";
            //    return View("~/Views/Auth/SupervisorRegister.cshtml");
            //}

            var company = _companyService.GetCompanyByName(companyName);
            if (company == null)
            {
                company = new Company
                {
                    CompanyName = companyName,
                    TaxNumber = taxNumber,
                    EmployeeCount = employeeCount,
                    Departments = departments,
                    Address = address,
                    PhoneNumber = companyPhoneNumber,
                    Website = website,
                    Industry = industry,
                    Email = companyEmail,
                    ManagerFirstName = managerFirstName,
                    ManagerLastName = managerLastName,
                    ManagerPhone = managerPhone,
                    ManagerEmail = managerEmail,
                    BankName = bankName,
                    BankBranch = bankBranch,
                    BankIbanNo = bankIbanNo
                };
                _companyService.AddCompany(company);
                company = _companyService.GetCompanyByName(companyName); // Get the company again to have the ID
            }

            var user = new User
            {
                UserName = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleID = 3 // Assuming 3 is for Supervisor
            };
            var userId = _userService.AddUser(user);

            var supervisor = new InternshipSupervisor
            {
                UserID = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CompanyId = company.CompanyId,
                ContactPhone = contactPhone, // Use contactPhone here
                Expertise = expertise
            };

            _supervisorService.AddSupervisor(supervisor);

            HttpContext.Session.SetString("Email", supervisor.User.Email);
            HttpContext.Session.SetString("UserName", supervisor.User.FirstName);
            HttpContext.Session.SetString("UserSurname", supervisor.User.LastName);
            HttpContext.Session.SetString("Expertise", supervisor.Expertise ?? string.Empty);
            HttpContext.Session.SetString("Role", "Supervisor");
            HttpContext.Session.SetInt32("SupervisorId", supervisor.SupervisorId);

            return RedirectToAction("SupervisorPage", "Supervisor");
        }

        // --- Registration / Profile Completion ---

        [HttpGet]
        public IActionResult CompleteInternshipInfo()
        {
            var studentId = GetStudentIdFromSession();
            if (studentId == null)
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            // Öğrencinin staj bilgilerinin tam olup olmadığını kontrol et
            var internshipDetails = _internshipOperationsService.GetInternshipByStudentId(studentId.Value);
            if (internshipDetails != null && internshipDetails.CompanyId > 0 && internshipDetails.SupervisorId.HasValue)
            {
                // Bilgiler tam ise öğrenciyi ana sayfasına yönlendir
                return RedirectToAction("StudentPage", "Student");
            }

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
            {
                return RedirectToAction("StudentLogin", "Auth");
            }

            // Store the student ID to pass to the view
            ViewData["StudentId"] = studentId.Value;

            ViewBag.Supervisors = _supervisorService.GetSupervisorsData();

            return View(student);
        }

        [HttpPost]
        public IActionResult CompleteInternshipInfo(
            int studentId,
            string companyName, int SupervisorId,
            DateTime startDate, DateTime endDate, string[] workDays, string internshipTitle)
        {
            var student = _studentService.GetStudentsData().FirstOrDefault(s => s.StudentID == studentId);
            if (student == null)
            {
                ViewBag.Hata = "Öğrenci bulunamadı.";
                return View(student);
            }

            var company = _companyService.GetCompaniesData()
                .FirstOrDefault(c => c.CompanyName == companyName);

            if (company == null)
            {
                ViewBag.Hata = "Girilen iş yeri bulunamadı.";
                return View(student);
            }

            var supervisor = _supervisorService.GetSupervisorsData()
                .FirstOrDefault(s => s.SupervisorId == SupervisorId);

            if (supervisor == null)
            {
                ViewBag.Hata = "Girilen sorumlu personel bulunamadı.";
                return View(student);
            }

            try
            {
                _internshipOperationsService.AddInternship(
                    studentId,
                    supervisor.SupervisorId,
                    company.CompanyId,
                    startDate,
                    endDate,
                    workDays,
                    internshipTitle
                );
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Staj bilgileri kaydedilirken bir hata oluştu: " + ex.Message;
                return View(student);
            }

            return RedirectToAction("StudentPage", "Student");
        }

        private int? GetStudentIdFromSession()
        {
            return HttpContext.Session.GetInt32("StudentID");
        }

        [HttpPost]
        public IActionResult SaveProfile(
            string firstName, string lastName, int academicYear, string nationalID, DateTime birthDate,
            string schoolNumber, string phoneNumber, string address, string department, string email,
            string password)
        {
            var sessionEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(sessionEmail))
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentsData().FirstOrDefault(s => s.Email == sessionEmail);

            int studentId;

            if (student == null)
            {
                var user = new User
                {
                    UserName = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    RoleID = 2
                };
                int userId = _userService.AddUser(user);

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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                };
                studentId = _studentService.AddStudentAndReturnId(student);
            }
            else
            {
                student.User.FirstName = firstName ?? student.User.FirstName;
                student.User.LastName = lastName ?? student.User.LastName;
                student.AcademicYear = academicYear != 0 ? academicYear : student.AcademicYear;
                student.NationalID = nationalID ?? student.NationalID;
                student.BirthDate = birthDate != DateTime.MinValue ? birthDate : student.BirthDate;
                student.SchoolNumber = schoolNumber ?? student.SchoolNumber;
                student.PhoneNumber = phoneNumber ?? student.PhoneNumber;
                student.Address = address ?? student.Address;
                student.Department = department ?? student.Department;
                student.User.Email = email ?? student.User.Email;

                try
                {
                    _studentService.UpdateStudent(student);
                }
                catch (Exception ex)
                {
                    ViewBag.Hata = "Bilgiler kaydedilirken bir hata oluştu: " + ex.Message;
                    return View("Register", student);
                }
                studentId = student.Id;
            }

            var internshipDetails = _internshipOperationsService.GetInternshipByStudentId(studentId);
            if (internshipDetails == null || internshipDetails.CompanyId == 0 || internshipDetails.SupervisorId == null)
            {
                return RedirectToAction("CompleteInternshipInfo", "Auth", new { studentId = studentId });
            }

            return RedirectToAction("StudentPage", "Student");
        }

        // --- Logout ---

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("KonyaTecnicalUnivercityIMEAutomation", "Home");
        }
    }
}
