using System.Collections.Generic;
using System.Linq;
using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;

namespace IMEAutomationDBOperations.Services
{
    public class CompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Company> GetCompaniesData()
        {
            return _context.Companies.ToList();
        }

        public int AddCompany(Company company)
        {
            if (company == null) return -1;
            
            _context.Companies.Add(company);
            _context.SaveChanges();
            return company.CompanyId;
        }

        public Company GetCompanyByName(string companyName)
        {
            return _context.Companies.FirstOrDefault(c => c.CompanyName == companyName);
        }
    }
}
