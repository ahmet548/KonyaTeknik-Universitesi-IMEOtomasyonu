using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.Json;
using System.Globalization;
using KonyaTeknikÜniversitesi_IMEOtomasyonu.Models; // Add this using statement

namespace IMEAutomationDBOperations.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentDashboardService _studentDashboardService;
        private readonly InternshipOperationsService _internshipOperationsService;
        private readonly StudentService _studentService; // AddNotePage/AddVideoPage'deki header için gerekli

        public StudentController(
            StudentDashboardService studentDashboardService,
            InternshipOperationsService internshipOperationsService,
            StudentService studentService)
        {
            _studentDashboardService = studentDashboardService ?? throw new ArgumentNullException(nameof(studentDashboardService));
            _internshipOperationsService = internshipOperationsService ?? throw new ArgumentNullException(nameof(internshipOperationsService));
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        }

        public IActionResult StudentPage()
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;
            var userName = student.FirstName;
            var userSurname = student.LastName;

            var (supervisor, company, internshipDetail, evaluationPersonel) = _internshipOperationsService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(email);

            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.Department = student.Department ?? "Bölüm Bilgisi Yok";
            ViewBag.Company = company as Company; ViewBag.Supervisor = supervisor;
            ViewBag.InternshipTitle = internshipDetail?.InternshipTitle ?? "Staj Defteri";

            var statistics = CalculateStatistics(email);
            var videos = _studentDashboardService.GetUserVideos(email);
            var videoUploadDays = videos.Select(v => v.UploadDate.ToString("yyyy-MM-dd")).ToList();
            var videoTitles = videos.ToDictionary(
                v => v.UploadDate.ToString("yyyy-MM-dd"),
                v => v.Title
            );

            var notes = _studentDashboardService.GetUserNotes(email);
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

            var leaveDetails = _studentDashboardService.GetLeaveDetailsByEmail(email);
            var excuseDaysList = leaveDetails?
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

            ViewBag.VideoUploadDays = videoUploadDays;
            ViewBag.VideoTitles = videoTitles;
            ViewBag.UserNotes = userNotesDict;
            ViewBag.ExcuseDays = excuseDaysList; // 'excuseDays' zaten CalculateStatistics içinde kullanılıyor, isim çakışmasını önle
            ViewBag.Notes = notes;
            ViewBag.Videos = videos;
            ViewBag.LeaveDetails = leaveDetails;

            ViewBag.UserNotesJson = JsonSerializer.Serialize(userNotesDict);
            ViewBag.VideoUploadDaysJson = JsonSerializer.Serialize(videoUploadDays);
            ViewBag.VideoTitlesJson = JsonSerializer.Serialize(videoTitles);
            ViewBag.ExcuseDaysJson = JsonSerializer.Serialize(excuseDaysList);
            ViewBag.CompletionRate = Math.Round(statistics.completionRate, 2)
                                .ToString(CultureInfo.InvariantCulture);
            ViewBag.TotalDays = statistics.totalDays;
            ViewBag.FilledDays = statistics.filledDays;
            ViewBag.VideoDays = statistics.videoDays;
            ViewBag.ExcuseDaysCount = statistics.excuseDays; // İsim çakışmasını önle

            ViewBag.Email = email;

            return View();
        }

        public IActionResult SuccessNotes()
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            ViewBag.UserName = student.FirstName;
            ViewBag.UserSurname = student.LastName;

            // Retrieve evaluation data
            var (supervisor, company, internshipDetail, evaluationPersonel) = _internshipOperationsService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(email);

            // Initialize scores
            decimal imeSorumlusuDegerlendirmeNotu = 0;
            decimal imeOgretimElemaniDegerlendirmeNotu = 0;
            decimal haftalikVideoSunumNotu = 0;
            decimal bolumImeKomisyonuNotu = 0;

            if (evaluationPersonel != null)
            {
                // Calculate IME Sorumlusu Değerlendirme Formu Genel Notu
                decimal totalScore = (evaluationPersonel.AttendanceScore +
                                      evaluationPersonel.ResponsibilityScore +
                                      evaluationPersonel.KnowledgeScore +
                                      evaluationPersonel.ProblemSolvingScore +
                                      evaluationPersonel.EquipmentUsageScore +
                                      evaluationPersonel.CommunicationScore +
                                      evaluationPersonel.MotivationScore +
                                      evaluationPersonel.ReportingScore +
                                      evaluationPersonel.TeamworkScore +
                                      evaluationPersonel.ExpressionScore);
                imeSorumlusuDegerlendirmeNotu = totalScore; // Max score is 10 * 10 = 100, so no further normalization needed.
            }

            // TODO: Fetch actual values for these scores from the database or other services.
            // For now, using placeholder values.
            // imeOgretimElemaniDegerlendirmeNotu = ...;
            // haftalikVideoSunumNotu = ...;
            // bolumImeKomisyonuNotu = ...;

            var viewModel = new SuccessNotesViewModel
            {
                ImeSorumlusuDegerlendirmeNotu = imeSorumlusuDegerlendirmeNotu,
                ImeOgretimElemaniDegerlendirmeNotu = imeOgretimElemaniDegerlendirmeNotu,
                HaftalikVideoSunumNotu = haftalikVideoSunumNotu,
                BolumImeKomisyonuNotu = bolumImeKomisyonuNotu,
                SupervisorEvaluation = evaluationPersonel,
                InstructorFeedback = internshipDetail?.InstructorFeedback,
                CommissionFeedback = internshipDetail?.CommissionFeedback
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetCalendarData()
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return Unauthorized();

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return Unauthorized();

            var email = student.Email;

            try
            {
                var videos = _studentDashboardService.GetUserVideos(email);
                var videoUploadDays = videos.Select(v => v.UploadDate.ToString("yyyy-MM-dd")).ToList();
                var videoTitles = videos.ToDictionary(
                    v => v.UploadDate.ToString("yyyy-MM-dd"),
                    v => v.Title
                );

                var notes = _studentDashboardService.GetUserNotes(email);
                var userNotes = notes
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

                var leaveDetails = _studentDashboardService.GetLeaveDetailsByEmail(email);
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

                var calendarData = new
                {
                    userNotes = userNotes,
                    videoUploadDays = videoUploadDays,
                    videoTitles = videoTitles,
                    excuseDays = excuseDays
                };

                return Ok(calendarData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Veriler alınırken hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddNote(string Title, string SubTitle, string Content)
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            var note = new Note
            {
                Title = Title ?? string.Empty,
                SubTitle = SubTitle ?? string.Empty,
                Content = Content ?? string.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _studentDashboardService.AddUserNote(email, note);

            return RedirectToAction("StudentPage");
        }

        [HttpGet]
        public IActionResult DeleteNote(int id)
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            _studentDashboardService.DeleteUserNote(email, id);
            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult UploadVideo(IFormFile videoFile, string Title, string Description)
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
            {
                TempData["UploadError"] = "Video uploads are only allowed on Mondays.";
                return RedirectToAction("StudentPage");
            }

            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            if (videoFile == null || videoFile.Length == 0)
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

            _studentDashboardService.AddUserVideo(email, video);

            return RedirectToAction("StudentPage");
        }

        [HttpGet]
        public IActionResult AddNotePage(int? noteId)
        {
            try
            {
                var studentId = HttpContext.Session.GetInt32("StudentID");
                if (studentId == null)
                    return RedirectToAction("StudentLogin", "Auth");

                var student = _studentService.GetStudentById(studentId.Value);
                if (student == null)
                    return RedirectToAction("StudentLogin", "Auth");

                var email = student.Email;
                var (supervisor, company, internshipDetail, evaluationPersonel) = _internshipOperationsService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(email);

                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserSurname = HttpContext.Session.GetString("UserSurname");
                ViewBag.Department = student?.Department ?? "Bölüm Bilgisi Yok";
                ViewBag.Company = company as Company;
                ViewBag.Supervisor = supervisor;
                ViewBag.InternshipTitle = internshipDetail?.InternshipTitle ?? "Staj Defteri";

                Note currentNote = null;
                if (noteId.HasValue)
                {
                    currentNote = _studentDashboardService.GetUserNotes(email).FirstOrDefault(n => n.NoteID == noteId.Value);
                }

                ViewBag.CurrentNote = currentNote;
                ViewBag.IsEditMode = noteId.HasValue && currentNote != null;
                ViewBag.CurrentDate = currentNote?.CreatedAt ?? DateTime.Now;

                var notes = _studentDashboardService.GetUserNotes(email).ToList();
                ViewBag.Notes = notes;

                return View("addnote");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddNotePage ERROR: {ex.Message}");
                return RedirectToAction("StudentPage");
            }
        }

        [HttpGet]
        public IActionResult AddVideoPage(int? noteId)
        {
            try
            {
                var studentId = HttpContext.Session.GetInt32("StudentID");
                if (studentId == null)
                    return RedirectToAction("StudentLogin", "Auth");

                var student = _studentService.GetStudentById(studentId.Value);
                if (student == null)
                    return RedirectToAction("StudentLogin", "Auth");

                var email = student.Email;
                var (supervisor, company, internshipDetail, evaluationPersonel) = _internshipOperationsService.GetSupervisorCompanyAndInternshipDetailsByStudentEmail(email);

                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                ViewBag.UserSurname = HttpContext.Session.GetString("UserSurname");
                ViewBag.Department = student?.Department ?? "Bölüm Bilgisi Yok";
                ViewBag.Company = company;
                ViewBag.Supervisor = supervisor;
                ViewBag.InternshipTitle = internshipDetail?.InternshipTitle ?? "Staj Defteri";

                Note currentNote = null;
                if (noteId.HasValue)
                {
                    currentNote = _studentDashboardService.GetUserNotes(email).FirstOrDefault(n => n.NoteID == noteId.Value);
                }

                ViewBag.CurrentNote = currentNote;
                ViewBag.IsEditMode = noteId.HasValue && currentNote != null;
                ViewBag.CurrentDate = currentNote?.CreatedAt ?? DateTime.Now;

                var notes = _studentDashboardService.GetUserNotes(email).ToList();
                ViewBag.Notes = notes;

                return View("addvideo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddVideoPage ERROR: {ex.Message}");
                return RedirectToAction("StudentPage");
            }
        }

        [HttpGet]
        public IActionResult DeleteVideo(int id)
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            var video = _studentDashboardService.GetUserVideos(email).FirstOrDefault(v => v.VideoID == id);
            if (video == null)
                return NotFound();

            _studentDashboardService.DeleteUserVideo(email, id);
            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult EditNote(int NoteID, string Title, string SubTitle, string Content)
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;
            _studentDashboardService.UpdateUserNote(email, NoteID, Title, SubTitle, Content);
            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult SubmitExcuse(DateTime LeaveStart, DateTime LeaveEnd, string LeaveReason, string ReasonDetail, string AddressDuringLeave)
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            _studentDashboardService.AddLeaveDetail(email, LeaveStart, LeaveEnd, LeaveReason, ReasonDetail, AddressDuringLeave);

            return RedirectToAction("StudentPage");
        }

        [HttpPost]
        public IActionResult DeleteLeave(int LeaveID)
        {
            var studentId = HttpContext.Session.GetInt32("StudentID");
            if (studentId == null)
                return RedirectToAction("StudentLogin", "Auth");

            var student = _studentService.GetStudentById(studentId.Value);
            if (student == null)
                return RedirectToAction("StudentLogin", "Auth");

            var email = student.Email;

            _studentDashboardService.DeleteLeaveDetail(email, LeaveID);
            return RedirectToAction("StudentPage");
        }

        private (int totalDays, int filledDays, int videoDays, int excuseDays, decimal completionRate) CalculateStatistics(string email)
        {
            var statisticsData = _studentDashboardService.GetStatisticsDataByEmail(email);

            DateTime startDate = statisticsData.InternshipDetails?.StartDate ?? DateTime.MinValue;
            DateTime endDate = statisticsData.InternshipDetails?.EndDate ?? DateTime.MaxValue;

            int totalDays = (endDate - startDate).Days + 1;
            int videoDays = statisticsData.Videos?.Count ?? 0;
            int filledDays = statisticsData.FilledDaysCount;

            int excuseDays = statisticsData.LeaveDetails?
                .Where(l => l.LeaveStatus == "Onaylandı" || l.LeaveStatus == "Onay Bekliyor")
                .SelectMany(l => Enumerable.Range(0, (l.LeaveEnd - l.LeaveStart).Days + 1)
                    .Select(offset => l.LeaveStart.AddDays(offset).ToString("yyyy-MM-dd")))
                .Distinct()
                .Count() ?? 0;

            decimal completionRate = totalDays > 0 ? Math.Round((decimal)filledDays / totalDays * 100, 2) : 0;

            return (totalDays, filledDays, videoDays, excuseDays, completionRate);
        }

    }
}