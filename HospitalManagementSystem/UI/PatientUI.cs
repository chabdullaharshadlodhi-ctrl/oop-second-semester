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
                Console.WriteLine("8. Filters");
                Console.WriteLine("9. paginatedView");
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
               
                else if (option == "8")
                    filterMenu();
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

        public static void filterMenu()
        {
            while (true)
            {
                Console.WriteLine("\nFILTERS");
                Console.WriteLine("1. Name (exact / contains / starts / ends)");
                Console.WriteLine("2. Disease (exact / contains / starts / ends)");
                Console.WriteLine("3. Age (exact / range / at least / at most)");
                Console.WriteLine("4. ID (exact / range / at least / at most)");
                Console.WriteLine("5. Disease AND age range");
                Console.WriteLine("6. Disease OR age range");
                Console.WriteLine("7. Either of two diseases");
                Console.WriteLine("8. Exclude a disease");
                Console.WriteLine("9. All patients (for statistics / sorting)");
                Console.WriteLine("0. Back to admin menu");
                Console.Write("Choose filter: ");
                string choice = Console.ReadLine();
                if (choice == "0") return;
                List<Patient> result;
                if (choice == "1" || choice == "2")
                {
                    string query = ConsoleUtility.readText("Search text: ");
                    int mode = readChoice("1 Exact, 2 Contains, 3 Starts, 4 Ends: ", 1, 4);
                    result = PatientService.filterText(query, mode, choice == "2");
                }
                else if (choice == "3" || choice == "4" || choice == "5" || choice == "6")
                {
                    string disease = "";
                    if (choice == "5" || choice == "6") disease = ConsoleUtility.readText("Exact disease: ");
                    bool useAge = choice != "4";
                    int minimum = 0;
                    int maximum = int.MaxValue;
                    if (useAge) maximum = 130;
                    int mode = 2;
                    if (choice == "3" || choice == "4")
                        mode = readChoice("1 Exact, 2 Range, 3 At least, 4 At most: ", 1, 4);
                    if (mode == 1)
                    {
                        minimum = readFilterNumber("Exact value: ", useAge);
                        maximum = minimum;
                    }
                    else
                    {
                        if (mode != 4) minimum = readFilterNumber("Minimum (included): ", useAge);
                        if (mode != 3) maximum = readFilterNumber("Maximum (included): ", useAge);
                    }
                    if (minimum > maximum)
                    {
                        Console.WriteLine("Minimum cannot be greater than maximum.");
                        ConsoleUtility.pause();
                        continue;
                    }
                    if (choice == "5" || choice == "6")
                        result = PatientService.filterDiseaseAndAge(disease, minimum, maximum, choice == "6");
                    else result = PatientService.filterRange(minimum, maximum, useAge);
                }
                else if (choice == "7")
                {
                    string first = ConsoleUtility.readText("First disease: ");
                    string second = ConsoleUtility.readText("Second disease: ");
                    result = PatientService.filterTwoDiseases(first, second);
                }
                else if (choice == "8")
                    result = PatientService.filterWithoutDisease(ConsoleUtility.readText("Disease to exclude: "));
                else if (choice == "9") result = PatientService.getAllPatients();
                else
                {
                    Console.WriteLine("Invalid filter option.");
                    continue;
                }
                showFilterResult(result);
                ConsoleUtility.pause();
            }
        }

        public static int readChoice(string prompt, int minimum, int maximum)
        {
            while (true)
            {
                int value = ConsoleUtility.readNumber(prompt);
                if (value >= minimum && value <= maximum) return value;
                Console.WriteLine("Choose a number from " + minimum + " to " + maximum + ".");
            }
        }

        public static int readFilterNumber(string prompt, bool useAge)
        {
            while (true)
            {
                int value = ConsoleUtility.readNumber(prompt);
                if (useAge && value <= 130) return value;
                if (!useAge && value > 0) return value;
                Console.WriteLine("Age must be 0 to 130; ID must be positive.");
            }
        }

        public static void showFilterResult(List<Patient> result)
        {
            showPatients(result);
            Console.WriteLine("Matching count: " + result.Count);
            if (result.Count == 0) return;
            Console.WriteLine("1 Back, 2 Statistics, 3 Sort by age, 4 Sort by name, 5 Delete matches");
            int action = readChoice("Action: ", 1, 5);
            if (action == 2)
            {
                Console.WriteLine("Total age: " + PatientService.totalAge(result));
                Console.WriteLine("Average age: " + PatientService.averageAge(result));
                Console.WriteLine("Youngest:");
                showPatients(PatientService.extremeAge(result, false));
                Console.WriteLine("Oldest:");
                showPatients(PatientService.extremeAge(result, true));
            }
            else if (action == 3 || action == 4)
            {
                int order = readChoice("1 Ascending, 2 Descending: ", 1, 2);
                showPatients(PatientService.sortFiltered(result, action == 4, order == 2));
            }
            else if (action == 5)
            {
                Console.WriteLine("Delete ALL shown patients and linked login accounts? History remains.");
                Console.Write("Type YES to confirm, anything else cancels: ");
                string confirmation = Console.ReadLine();
                if (confirmation == "YES")
                    Console.WriteLine("Deleted: " + PatientService.deleteFiltered(result, confirmation));
                else Console.WriteLine("Deletion cancelled.");
            }
        }
    }
}
