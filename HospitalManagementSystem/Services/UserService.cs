using HospitalManagement.Model;
using HospitalManagement.Repositories;
using HospitalManagement.Common;

namespace HospitalManagement.Services
{
    public class UserService
    {
        public static void loadData()
        {
            UserRepository.loadData();
        }

        public static MUser signIn(string name, string password)
        {
            for (int x = 0; x < UserRepository.users.Count; x++)
            {
                MUser user = UserRepository.users[x];
                if (user.name == name && user.password == password)
                    return user;
            }
            return null;
        }

        public static string signUp(MUser user)
        {
            if (!FileHelper.validText(user.name) || !FileHelper.validText(user.password))
                return "Username/password cannot be blank or contain commas.";
            if (user.role != "Patient")
                return "Sign-up creates patient accounts only.";
            if (PatientService.findById(user.patientId) == null)
                return "Ask the admin to add your patient record first.";

            for (int x = 0; x < UserRepository.users.Count; x++)
            {
                if (UserRepository.users[x].name == user.name)
                    return "Username already exists.";
                if (UserRepository.users[x].patientId == user.patientId)
                    return "This patient already has an account.";
            }
            UserRepository.users.Add(user);
            UserRepository.saveUsers();
            return "Account created. You can now sign in.";
        }

        public static void removePatientAccount(int id)
        {
            for (int x = UserRepository.users.Count - 1; x >= 0; x--)
            {
                if (UserRepository.users[x].role == "Patient" &&
                    UserRepository.users[x].patientId == id)
                    UserRepository.users.RemoveAt(x);
            }
            UserRepository.saveUsers();
        }
    }
}
