using System;
using System.Collections.Generic;
using System.Linq;
using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Services
{
    public class StudentDashboardService
    {
        private readonly ApplicationDbContext _context;

        public StudentDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Note> GetUserNotes(string email)
        {
            return _context.Notes
                .Include(n => n.Student).ThenInclude(s => s!.User)
                .Where(n => n.Student != null && n.Student.User != null && n.Student.User.Email == email)
                .ToList();
        }

        public List<Video> GetUserVideos(string email)
        {
            return _context.Videos
                .Include(v => v.Student).ThenInclude(s => s!.User)
                .Where(v => v.Student != null && v.Student.User != null && v.Student.User.Email == email)
                .ToList();
        }

        public Note? GetNoteById(int noteId)
        {
            return _context.Notes.FirstOrDefault(n => n.NoteID == noteId);
        }

        public List<LeaveDetails> GetLeaveDetailsByEmail(string email)
        {
            return _context.LeaveDetails
                .Include(l => l.Student).ThenInclude(s => s!.User)
                .Where(l => l.Student != null && l.Student.User != null && l.Student.User.Email == email)
                .ToList();
        }

        public void AddUserNote(string email, Note note)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.User != null && s.User.Email == email);
            if (student == null) throw new InvalidOperationException($"No student found with email: {email}");

            note.StudentID = student.Id;
            _context.Notes.Add(note);
            _context.SaveChanges();
        }

        public void DeleteUserNote(string email, int noteId)
        {
            var note = _context.Notes.Include(n => n.Student).ThenInclude(s => s!.User)
                .FirstOrDefault(n => n.NoteID == noteId && n.Student != null && n.Student.User != null && n.Student.User.Email == email);
            if (note != null)
            {
                _context.Notes.Remove(note);
                _context.SaveChanges();
            }
        }

        public void UpdateUserNote(string email, int noteId, string title, string subTitle, string content)
        {
            var note = _context.Notes.Include(n => n.Student).ThenInclude(s => s!.User)
                .FirstOrDefault(n => n.NoteID == noteId && n.Student != null && n.Student.User != null && n.Student.User.Email == email);
            if (note != null)
            {
                note.Title = title;
                note.SubTitle = subTitle;
                note.Content = content;
                note.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void AddUserVideo(string email, Video video)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.User != null && s.User.Email == email);
            if (student == null) throw new InvalidOperationException($"No student found with email: {email}");

            video.StudentID = student.Id;
            _context.Videos.Add(video);
            _context.SaveChanges();
        }

        public void DeleteUserVideo(string email, int videoId)
        {
            var video = _context.Videos.Include(v => v.Student).ThenInclude(s => s!.User)
                .FirstOrDefault(v => v.VideoID == videoId && v.Student != null && v.Student.User != null && v.Student.User.Email == email);
            if (video != null)
            {
                _context.Videos.Remove(video);
                _context.SaveChanges();
            }
        }

        public void AddLeaveDetail(string email, DateTime leaveStart, DateTime leaveEnd, string leaveReason, string reasonDetail, string addressDuringLeave)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.User != null && s.User.Email == email);
            if (student == null) throw new InvalidOperationException($"No student found with email: {email}");

            var leaveDetail = new LeaveDetails
            {
                StudentID = student.Id,
                LeaveStart = leaveStart,
                LeaveEnd = leaveEnd,
                LeaveReason = leaveReason,
                ReasonDetail = reasonDetail,
                AddressDuringLeave = addressDuringLeave,
                LeaveStatus = "Onay Bekliyor"
            };
            _context.LeaveDetails.Add(leaveDetail);
            _context.SaveChanges();
        }

        public void DeleteLeaveDetail(string email, int leaveId)
        {
            var leaveDetail = _context.LeaveDetails.Include(l => l.Student).ThenInclude(s => s!.User)
                .FirstOrDefault(l => l.LeaveID == leaveId && l.Student != null && l.Student.User != null && l.Student.User.Email == email);
            if (leaveDetail != null)
            {
                _context.LeaveDetails.Remove(leaveDetail);
                _context.SaveChanges();
            }
        }

        public (Internship? Internship, List<Video> Videos, int FilledDaysCount, List<LeaveDetails> LeaveDetails) GetStatisticsDataByEmail(string email)
        {
            var internshipDetails = GetInternshipByStudentEmail(email);
            var videos = GetUserVideos(email);
            var filledDaysCount = GetFilledDaysCount(email);
            var leaveDetails = GetLeaveDetailsByEmail(email);

            return (internshipDetails, videos, filledDaysCount, leaveDetails);
        }

        public Internship? GetInternshipByStudentEmail(string email)
        {
            return _context.Internships
                .Include(i => i.Student).ThenInclude(s => s!.User)
                .FirstOrDefault(i => i.Student != null && i.Student.User != null && i.Student.User.Email == email);
        }

        private int GetFilledDaysCount(string email)
        {
            return _context.Notes
                .Include(n => n.Student).ThenInclude(s => s!.User)
                .Where(n => n.Student != null && n.Student.User != null && n.Student.User.Email == email)
                .Select(n => n.CreatedAt.Date)
                .Distinct()
                .Count();
        }
    }
}
