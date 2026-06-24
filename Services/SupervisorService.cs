using System;
using System.Collections.Generic;
using System.Linq;
using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Services
{
    public class SupervisorService
    {
        private readonly ApplicationDbContext _context;

        public SupervisorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<InternshipSupervisor> GetSupervisorsData()
        {
            return _context.InternshipSupervisors.Include(s => s.User).Include(s => s.Company).ToList();
        }

        public List<Student> GetStudentsBySupervisorEmail(string supervisorEmail)
        {
            return _context.Internships
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Include(i => i.Supervisor)
                    .ThenInclude(sup => sup.User)
                .Where(i => i.Supervisor != null && i.Supervisor.User != null && i.Supervisor.User.Email == supervisorEmail && i.Student != null)
                .Select(i => i.Student!)
                .Distinct()
                .ToList();
        }

        public InternshipSupervisor? GetSupervisorByEmail(string email)
        {
            return _context.InternshipSupervisors
                .Include(s => s.User)
                .Include(s => s.Company)
                .FirstOrDefault(s => s.User != null && s.User.Email == email);
        }

        public Company? GetCompanyById(int companyId)
        {
            return _context.Companies.FirstOrDefault(c => c.CompanyId == companyId);
        }

        public void AddSupervisor(InternshipSupervisor supervisor)
        {
            _context.InternshipSupervisors.Add(supervisor);
            _context.SaveChanges();
        }

        public Student? SearchStudentByName(string searchTerm, string supervisorEmail)
        {
            return _context.Internships
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Include(i => i.Supervisor)
                    .ThenInclude(sup => sup.User)
                .Where(i => i.Supervisor != null && i.Supervisor.User != null && i.Supervisor.User.Email == supervisorEmail && i.Student != null && i.Student.User != null)
                .Where(i => (i.Student!.User!.FirstName + " " + i.Student.User.LastName).Contains(searchTerm) || 
                            i.Student.User.FirstName.Contains(searchTerm) || 
                            i.Student.User.LastName.Contains(searchTerm))
                .Select(i => i.Student)
                .FirstOrDefault();
        }

        public void UpdateSupervisor(InternshipSupervisor supervisor)
        {
            var existing = _context.InternshipSupervisors.Include(s => s.User).FirstOrDefault(s => s.Id == supervisor.Id);
            if (existing != null)
            {
                existing.ContactPhone = supervisor.ContactPhone;
                existing.Expertise = supervisor.Expertise;
                existing.CompanyId = supervisor.CompanyId;
                if (supervisor.User != null && existing.User != null)
                {
                    existing.User.FirstName = supervisor.User.FirstName;
                    existing.User.LastName = supervisor.User.LastName;
                    existing.User.Email = supervisor.User.Email;
                }
                _context.SaveChanges();
            }
        }
    }
}
