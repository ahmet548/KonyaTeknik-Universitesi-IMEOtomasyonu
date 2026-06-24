using System;
using System.Collections.Generic;
using System.Linq;
using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.EntityFrameworkCore;

namespace IMEAutomationDBOperations.Services
{
    public class StudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Student> GetStudentsData()
        {
            return _context.Students.Include(s => s.User).ToList();
        }

        public void UpdateStudent(Student student)
        {
            var existing = _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == student.Id);
            if (existing != null)
            {
                existing.NationalID = student.NationalID;
                existing.SchoolNumber = student.SchoolNumber;
                existing.AcademicYear = student.AcademicYear;
                existing.Department = student.Department;
                existing.BirthDate = student.BirthDate;
                existing.Address = student.Address;
                existing.PhoneNumber = student.PhoneNumber;
                if (student.User != null)
                {
                    existing.User.FirstName = student.User.FirstName;
                    existing.User.LastName = student.User.LastName;
                    existing.User.Email = student.User.Email;
                }
                _context.SaveChanges();
            }
        }

        public Student GetStudentById(int studentId)
        {
            return _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == studentId);
        }

        public int AddStudentAndReturnId(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return student.Id;
        }
    }
}
