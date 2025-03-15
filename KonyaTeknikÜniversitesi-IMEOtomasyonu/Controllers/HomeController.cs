using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMEAutomationDBOperations.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult UserList()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

    }
}
