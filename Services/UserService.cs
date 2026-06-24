using System;
using System.Collections.Generic;
using System.Linq;
using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using IMEAutomationDBOperations.Models.Enums;

namespace IMEAutomationDBOperations.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> GetUsersData()
        {
            return _context.Users.ToList();
        }

        public int AddUser(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                existingUser.PasswordHash = user.PasswordHash;
                _context.SaveChanges();
                return existingUser.Id;
            }
            else
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user.Id;
            }
        }

        public int AddUserAndReturnId(string email, string passwordHash, Role role)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Id;
        }

        public User? GetUserByUsername(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public void UpdatePassword(string email, string newPasswordHash)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.PasswordHash = newPasswordHash;
                _context.SaveChanges();
            }
        }
    }
}
