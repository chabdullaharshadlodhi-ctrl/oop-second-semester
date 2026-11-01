using System;
using HospitalManagement.Model;
using HospitalManagement.Services;

namespace HospitalManagement.UI
{
    public class UserUI
    {
        public static string mainMenu()
        {
            Console.WriteLine("\nHOSPITAL MANAGEMENT SYSTEM");
            Console.WriteLine("1. Sign In");
            Console.WriteLine("2. Sign Up (Patient)");
            Console.WriteLine("3. Exit");
            Console.Write("Choose option: ");
            return Console.ReadLine();
        }

        public static MUser signIn()
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                Console.WriteLine("\nLogin attempt " + attempt + " of 3");
                string name = ConsoleUtility.readText("Username: ");
                string password = ConsoleUtility.readText("Password: ");
                MUser user = UserService.signIn(name, password);
                if (user != null)
                {
                    Console.WriteLine("Login successful.");
                    return user;
                }
                Console.WriteLine("Incorrect username or password.");
            }
            Console.WriteLine("Three attempts used. Returning to main menu.");
            return null;
        }

        public static void signUp()
        {
            Console.WriteLine("\nCREATE PATIENT ACCOUNT");
            string name = ConsoleUtility.readText("Choose username: ");
            string password = ConsoleUtility.readText("Choose password: ");
            int id = ConsoleUtility.readNumber("Your existing patient ID: ");
            MUser user = new MUser(name, password, "Patient", id);
            Console.WriteLine(UserService.signUp(user));
        }
    }
}
