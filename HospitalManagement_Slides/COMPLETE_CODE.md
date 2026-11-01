# Complete source, for reading

The solution already includes these separate files. Do not paste this whole guide into Program.cs.

## Model/Patient.cs

```csharp
namespace HospitalManagement.Model
{
    public class Patient
    {
        public int id;
        public string name;
        public int age;
        public string disease;

        // Parameterized constructor: Week 2 Lecture 2.
        public Patient(int id, string name, int age, string disease)
        {
            this.id = id;
            this.name = name;
            this.age = age;
            this.disease = disease;
        }

        // Copy constructor: Week 3 Lecture 1.
        // History needs a separate object so updates do not change old history.
        public Patient(Patient p)
        {
            id = p.id;
            name = p.name;
            age = p.age;
            disease = p.disease;
        }
    }
}
```

## Model/MUser.cs

```csharp
namespace HospitalManagement.Model
{
    public class MUser
    {
        public string name;
        public string password;
        public string role;
        public int patientId;

        // Same user-and-role idea as Week 3 Lecture 3.
        // patientId connects a patient account to its patient record.
        public MUser(string name, string password, string role, int patientId)
        {
            this.name = name;
            this.password = password;
            this.role = role;
            this.patientId = patientId;
        }

        public bool isAdmin()
        {
            if (role == "Admin")
                return true;
            return false;
        }
    }
}
```

## Common/FileHelper.cs

```csharp
namespace HospitalManagement.Common
{
    public class FileHelper
    {
        // Manual comma counter from Week 1 Lecture 2, page 20.
        public static string parseData(string record, int field)
        {
            int comma = 1;
            string item = "";
            for (int x = 0; x < record.Length; x++)
            {
                if (record[x] == ',')
                    comma++;
                else if (comma == field)
                    item = item + record[x];
            }
            return item;
        }

        // Only loops and conditions: commas would break our file format.
        public static bool validText(string text)
        {
            if (text == null || text == "")
                return false;
            bool hasCharacter = false;
            for (int x = 0; x < text.Length; x++)
            {
                if (text[x] == ',' || text[x] == '\n' || text[x] == '\r')
                    return false;
                if (text[x] != ' ' && text[x] != '\t')
                    hasCharacter = true;
            }
            return hasCharacter;
        }
    }
}
```

## Repositories/PatientRepository.cs

```csharp
using System.Collections.Generic;
using System.IO;
using HospitalManagement.Model;
using HospitalManagement.Common;

namespace HospitalManagement.Repositories
{
    public class PatientRepository
    {
        // Repository = DL. It owns the lists and handles text files.
        public static List<Patient> patients = new List<Patient>();
        public static List<Patient> history = new List<Patient>();

        public static void loadData()
        {
            patients = readFile("patients.txt");
            history = readFile("history.txt");
        }

        public static List<Patient> readFile(string path)
        {
            List<Patient> records = new List<Patient>();
            if (File.Exists(path))
            {
                StreamReader file = new StreamReader(path);
                string record;
                while ((record = file.ReadLine()) != null)
                {
                    if (record != "")
                    {
                        int id = int.Parse(FileHelper.parseData(record, 1));
                        string name = FileHelper.parseData(record, 2);
                        int age = int.Parse(FileHelper.parseData(record, 3));
                        string disease = FileHelper.parseData(record, 4);
                        Patient p = new Patient(id, name, age, disease);
                        records.Add(p);
                    }
                }
                file.Close();
            }
            return records;
        }

        public static void savePatients()
        {
            writeFile("patients.txt", patients);
        }

        public static void saveHistory()
        {
            writeFile("history.txt", history);
        }

        public static void writeFile(string path, List<Patient> records)
        {
            // false overwrites: the file will match the updated list.
            StreamWriter file = new StreamWriter(path, false);
            for (int x = 0; x < records.Count; x++)
            {
                Patient p = records[x];
                file.WriteLine(p.id + "," + p.name + "," + p.age + "," + p.disease);
            }
            file.Close();
        }
    }
}
```

## Repositories/UserRepository.cs

```csharp
using System.Collections.Generic;
using System.IO;
using HospitalManagement.Model;
using HospitalManagement.Common;

namespace HospitalManagement.Repositories
{
    public class UserRepository
    {
        public static List<MUser> users = new List<MUser>();

        public static void loadData()
        {
            users = new List<MUser>();
            if (File.Exists("users.txt"))
            {
                StreamReader file = new StreamReader("users.txt");
                string record;
                while ((record = file.ReadLine()) != null)
                {
                    if (record != "")
                    {
                        string name = FileHelper.parseData(record, 1);
                        string password = FileHelper.parseData(record, 2);
                        string role = FileHelper.parseData(record, 3);
                        int id = int.Parse(FileHelper.parseData(record, 4));
                        users.Add(new MUser(name, password, role, id));
                    }
                }
                file.Close();
            }
        }

        public static void saveUsers()
        {
            StreamWriter file = new StreamWriter("users.txt", false);
            for (int x = 0; x < users.Count; x++)
            {
                MUser u = users[x];
                file.WriteLine(u.name + "," + u.password + "," + u.role + "," + u.patientId);
            }
            file.Close();
        }
    }
}
```

## Services/PatientService.cs

```csharp
using System.Collections.Generic;
using HospitalManagement.Model;
using HospitalManagement.Repositories;
using HospitalManagement.Common;

namespace HospitalManagement.Services
{
    public class PatientService
    {
        // Service = BL. No Console input/output and no StreamReader here.
        public static void loadData()
        {
            PatientRepository.loadData();
        }

        public static List<Patient> getAllPatients()
        {
            return PatientRepository.patients;
        }

        public static List<Patient> getHistory()
        {
            return PatientRepository.history;
        }

        public static Patient findById(int id)
        {
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                if (PatientRepository.patients[x].id == id)
                    return PatientRepository.patients[x];
            }
            return null;
        }

        public static List<Patient> searchByName(string name)
        {
            List<Patient> result = new List<Patient>();
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                if (PatientRepository.patients[x].name == name)
                    result.Add(PatientRepository.patients[x]);
            }
            return result;
        }

        public static bool validPatient(Patient p)
        {
            if (p.id <= 0 || p.age < 0 || p.age > 130)
                return false;
            if (!FileHelper.validText(p.name) || !FileHelper.validText(p.disease))
                return false;
            return true;
        }

        public static string addPatient(Patient p)
        {
            if (!validPatient(p))
                return "Invalid record. Check ID, age, name and disease.";
            if (PatientRepository.patients.Count >= 100)
                return "System full: maximum 100 patients.";
            if (findById(p.id) != null)
                return "This patient ID already exists.";

            PatientRepository.patients.Add(p);
            // Insert and RemoveAt are covered in Week 2 Lecture 3.
            PatientRepository.history.Insert(0, new Patient(p));
            if (PatientRepository.history.Count > 10)
                PatientRepository.history.RemoveAt(10);
            PatientRepository.savePatients();
            PatientRepository.saveHistory();
            return "Patient added and saved.";
        }

        public static string updatePatient(Patient updated)
        {
            Patient stored = findById(updated.id);
            if (stored == null)
                return "Patient not found.";
            if (!validPatient(updated))
                return "Invalid record. Check age, name and disease.";

            stored.name = updated.name;
            stored.age = updated.age;
            stored.disease = updated.disease;
            PatientRepository.savePatients();
            return "Patient updated and saved.";
        }

        public static string selfUpdate(int id, string name, int age)
        {
            Patient stored = findById(id);
            if (stored == null)
                return "Patient record not found.";
            // Keep the stored diagnosis; patients only change name and age.
            Patient updated = new Patient(id, name, age, stored.disease);
            return updatePatient(updated);
        }

        public static string deletePatient(int id)
        {
            for (int x = 0; x < PatientRepository.patients.Count; x++)
            {
                if (PatientRepository.patients[x].id == id)
                {
                    PatientRepository.patients.RemoveAt(x);
                    PatientRepository.savePatients();
                    UserService.removePatientAccount(id);
                    return "Patient and linked account deleted.";
                }
            }
            return "Patient not found.";
        }
    }
}
```

## Services/UserService.cs

```csharp
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
```

## UI/ConsoleUtility.cs

```csharp
using System;
using HospitalManagement.Common;

namespace HospitalManagement.UI
{
    public class ConsoleUtility
    {
        public static void showMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static void pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        public static string readText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = Console.ReadLine();
                if (FileHelper.validText(text))
                    return text;
                Console.WriteLine("Enter non-blank text without commas.");
            }
        }

        public static int readNumber(string prompt)
        {
            // A small loop checks the characters before using int.Parse.
            // Nine digits fit in int, so an enormous input cannot overflow.
            while (true)
            {
                Console.Write(prompt);
                string text = Console.ReadLine();
                bool valid = true;
                if (text == null || text == "")
                    valid = false;
                else if (text.Length > 9)
                    valid = false;
                else
                {
                    for (int x = 0; x < text.Length; x++)
                    {
                        if (text[x] < '0' || text[x] > '9')
                            valid = false;
                    }
                }
                if (valid)
                    return int.Parse(text);
                Console.WriteLine("Enter a non-negative whole number (maximum 9 digits).");
            }
        }
    }
}
```

## UI/PatientUI.cs

```csharp
using System;
using System.Collections.Generic;
using HospitalManagement.Model;
using HospitalManagement.Services;

namespace HospitalManagement.UI
{
    public class PatientUI
    {
        public static void adminMenu()
        {
            while (true)
            {
                Console.WriteLine("\nADMIN MENU");
                Console.WriteLine("1. Show all patients");
                Console.WriteLine("2. Search patient by name");
                Console.WriteLine("3. Update patient information");
                Console.WriteLine("4. Delete patient");
                Console.WriteLine("5. Add patient");
                Console.WriteLine("6. View last 10 added records");
                Console.WriteLine("7. Logout");
                Console.Write("Choose option: ");
                string option = Console.ReadLine();

                if (option == "1")
                    showPatients(PatientService.getAllPatients());
                else if (option == "2")
                {
                    string name = ConsoleUtility.readText("Enter exact patient name: ");
                    showPatients(PatientService.searchByName(name));
                }
                else if (option == "3")
                    updatePatient();
                else if (option == "4")
                    deletePatient();
                else if (option == "5")
                {
                    int id = ConsoleUtility.readNumber("Enter patient ID: ");
                    Patient p = takePatientInput(id);
                    Console.WriteLine(PatientService.addPatient(p));
                }
                else if (option == "6")
                    showPatients(PatientService.getHistory());
                else if (option == "7")
                {
                    Console.WriteLine("Logged out.");
                    break;
                }
                else
                    Console.WriteLine("Invalid option.");
                ConsoleUtility.pause();
            }
        }

        public static Patient takePatientInput(int id)
        {
            string name = ConsoleUtility.readText("Enter name: ");
            int age = ConsoleUtility.readNumber("Enter age: ");
            string disease = ConsoleUtility.readText("Enter disease: ");
            Patient p = new Patient(id, name, age, disease);
            return p;
        }

        public static void updatePatient()
        {
            int id = ConsoleUtility.readNumber("Patient ID to update: ");
            Patient old = PatientService.findById(id);
            if (old == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }
            Console.WriteLine("Old record:");
            showPatient(old);
            // Keep the same ID so the patient's login still points to this record.
            Patient updated = takePatientInput(id);
            Console.WriteLine(PatientService.updatePatient(updated));
        }

        public static void deletePatient()
        {
            int id = ConsoleUtility.readNumber("Patient ID to delete: ");
            Patient p = PatientService.findById(id);
            if (p == null)
            {
                Console.WriteLine("Patient not found.");
                return;
            }
            showPatient(p);
            Console.Write("Delete this patient and their login account? Enter Y: ");
            string answer = Console.ReadLine();
            if (answer == "Y" || answer == "y")
                Console.WriteLine(PatientService.deletePatient(id));
            else
                Console.WriteLine("Deletion cancelled.");
        }

        public static void patientMenu(MUser user)
        {
            while (true)
            {
                Console.WriteLine("\nPATIENT MENU");
                Console.WriteLine("1. View my profile");
                Console.WriteLine("2. Update my name and age");
                Console.WriteLine("3. Logout");
                Console.Write("Choose option: ");
                string option = Console.ReadLine();
                if (option == "1")
                    showPatient(PatientService.findById(user.patientId));
                else if (option == "2")
                {
                    string name = ConsoleUtility.readText("New name: ");
                    int age = ConsoleUtility.readNumber("New age: ");
                    Console.WriteLine(PatientService.selfUpdate(user.patientId, name, age));
                }
                else if (option == "3")
                {
                    Console.WriteLine("Logged out.");
                    break;
                }
                else
                    Console.WriteLine("Invalid option.");
                ConsoleUtility.pause();
            }
        }

        public static void showPatients(List<Patient> records)
        {
            if (records.Count == 0)
            {
                Console.WriteLine("No records found.");
                return;
            }
            Console.WriteLine("ID | Name | Age | Disease");
            for (int x = 0; x < records.Count; x++)
            {
                Patient p = records[x];
                Console.WriteLine(p.id + " | " + p.name + " | " + p.age + " | " + p.disease);
            }
        }

        public static void showPatient(Patient p)
        {
            if (p == null)
            {
                Console.WriteLine("Patient record not found.");
                return;
            }
            Console.WriteLine("ID: " + p.id);
            Console.WriteLine("Name: " + p.name);
            Console.WriteLine("Age: " + p.age);
            Console.WriteLine("Disease: " + p.disease);
        }
    }
}
```

## UI/UserUI.cs

```csharp
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
```

## Program.cs

```csharp
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
```

