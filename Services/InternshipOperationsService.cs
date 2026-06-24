using System;
using System.Linq;
using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Services
{
    public class InternshipOperationsService
    {
        private readonly ApplicationDbContext _context;

        public InternshipOperationsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public (InternshipSupervisor?, Company?, Internship?, InternshipEvaluation?) GetSupervisorCompanyAndInternshipByStudentEmail(string studentEmail)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.User != null && s.User.Email == studentEmail);
            if (student == null) return (null, null, null, null);

            var internship = _context.Internships
                .Include(i => i.Supervisor).ThenInclude(s => s!.User)
                .Include(i => i.Company)
                .Include(i => i.Evaluation)
                .FirstOrDefault(i => i.StudentId == student.Id);

            if (internship == null) return (null, null, null, null);

            var evaluation = internship.Evaluation;
            return (internship.Supervisor, internship.Company, internship, evaluation);
        }

        public void SaveEvaluation(InternshipEvaluation evaluation)
        {
            var existing = _context.Evaluations.FirstOrDefault(e => e.InternshipId == evaluation.InternshipId);
            if(existing != null)
            {
                existing.AttendanceScore = evaluation.AttendanceScore;
                existing.ResponsibilityScore = evaluation.ResponsibilityScore;
                existing.KnowledgeScore = evaluation.KnowledgeScore;
                existing.ProblemSolvingScore = evaluation.ProblemSolvingScore;
                existing.EquipmentUsageScore = evaluation.EquipmentUsageScore;
                existing.CommunicationScore = evaluation.CommunicationScore;
                existing.MotivationScore = evaluation.MotivationScore;
                existing.ReportingScore = evaluation.ReportingScore;
                existing.TeamworkScore = evaluation.TeamworkScore;
                existing.ExpressionScore = evaluation.ExpressionScore;
            }
            else
            {
                _context.Evaluations.Add(evaluation);
            }
            _context.SaveChanges();
        }

        public void AddStudent(Student student, int SupervisorId, int CompanyId)
        {
            _context.Students.Add(student);
            _context.SaveChanges();

            var internship = new Internship
            {
                StudentId = student.Id,
                SupervisorId = SupervisorId,
                CompanyId = CompanyId,
                Title = "Staj Baþlýðý",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                TotalTrainingDays = 30,
                LeaveDays = 0,
                WorkDays = "",
                PaidAmount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Internships.Add(internship);
            _context.SaveChanges();
        }

        public InternshipEvaluation? GetEvaluationByStudentId(int StudentId)
        {
            var internship = _context.Internships.Include(i => i.Evaluation).FirstOrDefault(i => i.StudentId == StudentId);
            if (internship == null) return null;
            return internship.Evaluation;
        }

        public Internship? GetInternshipByStudentId(int StudentId)
        {
            return _context.Internships.FirstOrDefault(i => i.StudentId == StudentId);
        }

        public void AddInternship(
            int StudentId, int SupervisorId, int CompanyId,
            DateTime startDate, DateTime endDate, string[] workDays, string internshipTitle)
        {
            var workDaysString = string.Join(",", workDays);
            var internship = new Internship
            {
                StudentId = StudentId,
                SupervisorId = SupervisorId,
                CompanyId = CompanyId,
                Title = internshipTitle,
                StartDate = startDate,
                EndDate = endDate,
                TotalTrainingDays = (int)(endDate - startDate).TotalDays + 1,
                LeaveDays = 0,
                WorkDays = workDaysString,
                PaidAmount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Internships.Add(internship);
            _context.SaveChanges();
        }
    }
}
