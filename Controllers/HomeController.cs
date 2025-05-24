using Microsoft.AspNetCore.Mvc;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using System;
using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Font;

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

            // Ad ve soyadı ayırma
            var nameParts = fullName.Split(' ', 2);
            student.FirstName = nameParts.Length > 0 ? nameParts[0] : student.FirstName;
            student.LastName = nameParts.Length > 1 ? nameParts[1] : student.LastName;

            student.AcademicYear = academicYear;
            student.Department = department;
            student.SchoolNumber = schoolNo;
            student.PhoneNumber = phoneNumber;
            student.Email = email;
            student.Address = address;

            // Veritabanını güncelle
            _databaseService.UpdateStudent(student);

            // Oturum bilgilerini güncelle
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
                return RedirectToAction("StudentPage", "Home");
            }

            ViewBag.Hata = "Hatalı kullanıcı adı veya şifre.";
            return View();
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
                return RedirectToAction("SupervisorPage", "Home");
            }

            ViewBag.Hata = "Hatalı personel adı veya şifre.";
            return View();
        }

        public IActionResult StudentPage()
        {
            var email = HttpContext.Session.GetString("Email");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Home");

            ViewBag.Email = email;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            return View();
        }

        public IActionResult SupervisorPage()
        {
            var email = HttpContext.Session.GetString("Email");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("StudentLogin", "Home");

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
                return RedirectToAction("SupervisorLogin", "Home");
            }

            var students = _databaseService.GetStudentsBySupervisorEmail(supervisorEmail);
            return View(students);
        }

        public IActionResult IsletmedeMeslekiEgitimSozlesmesi()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge2.pdf");
            if (!System.IO.File.Exists(filePath))
                return NotFound("PDF dosyası bulunamadı.");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/pdf");
        }

        [HttpGet("Home/IMEBaşvuruFormu")]
        public IActionResult GenerateIMEBasvuruFormu()
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "IMEBaşvuruFormu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge1.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var emailStudent = HttpContext.Session.GetString("Email");
            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(emailStudent);

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";
            var email = HttpContext.Session.GetString("Email") ?? "-";
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber") ?? "-";
            var address = HttpContext.Session.GetString("Address") ?? "-";
            var nationalID = HttpContext.Session.GetString("NationalID") ?? "-";
            var academicYear = HttpContext.Session.GetInt32("AcademicYear") ?? 0;
            var schoolNumber = HttpContext.Session.GetString("SchoolNumber") ?? "-";

            using (var pdfReader = new PdfReader(templatePath))
            using (var pdfWriter = new PdfWriter(outputPath))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                if (form == null)
                    return BadRequest("PDF form alanları bulunamadı.");

                var timesNewRomanFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                var timesNewRomanFont = PdfFontFactory.CreateFont(timesNewRomanFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                form.GetField("fullName")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("email")?.SetValue(email).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("phoneNo")?.SetValue(phoneNumber).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("address")?.SetValue(address).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("tcNo")?.SetValue(nationalID).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("academicYear")?.SetValue($"{academicYear}.Sınıf").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("schoolNo")?.SetValue(schoolNumber).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("totalTrainingDays")?.SetValue(internshipDetail?.TotalTrainingDays.ToString() ?? "0").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("dateRange")?.SetValue(
                    internshipDetail?.StartDate != null && internshipDetail?.EndDate != null
                        ? $"{internshipDetail.StartDate:dd/MM/yyyy} - {internshipDetail.EndDate:dd/MM/yyyy}"
                        : "N/A"
                ).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("companyName")?.SetValue(company?.CompanyName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("companyAddress")?.SetValue(company?.Address ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("industry")?.SetValue(company?.Industry ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("supervisorFullName")?.SetValue($"{supervisor?.FirstName ?? "-"} {supervisor?.LastName ?? "-"}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("supervisorPhoneNo")?.SetValue(supervisor?.ContactPhone?.ToString() ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("ManagerFullName")?.SetValue($"{company?.ManagerFirstName ?? "-"} {company?.ManagerLastName ?? "-"}").SetFont(timesNewRomanFont).SetFontSize(10);

                var workDays = internshipDetail?.WorkDays?.Split(',') ?? new string[0];
                form.GetField("Mndy")?.SetValue(workDays.Contains("Pazartesi") ? "Yes" : "Off");
                form.GetField("Tusdy")?.SetValue(workDays.Contains("Salı") ? "Yes" : "Off");
                form.GetField("Wnsdy")?.SetValue(workDays.Contains("Çarşamba") ? "Yes" : "Off");
                form.GetField("Thrsdy")?.SetValue(workDays.Contains("Perşembe") ? "Yes" : "Off");
                form.GetField("Fridy")?.SetValue(workDays.Contains("Cuma") ? "Yes" : "Off");
                form.GetField("Strday")?.SetValue(workDays.Contains("Cumartesi") ? "Yes" : "Off");

                var expertise = string.IsNullOrEmpty(supervisor?.Expertise) ? "Branş5" : supervisor.Expertise;
                form.GetField("expertise1")?.SetValue(expertise == "Bilgisayar Mühendisliği" ? "Yes" : "Off");
                form.GetField("expertise2")?.SetValue(expertise == "Yazılım Mühendisliği" ? "Yes" : "Off");
                form.GetField("expertise3")?.SetValue(expertise == "Yapay Zekâ Mühendisliği" ? "Yes" : "Off");
                form.GetField("expertise4")?.SetValue(expertise == "Elektrik Elektronik Mühendisliği" ? "Yes" : "Off");
                form.GetField("expertise5")?.SetValue(expertise != "" ? "Yes" : "Off");

                var department = string.IsNullOrEmpty(company?.Departments) ? "Branş5" : company.Departments;
                form.GetField("department1")?.SetValue(department == "Bilgisayar Mühendisliği" ? "Yes" : "Off");
                form.GetField("department2")?.SetValue(department == "Yazılım Mühendisliği" ? "Yes" : "Off");
                form.GetField("department3")?.SetValue(department == "Yapay Zekâ Mühendisliği" ? "Yes" : "Off");
                form.GetField("department4")?.SetValue(department == "Elektrik Elektronik Mühendisliği" ? "Yes" : "Off");
                form.GetField("department5")?.SetValue(department != "" ? "Yes" : "Off");

                form.FlattenFields();
            }

            var fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/pdf", "IMEBaşvuruFormu.pdf");
        }

        [HttpGet("Home/GenerateIMESozlesmeFormu")]
        public IActionResult GenerateIMESozlesmeFormu()
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "IMESozlesmeFormu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge2.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var emailStudent = HttpContext.Session.GetString("Email");
            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(emailStudent);

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber") ?? "-";

            using (var pdfReader = new PdfReader(templatePath))
            using (var pdfWriter = new PdfWriter(outputPath))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                if (form == null)
                    return BadRequest("PDF form alanları bulunamadı.");

                var timesNewRomanFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                var timesNewRomanFont = PdfFontFactory.CreateFont(timesNewRomanFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                form.GetField("fullName")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("phoneNo")?.SetValue(phoneNumber).SetFont(timesNewRomanFont).SetFontSize(10);

                form.GetField("managerFullName")?.SetValue($"{company?.ManagerFirstName} {company?.ManagerLastName}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("departments")?.SetValue(company?.Departments).SetFont(timesNewRomanFont).SetFontSize(10);

                form.FlattenFields();
            }

            var fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/pdf", "IMESozlesmeFormu.pdf");
        }

        [HttpGet("Home/GenerateIMEBilgiFormu")]
        public IActionResult GenerateIMEBilgiFormu()
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "GenerateIMEBilgiFormu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge3.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var emailStudent = HttpContext.Session.GetString("Email");
            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(emailStudent);

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";
            var email = HttpContext.Session.GetString("Email") ?? "-";
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber") ?? "-";
            var address = HttpContext.Session.GetString("Address") ?? "-";
            var nationalID = HttpContext.Session.GetString("NationalID") ?? "-";
            var academicYear = HttpContext.Session.GetInt32("AcademicYear") ?? 0;
            var schoolNumber = HttpContext.Session.GetString("SchoolNumber") ?? "-";
            var birthDate = HttpContext.Session.GetString("BirthDate");

            if (string.IsNullOrEmpty(birthDate))
            {
                Console.WriteLine("BirthDate oturumda bulunamadı veya boş.");
            }
            else
            {
                Console.WriteLine($"BirthDate oturumdan alındı: {birthDate}");
            }

            var department = HttpContext.Session.GetString("Department") ?? "-";

            using (var pdfReader = new PdfReader(templatePath))
            using (var pdfWriter = new PdfWriter(outputPath))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                if (form == null)
                    return BadRequest("PDF form alanları bulunamadı.");

                var timesNewRomanFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                var timesNewRomanFont = PdfFontFactory.CreateFont(timesNewRomanFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                var departmentsInCompany = (company?.Departments ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim()).ToList();

                form.GetField("Unit1")?.SetValue(departmentsInCompany.Contains("Ar-Ge") ? "Yes" : "Off");
                form.GetField("Unit2")?.SetValue(departmentsInCompany.Contains("Kalite Kontrol") ? "Yes" : "Off");
                form.GetField("Unit3")?.SetValue(departmentsInCompany.Contains("İmalat") ? "Yes" : "Off");
                form.GetField("Unit4")?.SetValue(departmentsInCompany.Contains("İnsan Kaynakları") ? "Yes" : "Off");


                var knownDepartments = new[] { "Ar-Ge", "Kalite Kontrol", "İmalat", "İnsan Kaynakları" };
                var otherDepartments = departmentsInCompany.Where(d => !knownDepartments.Contains(d)).ToList();
                form.GetField("otherUnit")?.SetValue(string.Join(", ", otherDepartments)).SetFont(timesNewRomanFont).SetFontSize(10);

                form.GetField("fullName1")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("fullName2")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("email")?.SetValue(email).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("phoneNo")?.SetValue(phoneNumber).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("birthDate")?.SetValue(
                    birthDate != null
                        ? birthDate
                        : "N/A"
                ).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("department")?.SetValue(department).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("address")?.SetValue(address).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("tcNo")?.SetValue(nationalID).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("academicYear")?.SetValue($"{academicYear}.Sınıf").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("schoolNo")?.SetValue(schoolNumber).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("totalTrainingDays")?.SetValue(internshipDetail?.TotalTrainingDays.ToString() ?? "0").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("startDate")?.SetValue(
                    internshipDetail?.StartDate != null
                        ? internshipDetail.StartDate.ToString("dd/MM/yyyy")
                        : "N/A"
                ).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("endDate")?.SetValue(
                    internshipDetail?.EndDate != null
                        ? internshipDetail.EndDate.ToString("dd/MM/yyyy")
                        : "N/A"
                ).SetFont(timesNewRomanFont).SetFontSize(10);

                form.GetField("companyName")?.SetValue(company?.CompanyName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("taxNo")?.SetValue(company?.TaxNumber ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("employeeCount")?.SetValue(company?.EmployeeCount.ToString() ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("companyPhoneNo")?.SetValue(company?.PhoneNumber ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("companyWebAddress")?.SetValue(company?.Website ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("companyEmail")?.SetValue(company?.Email ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("companyAddress")?.SetValue(company?.Address ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("managerFullName1")?.SetValue($"{company?.ManagerFirstName ?? "-"} {company?.ManagerLastName ?? "-"}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("managerFullName2")?.SetValue($"{company?.ManagerFirstName ?? "-"} {company?.ManagerLastName ?? "-"}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("managerPhoneNo")?.SetValue(company?.ManagerPhone ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("managerEmail")?.SetValue(company?.ManagerEmail ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("bankName")?.SetValue(company?.BankName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("bankBranch")?.SetValue(company?.BankBranch ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("bankIbanNo")?.SetValue(company?.BankIbanNo ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("paitAmount")?.SetValue(internshipDetail?.PaidAmount.ToString() ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);

                form.FlattenFields();
            }

            var fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/pdf", "GenerateIMEBilgiFormu.pdf");
        }

        [HttpGet("Home/GenerateIMEDeğerlendirmeFormu")]
        public IActionResult GenerateIMEDeğerlendirmeFormu()
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "IMEDeğerlendirmeFormu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge4.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var emailStudent = HttpContext.Session.GetString("Email");
            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(emailStudent);

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";
            var email = HttpContext.Session.GetString("Email") ?? "-";
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber") ?? "-";
            var address = HttpContext.Session.GetString("Address") ?? "-";
            var nationalID = HttpContext.Session.GetString("NationalID") ?? "-";
            var academicYear = HttpContext.Session.GetInt32("AcademicYear") ?? 0;
            var schoolNumber = HttpContext.Session.GetString("SchoolNumber") ?? "-";

            using (var pdfReader = new PdfReader(templatePath))
            using (var pdfWriter = new PdfWriter(outputPath))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                if (form == null)
                    return BadRequest("PDF form alanları bulunamadı.");

                var timesNewRomanFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                var timesNewRomanFont = PdfFontFactory.CreateFont(timesNewRomanFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                form.GetField("fullName")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("schoolNo")?.SetValue(schoolNumber).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("dateRange")?.SetValue(
                    internshipDetail?.StartDate != null && internshipDetail?.EndDate != null
                        ? $"{internshipDetail.StartDate:dd/MM/yyyy} - {internshipDetail.EndDate:dd/MM/yyyy}"
                        : "N/A"
                ).SetFont(timesNewRomanFont).SetFontSize(10);

                int totalPoint = (evaluationPersonel?.AttendanceScore ?? 0) +
                 (evaluationPersonel?.ResponsibilityScore ?? 0) +
                 (evaluationPersonel?.KnowledgeScore ?? 0) +
                 (evaluationPersonel?.ProblemSolvingScore ?? 0) +
                 (evaluationPersonel?.EquipmentUsageScore ?? 0) +
                 (evaluationPersonel?.CommunicationScore ?? 0) +
                 (evaluationPersonel?.MotivationScore ?? 0) +
                 (evaluationPersonel?.ReportingScore ?? 0) +
                 (evaluationPersonel?.TeamworkScore ?? 0) +
                 (evaluationPersonel?.ExpressionScore ?? 0);

                form.GetField("companyName")?.SetValue(company?.CompanyName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("managerFullName")?.SetValue($"{company?.ManagerFirstName ?? "-"} {company?.ManagerLastName ?? "-"}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point1")?.SetValue((evaluationPersonel?.AttendanceScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point2")?.SetValue((evaluationPersonel?.ResponsibilityScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point3")?.SetValue((evaluationPersonel?.KnowledgeScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point4")?.SetValue((evaluationPersonel?.ProblemSolvingScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point5")?.SetValue((evaluationPersonel?.EquipmentUsageScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point6")?.SetValue((evaluationPersonel?.CommunicationScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point7")?.SetValue((evaluationPersonel?.MotivationScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point8")?.SetValue((evaluationPersonel?.ReportingScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point9")?.SetValue((evaluationPersonel?.TeamworkScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("point10")?.SetValue((evaluationPersonel?.ExpressionScore.ToString() + " / 10" ?? "-" + " / 10").ToString()).SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("totalPoint")?.SetValue(totalPoint.ToString() + " / 100").SetFont(timesNewRomanFont).SetFontSize(10);
                form.FlattenFields();
            }

            var fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/pdf", "IMEDeğerlendirmeFormu.pdf");
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
