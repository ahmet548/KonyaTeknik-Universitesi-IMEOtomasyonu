using Microsoft.AspNetCore.Mvc;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using System;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Kernel.Font;
using iText.Kernel.Utils;

namespace KonyaTeknikÜniversitesi_IMEOtomasyonu.Controllers
{
    public class PDFController : Controller
    {
        private readonly DatabaseService _databaseService;

        public PDFController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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

                var workDays = internshipDetail?.WorkDays?.Split('-') ?? new string[0];
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

        [HttpGet("Home/IMESozlesmeFormu")]
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

        [HttpGet("Home/IMEBilgiFormu")]
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

        [HttpGet("Home/IMEDeğerlendirmeFormu")]
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

        [HttpGet("Home/MazeretIzinFormu")]
        public IActionResult GenerateMazeretIzinFormu(int leaveId)
        {
            var email = HttpContext.Session.GetString("Email");
            var leave = _databaseService.GetLeaveDetailsByEmail(email).FirstOrDefault(x => x.LeaveID == leaveId);

            if (leave == null)
                return NotFound("Mazeret bulunamadı.");

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "MazeretIzinFormu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge6.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";

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
                form.GetField("totalLeaveDays")?.SetValue(((leave.LeaveEnd - leave.LeaveStart).Days + 1).ToString() + " Gün").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("leaveDate")?.SetValue($"{leave.LeaveStart:dd.MM.yyyy} - {leave.LeaveEnd:dd.MM.yyyy}").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("explain")?.SetValue(leave.ReasonDetail ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                form.GetField("address")?.SetValue(leave.AddressDuringLeave ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);

                // LeaveReason checkboxlarını işaretle
                var reasons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Alttan kalan derslerin vize ve final sınavları", "leave1" },
            { "ÖSYM Sınavları", "leave2" },
            { "Hastalık ve sağlık problemleri", "leave3" },
            { "Ailevi sebepler", "leave4" },
            { "Diğer Mazeretler", "leave5" }
        };

                foreach (var kvp in reasons)
                {
                    form.GetField(kvp.Value)?.SetValue(
                        string.Equals(leave.LeaveReason, kvp.Key, StringComparison.OrdinalIgnoreCase) ? "Yes" : "Off"
                    );
                }

                form.FlattenFields();
            }

            var fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/pdf", "MazeretIzinFormu.pdf");
        }

        [HttpGet("Home/FaaliyetRaporu")]
        public IActionResult GenerateFaaliyetRaporu()
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "FaaliyetRaporu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge8.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var emailStudent = HttpContext.Session.GetString("Email");
            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(emailStudent);

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";
            var department = HttpContext.Session.GetString("Department") ?? "-";
            var schoolNumber = HttpContext.Session.GetString("SchoolNumber") ?? "-";

            var notes = _databaseService.GetUserNotes(emailStudent);

            // Notları parçalara böl (ör: her sayfada 3500 karakter olsun)
            int charsPerPage = 3500;
            var noteTexts = notes.Select(note =>
                $"Başlık: {note.Title}\nAlt Başlık: {note.SubTitle}\nTarih: {note.CreatedAt:dd.MM.yyyy}\nİçerik: {note.Content}\n"
            ).ToList();

            string allNotes = string.Join("\n----------------------\n", noteTexts);
            var pages = new List<string>();
            for (int i = 0; i < allNotes.Length; i += charsPerPage)
                pages.Add(allNotes.Substring(i, Math.Min(charsPerPage, allNotes.Length - i)));

            using (var ms = new MemoryStream())
            {
                PdfDocument pdfDoc = null;
                PdfAcroForm form = null;
                for (int pageNo = 0; pageNo < pages.Count; pageNo++)
                {
                    using (var pdfReader = new PdfReader(templatePath))
                    using (var pdfWriter = pageNo == 0 ? new PdfWriter(ms) : new PdfWriter(new MemoryStream()))
                    {
                        pdfDoc = pageNo == 0
                            ? new PdfDocument(pdfReader, pdfWriter)
                            : new PdfDocument(pdfReader, pdfWriter);

                        form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                        var timesNewRomanFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                        var timesNewRomanFont = PdfFontFactory.CreateFont(timesNewRomanFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                        // Diğer alanlar sadece ilk sayfada doldurulsun
                        if (pageNo == 0)
                        {
                            form.GetField("titleDepartment")?.SetValue(department).SetFont(timesNewRomanFont).SetFontSize(18);
                            form.GetField("titleSchoolNo")?.SetValue(schoolNumber).SetFont(timesNewRomanFont).SetFontSize(18);
                            form.GetField("titleFullName")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(18);
                            form.GetField("department")?.SetValue(department).SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("fullName")?.SetValue($"{userName} {userSurname}").SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("schoolNo")?.SetValue(schoolNumber).SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("totalTrainingDays")?.SetValue(internshipDetail?.TotalTrainingDays.ToString() ?? "0").SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("totalTrainingDays2")?.SetValue(internshipDetail?.TotalTrainingDays.ToString() ?? "0").SetFont(timesNewRomanFont).SetFontSize(10);
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
                            form.GetField("companyName2")?.SetValue(company?.CompanyName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("companyNameAddress")?.SetValue($"{company?.CompanyName ?? "-"} - {company?.Address ?? "-"}").SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("name")?.SetValue(supervisor?.FirstName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("lastName")?.SetValue(supervisor?.LastName ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                            form.GetField("expertise")?.SetValue(supervisor?.Expertise ?? "-").SetFont(timesNewRomanFont).SetFontSize(10);
                        }

                        // internPage ve pageNo alanlarını doldur
                        form.GetField("internPage")?.SetValue(pages[pageNo]).SetFont(timesNewRomanFont).SetFontSize(10);
                        form.GetField("pageNo")?.SetValue((pageNo + 1).ToString()).SetFont(timesNewRomanFont).SetFontSize(10);

                        form.FlattenFields();

                        pdfDoc.Close();

                        // Sonraki sayfalar için ms'e ekle
                        if (pageNo > 0)
                        {
                            var tempBytes = ((MemoryStream)pdfWriter.GetOutputStream()).ToArray();
                            var tempDoc = new PdfDocument(new PdfReader(new MemoryStream(tempBytes)), new PdfWriter(ms));
                            tempDoc.Close();
                        }
                    }
                }

                var fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf", "FaaliyetRaporu.pdf");
            }
        }

        [HttpGet("Home/VideoSunumFormu")]
        public IActionResult GenerateVideoSunumFormu()
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "VideoSunumFormu.pdf");
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs", "Belge9.pdf");

            if (!System.IO.File.Exists(templatePath))
                return NotFound("PDF şablonu bulunamadı.");

            var emailStudent = HttpContext.Session.GetString("Email");
            var (supervisor, company, internshipDetail, evaluationPersonel) = _databaseService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(emailStudent);

            var userName = HttpContext.Session.GetString("UserName") ?? "-";
            var userSurname = HttpContext.Session.GetString("UserSurname") ?? "-";
            var schoolNumber = HttpContext.Session.GetString("SchoolNumber") ?? "-";

            var videos = _databaseService.GetUserVideos(emailStudent);
            var uploadedDates = videos.Select(v => v.UploadDate.Date).ToList();

            var startDate = internshipDetail?.StartDate.Date ?? DateTime.Now.Date;
            var endDate = internshipDetail?.EndDate.Date ?? startDate.AddDays(7 * 14);

            using (var pdfReader = new PdfReader(templatePath))
            using (var pdfWriter = new PdfWriter(outputPath))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                if (form == null)
                    return BadRequest("PDF form alanları bulunamadı.");

                var timesNewRomanFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                var timesNewRomanFont = PdfFontFactory.CreateFont(timesNewRomanFontPath, iText.IO.Font.PdfEncodings.IDENTITY_H);

                form.GetField("schoolNo")?.SetValue(schoolNumber);
                form.GetField("fullName")?.SetValue($"{userName} {userSurname}");

                int totalUploadedWeeks = 0;

                for (int i = 0; i < 14; i++)
                {
                    var weekStart = startDate.AddDays(i * 7);
                    var weekEnd = weekStart.AddDays(7);

                    if (weekStart > endDate)
                    {
                        form.GetField($"week{i + 1}_done")?.SetValue("Off");
                        form.GetField($"week{i + 1}_notdone")?.SetValue("Off");
                        continue;
                    }

                    bool videoUploaded = uploadedDates.Any(d => d >= weekStart && d < weekEnd && d <= endDate);

                    if (videoUploaded)
                        totalUploadedWeeks++;

                    form.GetField($"week{i + 1}_done")?.SetValue(videoUploaded ? "Yes" : "Off");
                    form.GetField($"week{i + 1}_notdone")?.SetValue(videoUploaded ? "Off" : "Yes");
                }

                form.GetField("totalVideos")?.SetValue(totalUploadedWeeks.ToString() + " / 14");

                form.FlattenFields();
            }

            var fileBytes = System.IO.File.ReadAllBytes(outputPath);
            return File(fileBytes, "application/pdf", "VideoSunumFormu.pdf");
        }
    }
}