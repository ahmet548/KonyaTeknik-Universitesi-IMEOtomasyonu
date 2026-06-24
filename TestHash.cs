using System;

namespace IMEAutomationDBOperations {
    class TestHash {
        public static void Run() {
            try {
                bool isValid = BCrypt.Net.BCrypt.Verify("123456", "$2a$11$e/R/i.k2yN.Y3d7pTzG02evK./5QGk7v/3x2s5X0Z8a");
                Console.WriteLine("BCRYPT VALID: " + isValid);
            } catch (Exception e) {
                Console.WriteLine("BCRYPT ERROR: " + e.Message);
            }
        }
    }
}
