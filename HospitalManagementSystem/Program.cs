using HospitalManagement.Model;
using HospitalManagement.Services;
using HospitalManagement.UI;

namespace HospitalManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load once, before the loop: Week 3 Lecture 3.
            PatientService.loadData(); 
            UserService.loadData();

            while (true)
            {
                string option = UserUI.mainMenu();
                if (option == "1")
                {
                    MUser user = UserUI.signIn();
                    if (user != null)
                    {
                        if (user.isAdmin())
                            PatientUI.adminMenu();
                        else if (user.role == "Patient")
                            PatientUI.patientMenu(user);
                        else
                            ConsoleUtility.showMessage("Unknown role.");
                    }
                }
                else if (option == "2")
                    UserUI.signUp();
                else if (option == "3")
                {
                    ConsoleUtility.showMessage("Goodbye!");
                    break;
                }
                else
                    ConsoleUtility.showMessage("Invalid option.");
            }
        }
    }
}
