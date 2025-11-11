using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMEAutomationDBOperations.Services
{
    public class StudentDashboardService
    {
        private readonly IRepository _repository;

        public StudentDashboardService(IRepository repository)
        {
            _repository = repository;
        }

        public List<Note> GetUserNotes(string email)
        {
            var notes = new List<Note>();
            string query = "SELECT n.* FROM Notes n JOIN Students s ON n.StudentID = s.StudentID WHERE s.Email = @Email";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notes.Add(new Note
                        {
                            NoteID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            SubTitle = reader.GetString(3),
                            Content = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5),
                            UpdatedAt = reader.GetDateTime(6)
                        });
                    }
                }
            }
            return notes;
        }

        public List<Video> GetUserVideos(string email)
        {
            var videos = new List<Video>();
            string query = "SELECT v.* FROM Videos v JOIN Students s ON v.StudentID = s.StudentID WHERE s.Email = @Email";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        videos.Add(new Video
                        {
                            VideoID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            Description = reader.GetString(3),
                            UploadDate = reader.GetDateTime(4),
                            FilePath = reader.GetString(5)
                        });
                    }
                }
            }
            return videos;
        }

        public Note? GetNoteById(int noteId)
        {
            string query = "SELECT * FROM Notes WHERE NoteID = @NoteID";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NoteID", noteId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Note
                        {
                            NoteID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            Title = reader.GetString(2),
                            SubTitle = reader.GetString(3),
                            Content = reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5),
                            UpdatedAt = reader.GetDateTime(6)
                        };
                    }
                }
            }
            return null;
        }

        public List<LeaveDetails> GetLeaveDetailsByEmail(string email)
        {
            var leaveDetails = new List<LeaveDetails>();
            string query = "SELECT ld.LeaveID, ld.StudentID, ld.LeaveReason, ld.ReasonDetail, ld.AddressDuringLeave, ld.LeaveStatus, ld.LeaveStart, ld.LeaveEnd FROM LeaveDetails ld JOIN Students s ON ld.StudentID = s.StudentID WHERE s.Email = @Email";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        leaveDetails.Add(new LeaveDetails
                        {
                            LeaveID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            LeaveReason = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ReasonDetail = reader.IsDBNull(3) ? null : reader.GetString(3),
                            AddressDuringLeave = reader.IsDBNull(4) ? null : reader.GetString(4),
                            LeaveStatus = reader.IsDBNull(5) ? null : reader.GetString(5),
                            LeaveStart = reader.GetDateTime(6),
                            LeaveEnd = reader.GetDateTime(7)
                        });
                    }
                }
            }
            return leaveDetails;
        }

        public void AddUserNote(string email, Note note)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");
            }
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note), "Note cannot be null.");
            }

            string studentIdQuery = "SELECT StudentID FROM Students WHERE Email = @Email";
            object result;
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(studentIdQuery, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                result = command.ExecuteScalar();
            }

            if (result == null)
            {
                throw new InvalidOperationException($"No student found with email: {email}");
            }

            int studentId = (int)result;

            string insertQuery = "INSERT INTO Notes (StudentID, Title, SubTitle, Content, CreatedAt, UpdatedAt) VALUES (@StudentID, @Title, @SubTitle, @Content, @CreatedAt, @UpdatedAt)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@StudentID", studentId);
                command.Parameters.AddWithValue("@Title", note.Title);
                command.Parameters.AddWithValue("@SubTitle", note.SubTitle);
                command.Parameters.AddWithValue("@Content", note.Content);
                command.Parameters.AddWithValue("@CreatedAt", note.CreatedAt);
                command.Parameters.AddWithValue("@UpdatedAt", note.UpdatedAt);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteUserNote(string email, int noteId)
        {
            string query = "DELETE FROM Notes WHERE NoteID = @NoteID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NoteID", noteId);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateUserNote(string email, int noteId, string title, string subTitle, string content)
        {
            string query = "UPDATE Notes SET Title = @Title, SubTitle = @SubTitle, Content = @Content, UpdatedAt = @UpdatedAt WHERE NoteID = @NoteID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NoteID", noteId);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@SubTitle", subTitle);
                command.Parameters.AddWithValue("@Content", content);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddUserVideo(string email, Video video)
        {
            string studentIdQuery = "SELECT StudentID FROM Students WHERE Email = @Email";
            object result;
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(studentIdQuery, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                result = command.ExecuteScalar();
            }

            if (result == null)
            {
                throw new InvalidOperationException($"No student found with email: {email}");
            }

            int studentId = (int)result;

            string insertQuery = "INSERT INTO Videos (StudentID, Title, Description, FilePath, UploadDate) VALUES (@StudentID, @Title, @Description, @FilePath, @UploadDate)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@StudentID", studentId);
                command.Parameters.AddWithValue("@Title", video.Title);
                command.Parameters.AddWithValue("@Description", video.Description);
                command.Parameters.AddWithValue("@FilePath", video.FilePath);
                command.Parameters.AddWithValue("@UploadDate", video.UploadDate);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteUserVideo(string email, int videoId)
        {
            string query = "DELETE FROM Videos WHERE VideoID = @VideoID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@VideoID", videoId);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddLeaveDetail(string email, DateTime leaveStart, DateTime leaveEnd, string leaveReason, string reasonDetail, string addressDuringLeave)
        {
            string studentIdQuery = "SELECT StudentID FROM Students WHERE Email = @Email";
            object result;
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(studentIdQuery, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                result = command.ExecuteScalar();
            }

            if (result == null)
            {
                throw new InvalidOperationException($"No student found with email: {email}");
            }
            
            int studentId = (int)result;

            string insertQuery = "INSERT INTO LeaveDetails (StudentID, LeaveStart, LeaveEnd, LeaveReason, ReasonDetail, AddressDuringLeave, LeaveStatus) VALUES (@StudentID, @LeaveStart, @LeaveEnd, @LeaveReason, @ReasonDetail, @AddressDuringLeave, @LeaveStatus)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@StudentID", studentId);
                command.Parameters.AddWithValue("@LeaveStart", leaveStart);
                command.Parameters.AddWithValue("@LeaveEnd", leaveEnd);
                command.Parameters.AddWithValue("@LeaveReason", leaveReason);
                command.Parameters.AddWithValue("@ReasonDetail", reasonDetail);
                command.Parameters.AddWithValue("@AddressDuringLeave", addressDuringLeave);
                command.Parameters.AddWithValue("@LeaveStatus", "Onay Bekliyor");
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteLeaveDetail(string email, int leaveId)
        {
            string query = "DELETE FROM LeaveDetails WHERE LeaveID = @LeaveID AND StudentID = (SELECT StudentID FROM Students WHERE Email = @Email)";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LeaveID", leaveId);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public (InternshipDetails InternshipDetails, List<Video> Videos, int FilledDaysCount, List<LeaveDetails> LeaveDetails) GetStatisticsDataByEmail(string email)
        {
            var internshipDetails = GetInternshipDetailsByStudentEmail(email);
            var videos = GetUserVideos(email);
            var filledDaysCount = GetFilledDaysCount(email);
            var leaveDetails = GetLeaveDetailsByEmail(email);

            return (internshipDetails, videos, filledDaysCount, leaveDetails);
        }

        public InternshipDetails? GetInternshipDetailsByStudentEmail(string email)
        {
            string query = "SELECT i.* FROM InternshipDetails i JOIN Students s ON i.StudentID = s.StudentID WHERE s.Email = @Email";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new InternshipDetails
                        {
                            InternshipID = reader.GetInt32(0),
                            StudentID = reader.GetInt32(1),
                            CompanyID = reader.GetInt32(2),
                            SupervisorID = reader.GetInt32(3),
                            InternshipTitle = reader.GetString(4),
                            StartDate = reader.GetDateTime(5),
                            EndDate = reader.GetDateTime(6),
                            TotalTrainingDays = reader.GetInt32(7),
                            LeaveDays = reader.GetInt32(8),
                            WorkDays = reader.GetString(9),
                            PaidAmount = reader.GetDecimal(10),
                            CreatedAt = reader.GetDateTime(11),
                            UpdatedAt = reader.GetDateTime(12)
                        };
                    }
                }
            }
            return null;
        }

        private int GetFilledDaysCount(string email)
        {
            string query = "SELECT COUNT(DISTINCT CAST(CreatedAt AS DATE)) FROM Notes n JOIN Students s ON n.StudentID = s.StudentID WHERE s.Email = @Email";
            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : (int)result;
            }
        }
    }
}
