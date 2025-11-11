using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace IMEAutomationDBOperations.Services
{
    public class CompanyService
    {
        private readonly IRepository _repository;

        public CompanyService(IRepository repository)
        {
            _repository = repository;
        }

        public List<Company> GetCompaniesData()
        {
            string query = "SELECT * FROM Company";
            var companies = new List<Company>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var company = new Company
                            {
                                CompanyID = reader.GetInt32(0),
                                CompanyName = reader.GetString(1),
                                TaxNumber = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                EmployeeCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Departments = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                PhoneNumber = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Website = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                Industry = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                ManagerFirstName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                ManagerLastName = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                ManagerPhone = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                ManagerEmail = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                                BankName = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                BankBranch = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                                BankIbanNo = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                            };
                            companies.Add(company);
                        }
                    }
                }
            }

            return companies;
        }

        public int AddCompany(Company company)
        {
            if (company == null)
            {
                return -1; // Indicate an error
            }

            string query = @"
                INSERT INTO Company (CompanyName, TaxNumber, EmployeeCount, Departments, Address, PhoneNumber, Website, Industry, Email,
                                    ManagerFirstName, ManagerLastName, ManagerPhone, ManagerEmail, BankName, BankBranch, BankIbanNo)
                OUTPUT INSERTED.CompanyID
                VALUES (@CompanyName, @TaxNumber, @EmployeeCount, @Departments, @Address, @PhoneNumber, @Website, @Industry, @Email,
                        @ManagerFirstName, @ManagerLastName, @ManagerPhone, @ManagerEmail, @BankName, @BankBranch, @BankIbanNo)";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    command.Parameters.AddWithValue("@TaxNumber", (object)company.TaxNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmployeeCount", (object)company.EmployeeCount ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Departments", (object)company.Departments ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)company.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PhoneNumber", (object)company.PhoneNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Website", (object)company.Website ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Industry", (object)company.Industry ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)company.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ManagerFirstName", (object)company.ManagerFirstName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ManagerLastName", (object)company.ManagerLastName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ManagerPhone", (object)company.ManagerPhone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ManagerEmail", (object)company.ManagerEmail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@BankName", (object)company.BankName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@BankBranch", (object)company.BankBranch ?? DBNull.Value);
                    command.Parameters.AddWithValue("@BankIbanNo", (object)company.BankIbanNo ?? DBNull.Value);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public Company GetCompanyByName(string companyName)
        {
            string query = "SELECT * FROM Company WHERE CompanyName = @CompanyName";
            Company company = null;

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyName", companyName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            company = new Company
                            {
                                CompanyID = reader.GetInt32(0),
                                CompanyName = reader.GetString(1),
                                TaxNumber = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                EmployeeCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Departments = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                PhoneNumber = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Website = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                Industry = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                Email = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                ManagerFirstName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                ManagerLastName = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                ManagerPhone = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                ManagerEmail = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                                BankName = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                BankBranch = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                                BankIbanNo = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                            };
                        }
                    }
                }
            }

            return company;
        }
    }
}